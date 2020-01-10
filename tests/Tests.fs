namespace ConsoleStyle.Tests

module Tests =
    open System
    open System.IO
    open System.Text.RegularExpressions
    open Expecto
    open ConsoleStyle.Tests.Output
    open ConsoleStyle.Tests.ProgressBar
    open MF.ConsoleStyle

    let cases = [
        "default"
        "quiet"
        "normal"
        "verbose"
        "veryVerbose"
        "debug"
    ]

    let readFile (file: string) =
        Path.Combine(Environment.CurrentDirectory, file)
        |> File.ReadAllLines
        |> Array.toList

    let files prefix case =
        let expected =
            case
            |> sprintf "data/%s-expected-%s.txt" prefix
            |> readFile
        let result =
            case
            |> sprintf "data/%s-actual-%s.txt" prefix
            |> readFile
            |> List.map (fun line ->
                Regex.Replace(line, @"^\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\]", "[_____DATE_TIME_____]")
            )
        (expected, result)

    let testCases prefix cases =
        cases
        |> List.map (fun case ->
            testCase case <| fun _ ->
                let (expected, result) = case |> files prefix

                Expect.equal (result |> List.length) (expected |> List.length) ""

                expected
                |> List.iteri (fun i expectedLine ->
                    Expect.equal result.[i] expectedLine ""
                )
        )

    [<Tests>]
    let outputTest =
        cases
        |> testCases "output"
        |> testList "Check output file"

    [<Tests>]
    let errorOutputTest =
        cases
        |> testCases "error"
        |> testList "Check error output file"

    [<Tests>]
    let progressBarTest =
        cases
        |> List.filter (fun case -> case = "default" || case = "quiet")
        |> List.map (fun case ->
            testCase case <| fun _ ->
                Tests.skiptest "Progress bar test is skipped since it has no visible output"

                if Console.WindowWidth <= 0 then Tests.skiptest "Progress bar is not displayed here"

                let (expected, result) = case |> files "progress"

                Expect.equal (result |> List.length) (expected |> List.length) ""

                expected
                |> List.iteri (fun i expectedLine ->
                    Expect.equal result.[i] expectedLine ""
                )
        )
        |> testList "Check progress bar file"

    [<EntryPoint>]
    let main argv =
        match argv with
        | [|"prepare"; testType; verbosity|] ->
            let level =
                match verbosity with
                | "quiet" -> Verbosity.Quiet |> Some
                | "normal" -> Verbosity.Normal |> Some
                | "verbose" -> Verbosity.Verbose |> Some
                | "veryVerbose" -> Verbosity.VeryVerbose |> Some
                | "debug" -> Verbosity.Debug |> Some
                | _ -> None

            match testType with
            | "output" -> level |> OutputTest.prepare
            | "progress" -> level |> ProgressBarTest.prepare
            | _ -> 1
        | _ -> Tests.runTestsInAssembly defaultConfig argv
