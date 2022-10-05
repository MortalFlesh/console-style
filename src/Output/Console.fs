namespace MF.ConsoleStyle.Output

open System
open MF.ConsoleStyle

type ConsoleOutput (verbosity) =
    let mutable verbosity: Verbosity.Level = verbosity

    interface IOutput with
        // Verbosity
        member __.Verbosity
            with get() = verbosity
            and set (level) = verbosity <- level

        member _.IsQuiet() = Verbosity.isQuiet verbosity
        member _.IsNormal() = Verbosity.isNormal verbosity
        member _.IsVerbose() = Verbosity.isVerbose verbosity
        member _.IsVeryVerbose() = Verbosity.isVeryVerbose verbosity
        member _.IsDebug() = Verbosity.isDebug verbosity

        // Output
        member _.Write(message) =
            Console.Write(message)

        member _.WriteLine(message) =
            Console.WriteLine(message)

        member _.WriteError(message) =
            Console.Error.Write(message)

        member _.WriteErrorLine(message) =
            Console.Error.WriteLine(message)
