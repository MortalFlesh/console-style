namespace MF.ConsoleStyle

[<RequireQualifiedAccess>]
module Console =
    open System
    open Colorful
    open ShellProgressBar

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

    let private block = Render.block indentation true
    let private blockWithMarkup allowDateTime = Render.block indentation allowDateTime (Some (TextWithMarkup None))
    let private color = Render.color

    let private format1 render (format: Printf.StringFormat<'a -> string>) (valueA: 'a) =
        valueA
        |> sprintf format
        |> render

    let private format2 render (format: Printf.StringFormat<('a -> 'b -> string)>) (valueA: 'a) (valueB: 'b) =
        (valueA, valueB)
        ||> sprintf format
        |> render

    let private format3 render (format: Printf.StringFormat<('a -> 'b -> 'c -> string)>) (valueA: 'a) (valueB: 'b) (valueC: 'c) =
        (valueA, valueB, valueC)
        |||> sprintf format
        |> render

    [<CompiledName("Message")>]
    let message (message: string): unit =
        message
        |> blockWithMarkup true None false

    [<CompiledName("Messagef")>]
    let messagef format = format1 message format

    [<CompiledName("Messagef2")>]
    let messagef2 format = format2 message format

    [<CompiledName("Messagef3")>]
    let messagef3 format = format3 message format

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
    let mainTitlef format = format1 mainTitle format

    [<CompiledName("MainTitlef2")>]
    let mainTitlef2 format = format2 mainTitle format

    [<CompiledName("MainTitlef3")>]
    let mainTitlef3 format = format3 mainTitle format

    [<CompiledName("Title")>]
    let title (title: string): unit =
        title
        |> block (Some Title) (Some "=") true

    [<CompiledName("Titlef")>]
    let titlef format = format1 title format

    [<CompiledName("Titlef2")>]
    let titlef2 format = format2 title format

    [<CompiledName("Titlef3")>]
    let titlef3 format = format3 title format

    [<CompiledName("Section")>]
    let section (section: string): unit =
        section
        |> block (Some SubTitle) (Some "-") true

    [<CompiledName("Sectionf")>]
    let sectionf format = format1 section format

    [<CompiledName("Sectionf2")>]
    let sectionf2 format = format2 section format

    [<CompiledName("Sectionf3")>]
    let sectionf3 format = format3 section format

    [<CompiledName("SubTitle")>]
    let subTitle (subTitle: string): unit =
        subTitle
        |> block (Some SubTitle) None false

    [<CompiledName("SubTitlef")>]
    let subTitlef format = format1 subTitle format

    [<CompiledName("SubTitlef2")>]
    let subTitlef2 format = format2 subTitle format

    [<CompiledName("SubTitlef3")>]
    let subTitlef3 format = format3 subTitle format

    [<CompiledName("Error")>]
    let error (message: string): unit =
        message
        |> block (Some Error) None false

    [<CompiledName("Errorf")>]
    let errorf format = format1 error format

    [<CompiledName("Errorf2")>]
    let errorf2 format = format2 error format

    [<CompiledName("Errorf3")>]
    let errorf3 format = format3 error format

    [<CompiledName("Success")>]
    let success (message: string): unit =
        message
        |> block (Some Success) None true

    [<CompiledName("Successf")>]
    let successf format = format1 success format

    [<CompiledName("Successf2")>]
    let successf2 format = format2 success format

    [<CompiledName("Successf3")>]
    let successf3 format = format3 success format

    [<CompiledName("Indent")>]
    let indent (value: string): string =
        indentation + value

    //
    // Output many
    //

    [<CompiledName("Messages")>]
    let messages (prefix: string) (messages: seq<string>): unit =
        if Verbosity.isNormal() then
            messages
            |> Seq.iter (sprintf "%s%s" prefix >> blockWithMarkup false None false)

    [<CompiledName("Options")>]
    let options (title: string) (options: list<string list>): unit =
        options
        |> Options.optionsList (messages indentation) "-" title
        newLine()

    [<CompiledName("SimpleOptions")>]
    let simpleOptions (title: string) (options: list<string list>): unit =
        options
        |> Options.optionsList (messages indentation) "" title
        newLine()

    [<CompiledName("GroupedOptions")>]
    let groupedOptions (separator: string) (title: string) (options: list<string list>): unit =
        options
        |> Options.groupedOptionsList (messages indentation) separator title
        newLine()

    [<CompiledName("List")>]
    let list (messages: seq<string>): unit =
        if Verbosity.isNormal() then
            messages
            |> Seq.iter (sprintf " - %s" >> blockWithMarkup false None false)

    //
    // Complex components
    //

    [<CompiledName("Table")>]
    let table (header: list<string>) (rows: list<list<string>>): unit =
        if Verbosity.isNormal() then
            let renderHeaderLine (headerLine: string) =
                Console.WriteLine(headerLine, TableHeader |> color)

            let renderRowLine = blockWithMarkup false None false

            Table.render renderHeaderLine renderRowLine header rows
            newLine()

    type private ShellProgress = ShellProgressBar.ProgressBar

    type ProgressBar =
        private
        | Active of ShellProgress
        | Inactive

        with
            member this.Advance() =
                match this with
                | Active progressBar -> progressBar.Tick()
                | _ -> ()

            member this.Finish() =
                match this with
                | Active progressBar ->
                    progressBar.Message <- "Finished"
                    progressBar.Dispose()
                | _ -> ()

            interface IDisposable with
                member this.Dispose() = this.Finish()

    [<CompiledName("ProgressStart")>]
    let progressStart (initialMessage: string) (total: int): ProgressBar =
        if Verbosity.isNormal() && Console.WindowWidth > 0 then
            let options =
                ProgressBarOptions (
                    ForegroundColor = ConsoleColor.Yellow,
                    ForegroundColorDone = Nullable<ConsoleColor>(ConsoleColor.DarkGreen),

                    BackgroundColor = Nullable<ConsoleColor>(ConsoleColor.DarkGray),

                    DisplayTimeInRealTime = true,
                    ProgressBarOnBottom = true
                )

            new ShellProgress(total, initialMessage, options) |> Active
        else Inactive

    [<CompiledName("ProgressAdvance")>]
    let progressAdvance (progress: ProgressBar): unit =
        progress.Advance()

    [<CompiledName("ProgressFinish")>]
    let progressFinish (progress: ProgressBar): unit =
        progress.Finish()

    //
    // Inputs
    //

    [<CompiledName("Ask")>]
    let ask (question: string): string =
        Console.Write(question + " ", SubTitle |> color)
        Console.ReadLine()

    [<CompiledName("Askf")>]
    let askf format = format1 ask format

    [<CompiledName("Askf2")>]
    let askf2 format = format2 ask format

    [<CompiledName("Askf3")>]
    let askf3 format = format3 ask format
