namespace MF.ConsoleStyle

module private Options =
    open Render.Markup

    let private subTitle (subTitle: string): unit =
        subTitle
        |> Render.block "" false (Some SubTitle) None false

    let private message (message: string): unit =
        message
        |> Render.block "" false (Some (TextWithMarkup None)) None false

    let private toLine prefix maxLength (option, description) =
        match option with
        | HasMarkup parts ->
            let (lastPart, lastPartColor), rest =
                match parts |> List.rev with
                | [] -> ("", None), []  // this case should never happen, since `HasMarkup` only marks messages with at least one markup
                | (part, color) :: otherParts -> (part, color), otherParts

            let fullMessageLength = parts |> List.sumBy (fst >> String.length)
            let lastPartIndentation = maxLength - fullMessageLength + lastPart.Length
            let lastPartIndented = sprintf "%-*s" (lastPartIndentation + 1) lastPart

            let optionWithMarkup =
                (lastPartIndented, lastPartColor) :: rest
                |> List.rev
                |> toMessage

            sprintf "%s%s %s" prefix optionWithMarkup description
        | optionWithoutMarkup ->
            sprintf "%s%-*s %-s" prefix (maxLength + 1) optionWithoutMarkup description

    let optionsList messages linePrefix maxLength title options =
        subTitle title
        options
        |> Seq.map (toLine linePrefix maxLength)
        |> messages

    let groupedOptionsList messages maxLength (separator: string) (title: string) (options: seq<string * string>): unit =
        subTitle title
        options
        |> Seq.map (fun (option, description) ->
            let optionText = option |> removeMarkup

            let group =
                if optionText.Contains separator
                then Some (optionText.Split separator |> Array.head)
                else None

            (group, toLine "" maxLength (option, description))
        )
        |> Seq.sortBy fst
        |> Seq.groupBy fst
        |> Seq.iter (fun (group, options) ->
            group |> Option.map (sprintf " <c:dark-yellow>%s</c>" >> message) |> ignore
            options
            |> Seq.map snd
            |> Seq.sortBy removeMarkup
            |> messages
        )
