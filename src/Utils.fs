namespace MF.ConsoleStyle

[<AutoOpen>]
module internal Regexp =
    open System.Text.RegularExpressions

    // http://www.fssnip.net/29/title/Regular-expression-active-pattern
    let (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern)
        if m.Success then Some (List.tail [ for g in m.Groups -> g.Value ])
        else None

[<RequireQualifiedAccess>]
module internal Hex =
    open System

    let fromInt (value: int) =
        value.ToString("X")

    let fromByte (value: byte) =
        value.ToString("X2")

    let parse value =
        Convert.ToInt32(value, 16)

[<AutoOpen>]
module internal Utils =
    let tee f a =
        f a
        a
