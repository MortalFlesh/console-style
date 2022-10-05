namespace MF.ConsoleStyle

/// MarkupString could contain either full markup representation "<c:FOREGROUND|bg:BACKGROUND|MODIFIER>" or just a markup ":FOREGROUND|bg:BACKGROUND|MODIFIER"
type MarkupString = MarkupString of string

/// See https://misc.flogisoft.com/bash/tip_colors_and_formatting
type Markup = {
    /// Code: 1
    Bold: bool
    /// Code: 2
    Dim: bool
    /// Code: 3
    Italic: bool
    /// Code: 4
    Underline: bool
    // Code: 5 Blink: bool
    /// Code: 7
    Reverse: bool
    // Code: 8 Hidden: bool
    /// Code: 9
    Strikethrough: bool

    /// Could contain a RGB or RGBA hash starting with # or a color name
    Foreground: string option
    /// Could contain a RGB or RGBA hash starting with # or a color name
    Background: string option
}

type internal MessagePart = {
    Text: string
    Markup: Markup
}

[<RequireQualifiedAccess>]
module Markup =
    open System
    open System.Drawing
    open System.Text.RegularExpressions

    let empty = {
        Bold = false
        Dim = false
        Italic = false
        Underline = false
        Reverse = false
        Strikethrough = false
        Foreground = None
        Background = None
    }

    let hasMarkup (message: string) =
        message.Contains "<c:"

    let private parseModificators (modificators: string) markup =
        let modificators = modificators.ToLowerInvariant()

        { markup with
            Bold = modificators.Contains "b"
            Dim = modificators.Contains "d"
            Italic = modificators.Contains "i"
            Underline = modificators.Contains "u"
            Reverse = modificators.Contains "r"
            Strikethrough = modificators.Contains "s"
        }

    let parse: string -> Markup = function
        | Regex @"^:([^\|]+?)\|bg:([^\|]+?)\|(\w*)$" [ foreground; background; modificators ] ->
            { empty with Foreground = Some foreground; Background = Some background } |> parseModificators modificators

        | Regex @"^:([^\|]+?)\|bg:([^\|]+?)\|?$" [ foreground; background ] -> { empty with Foreground = Some foreground; Background = Some background }

        | Regex @"^:([^\|]+?)\|bg:\|(\w*)$" [ foreground; modificators ]
        | Regex @"^:([^\|]+?)\|{1,2}(\w*)$" [ foreground; modificators ] -> { empty with Foreground = Some foreground } |> parseModificators modificators

        | Regex @"^:([^\|]+?)\|bg:\|?$" [ foreground ]
        | Regex @"^:([^\|]+?)$" [ foreground ] -> { empty with Foreground = Some foreground }

        | Regex @"^:\|bg:([^\|]+?)\|(\w*)$" [ background; modificators ] -> { empty with Background = Some background } |> parseModificators modificators

        | Regex @"^:\|bg:([^\|]+?)\|?$" [ background ] -> { empty with Background = Some background }

        | Regex @"^:\|bg:\|(\w*)$" [ modificators ]
        | Regex @"^:\|{1,2}(\w*)$" [ modificators ] -> empty |> parseModificators modificators
        | _ -> empty

    let asString: Markup -> string = fun markup ->
        [
            match markup.Foreground with
            | Some foreground -> $":{foreground}"
            | _ -> ":"

            match markup.Background with
            | Some background -> $"bg:{background}"
            | _ -> ()

            let modifiers = [
                if markup.Bold then "b"
                if markup.Dim then "d"
                if markup.Italic then "i"
                if markup.Underline then "u"
                if markup.Reverse then "r"
                if markup.Strikethrough then "s"
            ]

            match modifiers with
            | [] -> ()
            | modifiers -> modifiers |> String.concat ""
        ]
        |> String.concat "|"

    [<RequireQualifiedAccess>]
    module private MessagePart =
        let ofText text = { Text = text; Markup = empty }
        let text ({ Text = text }: MessagePart) = text

    type private MessageParts = MessagePart list

    module internal Bash =
        /// See https://github.com/silkfire/Pastel/blob/master/src/ConsoleExtensions.cs#L42
        let [<Literal>] private FormatStart = "\u001b"
        let [<Literal>] private FormatEnd = "\u001b[0m"
        let [<Literal>] private ForegroundPrefix = 3
        let [<Literal>] private BackgroundPrefix = 4

        /// See Symfony\Component\Console\Color::convertHexColorToAnsi
        let private colorToAnsi (color: Color) =
            sprintf "8;2;%d;%d;%d"
                color.R
                color.G
                color.B

        let private parseColor (prefix: int) color =
            $"{prefix}{color |> colorToAnsi}"

        let private formatMarkup (markup: Markup) =
            [
                if markup.Bold then "1"
                if markup.Dim then "2"
                if markup.Italic then "3"
                if markup.Underline then "4"
                if markup.Reverse then "7"
                if markup.Strikethrough then "9"

                match markup.Foreground |> Color.parse with
                | Some foreground -> foreground |> parseColor ForegroundPrefix
                | _ -> ()

                match markup.Background |> Color.parse with
                | Some background -> background |> parseColor BackgroundPrefix
                | _ -> ()
            ]

        let formatWithMarkup markup =
            sprintf "%s[%sm" FormatStart markup

        let makeMarkupVisible (value: string) =
            value.Replace(FormatStart, @"\u001b")

        let removeMarkup value =
            Regex.Replace(value, @"\u001b\[.+?m", "")

        let formatPart: MessagePart -> string = function
            | { Text = null } | { Text = "" } -> ""
            | part ->
                match part.Markup |> formatMarkup with
                | [] -> part.Text
                | set ->
                    sprintf "%s%s%s"
                        (set |> String.concat ";" |> formatWithMarkup)
                        part.Text
                        FormatEnd

    let private addNotEmptyPart (parts: MessageParts) part =
        if part.Text <> ""
        then part :: parts
        else parts

    let internal parseMarkup (message: string): MessageParts =
        let rec parseMarkup (parts: MessageParts) (message: string) =
            if message |> hasMarkup then
                match message.Split("<c", 2) with
                | [| before; withMarkup |] ->
                    let parts = before |> MessagePart.ofText |> addNotEmptyPart parts

                    let ({ Text = message }: MessagePart as part) =
                        match withMarkup.Split(">", 2) with
                        | [| markupValue; text |] ->
                            let markup = markupValue |> parse

                            { Text = text; Markup = markup }
                        | _ -> message |> MessagePart.ofText

                    match message.Split("</c>", 2) with
                    | [| text; rest |] ->
                        let texts = { part with Text = text } |> addNotEmptyPart parts

                        if rest |> hasMarkup then parseMarkup texts rest
                        else rest |> MessagePart.ofText|> addNotEmptyPart texts

                    | _ -> message |> MessagePart.ofText |> addNotEmptyPart parts
                | _ -> message |> MessagePart.ofText |> addNotEmptyPart parts
            else message |> MessagePart.ofText |> addNotEmptyPart parts

        message
        |> parseMarkup []
        |> List.rev

    let private (|HasMarkup|_|) = function
        | message when message |> hasMarkup -> message |> parseMarkup |> Some
        | _ -> None

    let removeMarkup = Bash.removeMarkup << function
        | HasMarkup texts -> texts |> List.map MessagePart.text |> String.concat ""
        | text -> text

    //
    // Just render as string
    //

    let render message =
        let rec renderMarkup acc: MessageParts -> string = function
            | [] -> acc
            | { Text = text; Markup = markup } :: others when markup = empty -> others |> renderMarkup (acc + text)
            | part :: others ->
                let formattedText = Bash.formatPart part
                others |> renderMarkup (acc + formattedText)

        message
        |> parseMarkup
        |> renderMarkup ""

[<RequireQualifiedAccess>]
module MarkupString =
    let create: string -> MarkupString = function
        | markup when markup.StartsWith ":" -> MarkupString markup
        | markup when markup.StartsWith "<c:" -> MarkupString markup
        | markup -> MarkupString $":{markup}"

    let internal value (MarkupString markup) =
        if markup.StartsWith "<c:" then markup
        else $"<c{markup}>"
