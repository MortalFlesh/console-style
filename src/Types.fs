namespace MF.ConsoleStyle

open System.Drawing

type internal Message = {
    Text: string
    Length: int
    Lines: int
    LengthWithoutMarkup: int
    HasMarkup: bool
}
type internal RenderedMessage = RenderedMessage of string

[<RequireQualifiedAccess>]
module internal Message =
    let value ({ Text = text }: Message) = text
    let empty = {
        Text = ""
        Length = 0
        Lines = 0
        HasMarkup = false
        LengthWithoutMarkup = 0
    }

[<RequireQualifiedAccess>]
module internal RenderedMessage =
    let empty = RenderedMessage ""
    let value (RenderedMessage value) = value

[<RequireQualifiedAccess>]
type OutputType =
    | MainTitle
    | Title
    | SubTitle
    | Section
    | TableHeader
    | Success
    | Error
    | Warning
    | Note
    | TextWithMarkup

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

        | Some "lightyellow" -> Some <| Color.FromArgb(0xFFFCBB)
        | Some "yellow" -> Some Color.Yellow
        | Some "darkyellow" -> Some <| Color.FromArgb(0xFFC20F)

        | Some "lightorange" -> Some <| Color.FromArgb(0xFAA727)
        | Some "orange" -> Some <| Color.FromArgb(0xFF9603)
        | Some "darkorange" -> Some <| Color.FromArgb(0xF79333)

        | Some "lightred" -> Some <| Color.FromArgb(0xFFCCCB)
        | Some "red" -> Some Color.Red
        | Some "darkred" -> Some Color.DarkRed

        | Some "lightgreen" -> Some <| Color.FromArgb(0xA5CF4F)
        | Some "green" -> Some Color.LimeGreen
        | Some "darkgreen" -> Some Color.DarkGreen

        | Some "lightcyan" -> Some Color.LightCyan
        | Some "cyan" -> Some Color.Cyan
        | Some "darkcyan" -> Some Color.DarkCyan

        | Some "lightblue" -> Some <| Color.FromArgb(0x3FC8F4)
        | Some "blue" -> Some <| Color.FromArgb(0x01A8DD)
        | Some "darkblue" -> Some <| Color.FromArgb(0x0278BE)

        | Some "lightmagenta" -> Some <| Color.FromArgb(0xFF80FF)
        | Some "magenta" -> Some Color.Magenta
        | Some "darkmagenta" -> Some Color.DarkMagenta

        | Some "lightpink" -> Some Color.LightPink
        | Some "pink" -> Some Color.HotPink
        | Some "darkpink" -> Some <| Color.FromArgb(0xCE4998)

        | Some "lightpurple" -> Some Color.MediumPurple
        | Some "purple" -> Some <| Color.FromArgb(0x804B9D)
        | Some "darkpurple" -> Some <| Color.FromArgb(0x6A3390)

        | Some "lightgray" -> Some Color.LightGray
        | Some "gray" -> Some <| Color.FromArgb(0x999999)
        | Some "darkgray" -> Some <| Color.FromArgb(0x333333)

        | Some "black" -> Some Color.Black

        | Some "white" -> Some Color.White
        | _ -> None

[<RequireQualifiedAccess>]
module internal OutputType =
    let formatDateTime = sprintf "<c:gray>%s</c>"
    let formatMainTitle = sprintf "<c:cyan>%s</c>"
    let formatTitle = sprintf "<c:cyan|b>%s</c>"
    let formatSubTitle = sprintf "<c:yellow|u>%s</c>"
    let formatSection = sprintf "<c:dark-yellow|b>%s</c>"
    let formatTableHeader = sprintf "<c:dark-yellow>%s</c>"
    let formatError = sprintf "<c:white|bg:red>%s</c>"
    let formatSimpleError = sprintf "<c:red>%s</c>"
    let formatSuccess = sprintf "<c:black|bg:green>%s</c>"
    let formatWarning = sprintf "<c:black|bg:light-orange>%s</c>"
    let formatNote = sprintf "<c:dark-yellow|i> ! [NOTE] %s</c>"

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
            |> String.concat separator

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
