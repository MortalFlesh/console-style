namespace MF.ConsoleStyle

[<RequireQualifiedAccess>]
module internal Options =
    open Words
    open Render.Markup

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
        let toLine prefix maxWordLengths =
            Line.format removeMarkup (sprintf "%-*s") maxWordLengths
            >> fun words ->
                match prefix, words with
                | _, [] -> []
                | null, words
                | "", words -> words
                | prefix, (Word firstWord) :: words -> Word (sprintf "%s %s" prefix firstWord) :: words

    [<RequireQualifiedAccess>]
    module private Options =
        let toRawLines (Options options) =
            let maxWordLengths = options |> MaxWordLengths.perWordsInLine removeMarkup

            options, maxWordLengths

        let toLines linePrefix options =
            let options, maxWordLengths = options |> toRawLines

            options
            |> List.map (OptionLine.toLine linePrefix maxWordLengths >> Line.concat "  ")

    let private renderSubTitle (subTitle: string): unit =
        subTitle
        |> Render.block "" false (Some SubTitle) None false

    let private renderMessage (message: string): unit =
        message
        |> Render.block "" false (Some (TextWithMarkup None)) None false

    let optionsList renderLines linePrefix title (options: RawOptions) =
        renderSubTitle title
        options
        |> RawOptions.toOptions
        |> Options.toLines linePrefix
        |> renderLines

    let groupedOptionsList renderLines (separator: string) (title: string) (options: RawOptions): unit =
        let groupName optionLine =
            match optionLine with
            | [] -> ""
            | (Word groupName) :: _ -> groupName |> removeMarkup

        let options, maxWordLengths =
            options
            |> RawOptions.toOptions
            |> Options.toRawLines

        renderSubTitle title

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
            group |> Option.iter (sprintf " <c:dark-yellow>%s</c>" >> renderMessage)
            options
            |> List.map (snd >> (OptionLine.toLine "" maxWordLengths) >> Line.concat "  ")
            |> List.sortBy removeMarkup
            |> renderLines
        )
