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
        yield "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c:yellow>[default: \"./config.yaml\"]</c>"
        yield "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c:yellow>[default: \"./config.yaml\"] (missing end tag)"
        yield "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c>[default: \"./config.yaml\"] (undefined color)"
        yield "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c:>[default: \"./config.yaml\"] (undefined color)"
        yield "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c[default: \"./config.yaml\"] (incomplete tag)"
        yield "    --message",       "Some message"
        yield "    --parts",         "Required parts <c:yellow>[default: [\"foo\"; \"bar\"]]</c> <c:blue>(multiple values allowed)</c>"

        yield! [
            "lightyellow"
            "yellow"
            "darkyellow"

            "lightred"
            "red"
            "darkred"

            "lightgreen"
            "green"
            "darkgreen"

            "Light-cyan"
            "Light-Blue"
            "blue"
            "darkcyan"
            "darkblue"

            "pink"
            "dark-pink"

            "lightgray"
            "gray"
            "darkGray"

            "black"
            "white"
        ]
        |> List.map (fun c -> "    " + c, sprintf "colored <c:%s>message</c>!" c)
    ]

    let total = 10
    let progressBar = Console.progressStart "Starting..." total
    for _ in 1 .. total do
        System.Threading.Thread.Sleep 500
        progressBar |> Console.progressAdvance
    Console.progressFinish progressBar

    Console.newLine()

    0
