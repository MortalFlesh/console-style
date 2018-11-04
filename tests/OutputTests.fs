namespace ConsoleStyle.Tests.Output

module OutputTest =
    open System
    open System.IO
    open Expecto
    open MF.ConsoleStyle

    let readLines (filePath: string) = seq {
        use reader = new StreamReader (filePath)
        while not reader.EndOfStream do
            yield reader.ReadLine ()
    }

    let readFile (file: string) =
        Path.Combine(Environment.CurrentDirectory, file)
        |> readLines
        |> List.ofSeq

    [<Tests>]
    let outputSingleTest =
        testList "Check file" [
            testCase "file" <| fun _ ->
                let result = readFile "actual.txt"
                
                readFile "expected.txt"
                |> List.ofSeq
                |> List.iteri (fun i expectedLine ->
                    let result = result.[i]

                    Expect.equal result expectedLine ""
                )
        ]
