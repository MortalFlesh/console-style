namespace MF.ConsoleStyle

[<RequireQualifiedAccess>]
module internal Options =
    open Words

    type private RawOptions = (string list) list
    type private Options = Options of Line list

    [<RequireQualifiedAccess>]
    module private RawOptions =
        let toOptions (rawOptions: RawOptions) =
            rawOptions
            |> List.map (List.map Word)
            |> Options

    [<RequireQualifiedAccess>]
    module private OptionLine =
        let toLine removeMarkup prefix maxWordLengths =
            Line.format removeMarkup (sprintf "%-*s") maxWordLengths
            >> fun words ->
                match prefix, words with
                | _, [] -> []
                | null, words
                | "", words -> words
                | prefix, (Word firstWord) :: words -> Word (sprintf "%s %s" prefix firstWord) :: words
            >> Line.concat "  "
            >> Style.Message.ofString

    [<RequireQualifiedAccess>]
    module private Options =
        let toRawLines removeMarkup (Options options) =
            let maxWordLengths = options |> MaxWordLengths.perWordsInLine removeMarkup

            options, maxWordLengths

        let toLines removeMarkup linePrefix options =
            let options, maxWordLengths = options |> toRawLines removeMarkup

            options
            |> List.map (OptionLine.toLine removeMarkup linePrefix maxWordLengths)

    let private renderSubTitle verbosity style (subTitle: string) =
        subTitle
        |> Style.Message.ofString
        |> Render.message verbosity style SubTitle

    let private renderMessage verbosity style (message: string) =
        message
        |> Style.Message.ofString
        |> Render.message verbosity style TextWithMarkup

    let optionsList removeMarkup verbosity style linePrefix title (options: RawOptions) =
        [
            yield renderSubTitle verbosity style title

            yield!
                options
                |> RawOptions.toOptions
                |> Options.toLines removeMarkup linePrefix
                |> List.map (Render.message verbosity { style with ShowDateTime = None } TextWithMarkup)
        ]

    let groupedOptionsList removeMarkup verbosity style (separator: string) (title: string) (options: RawOptions) =
        let style = { style with ShowDateTime = None }
        let groupName optionLine =
            match optionLine with
            | [] -> ""
            | (Word groupName) :: _ -> groupName |> removeMarkup

        let options, maxWordLengths =
            options
            |> RawOptions.toOptions
            |> Options.toRawLines removeMarkup

        let renderedSubTitle = renderSubTitle verbosity style title

        let renderedOptions =
            options
            |> List.map (fun words ->
                let groupName = words |> groupName
                let group =
                    if groupName.Contains separator
                    then Some (groupName.Split separator |> Array.head)
                    else None

                (group, words)
            )
            |> List.sortBy fst
            |> List.groupBy fst
            |> List.collect (fun (group, options) ->
                [
                    match group with
                    | Some group -> yield group |> sprintf " <c:dark-yellow>%s</c>" |> renderMessage verbosity style
                    | _ -> ()

                    yield!
                        options
                        |> List.map (snd >> (OptionLine.toLine removeMarkup "" maxWordLengths))
                        |> List.sortBy (fun message -> if message.HasMarkup then message.Text |> removeMarkup else message.Text)
                        |> List.map (Render.message verbosity style TextWithMarkup)
                ]
            )

        renderedSubTitle :: renderedOptions
