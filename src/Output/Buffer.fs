namespace MF.ConsoleStyle.Output

open System
open MF.ConsoleStyle

type BufferOutput (verbosity) =
    let mutable verbosity: Verbosity.Level = verbosity
    let mutable buffer = ""
    let mutable errorBuffer = ""

    member _.Fetch () =
        let value = buffer
        buffer <- ""
        value

    member _.FetchError () =
        let value = errorBuffer
        errorBuffer <- ""
        value

    interface IDisposable with
        member _.Dispose () =
            buffer <- ""
            errorBuffer <- ""

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
            buffer <- buffer + message

        member _.WriteLine(message) =
            buffer <- buffer + message + "\n"

        member _.WriteError(message) =
            errorBuffer <- errorBuffer + message

        member _.WriteErrorLine(message) =
            errorBuffer <- errorBuffer + message + "\n"
