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
        |> Option.map (fun format -> DateTime.Now.ToString(format) |> sprintf "[%s]")

    let private renderUnderlineValue (Underline underline) length =
        underline |> String.replicate length

    let private renderSuccess' message =
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

    let private renderBlock format (prefix: string) maxLength message =
        let prefixLength = prefix.Length
        let length = max maxLength (prefixLength + message.LengthWithoutMarkup + 1)

        let line = String.replicate length " "
        let render = format >> Markup.render

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

    let private renderError message =
        renderBlock OutputType.formatError " " message.LengthWithoutMarkup message

    let private renderSuccess =
        renderBlock OutputType.formatSuccess " [OK] " 120

    let private renderNote =
        renderBlock OutputType.formatSuccess " !Note " 120

    let message verbosity (style: Style) outputType message: RenderedMessage =
        if verbosity |> Verbosity.isNormal then
            let ({ Text = text } as message: Message) = message |> Style.Message.applyCustomTags style
            let (Indentation indentation) = style.Indentation

            let mutable dateTimeLength = 0

            let parts: string list =
                [
                    match style.ShowDateTime |> renderDateTimeValue verbosity with
                    | Some dateTime ->
                        yield sprintf "<c:gray%s</c>" dateTime
                        dateTimeLength <- dateTimeLength + dateTime.Length

                        yield indentation
                        dateTimeLength <- dateTimeLength + indentation.Length
                    | _ -> ()

                    match outputType, message.HasMarkup with
                    | OutputType.Title, false -> yield text |> OutputType.formatTitle |> Markup.render
                    | OutputType.SubTitle, false -> yield text |> OutputType.formatSubTitle |> Markup.render
                    | OutputType.Section, false -> yield text |> OutputType.formatSection |> Markup.render
                    | OutputType.TableHeader, false -> yield text |> OutputType.formatTableHeader |> Markup.render
                    | OutputType.Note, false -> yield text |> OutputType.formatNote |> Markup.render

                    | OutputType.Error, _ -> yield renderError message
                    | OutputType.Success, _ -> yield renderSuccess message

                    | _, true -> yield Markup.render text
                    | _, false -> yield text

                    // Underline
                    match outputType, style with
                    | Title, _ -> yield "\n" + renderUnderlineValue style.TitleUnderline (message.LengthWithoutMarkup + dateTimeLength) |> OutputType.formatTitle |> Markup.render
                    | Section, _ -> yield "\n" + renderUnderlineValue style.SubTitleUnderline (message.LengthWithoutMarkup + dateTimeLength) |> OutputType.formatSection |> Markup.render

                    | _ -> ()
                ]

            parts |> String.concat "" |> RenderedMessage
        else RenderedMessage.empty
