// Learn more about F# at http://fsharp.org

open MF.ConsoleStyle

[<EntryPoint>]
let main argv =
    Console.title "Example"

    Console.groupedOptions ":" "Available commands:" [
        "list", "Lists commands"
        "deployment:list", "Lists environment"
        "deployment:release", "Release a package"
        "debug:configuration", "Dumps configuration"
        "test:d", "D"
        "test:c", "C"
        "test:a", "a"
        "test:b", "B"
    ]

    Console.simpleOptions "Options:" [
        yield "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c:yellow>[default: \"./config.yaml\"]</c>"
        yield "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c:yellow>[default: \"./config.yaml\"] (missing end tag)"
        yield "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c>[default: \"./config.yaml\"] (undefined color)"
        yield "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c:>[default: \"./config.yaml\"] (undefined color)"
        yield "-c, --config=CONFIG", "Path to deploy <c:dark-blue>config</c> <c[default: \"./config.yaml\"] (incomplete tag)"
        yield "    --message", "Some message"
        yield "    --parts", "Required parts <c:yellow>[default: [\"foo\"; \"bar\"]]</c> <c:blue>(multiple values allowed)</c>"

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

    0
