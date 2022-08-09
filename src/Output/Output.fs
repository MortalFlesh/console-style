namespace MF.ConsoleStyle.Output

open MF.ConsoleStyle
open System.Drawing

type Options = {
    Style: Style
    OutputType: OutputType
}

type IOutput =
    // Verbosity
    abstract member Verbosity: Verbosity.Level with get, set
    abstract member IsQuiet: unit -> bool
    abstract member IsNormal: unit -> bool
    abstract member IsVerbose: unit -> bool
    abstract member IsVeryVerbose: unit -> bool
    abstract member IsDebug: unit -> bool

    // Output
    abstract member Write: string -> unit
    abstract member WriteLine: string -> unit

    abstract member WriteError: string -> unit
    abstract member WriteErrorLine: string -> unit

    // abstract member WriteBig: Style -> string -> unit
