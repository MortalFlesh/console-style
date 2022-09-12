namespace MF.ConsoleStyle

open System
open MF.ConsoleStyle
// open ShellProgressBar

type ConsoleStyle (output: Output.IOutput, style) =
    let mutable style = style
    let removeMarkup message =
        message |> Style.removeMarkup style

    // Constructors

    new (style) = ConsoleStyle(Output.ConsoleOutput(Verbosity.Normal), style)
    new (output) = ConsoleStyle(output, Style.defaults)
    new (verbosity) = ConsoleStyle(Output.ConsoleOutput(verbosity))
    new () = ConsoleStyle (Verbosity.Normal)

    // Verbosity
    member _.Verbosity = output.Verbosity
    member _.IsQuiet() = output.IsQuiet()
    member _.IsNormal() = output.IsNormal()
    member _.IsVerbose() = output.IsVerbose()
    member _.IsVeryVerbose() = output.IsVeryVerbose()
    member _.IsDebug() = output.IsDebug()

    // Style
    member _.ChangeStyle(newStyle) = style <- newStyle
    member _.UpdateStyle(f) = style <- f style
    member _.Indent message = sprintf "%s%s" (style.Indentation |> Indentation.value) message
    member _.RemoveMarkup(message) = removeMarkup message

    // Output
    (* member _.Message(message: string): unit =
        message
        |> blockWithMarkup true None false *)

    //member _.Write (message: string): unit = message |> Render.block indentation false (Some OnLine) None false
    member _.Write (message: string) =
        message
        |> Style.Message.ofString
        |> Render.message output.Verbosity style TextWithMarkup
        |> RenderedMessage.value
        |> output.Write

    member this.Write (format, a) = sprintf format a |> this.Write
    member this.Write (format, a, b) = sprintf format a b |> this.Write
    member this.Write (format, a, b, c) = sprintf format a b c |> this.Write
    member this.Write (format, a, b, c, d) = sprintf format a b c d |> this.Write
    member this.Write (format, a, b, c, d, e) = sprintf format a b c d e |> this.Write

    /// It works as WriteLine
    member _.Message (message: string) =
        message
        |> Style.Message.ofString
        |> Render.message output.Verbosity style TextWithMarkup
        |> RenderedMessage.value
        |> output.WriteLine

    member this.Message (format, a) = sprintf format a |> this.Message
    member this.Message (format, a, b) = sprintf format a b |> this.Message
    member this.Message (format, a, b, c) = sprintf format a b c |> this.Message
    member this.Message (format, a, b, c, d) = sprintf format a b c d |> this.Message
    member this.Message (format, a, b, c, d, e) = sprintf format a b c d e |> this.Message

    member _.Title (title: string) =
        title
        |> Style.Message.ofString
        |> Render.message output.Verbosity style Title
        |> RenderedMessage.value
        |> sprintf "%s\n"
        |> output.WriteLine

    member this.Title (format, a) = sprintf format a |> this.Title
    member this.Title (format, a, b) = sprintf format a b |> this.Title
    member this.Title (format, a, b, c) = sprintf format a b c |> this.Title
    member this.Title (format, a, b, c, d) = sprintf format a b c d |> this.Title
    member this.Title (format, a, b, c, d, e) = sprintf format a b c d e |> this.Title

    member _.SubTitle (subTitle: string) =
        subTitle
        |> Style.Message.ofString
        |> Render.message output.Verbosity style SubTitle
        |> RenderedMessage.value
        |> output.WriteLine

    member this.SubTitle (format, a) = sprintf format a |> this.SubTitle
    member this.SubTitle (format, a, b) = sprintf format a b |> this.SubTitle
    member this.SubTitle (format, a, b, c) = sprintf format a b c |> this.SubTitle
    member this.SubTitle (format, a, b, c, d) = sprintf format a b c d |> this.SubTitle
    member this.SubTitle (format, a, b, c, d, e) = sprintf format a b c d e |> this.SubTitle

    member _.Section (section: string) =
        section
        |> Style.Message.ofString
        |> Render.message output.Verbosity style Section
        |> RenderedMessage.value
        |> sprintf "%s\n"
        |> output.WriteLine

    member this.Section (format, a) = sprintf format a |> this.Section
    member this.Section (format, a, b) = sprintf format a b |> this.Section
    member this.Section (format, a, b, c) = sprintf format a b c |> this.Section
    member this.Section (format, a, b, c, d) = sprintf format a b c d |> this.Section
    member this.Section (format, a, b, c, d, e) = sprintf format a b c d e |> this.Section

    member _.NewLine (): unit = output.WriteLine("")

    // todo
    member _.MainTitle (title: string): unit =
        (* Message title
        |> output.WriteBig { style with NewLine = Some (NewLines 2) } *)
        ()

    member this.MainTitle (format, a) = sprintf format a |> this.MainTitle
    member this.MainTitle (format, a, b) = sprintf format a b |> this.MainTitle
    member this.MainTitle (format, a, b, c) = sprintf format a b c |> this.MainTitle
    member this.MainTitle (format, a, b, c, d) = sprintf format a b c d |> this.MainTitle
    member this.MainTitle (format, a, b, c, d, e) = sprintf format a b c d e |> this.MainTitle

    member this.MainTitleFigle (title: string): unit =
        // todo
        (* if Verbosity.isNormal verbosity then
            let font = FigletFont.Load("chunky.flf")
            let figlet = Figlet(font)
            let figletString = figlet.ToAscii(title)

            let linelength =
                match figletString.ConcreteValue.Split("\n") |> Seq.toList with
                | [] -> figletString.ConcreteValue.Length
                | lines -> lines |> List.map String.length |> List.maxBy id

            //Console.WriteLine(figlet.ToAscii(title), Drawing.ColorTranslator.FromHtml("#D2000"))
            Console.Write(figletString, color Title)
            Console.WriteLine(String.replicate linelength "=", color Title)
            this.NewLine() *)
        ()

    member this.MainTitleFigle (format, a) = sprintf format a |> this.MainTitleFigle
    member this.MainTitleFigle (format, a, b) = sprintf format a b |> this.MainTitleFigle
    member this.MainTitleFigle (format, a, b, c) = sprintf format a b c |> this.MainTitleFigle
    member this.MainTitleFigle (format, a, b, c, d) = sprintf format a b c d |> this.MainTitleFigle
    member this.MainTitleFigle (format, a, b, c, d, e) = sprintf format a b c d e |> this.MainTitleFigle

    member _.Error (error: string) =
        error
        |> Style.Message.ofString
        |> Render.message output.Verbosity style Error
        |> RenderedMessage.value
        |> sprintf "%s\n"
        |> output.WriteErrorLine

    member this.Error (format, a) = sprintf format a |> this.Error
    member this.Error (format, a, b) = sprintf format a b |> this.Error
    member this.Error (format, a, b, c) = sprintf format a b c |> this.Error
    member this.Error (format, a, b, c, d) = sprintf format a b c d |> this.Error
    member this.Error (format, a, b, c, d, e) = sprintf format a b c d e |> this.Error

    // todo - add Warning

    member _.Success (success: string) =
        success
        |> Style.Message.ofString
        |> Render.message output.Verbosity style Success
        |> RenderedMessage.value
        |> sprintf "%s\n"
        |> output.WriteLine

    member this.Success (format, a) = sprintf format a |> this.Success
    member this.Success (format, a, b) = sprintf format a b |> this.Success
    member this.Success (format, a, b, c) = sprintf format a b c |> this.Success
    member this.Success (format, a, b, c, d) = sprintf format a b c d |> this.Success
    member this.Success (format, a, b, c, d, e) = sprintf format a b c d e |> this.Success

    member _.Note (note: string) =
        note
        |> Style.Message.ofString
        |> Render.message output.Verbosity style Note
        |> RenderedMessage.value
        |> sprintf "%s\n"
        |> output.WriteLine

    member this.Note (format, a) = sprintf format a |> this.Note
    member this.Note (format, a, b) = sprintf format a b |> this.Note
    member this.Note (format, a, b, c) = sprintf format a b c |> this.Note
    member this.Note (format, a, b, c, d) = sprintf format a b c d |> this.Note
    member this.Note (format, a, b, c, d, e) = sprintf format a b c d e |> this.Note

    //
    // Output many
    //

    member this.Messages (prefix: string) (messages: seq<string>): unit =
        if this.IsNormal() then
            messages
            |> Seq.iter (sprintf "%s%s" prefix >> this.Message)

    member this.List (messages: seq<string>): unit =
        this.Messages " - " messages

    member private this.RenderOptions messages =
        messages
        |> List.iter (RenderedMessage.value >> output.WriteLine)
        |> this.NewLine

    member this.Options (title: string) (options: list<string list>): unit =
        if this.IsNormal() then
            options
            |> Options.optionsList removeMarkup output.Verbosity style "-" title
            |> this.RenderOptions

    member this.SimpleOptions (title: string) (options: list<string list>): unit =
        if this.IsNormal() then
            options
            |> Options.optionsList removeMarkup output.Verbosity style "" title
            |> this.RenderOptions

    member this.GroupedOptions (separator: string) (title: string) (options: list<string list>): unit =
        if this.IsNormal() then
            options
            |> Options.groupedOptionsList removeMarkup output.Verbosity style separator title
            |> this.RenderOptions

    //
    // Complex components
    //

    member this.Table header rows =
        if this.IsNormal() then
            let renderHeaderLine (headerLine: string) =
                headerLine
                |> Style.Message.ofString
                |> Render.message output.Verbosity style TableHeader
                |> RenderedMessage.value
                |> output.WriteLine

            let renderRowLine = this.Message

            Table.render removeMarkup renderHeaderLine renderRowLine
                header
                rows

            this.NewLine()
        else ()

    member this.Tabs(tabs) =
        if this.IsNormal() then
            Tabs.render removeMarkup this.Message tabs
            this.NewLine()
        else ()

    member this.Tabs(tabs, length) =
        if this.IsNormal() then
            Tabs.renderInLength removeMarkup this.Message length tabs
            this.NewLine()
        else ()

    //
    // User input
    //

    member _.Ask (question: string): string =
        question
        |> Style.Message.ofString
        |> Render.message output.Verbosity style SubTitle
        |> RenderedMessage.value
        |> sprintf "%s "
        |> output.Write

        Console.ReadLine()

    member this.Ask (format, a) = sprintf format a |> this.Ask
    member this.Ask (format, a, b) = sprintf format a b |> this.Ask
    member this.Ask (format, a, b, c) = sprintf format a b c |> this.Ask
    member this.Ask (format, a, b, c, d) = sprintf format a b c d |> this.Ask
    member this.Ask (format, a, b, c, d, e) = sprintf format a b c d e |> this.Ask
(*
[<RequireQualifiedAccess>]
module Console =
    open System
    open Colorful
    open ShellProgressBar

    let removeMarkup = Markup.removeMarkup

    //
    // Verbosity
    //
    let mutable private verbosity = Verbosity.Normal

    let setVerbosity level = verbosity <- level
    let isQuiet () = Verbosity.isQuiet verbosity
    let isNormal () = Verbosity.isNormal verbosity
    let isVerbose () = Verbosity.isVerbose verbosity
    let isVeryVerbose () = Verbosity.isVeryVerbose verbosity
    let isDebug () = Verbosity.isDebug verbosity

    //
    // Output style
    //
    let indentation: string = Style.DefaultIndentation

    let private block = Render.block verbosity indentation true
    let private blockWithMarkup allowDateTime = Render.block verbosity indentation allowDateTime (Some TextWithMarkup)
    let private color = OutputType.color

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

    let message (message: string): unit =
        message
        |> blockWithMarkup true None false

    let write (message: string): unit =
        message
        |> Render.block verbosity indentation false (Some OnLine) None false

    let messagef format = format1 message format
    let messagef2 format = format2 message format
    let messagef3 format = format3 message format

    let newLine (): unit =
        if isNormal() then
            printfn ""

    let mainTitle (title: string): unit =
        if isNormal() then
            Console.WriteAscii(title, color Title)
            Console.WriteLine(String.replicate (title.Length * 6) "=", color Title)
            newLine()

    let mainTitleX (title: string): unit =
        if isNormal() then
            let font = FigletFont.Load("chunky.flf")
            let figlet = Figlet(font)
            let figletString = figlet.ToAscii(title)

            let linelength =
                match figletString.ConcreteValue.Split("\n") |> Seq.toList with
                | [] -> figletString.ConcreteValue.Length
                | lines -> lines |> List.map String.length |> List.maxBy id

            //Console.WriteLine(figlet.ToAscii(title), Drawing.ColorTranslator.FromHtml("#D2000"))
            Console.Write(figletString, color Title)
            Console.WriteLine(String.replicate linelength "=", color Title)
            newLine()

    let mainTitlef format = format1 mainTitle format
    let mainTitlef2 format = format2 mainTitle format
    let mainTitlef3 format = format3 mainTitle format

    let title (title: string): unit =
        title
        |> block (Some Title) (Some "=") true

    let titlef format = format1 title format
    let titlef2 format = format2 title format
    let titlef3 format = format3 title format

    let section (section: string): unit =
        section
        |> block (Some SubTitle) (Some "-") true

    let sectionf format = format1 section format
    let sectionf2 format = format2 section format
    let sectionf3 format = format3 section format

    let subTitle (subTitle: string): unit =
        subTitle
        |> block (Some SubTitle) None false

    let subTitlef format = format1 subTitle format
    let subTitlef2 format = format2 subTitle format
    let subTitlef3 format = format3 subTitle format

    let error (message: string): unit =
        message
        |> block (Some Error) None false

    let errorf format = format1 error format
    let errorf2 format = format2 error format
    let errorf3 format = format3 error format

    let success (message: string): unit =
        message
        |> block (Some Success) None true

    let successf format = format1 success format
    let successf2 format = format2 success format
    let successf3 format = format3 success format

    let indent (value: string): string =
        indentation + value

    //
    // Output many
    //

    let messages (prefix: string) (messages: seq<string>): unit =
        if isNormal() then
            messages
            |> Seq.iter (sprintf "%s%s" prefix >> blockWithMarkup false None false)

    let options (title: string) (options: list<string list>): unit =
        options
        |> Options.optionsList removeMarkup (messages indentation) verbosity "-" title
        newLine()

    let simpleOptions (title: string) (options: list<string list>): unit =
        options
        |> Options.optionsList removeMarkup (messages indentation) verbosity "" title
        newLine()

    let groupedOptions (separator: string) (title: string) (options: list<string list>): unit =
        options
        |> Options.groupedOptionsList removeMarkup (messages indentation) verbosity separator title
        newLine()

    let list (messages: seq<string>): unit =
        if isNormal() then
            messages
            |> Seq.iter (sprintf " - %s" >> blockWithMarkup false None false)

    //
    // Complex components
    //

    let table (header: list<string>) (rows: list<list<string>>): unit =
        if isNormal() then
            let renderHeaderLine (headerLine: string) =
                Console.WriteLine(headerLine, TableHeader |> color)

            let renderRowLine = blockWithMarkup false None false

            Table.render removeMarkup renderHeaderLine renderRowLine header rows
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

    let progressStart (initialMessage: string) (total: int): ProgressBar =
        if isNormal() && Console.WindowWidth > 0 then
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

    let progressAdvance (progress: ProgressBar): unit =
        progress.Advance()

    let progressFinish (progress: ProgressBar): unit =
        progress.Finish()

    //
    // Inputs
    //

    let ask (question: string): string =
        Console.Write(question + " ", SubTitle |> color)
        Console.ReadLine()

    let askf format = format1 ask format
    let askf2 format = format2 ask format
    let askf3 format = format3 ask format
 *)

[<RequireQualifiedAccess>]
module ConsoleStyle =
    let createWithVerbosity verbosity =
        let consoleOutput = Output.ConsoleOutput(verbosity)
        ConsoleStyle(consoleOutput, Style.defaults)

    let create () =
        createWithVerbosity Verbosity.Normal
