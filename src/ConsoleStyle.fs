namespace MF.ConsoleStyle

[<RequireQualifiedAccess>]
module Console =
    open System
    open Colorful
    open ShellProgressBar
    open Types

    let private getMaxLengthForOptions options =
        options
        |> Seq.map fst
        |> Seq.maxBy String.length
        |> String.length

    //
    // Verbosity
    //

    [<CompiledName("SetVerbosity")>]
    let setVerbosity = Verbosity.setVerbosity

    [<CompiledName("IsQuiet")>]
    let isQuiet = Verbosity.isQuiet

    [<CompiledName("IsNormal")>]
    let isNormal = Verbosity.isNormal

    [<CompiledName("IsVerbose")>]
    let isVerbose = Verbosity.isVerbose

    [<CompiledName("IsVeryVerbose")>]
    let isVeryVerbose = Verbosity.isVeryVerbose

    [<CompiledName("IsDebug")>]
    let isDebug = Verbosity.isDebug

    //
    // Output style
    //
    [<CompiledName("Indentation")>]
    let indentation: string = "    "

    let private block = Render.block indentation
    let private color = Render.color

    [<CompiledName("Message")>]
    let message (message: string): unit =
        message
        |> block None None false

    [<CompiledName("Messagef")>]
    let messagef (format: Printf.StringFormat<('a -> string)>) (text: 'a): unit =
        text
        |> sprintf format
        |> message

    [<CompiledName("NewLine")>]
    let newLine (): unit =
        if Verbosity.isNormal() then
            printfn ""

    [<CompiledName("MainTitle")>]
    let mainTitle (title: string): unit =
        if Verbosity.isNormal() then
            Console.WriteAscii(title, color Title)
            Console.WriteLine(String.replicate (title.Length * 6) "=", color Title)
            newLine()

    [<CompiledName("MainTitlef")>]
    let mainTitlef (format: Printf.StringFormat<('a -> string)>) (title: 'a): unit =
        title
        |> sprintf format
        |> mainTitle

    [<CompiledName("Title")>]
    let title (title: string): unit =
        title
        |> block (Some Title) (Some "=") true

    [<CompiledName("Titlef")>]
    let titlef (format: Printf.StringFormat<('a -> string)>) (value: 'a): unit =
        value
        |> sprintf format
        |> title

    [<CompiledName("Section")>]
    let section (section: string): unit =
        section
        |> block (Some SubTitle) (Some "-") true

    [<CompiledName("Sectionf")>]
    let sectionf (format: Printf.StringFormat<('a -> string)>) (value: 'a): unit =
        value
        |> sprintf format
        |> section

    [<CompiledName("SubTitle")>]
    let subTitle (subTitle: string): unit =
        subTitle
        |> block (Some SubTitle) None false

    [<CompiledName("SubTitlef")>]
    let subTitlef (format: Printf.StringFormat<('a -> string)>) (value: 'a): unit =
        value
        |> sprintf format
        |> subTitle

    [<CompiledName("Error")>]
    let error (message: string): unit =
        message
        |> block (Some Error) None false

    [<CompiledName("Errorf")>]
    let errorf (format: Printf.StringFormat<('a -> string)>) (message: 'a): unit =
        message
        |> sprintf format
        |> error

    [<CompiledName("Success")>]
    let success (message: string): unit =
        message
        |> block (Some Success) None true

    [<CompiledName("Successf")>]
    let successf (format: Printf.StringFormat<('a -> string)>) (message: 'a): unit =
        message
        |> sprintf format
        |> success

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
        if Verbosity.isNormal() then
            messages
            |> Seq.iter (printfn "%s%s" prefix)

    let private optionsList maxLength title options =
        subTitle title
        options
        |> Seq.map (fun (option, description) ->
            sprintf "- %-*s %-s" (maxLength + 1) option description
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
        if Verbosity.isNormal() then
            messages
            |> Seq.iter (printfn " - %s")

    //
    // Complex components
    //

    [<CompiledName("Table")>]
    let table (header: seq<string>) (rows: seq<seq<string>>): unit =
        if Verbosity.isNormal() then
            let renderHeaderRow (row: string) =
                Console.WriteLine(row, TableHeader |> color)

            Table.renderTable renderHeaderRow header rows
            newLine()

    [<CompiledName("ProgressStart")>]
    let progressStart (initialMessage: string) (total: int): ProgressBar option =
        if Verbosity.isNormal() && Console.WindowWidth > 0 then
            let options =
                new ProgressBarOptions (
                    ForegroundColor = ConsoleColor.Yellow,
                    ForegroundColorDone = Nullable<ConsoleColor>(ConsoleColor.DarkGreen),
                    BackgroundColor = Nullable<ConsoleColor>(ConsoleColor.DarkGray),
                    BackgroundCharacter = Nullable<char>('\u2593'),
                    DisplayTimeInRealTime = true,
                    ProgressBarOnBottom = true
                )

            new ProgressBar(total, initialMessage, options) |> Some
        else None

    [<CompiledName("ProgressAdvance")>]
    let progressAdvance (progress: ProgressBar option): unit =
        match progress with
        | Some progress -> progress.Tick()
        | _ -> ()

    [<CompiledName("ProgressFinish")>]
    let progressFinish (progress: ProgressBar option): unit =
        match progress with
        | Some progress ->
            progress.Message <- "Finished"
            progress.Dispose()
        | _ -> ()

    //
    // Inputs
    //

    [<CompiledName("Ask")>]
    let ask (question: string): string =
        Console.Write(question + " ", SubTitle |> color)
        Console.ReadLine()

    [<CompiledName("Askf")>]
    let askf format question: string =
        question
        |> sprintf format
        |> ask
