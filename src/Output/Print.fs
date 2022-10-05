namespace MF.ConsoleStyle.Output

open MF.ConsoleStyle

type PrintOutput (verbosity) =
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
            printf "%s" message

        member _.WriteLine(message) =
            printfn "%s" message

        member _.WriteError(message) =
            eprintf "%s" message

        member _.WriteErrorLine(message) =
            eprintfn "%s" message
