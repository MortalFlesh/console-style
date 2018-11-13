namespace MF.ConsoleStyle

[<RequireQualifiedAccess>]
module Console =
    open System
    open System.Drawing
    open Colorful
    open ShellProgressBar

    type private OutputType =
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
        Console.WriteAscii(title, color OutputType.Title)
        Console.WriteLine(String.replicate (title.Length * 6) "=", color OutputType.Title)
        newLine()

    [<CompiledName("MainTitlef")>]
    let mainTitlef (format: Printf.StringFormat<('a -> string)>) (title: 'a): unit =
        title
        |> sprintf format
        |> mainTitle

    [<CompiledName("Title")>]
    let title (title: string): unit =
        Console.WriteLine(title, color OutputType.Title)
        Console.WriteLine(String.replicate title.Length "=", color OutputType.Title)
        newLine()

    [<CompiledName("Titlef")>]
    let titlef (format: Printf.StringFormat<('a -> string)>) (value: 'a): unit =
        value
        |> sprintf format
        |> title

    [<CompiledName("Section")>]
    let section (section: string): unit =
        Console.WriteLine(section, color OutputType.Section)
        Console.WriteLine(String.replicate section.Length "-", color OutputType.Section)
        newLine()

    [<CompiledName("Sectionf")>]
    let sectionf (format: Printf.StringFormat<('a -> string)>) (value: 'a): unit =
        value
        |> sprintf format
        |> section

    [<CompiledName("SubTitle")>]
    let subTitle (subTitle: string): unit =
        Console.WriteLine(subTitle, color OutputType.SubTitle)

    [<CompiledName("SubTitlef")>]
    let subTitlef (format: Printf.StringFormat<('a -> string)>) (value: 'a): unit =
        value
        |> sprintf format
        |> subTitle

    [<CompiledName("Error")>]
    let error (message: string): unit =
        Console.WriteLine(message, color OutputType.Error)
        newLine()

    [<CompiledName("Errorf")>]
    let errorf (format: Printf.StringFormat<('a -> string)>) (message: 'a): unit =
        message
        |> sprintf format
        |> error

    [<CompiledName("Success")>]
    let success (message: string): unit =
        Console.WriteLine(message, color OutputType.Success)
        newLine()

    [<CompiledName("Successf")>]
    let successf (format: Printf.StringFormat<('a -> string)>) (message: 'a): unit =
        message
        |> sprintf format
        |> success

    [<CompiledName("Indentation")>]
    let indentation: string = "    "

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
        options
        |> optionsList (options |> getMaxLengthForOptions) title
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
            Console.WriteLine(row, color OutputType.TableHeader)

        Table.renderTable renderHeaderRow header rows
        newLine()

    [<CompiledName("ProgressStart")>]
    let progressStart (initialMessage: string) (total: int): ProgressBar =
        let options =
            new ProgressBarOptions (
                ForegroundColor = ConsoleColor.Yellow,
                ForegroundColorDone = Nullable<ConsoleColor>(ConsoleColor.DarkGreen),
                BackgroundColor = Nullable<ConsoleColor>(ConsoleColor.DarkGray),
                BackgroundCharacter = Nullable<char>('\u2593'),
                DisplayTimeInRealTime = true,
                ProgressBarOnBottom = true
            )

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
        Console.Write(question + " ", color OutputType.SubTitle)
        Console.ReadLine()        

    [<CompiledName("Askf")>]
    let askf format question: string =
        question
        |> sprintf format
        |> ask
