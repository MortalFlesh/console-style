namespace ConsoleStyle.Tests.Output

module OutputTest =
    open System
    open System.IO
    open Expecto

    let readFile (file: string) =
        Path.Combine(Environment.CurrentDirectory, file)
        |> File.ReadAllLines
        |> Array.toList

    [<Tests>]
    let outputSingleTest =
        testList "Check file" [
            testCase "file" <| fun _ ->
                let result = readFile "actual.txt"
                
                readFile "expected.txt"
                |> List.iteri (fun i expectedLine ->
                    let result = result.[i]

                    Expect.equal result expectedLine ""
                )
        ]
