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

    [<RequireQualifiedAccess>]
    module private Options =
        let toRawLines removeMarkup (Options options) =
            let maxWordLengths = options |> MaxWordLengths.perWordsInLine removeMarkup

            options, maxWordLengths

        let toLines removeMarkup linePrefix options =
            let options, maxWordLengths = options |> toRawLines removeMarkup

            options
            |> List.map (OptionLine.toLine removeMarkup linePrefix maxWordLengths >> Line.concat "  ")

    let private renderSubTitle verbosity (subTitle: string): unit =
        subTitle
        |> Render.block verbosity "" false (Some SubTitle) None false

    let private renderMessage verbosity (message: string): unit =
        message
        |> Render.block verbosity "" false (Some TextWithMarkup) None false

    let optionsList removeMarkup renderLines verbosity linePrefix title (options: RawOptions) =
        renderSubTitle verbosity title
        options
        |> RawOptions.toOptions
        |> Options.toLines removeMarkup linePrefix
        |> renderLines

    let groupedOptionsList removeMarkup renderLines verbosity (separator: string) (title: string) (options: RawOptions): unit =
        let groupName optionLine =
            match optionLine with
            | [] -> ""
            | (Word groupName) :: _ -> groupName |> removeMarkup

        let options, maxWordLengths =
            options
            |> RawOptions.toOptions
            |> Options.toRawLines removeMarkup

        renderSubTitle verbosity title

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
        |> List.iter (fun (group, options) ->
            group |> Option.iter (sprintf " <c:dark-yellow>%s</c>" >> renderMessage verbosity)
            options
            |> List.map (snd >> (OptionLine.toLine removeMarkup "" maxWordLengths) >> Line.concat "  ")
            |> List.sortBy removeMarkup
            |> renderLines
        )
