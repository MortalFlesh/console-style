// Learn more about F# at http://fsharp.org

open System
open MF.ConsoleStyle

[<EntryPoint>]
let main argv =
    Console.mainTitlef "Hello World from %s!" "F#"
    Console.titlef "Hello World from %s!" "F#"
    Console.sectionf "Hello World from %s!" "F#"
    Console.subTitlef "Hello World from %s!" "F#"
    Console.messagef "Hello World from %s!" "F#"
    Console.askf "Hello World from %s!" "F#" |> ignore
    Console.errorf "Hello World from %s!" "F#"
    Console.successf "Hello World from %s!" "F#"
    0 // return an integer exit code
