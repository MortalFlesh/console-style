// Learn more about F# at http://fsharp.org

open System
open MF.ConsoleStyle

[<EntryPoint>]
let main argv =
    
    Console.table ["FirstName"; "Surname"] [
        ["Jon"; "Snow"]
        ["Peter"; "Parker"]
    ]

    Console.messages "prefix" ["line 1"; "line 2"]

    Console.options "Foo options" [("first", "Description of the 1st"); ("second", "Description of the 2nd")]

    Console.list ["line 1"; "line 2"]

    Console.mainTitle "ConsoleStyle"
    Console.mainTitlef "Hello World from %s!" "F#"
    Console.titlef "Hello World from %s!" "F#"
    Console.sectionf "Hello World from %s!" "F#"
    Console.subTitlef "Hello World from %s!" "F#"
    Console.messagef "Hello World from %s!" "F#"
    Console.ask "What's your name?" |> ignore
    Console.errorf "Hello World from %s!" "F#"
    Console.successf "Hello World from %s!" "F#"

    let total = 10

    let progressBar = Console.progressStart "Starting..." total

    for _ in 1 .. total do
        progressBar.Tick()

    Console.progressFinish progressBar

    0 // return an integer exit code
