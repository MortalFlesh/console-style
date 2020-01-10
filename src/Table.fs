namespace MF.ConsoleStyle

[<RequireQualifiedAccess>]
module private Table =
    let private tee f a =
        f a
        a

    type private WordLength = WordLength of int

    [<AutoOpen>]
    module private WordLengthOperators =
        let inline (<+>) (WordLength a) (WordLength b) = WordLength (a + b)
        let inline (<->) (WordLength a) (WordLength b) = WordLength (a - b)

    type private ColumnIndex = ColumnIndex of int

    type private MaxWordLengths = MaxWordLengths of Map<ColumnIndex, WordLength>

    type private Column = Column of string

    [<RequireQualifiedAccess>]
    module private Column =
        let value (Column column) = column
        let length removeMarkup (Column column) = WordLength (column |> removeMarkup |> String.length)

    type private RowLine = RowLine of string

    [<RequireQualifiedAccess>]
    module private RowLine =
        let ofColumns columns = columns |> List.map Column.value |> String.concat " " |> RowLine
        let toColumns (RowLine line) = line.Split " " |> Array.toList |> List.map Column

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

        let columnsLengths removeMarkup = columns >> List.mapi (fun columnIndex column -> (ColumnIndex columnIndex, column |> Column.length removeMarkup))

        let format removeMarkup (MaxWordLengths maxWordLengths) = function
            | Line line -> line
            | Columns columns ->
                columns
                |> List.mapi (fun index (Column word) ->
                    let (WordLength columnLength) = maxWordLengths |> Map.find (ColumnIndex index)

                    let markupLength =
                        let lengthWithoutMarkup = word |> removeMarkup |> String.length
                        word.Length - lengthWithoutMarkup

                    let realColumnLength =
                        match markupLength with
                        | markupLength when markupLength > 0 -> columnLength + markupLength
                        | _ -> columnLength

                    Column (sprintf " %-*s" (realColumnLength - 1) word)
                )
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
                |> Column

            wordLengths
            |> Map.toList
            |> List.map (snd >> createSeparatorForWord)
            |> Columns
            |> Row.line
            |> renderLine

    [<RequireQualifiedAccess>]
    module private MaxWordLengths =
        let perColumnsInRows removeMarkup rows =
            let rec findMaxLengths maxLengths = function
                | [] ->
                    maxLengths
                    |> Map.map (fun _ length -> length <+> WordLength 2)    // +1 space for both sides of the word ` word `
                    |> MaxWordLengths
                | (columnIndex, wordLength) :: rows ->
                    let maxLengthForColumn =
                        match maxLengths |> Map.tryFind columnIndex with
                        | Some currentMaxWordLength -> max currentMaxWordLength wordLength
                        | _ -> wordLength

                    rows |> findMaxLengths (maxLengths.Add (columnIndex, maxLengthForColumn))

            rows
            |> Rows.rows
            |> List.collect (Row.columnsLengths removeMarkup)
            |> findMaxLengths Map.empty

    let render
        (removeMarkup: string -> string)
        (renderHeaderLine: string -> unit)
        (renderRowLine: string -> unit)
        (header: string list)
        (rows: (string list) list) =

        let formatRow maxWordLengths = Row.format removeMarkup maxWordLengths >> RowLine.value

        let renderHeaderRow maxWordLengths = formatRow maxWordLengths >> renderHeaderLine
        let renderRow maxWordLengths = formatRow maxWordLengths >> renderRowLine
        let renderLine = RowLine.value >> renderRowLine

        let renderSeparatorRow wordLengths =
            Separator '-'
            |> Separator.renderRow renderLine wordLengths

        let getMaxWordLengthsPerColumn = MaxWordLengths.perColumnsInRows removeMarkup

        let header =
            match header with
            | [] -> Header.Empty
            | words ->
                words
                |> List.map Column
                |> Columns
                |> Header.Row

        let rows =
            match rows with
            | [] -> Rows.Empty
            | rows ->
                rows
                |> List.map (List.map Column >> Columns)
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
