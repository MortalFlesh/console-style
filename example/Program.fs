// Learn more about F# at http://fsharp.org

open MF.ConsoleStyle

[<EntryPoint>]
let main argv =
    Console.title "Example"

    Console.groupedOptions ":" "Available commands:" [
        "list", "Lists commands"
        "<c:blue>deployment:list</c>", "Lists environment"
        "deployment:list", "Release a package"
        "<c:blue>debug</c>:<c:dark-pink>configuration</c>", "Dumps configuration"
        "<c:red>test</c>:<c:pink>d</c>", "D"
        "<c:dark-red>test</c>:<c:yellow>c</c>", "C"
        "<c:gray>test</c>:<c:purple>a</c>", "a"
        "<c:white>test</c>:<c:black>b</c>", "B"
    ]

    Console.simpleOptions "Options:" [
        "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c:yellow>[default: \"./config.yaml\"]</c>"
        "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c:yellow>[default: \"./config.yaml\"] (missing end tag)"
        "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c>[default: \"./config.yaml\"] (undefined color)"
        "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c:>[default: \"./config.yaml\"] (undefined color)"
        "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c[default: \"./config.yaml\"] (incomplete tag)"
        "    --message",       "Some message"
        "    --parts",         "Required parts <c:yellow>[default: [\"foo\"; \"bar\"]]</c> <c:blue>(multiple values allowed)</c>"
    ]

    let total = 10
    let progressBar = Console.progressStart "Starting..." total
    for _ in 1 .. total do
        System.Threading.Thread.Sleep 200
        progressBar |> Console.progressAdvance
    Console.progressFinish progressBar

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
        |> Console.table [ "Color Group"; "Colors" ]

    Console.newLine()

    0
