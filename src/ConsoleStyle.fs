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
            match hasRows && not hasHeaders with
            | true -> rows
            | false -> rows |> Seq.append [header]

        let getColumnWordLength row =
            row
            |> Seq.mapi (fun column word -> (column, word |> String.length))

        allRows
        |> Seq.collect getColumnWordLength
        |> Seq.fold (fun maxWordLengthsPerColumn (column, wordLength) ->
            let isColumnMaxLengthStored = maxWordLengthsPerColumn |> Map.containsKey column
            let isNewColumnLength = not isColumnMaxLengthStored
            let isCurrentWordLongerThanStored = isColumnMaxLengthStored && wordLength > maxWordLengthsPerColumn.[column]

            match isCurrentWordLongerThanStored || isNewColumnLength with
            | true -> maxWordLengthsPerColumn |> Map.add column wordLength
            | false -> maxWordLengthsPerColumn
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

[<RequireQualifiedAccess>]
module Console =
    open System
    open System.Drawing
    open Colorful
    open ShellProgressBar

    type private Color =
        | Title
        | SubTitle
        | Section
        | TableHeader
        | Success
        | Error

    let private color = function
        | Title -> Color.Cyan
        | SubTitle -> Color.Yellow
        | Section -> Color.Yellow
        | TableHeader -> Color.DarkGoldenrod
        | Success -> Color.LimeGreen
        | Error -> Color.Red

    let private getMaxLengthForOptions options =
        options
        |> Seq.map fst
        |> Seq.maxBy String.length
        |> String.length

    //
    // Output style
    //

    [<CompiledName("Messagef")>]
    let messagef (format: Printf.TextWriterFormat<('a -> unit)>) (message: 'a): unit =
        message |> printfn format

    [<CompiledName("Message")>]
    let message (message: string): unit =
        message |> messagef "%s"

    [<CompiledName("NewLine")>]
    let newLine (): unit =
        message ""

    [<CompiledName("MainTitle")>]
    let mainTitle (title: string): unit =
        Console.WriteAscii(title, color Color.Title)
        Console.WriteLine(String.replicate (title.Length * 6) "=", color Color.Title)
        newLine()

    [<CompiledName("MainTitlef")>]
    let mainTitlef (format: Printf.StringFormat<('a -> string)>) (title: 'a): unit =
        title
        |> sprintf format
        |> mainTitle

    [<CompiledName("Title")>]
    let title (title: string): unit =
        Console.WriteLine(title, color Color.Title)
        Console.WriteLine(String.replicate title.Length "=", color Color.Title)
        newLine()

    [<CompiledName("Titlef")>]
    let titlef (format: Printf.StringFormat<('a -> string)>) (value: 'a): unit =
        value
        |> sprintf format
        |> title

    [<CompiledName("Section")>]
    let section (section: string): unit =
        Console.WriteLine(section, color Color.Section)
        Console.WriteLine(String.replicate section.Length "-", color Color.Section)
        newLine()

    [<CompiledName("Sectionf")>]
    let sectionf (format: Printf.StringFormat<('a -> string)>) (value: 'a): unit =
        value
        |> sprintf format
        |> section

    [<CompiledName("SubTitle")>]
    let subTitle (subTitle: string): unit =
        Console.WriteLine(subTitle, color Color.SubTitle)

    [<CompiledName("SubTitlef")>]
    let subTitlef (format: Printf.StringFormat<('a -> string)>) (value: 'a): unit =
        value
        |> sprintf format
        |> subTitle

    [<CompiledName("Error")>]
    let error (message: string): unit =
        Console.WriteLine(message, color Color.Error)
        newLine()

    [<CompiledName("Errorf")>]
    let errorf (format: Printf.StringFormat<('a -> string)>) (message: 'a): unit =
        message
        |> sprintf format
        |> error

    [<CompiledName("Success")>]
    let success (message: string): unit =
        Console.WriteLine(message, color Color.Success)
        newLine()

    [<CompiledName("Successf")>]
    let successf (format: Printf.StringFormat<('a -> string)>) (message: 'a): unit =
        message
        |> sprintf format
        |> success

    [<CompiledName("Indentation")>]
    let indentation: string =
        String.replicate 4 " "

    [<CompiledName("Indent")>]
    let indent (value: string): string =
        indentation + value

    [<CompiledName("Indentf")>]
    let indentf (format: Printf.StringFormat<('a -> string)>) (value: 'a): string =
        value
        |> sprintf format
        |> indent

    //
    // Output many
    //

    [<CompiledName("Messages")>]
    let messages (prefix: string) (messages: seq<string>): unit =
        messages
        |> Seq.map (sprintf "%s%s" prefix)
        |> Seq.iter message

    let private optionsList maxLength title options =
        subTitle title
        options
        |> Seq.map (fun (command, description) ->
            sprintf "- %-*s %-s" (maxLength + 1) command description
        )
        |> messages indentation

    [<CompiledName("Options")>]
    let options (title: string) (options: seq<string * string>): unit =
        optionsList (options |> getMaxLengthForOptions) title options
        newLine()

    [<CompiledName("Optionsf")>]
    let optionsf (format: Printf.StringFormat<('a -> string)>) (title: 'a) (options: seq<string * string>): unit =
        options
        |> optionsList (options |> getMaxLengthForOptions) (title |> sprintf format)
        newLine()

    [<CompiledName("List")>]
    let list (messages: seq<string>): unit =
        messages
        |> Seq.map (sprintf " - %s")
        |> Seq.iter message

    //
    // Complex components
    //

    [<CompiledName("Table")>]
    let table (header: seq<string>) (rows: seq<seq<string>>): unit =
        let renderHeaderRow (row: string) =
            Console.WriteLine(row, color Color.TableHeader)

        Table.renderTable renderHeaderRow header rows
        newLine()

    [<CompiledName("ProgressStart")>]
    let progressStart (initialMessage: string) (total: int): ProgressBar =
        let options = new ProgressBarOptions()
        options.ForegroundColor <- ConsoleColor.Yellow
        options.ForegroundColorDone <- Nullable<ConsoleColor>(ConsoleColor.DarkGreen)
        options.BackgroundColor <- Nullable<ConsoleColor>(ConsoleColor.DarkGray)
        options.BackgroundCharacter <- Nullable<char>('\u2593')
        options.DisplayTimeInRealTime <- true
        options.ProgressBarOnBottom <- true

        new ProgressBar(total, initialMessage, options)

    [<CompiledName("ProgressFinish")>]
    let progressFinish (progress: ProgressBar): unit =
        progress.Message <- "Finished"
        progress.Dispose()

    //
    // Inputs
    //

    [<CompiledName("Ask")>]
    let ask (question: string): string =
        Console.Write(question + " ", color Color.SubTitle)
        Console.ReadLine()        

    [<CompiledName("Askf")>]
    let askf format question: string =
        question
        |> sprintf format
        |> ask
