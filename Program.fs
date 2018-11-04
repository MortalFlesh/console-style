// Learn more about F# at http://fsharp.org

open System
open MF.ConsoleStyle

let example () =
    Console.messages "prefix" ["line 1"; "line 2"]

    Console.options "Foo options" [("first", "Description of the 1st"); ("second", "Description of the 2nd")]
    Console.optionsf "%s options" "foo" [("first", "desc 1"); ("second", "desc 2")]

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

[<EntryPoint>]
let main argv =
    // todo
    // - remove This entrypoint
    // - add tests

    Console.table ["FirstName"; "Surname"] [
        ["Jon"; "Snow"]
        ["Peter"; "Parker"]
    ]
    
    Console.table [] [
        ["Jon"; "Snow"]
        ["Peter"; "Parker"]
    ]
    
    Console.table ["FirstName"; "Surname"] []
    
    Console.table [] []

    0 // return an integer exit code
