namespace MF.ConsoleStyle

module private Options =
    let private subTitle (subTitle: string): unit =
        subTitle
        |> Render.block "" false (Some SubTitle) None false

    let private toLine prefix maxLength (option, description) =
        sprintf "%s%-*s %-s" prefix (maxLength + 1) option description

    let optionsList messages linePrefix maxLength title options =
        subTitle title
        options
        |> Seq.map (toLine linePrefix maxLength)
        |> messages

    let groupedOptionsList messages maxLength (separator: string) (title: string) (options: seq<string * string>): unit =
        subTitle title
        options
        |> Seq.map (fun (option, description) ->
            let group =
                if option.Contains separator
                then Some (option.Split separator |> Array.head)
                else None

            (group, toLine "" maxLength (option, description))
        )
        |> Seq.sortBy fst
        |> Seq.groupBy fst
        |> Seq.iter (fun (group, options) ->
            group |> Option.map (sprintf " %s" >> subTitle) |> ignore
            options
            |> Seq.map snd
            |> Seq.sort
            |> messages
        )
