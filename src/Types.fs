namespace MF.ConsoleStyle

open System.Drawing

[<AutoOpen>]
module internal Utils =
    let tee f a =
        f a
        a

type Message = {
    Text: string
    Length: int
    LengthWithoutMarkup: int
    HasMarkup: bool
}
type RenderedMessage = RenderedMessage of string

[<RequireQualifiedAccess>]
module internal Message =
    let value ({ Text = text }: Message) = text
    let empty = {
        Text = ""
        Length = 0
        HasMarkup = false
        LengthWithoutMarkup = 0
    }

[<RequireQualifiedAccess>]
module internal RenderedMessage =
    let empty = RenderedMessage ""
    let value (RenderedMessage value) = value

type OutputType =
    | Title
    | SubTitle
    | Section
    | TableHeader
    | Success
    | Error
    | Number
    | TextWithMarkup
    | OnLine

[<RequireQualifiedAccess>]
module internal Color =
    let normalize (string: string) =
        string.ToLower().Trim().Replace("_", "").Replace("-", "")

    let hash (color: Color) =
        sprintf "#%s%s%s"
            (color.R |> Hex.fromByte)
            (color.G |> Hex.fromByte)
            (color.B |> Hex.fromByte)

    let (|RGBA|_|) = function
        | (Regex @"^#([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})$" [ r; g; b; a ]) ->
            try
                Color.FromArgb(
                    a |> Hex.parse,
                    r |> Hex.parse,
                    g |> Hex.parse,
                    b |> Hex.parse
                )
                |> Some
            with _ -> None
        | _ -> None

    let (|RGB|_|) = function
        | (Regex @"^#([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})$" [ r; g; b ]) ->
            try
                Color.FromArgb(
                    r |> Hex.parse,
                    g |> Hex.parse,
                    b |> Hex.parse
                )
                |> Some
            with _ -> None
        | _ -> None

    let parse color =
        match color |> Option.map normalize with
        | Some (RGBA rgbColor)
        | Some (RGB rgbColor) -> Some rgbColor

        | Some "lightyellow"
        | Some "yellow" -> Some Color.Yellow
        | Some "darkyellow" -> Some Color.DarkGoldenrod

        | Some "lightorange" -> Some <| Color.FromArgb(0xFA, 0xA7, 0x27)
        | Some "orange" -> Some <| Color.FromArgb(0xFF, 0x96, 0x03)
        | Some "darkorange" -> Some <| Color.FromArgb(0xF7, 0x93, 0x33)

        | Some "lightred"
        | Some "red" -> Some Color.Red
        | Some "darkred" -> Some Color.DarkRed

        | Some "lightgreen"
        | Some "green" -> Some Color.LimeGreen
        | Some "darkgreen" -> Some Color.DarkGreen

        | Some "lightcyan"
        | Some "cyan"
        | Some "lightblue" -> Some Color.Cyan
        | Some "darkcyan"
        | Some "blue" -> Some Color.DarkCyan
        | Some "darkblue" -> Some Color.MidnightBlue

        | Some "lightpink"
        | Some "pink"
        | Some "lightmagenta"
        | Some "magenta" -> Some Color.Magenta
        | Some "darkpink"
        | Some "darkmagenta"
        | Some "purple" -> Some Color.Purple

        | Some "lightgray" -> Some Color.LightGray
        | Some "gray"
        | Some "darkgray"-> Some Color.Silver

        | Some "black" -> Some Color.Black

        | Some "white" -> Some Color.White
        | _ -> None

[<RequireQualifiedAccess>]
module internal OutputType =
    [<System.Obsolete("It should not be necessary anymore and color is defined elsewhere")>]
    let color = function
        | Title -> Color.Cyan
        | SubTitle -> Color.Yellow
        | Section -> Color.DarkGoldenrod
        | TableHeader -> Color.DarkGoldenrod
        | Success -> Color.LimeGreen
        | Error -> Color.Red
        | Number -> Color.Magenta
        | TextWithMarkup -> Color.White
        | OnLine -> Color.White

    let formatTitle = sprintf "<c:cyan>%s</c>"
    let formatSubTitle = sprintf "<c:yellow>%s</c>"
    let formatSection = sprintf "<c:dark-yellow>%s</c>"
    let formatTableHeader = sprintf "<c:dark-yellow>%s</c>"
    let formatSuccess = sprintf "<c:black|bg:green>%s</c>"

module internal Words =
    type WordLength = WordLength of int

    [<RequireQualifiedAccess>]
    module WordLength =
        module Operators =
            let inline (<+>) (WordLength a) (WordLength b) = WordLength (a + b)
            let inline (<->) (WordLength a) (WordLength b) = WordLength (a - b)

    type Word = Word of string

    [<RequireQualifiedAccess>]
    module Word =
        open WordLength.Operators

        let value (Word word) = word
        let length (Word word) = WordLength (word.Length)

        let lengthWithoutMarkup removeMarkup (Word word) =
            word
            |> removeMarkup
            |> String.length
            |> WordLength

        let markupLength removeMarkup word =
            let lengthWithoutMarkup = word |> lengthWithoutMarkup removeMarkup
            let wordLength = word |> length
            wordLength <-> lengthWithoutMarkup

    type WordIndex = WordIndex of int

    type Line = Word list
    type MaxWordLengths = MaxWordLengths of Map<WordIndex, WordLength>

    [<RequireQualifiedAccess>]
    module Line =
        let concat separator (words: Line) =
            words
            |> List.map Word.value
            |> String.concat separator  // todo - tohle se pouziva jen v options, tam by chtela ale cusstom, ktera prvni da jednu mezeru a pak mezi vsechno krom prefixu 2 mezery

        let format removeMarkup formatWord (MaxWordLengths maxWordLengths) (words: Line) =
            words
            |> List.mapi (fun index word ->
                let (WordLength columnLength) = maxWordLengths |> Map.find (WordIndex index)
                let markupLength = word |> Word.markupLength removeMarkup

                let realWordLength =
                    match markupLength with
                    | (WordLength markupLength) when markupLength > 0 -> columnLength + markupLength
                    | _ -> columnLength

                word
                |> Word.value
                |> formatWord realWordLength
                |> Word
            )

        let wordLengths removeMarkup (words: Line) =
            words
            |> List.mapi (fun wordIndex word ->
                WordIndex wordIndex,
                word |> Word.lengthWithoutMarkup removeMarkup
            )

        let max (MaxWordLengths maxWordLengths) =
            maxWordLengths
            |> Map.toList
            |> List.map snd
            |> List.max

    [<RequireQualifiedAccess>]
    module MaxWordLengths =
        let perWordsInLine removeMarkup lines =
            let rec findMaxLengths maxLengths = function
                | [] -> MaxWordLengths maxLengths
                | (wordIndex, wordLength) :: words ->
                    let maxLengthForColumn =
                        match maxLengths |> Map.tryFind wordIndex with
                        | Some currentMaxWordLength -> max currentMaxWordLength wordLength
                        | _ -> wordLength

                    words |> findMaxLengths (maxLengths.Add (wordIndex, maxLengthForColumn))

            lines
            |> List.collect (Line.wordLengths removeMarkup)
            |> findMaxLengths Map.empty

        let map f (MaxWordLengths maxWordLengths) =
            maxWordLengths
            |> Map.map (fun _ -> f)
            |> MaxWordLengths

type Underline = Underline of string
type Indentation = Indentation of string
type ShowDateTime =
    | ShowDateTime
    | ShowDateTimeAs of format: string
    | ShowDateTimeFrom of Verbosity.Level
    | ShowDateTimeFromAs of Verbosity.Level * format: string

type AddNewLine =
    | NewLine
    | NewLines of int

type Style = {
    Underline: Underline option
    Indentation: Indentation option
    ShowDateTime: ShowDateTime option
    NewLine: AddNewLine option
    DateTimeFormat: string option
}

[<RequireQualifiedAccess>]
module internal Style =
    let empty = {
        Underline = None
        Indentation = None
        ShowDateTime = None
        NewLine = None
        DateTimeFormat = None
    }

    let (|IsIndentated|_|) = function
        | { Indentation = Some (Indentation value) } -> Some value
        | _ -> None

    let (|HasShowDateTime|_|) = function
        | { ShowDateTime = Some showDateTime } -> Some showDateTime
        | _ -> None

    let (|ShowUnderline|_|) = function
        | { Underline = Some (Underline style) } -> Some style
        | _ -> None

    let (|HasNewLine|_|) = function
        | { NewLine = Some newLine } -> Some newLine
        | _ -> None
