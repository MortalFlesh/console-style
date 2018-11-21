namespace MF.ConsoleStyle

module private Table =
    let private isNotEmpty seq =
        seq
        |> Seq.isEmpty
        |> not

    let private createRow items =
        items
        |> String.concat " "

    let private renderSeparatorRow wordLengths =
        let createSeparatorForWord wordLength =
            String.replicate wordLength "-"

        wordLengths
        |> List.map createSeparatorForWord
        |> createRow
        |> printfn "%s"

    let private getMaxWordLengthsPerColumn hasHeaders hasRows header rows =
        let allRows =
            if hasRows && not hasHeaders
            then rows
            else rows |> Seq.append [header]

        let getColumnWordLength row =
            row
            |> Seq.mapi (fun column word -> (column, word |> String.length))

        allRows
        |> Seq.collect getColumnWordLength
        |> Seq.fold (fun maxWordLengthsPerColumn (column, wordLength) ->
            maxWordLengthsPerColumn
            |> Map.add
                column
                (match maxWordLengthsPerColumn.TryFind column with
                | Some currentMaxWordLength -> max currentMaxWordLength wordLength
                | _ -> wordLength)
        ) Map.empty<int,int>
        |> Map.toList
        |> List.map snd
        |> List.map ((+) 2)    // +1 space for both sides of the word ` word `

    let private formatRow (maxWordLengths: int list) items =
        items
        |> Seq.mapi (fun i item ->
            sprintf " %-*s" (maxWordLengths.[i] - 1) item
        )

    let private renderTableHeader renderHeaderRow maxWordLengths headerWords =
        headerWords
        |> formatRow maxWordLengths
        |> createRow
        |> renderHeaderRow
        maxWordLengths |> renderSeparatorRow

    let private renderTableRows maxWordLengths rows =
        rows
        |> Seq.map ((formatRow maxWordLengths) >> createRow)
        |> Seq.iter (printfn "%s")
        maxWordLengths |> renderSeparatorRow

    let renderTable renderHeaderRow header rows =
        let renderTableHeader' = renderTableHeader renderHeaderRow
        let hasHeaders = header |> isNotEmpty
        let hasRows = rows |> isNotEmpty

        if hasHeaders || hasRows then
            let wordMaxLengths = getMaxWordLengthsPerColumn hasHeaders hasRows header rows

            wordMaxLengths |> renderSeparatorRow
            if hasHeaders then header |> renderTableHeader' wordMaxLengths
            if hasRows then rows |> renderTableRows wordMaxLengths
