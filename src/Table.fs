namespace MF.ConsoleStyle

[<RequireQualifiedAccess>]
module private Table =
    open Words
    open WordLength.Operators

    type private Column = Column of Word

    [<RequireQualifiedAccess>]
    module private Column =
        let word (Column word) = word
        let value = word >> Word.value

    type private RowLine = RowLine of string

    [<RequireQualifiedAccess>]
    module private RowLine =
        let ofColumns columns = columns |> List.map Column.value |> String.concat " " |> RowLine
        let toColumns (RowLine line) = line.Split " " |> Array.toList |> List.map (Word >> Column)

        let value (RowLine line) = line

    type private Row =
        | Line of RowLine
        | Columns of Column list

    [<RequireQualifiedAccess>]
    module private Row =
        let line = function
            | Line line -> line
            | Columns columns -> columns |> RowLine.ofColumns

        let columns = function
            | Line line -> line |> RowLine.toColumns
            | Columns columns -> columns

        let toWords = columns >> List.map Column.word

        let format removeMarkup maxWordLengths = function
            | Line line -> line
            | Columns columns ->
                columns
                |> List.map Column.word
                |> Line.format removeMarkup (fun realColumnLength -> sprintf " %-*s" (realColumnLength - 1)) maxWordLengths
                |> List.map Column
                |> RowLine.ofColumns

    [<RequireQualifiedAccess>]
    type private Header =
        | Empty
        | Row of Row

    [<RequireQualifiedAccess>]
    module private Header =
        let render renderSeparatorRow renderRow = function
            | Header.Empty -> ()
            | Header.Row headerRow ->
                headerRow
                |> renderRow
                |> tee renderSeparatorRow

    [<RequireQualifiedAccess>]
    type private Rows =
        | Empty
        | Rows of Row list

    [<RequireQualifiedAccess>]
    module private Rows =
        let rows = function
            | Rows.Empty -> []
            | Rows.Rows rows -> rows

        let toLines = rows >> List.map Row.toWords

        let render renderSeparatorRow renderRow = function
            | Rows.Empty -> ()
            | Rows.Rows rows ->
                rows
                |> List.iter renderRow
                |> tee renderSeparatorRow

    type private Separator = Separator of char

    [<RequireQualifiedAccess>]
    module private Separator =
        let renderRow renderLine (MaxWordLengths wordLengths) (Separator separator) =
            let createSeparatorForWord (WordLength wordLength) =
                separator
                |> string
                |> String.replicate wordLength
                |> Word
                |> Column

            wordLengths
            |> Map.toList
            |> List.map (snd >> createSeparatorForWord)
            |> Columns
            |> Row.line
            |> renderLine

    let render
        (renderHeaderLine: string -> unit)
        (renderRowLine: string -> unit)
        (header: string list)
        (rows: (string list) list) =

        let removeMarkup = Markup.removeMarkup

        let formatRow maxWordLengths = Row.format removeMarkup maxWordLengths >> RowLine.value

        let renderHeaderRow maxWordLengths = formatRow maxWordLengths >> renderHeaderLine
        let renderRow maxWordLengths = formatRow maxWordLengths >> renderRowLine
        let renderLine = RowLine.value >> renderRowLine

        let renderSeparatorRow wordLengths =
            Separator '-'
            |> Separator.renderRow renderLine wordLengths

        let getMaxWordLengthsPerColumn rows =
            rows
            |> Rows.toLines
            |> MaxWordLengths.perWordsInLine removeMarkup
            |> MaxWordLengths.map ((<+>) (WordLength 2))    // +1 space for both sides of the word ` word `

        let header =
            match header with
            | [] -> Header.Empty
            | words ->
                words
                |> List.map (removeMarkup >> Word >> Column)
                |> Columns
                |> Header.Row

        let rows =
            match rows with
            | [] -> Rows.Empty
            | rows ->
                rows
                |> List.map (List.map (Word >> Column) >> Columns)
                |> Rows.Rows

        let wordMaxLengths =
            match header, rows with
            | Header.Row headerRow, Rows.Rows rows -> Rows.Rows (headerRow :: rows)
            | Header.Row headerRow, Rows.Empty -> Rows.Rows [ headerRow ]
            | Header.Empty, Rows.Rows _ -> rows
            | _ -> Rows.Empty
            |> getMaxWordLengthsPerColumn

        let renderSeparatorRow () = renderSeparatorRow wordMaxLengths
        let renderHeader = Header.render renderSeparatorRow (renderHeaderRow wordMaxLengths)
        let renderRows = Rows.render renderSeparatorRow (renderRow wordMaxLengths)

        match header, rows with
        | Header.Empty, Rows.Empty -> ()
        | header, rows ->
            renderSeparatorRow()
            header |> renderHeader
            rows |> renderRows
