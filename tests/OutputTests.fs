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

    let prepare (console: ConsoleStyle) =
        console.Note("Verbosity: %A", console.Verbosity)

        // verbosity
        console.Note("Is quiet %s", (if console.IsQuiet() then "yes" else "no"))
        console.Note("Is normal %s", (if console.IsNormal() then "yes" else "no"))
        console.Note("Is verbose %s", (if console.IsVerbose() then "yes" else "no"))
        console.Note("Is very verbose %s", (if console.IsVeryVerbose() then "yes" else "no"))
        console.Note("Is debug %s", (if console.IsDebug() then "yes" else "no"))

        // output single
        console.Title "Simple output"
        console.MainTitle "ConsoleStyle"

        let font = Colorful.FigletFont.Load("chunky.flf")
        console.MainTitle("Title with font", font)

        let figlet = Colorful.Figlet(font)
        console.MainTitle("Figlet title", figlet)

        console.Title("Multiple messages output")

        console.GroupedOptions ":" "Available commands:" [
            [ "list"; "Lists commands" ]
            [ "<c:blue>deployment:list</c>"; "Lists environment" ]
            [ "deployment:release"; "Release a package" ]
            [ "<c:blue>debug</c>:<c:dark-pink>configuration</c>"; "Dumps configuration" ]
            [ "<c:red>test</c>:<c:pink>d</c>"; "D" ]
            [ "<c:dark-red>test</c>:<c:yellow>c</c>"; "C" ]
            [ "<c:gray>test</c>:<c:purple>a</c>"; "a" ]
            [ "<c:white>test</c>:<c:black|bg:gray>b</c>"; "B" ]
        ]

        console.SimpleOptions "Options:" [
            [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c:yellow>[default: \"./config.yaml\"]</c>" ]
            [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c:yellow>[default: \"./config.yaml\"] (missing end tag)" ]
            [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c>[default: \"./config.yaml\"] (undefined color)" ]
            [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c:>[default: \"./config.yaml\"] (undefined color)" ]
            [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c[default: \"./config.yaml\"] (incomplete tag)" ]
            [ "    --message";       "Some message" ]
            [ "    --parts";         "Required parts <c:yellow>[default: [\"foo\"; \"bar\"]]</c> <c:blue>(multiple values allowed)</c>" ]
        ]

        console.Title "Color Example"

        console.Section("Spectrum with %s and %s colors", "<c:dark-yellow>foreground</c>", "<c:black|bg:dark-yellow>background</c>")
        let spectrum = [
            "#124542", "a"
            "#185C58", "b"
            "#1E736E", "c"
            "#248A84", "d"
            "#20B2AA", "e"
            "#3FBDB6", "f"
            "#5EC8C2", "g"
            "#7DD3CE", "i"
            "#9CDEDA", "j"
            "#BBE9E6", "k"
        ]
        console.SubTitle("Spectrum with foreground")
        spectrum
        |> List.iter (fun (color, letter) -> console.Write("<c:%s>%s</c>", color, letter))
        |> console.NewLine
        |> console.NewLine

        console.SubTitle("Spectrum with background")
        spectrum
        |> List.iter (fun (color, letter) -> console.Write("<c:black|bg:%s>%s</c>", color, letter))
        |> console.NewLine
        |> console.NewLine

        [
            "yellow", [
                "light-yellow"
                "yellow"
                "dark-yellow"
            ]

            "orange", [
                "light-orange"
                "orange"
                "dark-orange"
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

            "cyan", [
                "Light-cyan"
                "cyan"
                "darkcyan"
            ]

            "blue", [
                "Light-Blue"
                "blue"
                "darkblue"
            ]

            "magenta", [
                "light-magenta"
                "magenta"
                "dark-magenta"
            ]

            "pink", [
                "light-pink"
                "pink"
                "dark-pink"
            ]

            "purple", [
                "light-purple"
                "purple"
                "dark-purple"
            ]

            "gray", [
                "lightgray"
                "gray"
                "darkGray"
            ]

            "black & white", [
                "black"
                "white"
            ]
        ]
        |> List.map (fun (colorGroup, colors) ->
            let colorize color = sprintf "<c:%s>%s</c>" color color

            [
                colorize colorGroup
                colors |> List.map colorize |> String.concat ", "
            ]
        )
        |> console.Table [ "Color Group"; "Colors" ]

        [ "red"; "green"; "yellow"; "blue"; "purple"; "orange"; "gray" ]
        |> List.map (fun color -> { Tab.parseColor color "Sample" with Value = Some color })
        |> console.Tabs

        [ "#ed1017"; "#67c355"; "#f3d22b"; "#1996f0"; "#9064cb"; "#ff9603"; "#babab8" ]
        |> List.map (fun color -> { Tab.parseColor color "Sample" with Value = Some color })
        |> fun tabs -> console.Tabs(tabs, 10)

        [
            "#ed1017", "#9e2e22"
            "#67c355", "#087a3f"
            "#f3d22b", "#faa727"
            "#1996f0", "#0278be"
            "#9064cb", "#6a3390"
            "#ff9603", "#faa727"
            "#babab8", "#333333"
        ]
        |> List.mapi (fun i (color, darker) -> {
            Tab.parseColor color (sprintf "<c:dark-gray|bg:%s|ub>Metric</c>" color)
                with Value = Some <| sprintf "<c:magenta|bg:%s> %02d </c><c:|bg:%s>%% </c>" darker (i * 10) darker
            }
        )
        |> fun tabs -> console.Tabs(tabs, 10)

        let colorSquare color =
            sprintf "<c:|bg:#%s>    </c>" color

        let colors = [
        //     1         2         3         4         5         6         7         8         9         10        11        12        13        14        15        16        17        18        19        20
            "ffc20f"; "faa727"; "f79333"; "f46f2c"; "f04d2d"; "cb3430"; "9e2e22"; "d71f43"; "ee2866"; "ef4b7e"; "ce1984"; "ad308c"; "ce4998"; "c8619e"; "bf71aa"; "c689a7"; "9a769c"; "976aad"; "804b9d"; "6a3390"
            "a5cf4f"; "8cc747"; "2faa4f"; "2d9848"; "087a3f"; "096232"; "037957"; "119b7a"; "23ae91"; "16baaf"; "00aea5"; "008886"; "07636e"; "0c4d77"; "0b6695"; "0278be"; "078dca"; "01a8dd"; "09b1e2"; "3fc8f4"
        ]

        colors
        |> List.map colorSquare
        |> List.splitInto 2
        |> List.collect (fun line -> [ line; line ])
        |> List.iter (String.concat "" >> console.WriteLine)
        |> console.NewLine

        let colorLetter (i: int) color =
            sprintf "<c:#%s> %s%s </c>" color (System.Convert.ToChar(i + 65) |> string) (System.Convert.ToChar(i + 97) |> string)

        colors
        |> List.splitInto 2
        |> List.map (List.mapi colorLetter)
        |> List.iter (String.concat "" >> console.WriteLine)
        |> console.NewLine

        console.Section "Custom tags"
        console.Message ("Now the <customTag>custom tags</customTag> example")
        console.Table [ "Tag"; "Value" ] [
            [ "service"; "<service>domain-context</service>" ]
            [ "name"; "<name>Jon Snow</name>" ]
        ]

        console.Error "Error message"
        console.Error "Error\nmessage"
        console.Error("Error %s", "<c:black|bg:magenta>in</c>-<c:white|bg:dark-cyan>style</c>")
        console.Error(String.concat "\n\n" ["error"; "with"; "multiple"; "lines"])

        console.Warning "Warning message"
        console.Warning "Warning\nmessage"
        console.Warning("Warning %s", "<c:black|bg:magenta>in</c>-<c:white|bg:dark-cyan>style</c>")

        console.Success "Done"
        console.Success "Done\non more lines"
        console.Success("Done %s", "<c:black|bg:magenta>in</c>-<c:white|bg:dark-cyan>style</c>")

        console.NewLine()

    let cases: string list = [
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

    let fileName fileType testType verbosity =
        let prefix =
            match testType with
            | Output -> "output"
            | ErrorOutput -> "error"

        sprintf "data/%s-%s-%s.txt" prefix fileType verbosity

    let expected testType = fileName "expected" testType >> readFile

    let normalizeTime line =
        let line = Regex.Replace(line, @"^\[\d{2}:\d{2}:\d{2}\]", "[__TIME__]")
        Regex.Replace(line, @"^\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\]", "[_____DATE_TIME_____]")

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

                let bufferContent =
                    match testType with
                    | Output -> bufferdOutput.Fetch()
                    | ErrorOutput -> bufferdOutput.FetchError()

                let actual = bufferContent |> console.RemoveMarkup
                let lines =
                    actual.Split "\n"
                    |> Seq.map normalizeTime
                    |> List.ofSeq

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

    [<Tests>]
    let errorOutputTest =
        cases
        |> testCases ErrorOutput
        |> testList "Check error output file"

    open Output.CombinedOutput.Operators

    [<Tests>]
    let combinedOutputTest =
        testList "Combined output" [
            testCase "should output to all outputs" <| fun _ ->
                let verbosity = Verbosity.Normal

                use buffer1 = new Output.BufferOutput(verbosity)
                use buffer2 = new Output.BufferOutput(verbosity)
                use buffer3 = new Output.BufferOutput(verbosity)

                let combined = buffer1 <+> buffer2 <+> buffer3
                let console = ConsoleStyle(combined)

                console.Write "Message"

                let outputs = [
                    buffer1.Fetch()
                    buffer2.Fetch()
                    buffer3.Fetch()
                ]

                let expected = [
                    "Message"
                    "Message"
                    "Message"
                ]

                Expect.equal outputs expected "Combined Buffered outputs should all have a message."
        ]

    [<Tests>]
    let noMarkupOutputTest =
        testList "No Markup output" [
            let style = Style.defaults

            testCase "should output to given output without *any* markup" <| fun _ ->
                let verbosity = Verbosity.Normal

                use buffer = new Output.BufferOutput(verbosity)
                let noMarkup = Output.NoMarkup(style, buffer)

                let console = ConsoleStyle(noMarkup)

                console.Write "<c:yellow>Message</c>"

                let outputs = buffer.Fetch()
                let expected = "Message"

                Expect.equal outputs expected "Buffered outputs should have a message without markup."

            testCase "should output to combined output without *any* markup" <| fun _ ->
                let verbosity = Verbosity.Normal

                use buffer1 = new Output.BufferOutput(verbosity)
                use buffer2 = new Output.BufferOutput(verbosity)
                use buffer3 = new Output.BufferOutput(verbosity)

                let combined = buffer1 <+> buffer2 <+> buffer3
                let noMarkup = Output.NoMarkup(style, combined)
                let console = ConsoleStyle(noMarkup)

                console.Write "<c:yellow>Message</c>"

                let outputs = [
                    buffer1.Fetch()
                    buffer2.Fetch()
                    buffer3.Fetch()
                ]

                let expected = [
                    "Message"
                    "Message"
                    "Message"
                ]

                Expect.equal outputs expected "Combined Buffered outputs should all have a message without markup."
        ]
