#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.Tools.Git

type ToolDir =
    /// Global tool dir must be in PATH - ${PATH}:/root/.dotnet/tools
    | Global
    /// Just a dir name, the location will be used as: ./{LocalDirName}
    | Local of string

// ========================================================================================================
// === F# / Library fake build ====================================================== custom = 2020-01-09 =
// --------------------------------------------------------------------------------------------------------
// Options:
//  - no-clean   - disables clean of dirs in the first step (required on CI)
//  - no-lint    - lint will be executed, but the result is not validated
// --------------------------------------------------------------------------------------------------------
// Table of contents:
//      1. Information about project, configuration
//      2. Utilities, DotnetCore functions
//      3. FAKE targets
//      4. FAKE targets hierarchy
// ========================================================================================================

// --------------------------------------------------------------------------------------------------------
// 1. Information about the project to be used at NuGet and in AssemblyInfo files and other FAKE configuration
// --------------------------------------------------------------------------------------------------------

let project = "MF/ConsoleStyle"
let summary = "Library to help with basic Console I/O"

let release = ReleaseNotes.parse (System.IO.File.ReadAllLines "CHANGELOG.md" |> Seq.filter ((<>) "## Unreleased"))
let gitCommit = Information.getCurrentSHA1(".")
let gitBranch = Information.getBranchName(".")

let toolsDir = Local "tools"

// --------------------------------------------------------------------------------------------------------
// 2. Utilities, DotnetCore functions, etc.
// --------------------------------------------------------------------------------------------------------

[<AutoOpen>]
module private Utils =
    let tee f a =
        f a
        a

    let skipOn option action p =
        if p.Context.Arguments |> Seq.contains option
        then Trace.tracefn "Skipped ..."
        else action p

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

    let runInRoot cmd = run cmd "."
    let runInRootOrFail cmd = runOrFail cmd "."

    let installOrUpdateTool toolDir tool =
        let toolCommand action =
            match toolDir with
            | Global -> sprintf "tool %s --global %s" action tool
            | Local dir -> sprintf "tool %s --tool-path ./%s %s" action dir tool

        match runInRoot (toolCommand "install") with
        | { ExitCode = code } when code <> 0 -> runInRootOrFail (toolCommand "update")
        | _ -> ()

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

        if proc.ExitCode <> 0 then failwithf "Command '%s' failed in %s.\n%A" command dir (proc.StandardError.ReadToEnd())
        (proc.StandardOutput.ReadToEnd(), proc.StandardError.ReadToEnd())

// --------------------------------------------------------------------------------------------------------
// 3. Targets for FAKE
// --------------------------------------------------------------------------------------------------------

Target.create "Clean" <| skipOn "no-clean" (fun _ ->
    !! "./**/bin/Release"
    ++ "./**/bin/Debug"
    ++ "./**/obj"
    |> Shell.cleanDirs
)

Target.create "AssemblyInfo" (fun _ ->
    let getAssemblyInfoAttributes projectName =
        [
            AssemblyInfo.Title projectName
            AssemblyInfo.Product project
            AssemblyInfo.Description summary
            AssemblyInfo.Version release.AssemblyVersion
            AssemblyInfo.FileVersion release.AssemblyVersion
            AssemblyInfo.InternalsVisibleTo "tests"
            AssemblyInfo.Metadata("gitbranch", gitBranch)
            AssemblyInfo.Metadata("gitcommit", gitCommit)
        ]

    let getProjectDetails projectPath =
        let projectName = System.IO.Path.GetFileNameWithoutExtension(projectPath)
        (
            projectPath,
            projectName,
            System.IO.Path.GetDirectoryName(projectPath),
            (getAssemblyInfoAttributes projectName)
        )

    !! "**/*.*proj"
    -- "example/**/*.*proj"
    |> Seq.map getProjectDetails
    |> Seq.iter (fun (projFileName, _, folderName, attributes) ->
        match projFileName with
        | proj when proj.EndsWith("fsproj") -> AssemblyInfoFile.createFSharp (folderName </> "AssemblyInfo.fs") attributes
        | _ -> ()
    )
)

Target.create "Build" (fun _ ->
    !! "**/*.*proj"
    -- "example/**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

Target.create "Lint" <| skipOn "no-lint" (fun _ ->
    DotnetCore.installOrUpdateTool toolsDir "dotnet-fsharplint"

    let checkResult (messages: string list) =
        let rec check: string list -> unit = function
            | [] -> failwithf "Lint does not yield a summary."
            | head :: rest ->
                if head.Contains "Summary" then
                    match head.Replace("= ", "").Replace(" =", "").Replace("=", "").Replace("Summary: ", "") with
                    | "0 warnings" -> Trace.tracefn "Lint: OK"
                    | warnings -> failwithf "Lint ends up with %s." warnings
                else check rest
        messages
        |> List.rev
        |> check

    !! "**/*.fsproj"
    |> Seq.map (fun fsproj ->
        match toolsDir with
        | Global ->
            DotnetCore.runInRoot (sprintf "fsharplint -f %s" fsproj)
            |> fun (result: ProcessResult) -> result.Messages
        | Local dir ->
            DotnetCore.execute "dotnet-fsharplint" ["-f"; fsproj] ("./" + dir)
            |> fst
            |> tee (Trace.tracefn "%s")
            |> String.split '\n'
            |> Seq.toList
    )
    |> Seq.iter checkResult
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

Target.create "Release" (fun _ ->
    match UserInput.getUserInput "Are you sure - is it tagged yet? [y|n]: " with
    | "y"
    | "yes" ->
        match UserInput.getUserPassword "Nuget ApiKey: " with
        | "" -> failwithf "You have to provide an api key for nuget."
        | apiKey ->
            !! "*.*proj"
            |> Seq.iter (DotNet.pack id)

            Directory.ensure "release"

            !! "bin/**/*.nupkg"
            |> Seq.map (tee (DotNet.nugetPush (fun defaults ->
                { defaults with
                    PushParams = {
                        defaults.PushParams with
                            ApiKey = Some apiKey
                            Source = Some "https://api.nuget.org/v3/index.json"
                    }
                }
            )))
            |> Seq.iter (Shell.moveFile "release")
    | _ -> ()
)

// --------------------------------------------------------------------------------------------------------
// 4. FAKE targets hierarchy
// --------------------------------------------------------------------------------------------------------

"Clean"
    ==> "AssemblyInfo"
    ==> "Build"
    ==> "Lint"
    ==> "Tests"
    ==> "Release"

"Build"
    ?=> "ClearTests"
    ==> "PrepareTests"
    ==> "Tests"

Target.runOrDefaultWithArguments "Build"
