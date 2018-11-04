namespace MF.ConsoleStyle

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

    let private getMaxLengthsPerColumn lines =
        lines
        |> Seq.collect (fun line ->
            line
            |> Seq.mapi (fun i value -> (i, value |> String.length))
        )
        |> Seq.fold (fun lengths (i, length) ->
            let isIn = lengths |> Map.containsKey i

            if (isIn && lengths.[i] < length) || not isIn
            then lengths |> Map.add i length
            else lengths
        ) Map.empty<int,int>
        |> Map.toList
        |> List.map snd

    //
    // Output style
    //

    // todo - add explicit format for format

    [<CompiledName("Messagef")>]
    let messagef format message: unit =
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
    let titlef format value: unit =
        value
        |> sprintf format
        |> title

    [<CompiledName("Section")>]
    let section (section: string): unit =
        Console.WriteLine(section, color Color.Section)
        Console.WriteLine(String.replicate section.Length "-", color Color.Section)
        newLine()

    [<CompiledName("Sectionf")>]
    let sectionf format value: unit =
        value
        |> sprintf format
        |> section

    [<CompiledName("SubTitle")>]
    let subTitle (subTitle: string): unit =
        Console.WriteLine(subTitle, color Color.SubTitle)

    [<CompiledName("SubTitlef")>]
    let subTitlef format value: unit =
        value
        |> sprintf format
        |> subTitle

    [<CompiledName("Error")>]
    let error (message: string): unit =
        Console.WriteLine(message, color Color.Error)
        newLine()

    [<CompiledName("Errorf")>]
    let errorf format message: unit =
        message
        |> sprintf format
        |> error

    [<CompiledName("Success")>]
    let success (message: string): unit =
        Console.WriteLine(message, color Color.Success)
        newLine()

    [<CompiledName("Successf")>]
    let successf format message: unit =
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
    let indentf format value: string =
        value
        |> sprintf format
        |> indent

    //
    // Output many
    //

    [<CompiledName("Messages")>]
    let messages (prefix: string) (messages: seq<string>) =
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
        let isNotEmpty seq =
            seq
            |> Seq.isEmpty
            |> not

        let hasHeaders =
            header
            |> isNotEmpty
        let hasRows =
            rows
            |> isNotEmpty
 
        let row values =
            values
            |> String.concat " "

        let separatorLine length =
            String.replicate length "-"

        let getMaxLengths header rows =
            let values =
                match hasRows && hasHeaders with
                | true -> rows |> Seq.append [header]
                | false ->
                match hasRows with
                | true -> rows
                | _ -> [] |> Seq.append [header]

            values
            |> getMaxLengthsPerColumn
            |> List.map ((+) 2)

        let separators lengths =
            lengths
            |> List.map separatorLine
            |> row
            |> message

        let tableRow (lengths: int list) values =
            values
            |> Seq.mapi (fun i v ->
                sprintf " %-*s" (lengths.[i] - 1) v
            )

        let tableHeader lengths header =
            header
            |> tableRow lengths
            |> row
            |> fun r -> Console.WriteLine(r, color Color.TableHeader)
            lengths |> separators

        let tableRows lengths rows =
            rows
            |> Seq.map ((tableRow lengths) >> row)
            |> Seq.iter message
            lengths |> separators

        let renderTable () =
            if hasHeaders || hasRows then
                let lengths = getMaxLengths header rows

                lengths |> separators
                if hasHeaders then header |> tableHeader lengths
                if hasRows then rows |> tableRows lengths

        renderTable()        
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
