namespace MF.ConsoleStyle

module internal Render =
    open System

    let private renderDateTimeValue verbosity showDateTime: string option =
        let format =
            match showDateTime with
            | ShowDateTimeAs format -> Some format
            | ShowDateTimeFor formats when formats.ContainsKey verbosity -> Some formats[verbosity]
            | _ -> None

        format
        |> Option.map (DateTime.Now.ToString >> sprintf "[%s]")

    [<RequireQualifiedAccess>]
    module private Block =
        let private renderBlock dateTime format (prefix: string) maxLength prefixLengthFixer (message: Message) =
            let prefix =
                match dateTime with
                | Some dateTime -> sprintf "%s%s" dateTime prefix
                | _ -> prefix
            let prefixLength = prefix.Length

            let trailingSpace = 1
            let render = format >> Markup.render

            let line length = String.replicate length " "
            let firstMessageLine length message =
                sprintf "%s%s%s"
                    prefix
                    (if message.HasMarkup then message.Text |> Markup.render else message.Text)
                    (String.replicate (length - message.LengthWithoutMarkup - prefixLength + prefixLengthFixer) " " |> render)

            let lines =
                if message.Text.Contains "\n" then
                    let lines = message.Text.Split("\n")
                    let messagesLines =
                        lines
                        |> Seq.map Style.Message.ofString
                        |> List.ofSeq

                    let maxLineLength =
                        messagesLines
                        |> Seq.map (fun m -> m.LengthWithoutMarkup)
                        |> Seq.max

                    let lineLength = max maxLength (prefixLength + maxLineLength + trailingSpace)
                    let line = line lineLength

                    let messageLine i message =
                        if i = 0 then firstMessageLine lineLength message
                        else
                            let indentionLenght = prefixLength - prefixLengthFixer
                            sprintf "%s%s%s"
                                (String.replicate indentionLenght " ")
                                (if message.HasMarkup then message.Text |> Markup.render else message.Text)
                                (String.replicate (lineLength - message.LengthWithoutMarkup - indentionLenght) " " |> render)

                    [
                        yield line

                        yield!
                            message.Text.Split("\n")
                            |> Seq.mapi (fun i -> Style.Message.ofString >> messageLine i)

                        yield line
                    ]
                else
                    let lineLength = max maxLength (prefixLength + message.LengthWithoutMarkup + trailingSpace)
                    let line = line lineLength

                    [
                        line
                        firstMessageLine lineLength message
                        line
                    ]

            lines
            |> List.map render
            |> String.concat "\n"

        let renderError dateTime message =
            message
            |> renderBlock dateTime OutputType.formatError " ☠️  " message.LengthWithoutMarkup +1

        let renderSuccess style dateTime =
            renderBlock dateTime OutputType.formatSuccess " ✅ " style.BlockLength -1

        let renderWarning style dateTime =
            renderBlock dateTime OutputType.formatWarning " ⚠️  " style.BlockLength +1

    let message verbosity (style: Style) outputType message: RenderedMessage =
        if verbosity |> Verbosity.isNormal then
            let ({ Text = text } as message: Message) = message |> Style.Message.applyCustomTags style

            let dateTime =
                style.ShowDateTime
                |> renderDateTimeValue verbosity
                |> Option.map (fun dateTime -> dateTime + (style.Indentation |> Indentation.value))

            let parts: string list =
                [
                    match outputType, dateTime with
                    | OutputType.SubTitle, Some dateTime
                    | OutputType.Note, Some dateTime
                    | OutputType.TextWithMarkup, Some dateTime -> yield dateTime |> OutputType.formatDateTime |> Markup.render
                    | OutputType.Error, Some dateTime when message.Lines > 2 -> yield dateTime |> OutputType.formatDateTime |> Markup.render
                    | _ -> ()

                    match outputType, message with
                    | OutputType.MainTitle, _ -> yield text |> OutputType.formatMainTitle |> Markup.render
                    | OutputType.Title, { HasMarkup = false } -> yield text |> OutputType.formatTitle |> Markup.render
                    | OutputType.SubTitle, { HasMarkup = false } -> yield text |> OutputType.formatSubTitle |> Markup.render
                    | OutputType.Section, { HasMarkup = false } -> yield text |> OutputType.formatSection |> Markup.render
                    | OutputType.TableHeader, { HasMarkup = false } -> yield text |> OutputType.formatTableHeader |> Markup.render
                    | OutputType.Note, { HasMarkup = false } -> yield text |> OutputType.formatNote |> Markup.render

                    | OutputType.Error, { Lines = lines } when lines > 2 -> yield text |> OutputType.formatSimpleError |> Markup.render
                    | OutputType.Error, _ -> yield message |> Block.renderError dateTime
                    | OutputType.Success, _ -> yield message |> Block.renderSuccess style dateTime
                    | OutputType.Warning, _ -> yield message |> Block.renderWarning style dateTime

                    | _, { HasMarkup = true } -> yield Markup.render text
                    | _, { HasMarkup = false } -> yield text

                    // Underline
                    match outputType, style with
                    | OutputType.MainTitle, { MainTitleUnderline = Underline.IsSet underline } ->
                        let length = message.Text.Split("\n") |> Seq.map Seq.length |> Seq.max
                        yield "\n" + (underline |> Underline.inLength length) |> OutputType.formatMainTitle |> Markup.render

                    | OutputType.Title, { TitleUnderline = Underline.IsSet underline } ->
                        yield "\n" + (underline |> Underline.inLength message.LengthWithoutMarkup) |> OutputType.formatTitle |> Markup.render

                    | OutputType.Section, { SectionUnderline = Underline.IsSet underline } ->
                        yield "\n" + (underline |> Underline.inLength message.LengthWithoutMarkup) |> OutputType.formatSection |> Markup.render

                    | _ -> ()
                ]

            parts |> String.concat "" |> RenderedMessage
        else RenderedMessage.empty
