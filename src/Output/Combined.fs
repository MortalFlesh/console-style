namespace MF.ConsoleStyle.Output

open System
open MF.ConsoleStyle

type internal CombinedOutput(outputs: IOutput list) =
    let mutable verbosity: Verbosity.Level = (outputs |> List.head).Verbosity

    member _.Outputs = outputs

    interface IOutput with
        // Verbosity
        member __.Verbosity
            with get() = verbosity
            and set (level) =
                outputs
                |> List.iter (fun output -> output.Verbosity <- verbosity)

                verbosity <- level

        member _.IsQuiet() = Verbosity.isQuiet verbosity
        member _.IsNormal() = Verbosity.isNormal verbosity
        member _.IsVerbose() = Verbosity.isVerbose verbosity
        member _.IsVeryVerbose() = Verbosity.isVeryVerbose verbosity
        member _.IsDebug() = Verbosity.isDebug verbosity

        // Output
        member _.Write(message) =
            outputs |> List.iter (fun output -> output.Write(message))

        member _.WriteLine(message) =
            outputs |> List.iter (fun output -> output.WriteLine(message))

        member _.WriteError(message) =
            outputs |> List.iter (fun output -> output.WriteError(message))

        member _.WriteErrorLine(message) =
            outputs |> List.iter (fun output -> output.WriteErrorLine(message))

[<RequireQualifiedAccess>]
module CombinedOutput =
    let combine (aO: IOutput) (bO: IOutput): IOutput =
        if aO.Verbosity <> bO.Verbosity then
            failwithf "Both outputs must have the same level of verbosity."

        let outputs: IOutput -> IOutput list = function
            | :? CombinedOutput as combined -> combined.Outputs
            | output -> [ output ]

        new CombinedOutput(outputs aO @ outputs bO)

    module Operators =
        let (<+>) aO bO = combine aO bO
