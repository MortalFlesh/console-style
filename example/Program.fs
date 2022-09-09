// Learn more about F# at http://fsharp.org

let orFail = function
    | Ok value -> value
    | Error error -> failwithf "Error: %A" error

open MF.ConsoleStyle

let showOldConsole() =
    (* Console.mainTitle "Serverless"
    Console.mainTitleX "DoIt"
    Console.mainTitleX "Backup" *)

    //let answer = Console.ask "What is your answer?"
    //Console.messagef "Thats <c:magenta>your</c> answer: <c:cyan>%A</c>" answer

    Console.messagef "Hello <c:red>%s</c>" "world"

    (* Console.title "Example" *)

    (* Console.groupedOptions ":" "Available commands:" [
        [ "list"; "Lists commands" ]
        [ "<c:blue>deployment:list</c>"; "Lists environment" ]
        [ "deployment:list"; "Release a package" ]
        [ "<c:blue>debug</c>:<c:dark-pink>configuration</c>"; "Dumps configuration" ]
        [ "<c:red>test</c>:<c:pink>d</c>"; "D" ]
        [ "<c:dark-red>test</c>:<c:yellow>c</c>"; "C" ]
        [ "<c:gray>test</c>:<c:purple>a</c>"; "a" ]
        [ "<c:white>test</c>:<c:black>b</c>"; "B" ]
    ] *)

    (* Console.simpleOptions "Options:" [
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c:yellow>[default: \"./config.yaml\"]</c>" ]
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c:yellow>[default: \"./config.yaml\"] (missing end tag)" ]
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c>[default: \"./config.yaml\"] (undefined color)" ]
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c:>[default: \"./config.yaml\"] (undefined color)" ]
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c[default: \"./config.yaml\"] (incomplete tag)" ]
        [ "    --message";       "Some message" ]
        [ "    --parts";         "Required parts <c:yellow>[default: [\"foo\"; \"bar\"]]</c> <c:blue>(multiple values allowed)</c>" ]
    ] *)

    (* let total = 10
    let progressBar = Console.progressStart "Starting..." total
    for _ in 1 .. total do
        System.Threading.Thread.Sleep 200
        progressBar |> Console.progressAdvance
    Console.progressFinish progressBar *)

    (* [
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
        |> Console.table [ "Color Group"; "Colors" ] *)

    Console.newLine()

let showNewConsole (console: ConsoleStyle) =
    (* console.MainTitle "ConsoleStyle"
    console.MainTitleFigle "Console"
    console.MainTitleFigle "Style" *)

    // let answer = console.Ask "What is your answer?"
    // console.Message("Thats <c:magenta>your</c> answer: <c:cyan>%A</c>", answer)

    console.Message("<c:|bg:yellow>Hello</c> <c:white|bg:#D20000>%s</c> <c:#DEB887FF>and</c> everyone <c:black|bg:#43ff64d9>else</c>.", "world")
    console.NewLine()

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

    (* console.GroupedOptions ":" "Available commands:" [
        [ "list"; "Lists commands" ]
        [ "<c:blue>deployment:list</c>"; "Lists environment" ]
        [ "deployment:list"; "Release a package" ]
        [ "<c:blue>debug</c>:<c:dark-pink>configuration</c>"; "Dumps configuration" ]
        [ "<c:red>test</c>:<c:pink>d</c>"; "D" ]
        [ "<c:dark-red>test</c>:<c:yellow>c</c>"; "C" ]
        [ "<c:gray>test</c>:<c:purple>a</c>"; "a" ]
        [ "<c:white>test</c>:<c:black>b</c>"; "B" ]
    ] *)

    (* console.SimpleOptions "Options:" [
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c:yellow>[default: \"./config.yaml\"]</c>" ]
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c:yellow>[default: \"./config.yaml\"] (missing end tag)" ]
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c>[default: \"./config.yaml\"] (undefined color)" ]
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c:>[default: \"./config.yaml\"] (undefined color)" ]
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c[default: \"./config.yaml\"] (incomplete tag)" ]
        [ "    --message";       "Some message" ]
        [ "    --parts";         "Required parts <c:yellow>[default: [\"foo\"; \"bar\"]]</c> <c:blue>(multiple values allowed)</c>" ]
    ] *)

    (* let total = 10
    let progressBar = console.ProgressStart "Starting..." total
    for _ in 1 .. total do
        System.Threading.Thread.Sleep 200
        progressBar |> console.ProgressAdvance
    console.ProgressFinish progressBar *)

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
    |> List.iter (String.concat "" >> console.Message)
    |> console.NewLine

    let colorLetter (i: int) color =
        sprintf "<c:#%s> %s%s </c>" color (System.Convert.ToChar(i + 65) |> string) (System.Convert.ToChar(i + 97) |> string)

    colors
    |> List.splitInto 2
    |> List.map (List.mapi colorLetter)
    |> List.iter (String.concat "" >> console.Message)
    |> console.NewLine

    console.Section "Custom tags"
    console.Message ("Now the <customTag>custom tags</customTag> example")
    console.Table [ "Tag"; "Value" ] [
        [ "service"; "<service>domain-context</service>" ]
        [ "name"; "<name>Jon Snow</name>" ]
    ]

    console.Success "Done"
    console.Success("Done %s", "<c:black|bg:magenta>in</c>-<c:white|bg:dark-cyan>style</c>")

    console.NewLine()

[<EntryPoint>]
let main argv =
    //showOldConsole()

    let consoleOutput =
        Output.ConsoleOutput(
            Verbosity.Normal
        )

    let style = {
        Underline = None    // todo - tohle musi byt zvlast pro kazdy nadpis
        Indentation = Some (Indentation "  ")
        ShowDateTime = None
        NewLine = None
        DateTimeFormat = None
        CustomTags = [
            CustomTag.createWithMarkup (TagName "customTag") {
                Bold = true
                Dim = true
                Italic = true
                Underline = true
                Reverse = true
                Strikethrough = true
                Foreground = Some "black"
                Background = Some "red"
            }
            CustomTag.createAndParseMarkup (TagName "service") ":#0b6695|bg:#f79333|u" |> orFail
            {
                Tag = TagName "name"
                Markup = MarkupString.create "#23ae91"
            }
        ]
    }

    let console = ConsoleStyle(consoleOutput, style)
    showNewConsole console

    0
