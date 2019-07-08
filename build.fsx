#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

let tee f a =
    f a
    a

module private DotnetCore =
    let run cmd workingDir =
        let options =
            DotNet.Options.withWorkingDirectory workingDir
            >> DotNet.Options.withRedirectOutput true

        DotNet.exec options cmd ""

    let runOrFail cmd workingDir =
        run cmd workingDir
        |> tee (fun result ->
            if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir
        )
        |> ignore

    let execute command args (dir: string) =
        let cmd =
            sprintf "%s/%s"
                (dir.TrimEnd('/'))
                command

        let processInfo = System.Diagnostics.ProcessStartInfo(cmd)
        processInfo.RedirectStandardOutput <- true
        processInfo.RedirectStandardError <- true
        processInfo.UseShellExecute <- false
        processInfo.CreateNoWindow <- true
        processInfo.Arguments <- args |> String.concat " "

        use proc =
            new System.Diagnostics.Process(
                StartInfo = processInfo
            )
        if proc.Start() |> not then failwith "Process was not started."
        proc.WaitForExit()

        if proc.ExitCode <> 0 then failwithf "Command '%s' failed in %s." command dir
        (proc.StandardOutput.ReadToEnd(), proc.StandardError.ReadToEnd())

let skipOn option action p =
    if p.Context.Arguments |> Seq.contains option
    then Trace.tracefn "Skipped ..."
    else action p

Target.create "Clean" <| skipOn "no-clean" (fun _ ->
    !! "./**/bin"
    ++ "./**/obj"
    |> Shell.cleanDirs
)

Target.create "Build" (fun _ ->
    !! "./**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

Target.create "ClearTests" (fun _ ->
    !! "tests/data/*actual*.txt"
    |> Seq.iter File.delete
)

let findTestsDll from =
    !! "tests/bin/**/tests.dll"
    |> Seq.head
    |> String.split '/'
    |> Seq.skipWhile ((<>) from)
    |> String.concat "/"

Target.create "PrepareTests" (fun _ ->
    let testsDll = findTestsDll "tests"

    let prepare prepareType level =
        Trace.tracefn " - prepare %s: \"%s\" ..." prepareType level
        let (output, errorOutput) = DotnetCore.execute "dotnet" [testsDll; "prepare"; prepareType; level] "."

        match prepareType with
        | "output" ->
            output |> File.writeString false (sprintf "tests/data/output-actual-%s.txt" level)
            errorOutput |> File.writeString false (sprintf "tests/data/error-actual-%s.txt" level)
        | "progress" ->
            output |> File.writeString false (sprintf "tests/data/progress-actual-%s.txt" level)
        | unknownType ->
            failwithf "Unknown type (\%s\") passed for preparing." unknownType

    [
        "default"
        "quiet"
        "normal"
        "verbose"
        "veryVerbose"
        "debug"
    ]
    |> List.iter (prepare "output")

    [
        "default"
        "quiet"
    ]
    |> List.iter (prepare "progress")
)

Target.create "Tests" (fun _ ->
    let testsDll = findTestsDll "bin"
    DotnetCore.runOrFail testsDll "tests"
)

"Clean"
    ==> "Build"
    ==> "ClearTests"
    ==> "PrepareTests"
    ==> "Tests"

Target.runOrDefaultWithArguments "Build"
