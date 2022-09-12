namespace ConsoleStyle.Tests.Output

module OutputTest =
    open System
    open System.IO
    open System.Text.RegularExpressions
    open Expecto
    open MF.ConsoleStyle

    type OutputTest =
        | Output
        | ErrorOutput
        | Progress

    let prepare (console: ConsoleStyle) =
        // todo - doplnit dalsi funkce a upravit podle examplu a mozna i rozdelit - bude se lepe debugovat (zas toho bude hodne)
        console.Note("Verbosity: %A", console.Verbosity)

        // verbosity
        console.Note("Is quiet %s", (if console.IsQuiet() then "yes" else "no"))
        console.Note("Is normal %s", (if console.IsNormal() then "yes" else "no"))
        console.Note("Is verbose %s", (if console.IsVerbose() then "yes" else "no"))
        console.Note("Is very verbose %s", (if console.IsVeryVerbose() then "yes" else "no"))
        console.Note("Is debug %s", (if console.IsDebug() then "yes" else "no"))

        // output single
        console.MainTitle "This is mainTitle!"
        console.MainTitle ("Formatted mainTitle %s!", "F#")
        console.MainTitle ("Formatted mainTitle %s and %i!", "F#", 42)
        console.MainTitle ("Formatted mainTitle %s with %s and %i!", "foo", "bar", 42)

        console.Title "This is title!"
        console.Title ("Formatted title %s!", "F#")
        console.Title ("Formatted title %s and %i!", "F#", 42)
        console.Title ("Formatted title %s with %s and %i!", "foo", "bar", 42)

        console.Section "This is section!"
        console.Section ("Formatted section %s!", "F#")
        console.Section ("Formatted section %s and %i!", "F#", 42)
        console.Section ("Formatted section %s with %s and %i!", "foo", "bar", 42)

        console.SubTitle "This is subTitle!"
        console.SubTitle ("Formatted subTitle %s!", "F#")
        console.SubTitle ("Formatted subTitle %s and %i!", "F#", 42)
        console.SubTitle ("Formatted subTitle %s with %s and %i!", "foo", "bar", 42)

        console.Message "This is <c:green>message</c>!"
        console.Message ("Formatted <c:green>message</c> %s!", "F#")
        console.Message ("Formatted <c:green>message</c> %s and %i!", "F#", 42)
        console.Message ("Formatted <c:green>message</c> %s with %s and %i!", "foo", "bar", 42)

        console.Error "This is error!"
        console.Error ("Formatted error %s!", "F#")
        console.Error ("Formatted error %s and %i!", "F#", 42)
        console.Error ("Formatted error %s with %s and %i!", "foo", "bar", 42)

        console.Success "This is success!"
        console.Success ("Formatted success %s!", "F#")
        console.Success ("Formatted success %s and %i!", "F#", 42)
        console.Success ("Formatted success %s with %s and %i!", "foo", "bar", 42)

        "Indented message" |> console.Indent |> console.Message

        console.NewLine()

        // output many
        console.Messages "prefix" [ "<c:yellow>line 1</c>"; "line 2" ]
        console.Options "Foo options" [
            [ "first"; "Description of the <c:blue>1st</c>" ]
            [ "second"; "Description of the 2nd" ]
        ]
        console.Options "Different options" [
            [ "first"; "Description of the <c:blue>1st</c>" ]
            [ "second"; "Description of the 2nd" ]
            [ "third"; "Description"; "of the 3rd" ]
        ]
        console.SimpleOptions "Foo simple options" [
            [ "first"; "Description of the <c:blue>1st</c>" ]
            [ "second"; "Description of the 2nd" ]
        ]
        console.GroupedOptions ":" "Grouped options" [
            [ "<c:green>first</c>"; "desc <c:darkgreen>1</c>" ]
            [ "<c:green>group</c>:<c:dark-green>first</c>"; "desc group 1" ]
            [ "<c:green>group:second</c>"; "desc group 2" ]
            [ "second"; "desc 2" ]
        ]
        console.List [
            "<c:yellow>line 1"  // missing end tag
            "<c:>line 2</c>"    // missing color
        ]

        // table
        console.Table ["FirstName"; "Surname"] [
            ["Jon"; "Snow"]
            ["Peter"; "Parker"]
        ]
        console.Table [] [
            ["Jon"; "Snow"]
            ["Peter"; "Parker"]
        ]
        console.Table ["FirstName"; "Surname"] []
        console.Table [] []

        [
            "yellow", [
                "lightyellow"
                "yellow"
                "darkyellow"
            ]

            "red", [
                "lightred"
                "red"
                "darkred"
            ]

            "green", [
                "lightgreen"
                "green"
                "darkgreen"
            ]

            "blue", [
                "Light-cyan"
                "Light-Blue"
                "blue"
                "darkcyan"
                "darkblue"
            ]

            "pink", [
                "pink"
                "dark-pink"
            ]

            "gray", [
                "lightgray"
                "gray"
                "darkGray"
            ]

            "black", []
            "white", []
        ]
        |> List.map (fun (colorGroup, colors) ->
            let colorize color = sprintf "<c:%s>%s</c>" color color

            [
                colorize colorGroup
                colors |> List.map colorize |> String.concat ", "
            ]
        )
        |> console.Table [ "Color Group"; "Colors" ]

    let cases: string list = [
        "default"
        // "quiet"
        "normal"
        // "verbose"
        // "veryVerbose"
        // "debug"
    ]

    let readFile (file: string) =
        Path.Combine(Environment.CurrentDirectory, file)
        |> File.ReadAllLines
        |> Array.toList

    let fileName fileType testType verbosity =
        let prefix =
            match testType with
            | Output -> "output"
            | ErrorOutput -> "error"
            | Progress -> "progress"

        sprintf "data/%s-%s-%s.txt" prefix fileType verbosity

    let expected testType = fileName "expected" testType >> readFile

    (* let files prefix case =
        let result =
            case
            |> sprintf "data/%s-actual-%s.txt" prefix
            |> readFile
            |> List.map (fun line ->
                Regex.Replace(line, @"^\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\]", "[_____DATE_TIME_____]")
            )
        (expected, result) *)

    let testCases testType cases =
        cases
        |> List.map (fun verbosity ->
            testCase $"{testType} - {verbosity}" <| fun _ ->
                let level =
                    match verbosity with
                    | "quiet" -> Verbosity.Quiet
                    | "normal" -> Verbosity.Normal
                    | "verbose" -> Verbosity.Verbose
                    | "veryVerbose" -> Verbosity.VeryVerbose
                    | "debug" -> Verbosity.Debug
                    | _ -> Verbosity.Normal

                use bufferdOutput = new Output.BufferOutput(level)
                let console = ConsoleStyle(bufferdOutput)
                prepare console |> ignore

                let actual = bufferdOutput.Fetch() |> console.RemoveMarkup
                let lines = actual.Split "\n" |> List.ofSeq

                let expected = expected testType verbosity

                try
                    Expect.equal (lines |> List.length) (expected |> List.length) ""

                    expected
                    |> List.iteri (fun i expectedLine ->
                        Expect.equal lines.[i] expectedLine ""
                    )
                with e ->
                    File.WriteAllText((fileName "actual" testType verbosity), actual)
                    raise e
        )

    [<Tests>]
    let outputTest =
        cases
        |> testCases Output
        |> testList "Check output file"

    (* [<Tests>]
    let errorOutputTest =
        cases
        |> testCases ErrorOutput
        |> testList "Check error output file" *)

    (* [<Tests>]
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
        |> testList "Check progress bar file" *)
