namespace MF.ConsoleStyle

[<AutoOpen>]
module internal Utils =
    let tee f a =
        f a
        a

type internal OutputType =
    | Title
    | SubTitle
    | TableHeader
    | Success
    | Error
    | Number
    | TextWithMarkup of string option

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
