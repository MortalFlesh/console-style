namespace MF.ConsoleStyle

module internal Render =
    open System
    open System.Drawing
    open Colorful

    type private SystemConsole = System.Console
    type private ColorfulConsole = Colorful.Console
    type [<Obsolete("use expclicit console")>] private Console = Console

    type private Render = {
        [<Obsolete("Is it necessary?")>] OnLine: unit -> unit
        Normal: unit -> unit
        WithType: OutputType -> unit
        Error: unit -> unit
        WithMarkup: unit -> unit
    }

    [<Obsolete("Dont have default")>]
    let [<Literal>] DefaultIndentation = "    "

    let private eprintfn format =
        format |>
        Printf.kprintf (fun message ->
            let old = SystemConsole.ForegroundColor
            try
                SystemConsole.ForegroundColor <- ConsoleColor.Red
                SystemConsole.Error.WriteLine message
            finally
                SystemConsole.ForegroundColor <- old
        )

    let private render configuration = function
        | Some Error -> configuration.Error()
        | Some (TextWithMarkup _) -> configuration.WithMarkup()
        | Some OnLine -> configuration.OnLine()
        | Some t -> configuration.WithType t
        | _ -> configuration.Normal()

    let private renderDateTimeValue verbosity showDateTime: string option =
        let format =
            match showDateTime with
            | ShowDateTime -> Some "yyyy-MM-dd HH:mm:ss"
            | ShowDateTimeAs format -> Some format
            | ShowDateTimeFrom fromVerbosity when fromVerbosity |> Verbosity.isSameOrAbove verbosity -> Some "yyyy-MM-dd HH:mm:ss"
            | ShowDateTimeFromAs (fromVerbosity, format) when fromVerbosity |> Verbosity.isSameOrAbove verbosity -> Some format
            | _ -> None

        format
        |> Option.map (fun format -> DateTime.Now.ToString(format) |> sprintf "[%s]")

    [<Obsolete("Use a renderDateTimeValue")>]
    let private renderDateTime verbosity indentation outputType =
        // todo - allow to set a verbosity for a date time to show (or use a default)
        if Verbosity.isVeryVerbose verbosity then
            let renderNormalDateTime _ =
                SystemConsole.Write("[")
                SystemConsole.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Number |> OutputType.color)
                SystemConsole.Write(sprintf "]%s" indentation)

            outputType
            |> render {
                OnLine = renderNormalDateTime
                Normal = renderNormalDateTime
                WithType = renderNormalDateTime
                WithMarkup = renderNormalDateTime
                Error = (fun _ ->
                    SystemConsole.Error.Write("[")
                    SystemConsole.Error.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Number |> OutputType.color)
                    SystemConsole.Error.Write(sprintf "]%s" indentation)
                )
            }

    let private renderUnderlineValue underline length =
        underline |> String.replicate length

    [<Obsolete("Use a renderUnderlineValue")>]
    let private renderUnderline verbosity underline length outputType =
        match Verbosity.isNormal verbosity, underline with
        | true, Some char ->
            let underline = char |> String.replicate length

            outputType
            |> render {
                OnLine = ignore
                Normal = fun _ -> SystemConsole.WriteLine underline
                WithMarkup = fun _ -> SystemConsole.WriteLine underline
                WithType = fun t -> ColorfulConsole.WriteLine(underline, t |> OutputType.color)
                Error = fun _ -> SystemConsole.Error.WriteLine underline
            }
        | _ -> ()

    let private renderSuccess message =
        let prefix = " [OK] "
        let prefixLength = prefix.Length
        let length = max 120 (prefixLength + message.LengthWithoutMarkup + 1)

        let line = String.replicate length " "
        let render = OutputType.formatSuccess >> Markup.render

        [
            line
            sprintf "%s%s%s"
                prefix
                (if message.HasMarkup then message.Text |> Markup.render else message.Text)
                (String.replicate (length - message.LengthWithoutMarkup - prefixLength) " " |> render)
            line
        ]
        |> List.map render
        |> String.concat "\n"

    let internal block verbosity indentation allowDateTime outputType underline withNewLine (message: string) =
        if allowDateTime then
            outputType
            |> renderDateTime verbosity indentation

        if Verbosity.isNormal verbosity then
            outputType
            |> render {
                OnLine = fun _ -> SystemConsole.Write message
                Normal = fun _ -> SystemConsole.WriteLine message
                WithType = fun t -> SystemConsole.WriteLine(message, t |> OutputType.color)
                Error = fun _ -> SystemConsole.Error.WriteLine message
                WithMarkup = fun _ -> message |> Markup.render |> SystemConsole.WriteLine
            }

            let underlineLength =
                /// 21 is length of time string in [DD/MM/YYYY HH:MM:SS]
                let dateTimeLength = 21

                if Verbosity.isVerbose verbosity && allowDateTime
                    then message.Length + indentation.Length + dateTimeLength
                    else message.Length

            outputType
            |> renderUnderline verbosity underline underlineLength

            if withNewLine then
                outputType
                |> render {
                    OnLine = ignore
                    Normal = fun _ -> SystemConsole.WriteLine()
                    WithMarkup = fun _ -> SystemConsole.WriteLine()
                    WithType = fun _ -> SystemConsole.WriteLine()
                    Error = fun _ -> SystemConsole.Error.WriteLine()
                }

    let message verbosity (style: Style) outputType ({ Text = text } as message: Message): RenderedMessage =
        if verbosity |> Verbosity.isNormal then
            let indentation =
                match style with
                | Style.IsIndentated indentation -> indentation
                | _ -> DefaultIndentation

            let mutable dateTimeLength = 0

            // todo handle outputType (probably with some markings before/after messages/parts)

            let parts: string list =
                [
                    match style with
                    | Style.HasShowDateTime showDateTime ->
                        // yield color outputType // todo - render date time in a color of the output
                        match renderDateTimeValue verbosity showDateTime with
                        | Some dateTime ->
                            yield dateTime
                            dateTimeLength <- dateTimeLength + dateTime.Length

                            yield indentation
                            dateTimeLength <- dateTimeLength + indentation.Length
                        | _ -> ()
                    | _ -> ()

                    match outputType, message.HasMarkup with
                    | OutputType.OnLine, _ -> yield text

                    | OutputType.Title, false -> yield text |> OutputType.formatTitle |> Markup.render
                    | OutputType.SubTitle, false -> yield text |> OutputType.formatSubTitle |> Markup.render
                    | OutputType.Section, false -> yield text |> OutputType.formatSection |> Markup.render
                    | OutputType.TableHeader, false -> yield text |> OutputType.formatTableHeader |> Markup.render

                    | OutputType.Success, _ -> yield renderSuccess message

                    | _, true -> yield Markup.render text
                    | _, false -> yield text

                    // Underline
                    match outputType, style with
                    // todo - title/subtitle with markup wont have underline atm
                    | Title, _ -> yield "\n" + renderUnderlineValue "=" (message.LengthWithoutMarkup + dateTimeLength) |> OutputType.formatTitle |> Markup.render
                    | Section, _ -> yield "\n" + renderUnderlineValue "-" (message.LengthWithoutMarkup + dateTimeLength) |> OutputType.formatSection |> Markup.render

                    | _, Style.ShowUnderline style -> yield renderUnderlineValue style (message.Length + dateTimeLength)
                    | _ -> ()
                ]

            parts |> String.concat "" |> RenderedMessage
        else RenderedMessage.empty
