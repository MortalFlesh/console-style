namespace MF.ConsoleStyle.Output

open System
open MF.ConsoleStyle

type NoMarkup (style: Style, output: IOutput) =
    let removeMarkup = Style.removeMarkup style

    interface IOutput with
        // Verbosity
        member __.Verbosity
            with get() = output.Verbosity
            and set (level) = output.Verbosity <- level

        member _.IsQuiet() = output.IsQuiet()
        member _.IsNormal() = output.IsNormal()
        member _.IsVerbose() = output.IsVerbose()
        member _.IsVeryVerbose() = output.IsVeryVerbose()
        member _.IsDebug() = output.IsDebug()

        // Output
        member _.Write(message) =
            output.Write(removeMarkup message)

        member _.WriteLine(message) =
            output.WriteLine(removeMarkup message)

        member _.WriteError(message) =
            output.WriteError(removeMarkup message)

        member _.WriteErrorLine(message) =
            output.WriteErrorLine(removeMarkup message)
