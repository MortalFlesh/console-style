namespace MF.ConsoleStyle

open System

type ConsoleStyle (output: Output.IOutput, style) =
    let mutable output = output
    let mutable style = style
    let removeMarkup = Style.removeMarkup style

    // Constructors
    new (style) = ConsoleStyle(Output.ConsoleOutput(Verbosity.Normal), style)
    new (output) = ConsoleStyle(output, Style.defaults)
    new (verbosity) = ConsoleStyle(Output.ConsoleOutput(verbosity))
    new () = ConsoleStyle (Verbosity.Normal)

    // Verbosity
    member _.Verbosity
        with get() = output.Verbosity
        and set(value) = output.Verbosity <- value

    member _.IsQuiet() = output.IsQuiet()
    member _.IsNormal() = output.IsNormal()
    member _.IsVerbose() = output.IsVerbose()
    member _.IsVeryVerbose() = output.IsVeryVerbose()
    member _.IsDebug() = output.IsDebug()

    // Output
    member _.ChangeOutput(newOutput) = output <- newOutput

    // Style
    member _.ChangeStyle(newStyle) = style <- newStyle
    member _.UpdateStyle(f) = style <- f style
    member _.Indent message = sprintf "%s%s" (style.Indentation |> Indentation.value) message
    member _.RemoveMarkup(message) = removeMarkup message

    // Output

    /// It will never show a date time
    member this.Write (message: string) =
        if this.IsNormal() then
            message
            |> Style.Message.ofString
            |> Render.message output.Verbosity { style with ShowDateTime = NoDateTime } OutputType.TextWithMarkup
            |> RenderedMessage.value
            |> output.Write

    member this.Write (format, a) = sprintf format a |> this.Write
    member this.Write (format, a, b) = sprintf format a b |> this.Write
    member this.Write (format, a, b, c) = sprintf format a b c |> this.Write
    member this.Write (format, a, b, c, d) = sprintf format a b c d |> this.Write
    member this.Write (format, a, b, c, d, e) = sprintf format a b c d e |> this.Write

    /// It will never show a date time
    member this.WriteLine (message: string) =
        if this.IsNormal() then
            message
            |> Style.Message.ofString
            |> Render.message output.Verbosity { style with ShowDateTime = NoDateTime } OutputType.TextWithMarkup
            |> RenderedMessage.value
            |> output.WriteLine

    member this.WriteLine (format, a) = sprintf format a |> this.WriteLine
    member this.WriteLine (format, a, b) = sprintf format a b |> this.WriteLine
    member this.WriteLine (format, a, b, c) = sprintf format a b c |> this.WriteLine
    member this.WriteLine (format, a, b, c, d) = sprintf format a b c d |> this.WriteLine
    member this.WriteLine (format, a, b, c, d, e) = sprintf format a b c d e |> this.WriteLine

    /// It works as WriteLine but adds a date time when style allows it
    member this.Message (message: string) =
        if this.IsNormal() then
            message
            |> Style.Message.ofString
            |> Render.message output.Verbosity style OutputType.TextWithMarkup
            |> RenderedMessage.value
            |> output.WriteLine

    member this.Message (format, a) = sprintf format a |> this.Message
    member this.Message (format, a, b) = sprintf format a b |> this.Message
    member this.Message (format, a, b, c) = sprintf format a b c |> this.Message
    member this.Message (format, a, b, c, d) = sprintf format a b c d |> this.Message
    member this.Message (format, a, b, c, d, e) = sprintf format a b c d e |> this.Message

    member this.Title (title: string) =
        if this.IsNormal() then
            title
            |> Style.Message.ofString
            |> Render.message output.Verbosity style OutputType.Title
            |> RenderedMessage.value
            |> sprintf "%s\n"
            |> output.WriteLine

    member this.Title (format, a) = sprintf format a |> this.Title
    member this.Title (format, a, b) = sprintf format a b |> this.Title
    member this.Title (format, a, b, c) = sprintf format a b c |> this.Title
    member this.Title (format, a, b, c, d) = sprintf format a b c d |> this.Title
    member this.Title (format, a, b, c, d, e) = sprintf format a b c d e |> this.Title

    member this.SubTitle (subTitle: string) =
        if this.IsNormal() then
            subTitle
            |> Style.Message.ofString
            |> Render.message output.Verbosity style OutputType.SubTitle
            |> RenderedMessage.value
            |> output.WriteLine

    member this.SubTitle (format, a) = sprintf format a |> this.SubTitle
    member this.SubTitle (format, a, b) = sprintf format a b |> this.SubTitle
    member this.SubTitle (format, a, b, c) = sprintf format a b c |> this.SubTitle
    member this.SubTitle (format, a, b, c, d) = sprintf format a b c d |> this.SubTitle
    member this.SubTitle (format, a, b, c, d, e) = sprintf format a b c d e |> this.SubTitle

    member this.Section (section: string) =
        if this.IsNormal() then
            section
            |> Style.Message.ofString
            |> Render.message output.Verbosity style OutputType.Section
            |> RenderedMessage.value
            |> sprintf "%s\n"
            |> output.WriteLine

    member this.Section (format, a) = sprintf format a |> this.Section
    member this.Section (format, a, b) = sprintf format a b |> this.Section
    member this.Section (format, a, b, c) = sprintf format a b c |> this.Section
    member this.Section (format, a, b, c, d) = sprintf format a b c d |> this.Section
    member this.Section (format, a, b, c, d, e) = sprintf format a b c d e |> this.Section

    member this.NewLine (): unit =
        if this.IsNormal() then
            output.WriteLine("")

    member this.MainTitle (title: string, figlet: Colorful.Figlet): unit =
        if this.IsNormal() then
            figlet.ToAscii(title |> removeMarkup).ConcreteValue
            |> Style.Message.ofString
            |> Render.message output.Verbosity style OutputType.MainTitle
            |> RenderedMessage.value
            |> sprintf "%s\n"
            |> output.WriteLine

    member this.MainTitle (title: string, font: Colorful.FigletFont): unit =
        this.MainTitle(title, Colorful.Figlet(font))

    member this.MainTitle (title: string): unit =
        this.MainTitle(title, Colorful.Figlet())

    member this.Error (error: string) =
        if this.IsNormal() then
            error
            |> Style.Message.ofString
            |> Render.message output.Verbosity style OutputType.Error
            |> RenderedMessage.value
            |> sprintf "%s\n"
            |> output.WriteErrorLine

    member this.Error (format, a) = sprintf format a |> this.Error
    member this.Error (format, a, b) = sprintf format a b |> this.Error
    member this.Error (format, a, b, c) = sprintf format a b c |> this.Error
    member this.Error (format, a, b, c, d) = sprintf format a b c d |> this.Error
    member this.Error (format, a, b, c, d, e) = sprintf format a b c d e |> this.Error

    member this.Warning (warning: string) =
        if this.IsNormal() then
            warning
            |> Style.Message.ofString
            |> Render.message output.Verbosity style OutputType.Warning
            |> RenderedMessage.value
            |> sprintf "%s\n"
            |> output.WriteLine

    member this.Warning (format, a) = sprintf format a |> this.Warning
    member this.Warning (format, a, b) = sprintf format a b |> this.Warning
    member this.Warning (format, a, b, c) = sprintf format a b c |> this.Warning
    member this.Warning (format, a, b, c, d) = sprintf format a b c d |> this.Warning
    member this.Warning (format, a, b, c, d, e) = sprintf format a b c d e |> this.Warning

    member this.Success (success: string) =
        if this.IsNormal() then
            success
            |> Style.Message.ofString
            |> Render.message output.Verbosity style OutputType.Success
            |> RenderedMessage.value
            |> sprintf "%s\n"
            |> output.WriteLine

    member this.Success (format, a) = sprintf format a |> this.Success
    member this.Success (format, a, b) = sprintf format a b |> this.Success
    member this.Success (format, a, b, c) = sprintf format a b c |> this.Success
    member this.Success (format, a, b, c, d) = sprintf format a b c d |> this.Success
    member this.Success (format, a, b, c, d, e) = sprintf format a b c d e |> this.Success

    member this.Note (note: string) =
        if this.IsNormal() then
            note
            |> Style.Message.ofString
            |> Render.message output.Verbosity style OutputType.Note
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
            |> Seq.iter (sprintf "%s%s" prefix >> this.WriteLine)

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
                |> Render.message output.Verbosity style OutputType.TableHeader
                |> RenderedMessage.value
                |> output.WriteLine

            let renderRowLine = this.WriteLine

            Table.render removeMarkup renderHeaderLine renderRowLine
                header
                rows

            this.NewLine()

    member this.Tabs(tabs) =
        if this.IsNormal() then
            Tabs.render removeMarkup this.WriteLine tabs
            this.NewLine()

    member this.Tabs(tabs, length) =
        if this.IsNormal() then
            Tabs.renderInLength removeMarkup this.WriteLine length tabs
            this.NewLine()

    //
    // User input
    //

    member _.Ask (question: string): string =
        question
        |> Style.Message.ofString
        |> Render.message output.Verbosity style OutputType.SubTitle
        |> RenderedMessage.value
        |> sprintf "%s "
        |> output.Write

        Console.ReadLine()

    member this.Ask (format, a) = sprintf format a |> this.Ask
    member this.Ask (format, a, b) = sprintf format a b |> this.Ask
    member this.Ask (format, a, b, c) = sprintf format a b c |> this.Ask
    member this.Ask (format, a, b, c, d) = sprintf format a b c d |> this.Ask
    member this.Ask (format, a, b, c, d, e) = sprintf format a b c d e |> this.Ask

    //
    // Progress bar
    //

    member this.ProgressStart (initialMessage: string) (total: int): ProgressBar =
        if this.IsNormal() then ProgressBar.start initialMessage total
        else Inactive

    member this.ProgressStartChild (progress: ProgressBar) (initialMessage: string) (total: int): ProgressBar =
        if this.IsNormal() then ProgressBar.startAsChild progress initialMessage total
        else Inactive

    member this.ProgressStartChildAndKeepIt (progress: ProgressBar) (initialMessage: string) (total: int): ProgressBar =
        if this.IsNormal() then ProgressBar.startAsChildAndKeepIt progress initialMessage total
        else Inactive

    member _.ProgressAdvance (progress: ProgressBar): unit =
        progress.Advance()

    member _.ProgressFinish (progress: ProgressBar): unit =
        progress.Finish()
