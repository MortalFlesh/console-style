namespace MF.ConsoleStyle

module internal Render =
    open System

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

    let private renderUnderlineValue underline length =
        underline |> String.replicate length

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

    let message verbosity (style: Style) outputType message: RenderedMessage =
        if verbosity |> Verbosity.isNormal then
            let ({ Text = text } as message: Message) = message |> Style.Message.applyCustomTags style

            let indentation =
                match style with
                | Style.IsIndentated indentation -> indentation
                | _ -> Style.DefaultIndentation

            let mutable dateTimeLength = 0

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
