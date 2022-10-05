namespace MF.ConsoleStyle.Output

open System
open System.IO
open System.Text

open MF.ConsoleStyle

type StreamOutput (verbosity, stream: IO.Stream, errorStream: IO.Stream) =
    let mutable verbosity: Verbosity.Level = verbosity

    new (verbosity, stream) = new StreamOutput(verbosity, stream, stream)

    interface IDisposable with
        member _.Dispose () =
            stream.Dispose()

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
            stream.Write(Encoding.ASCII.GetBytes(message))

        member _.WriteLine(message) =
            stream.Write(Encoding.ASCII.GetBytes(message + "\n"))

        member _.WriteError(message) =
            errorStream.Write(Encoding.ASCII.GetBytes(message))

        member _.WriteErrorLine(message) =
            errorStream.Write(Encoding.ASCII.GetBytes(message + "\n"))
