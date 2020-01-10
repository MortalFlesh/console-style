namespace MF.ConsoleStyle

module internal Render =
    open System
    open System.Drawing
    open Colorful

    type private Render = {
        Normal: unit -> unit
        WithType: OutputType -> unit
        Error: unit -> unit
        WithMarkup: unit -> unit
    }

    let private normalizeColor (string: string) =
        string.ToLower().Trim().Replace("_", "").Replace("-", "")

    let internal color = function
        | Title -> Color.Cyan
        | SubTitle -> Color.Yellow
        | TableHeader -> Color.DarkGoldenrod
        | Success -> Color.LimeGreen
        | Error -> Color.Red
        | Number -> Color.Magenta
        | TextWithMarkup color ->
            match color |> Option.map normalizeColor with
            | Some "lightyellow"
            | Some "yellow" -> Color.Yellow
            | Some "darkyellow" -> Color.DarkGoldenrod

            | Some "lightred"
            | Some "red" -> Color.Red
            | Some "darkred" -> Color.DarkRed

            | Some "lightgreen"
            | Some "green" -> Color.LimeGreen
            | Some "darkgreen" -> Color.DarkGreen

            | Some "lightcyan"
            | Some "cyan"
            | Some "lightblue" -> Color.Cyan
            | Some "darkcyan"
            | Some "blue" -> Color.DarkCyan
            | Some "darkblue" -> Color.MidnightBlue

            | Some "lightpink"
            | Some "pink"
            | Some "lightmagenta"
            | Some "magenta" -> Color.Magenta
            | Some "darkpink"
            | Some "darkmagenta"
            | Some "purple" -> Color.Purple

            | Some "lightgray" -> Color.LightGray
            | Some "gray"
            | Some "darkgray"-> Color.Silver

            | Some "black" -> Color.Black

            | Some "white"
            | _ -> Color.White

    let (|ColorName|_|) = function
        | Some (color: Color) ->
            let color = color.Name
            let colorName =
                if color = Color.Yellow.Name then "yellow"
                elif color = Color.DarkGoldenrod.Name then "darkyellow"
                elif color = Color.Red.Name then "red"
                elif color = Color.DarkRed.Name then "darkred"
                elif color = Color.LimeGreen.Name then "green"
                elif color = Color.DarkGreen.Name then "darkgreen"
                elif color = Color.Cyan.Name then "lightblue"
                elif color = Color.DarkCyan.Name then "blue"
                elif color = Color.MidnightBlue.Name then "darkblue"
                elif color = Color.Magenta.Name then "pink"
                elif color = Color.Purple.Name then "darkpink"
                elif color = Color.LightGray.Name then "lightgray"
                elif color = Color.Silver.Name then "gray"
                elif color = Color.Black.Name then "black"
                elif color = Color.White.Name then "white"
                else ""
            if colorName <> "" then Some colorName
            else None
        | _ -> None

    let private eprintfn format =
        format |>
        Printf.kprintf (fun message ->
            let old = System.Console.ForegroundColor
            try
                System.Console.ForegroundColor <- ConsoleColor.Red
                System.Console.Error.WriteLine message
            finally
                System.Console.ForegroundColor <- old
        )

    let private render configuration = function
        | Some t ->
            match t with
            | Error -> configuration.Error()
            | TextWithMarkup _ -> configuration.WithMarkup()
            | _ -> configuration.WithType t
        | _ -> configuration.Normal()

    let private renderDateTime indentation outputType =
        if Verbosity.isVerbose() then
            let renderNormalDateTime _ =
                Console.Write("[")
                Console.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Number |> color)
                Console.Write(sprintf "]%s" indentation)

            outputType
            |> render {
                Normal = renderNormalDateTime
                WithType = renderNormalDateTime
                WithMarkup = renderNormalDateTime
                Error = (fun _ ->
                    Console.Error.Write("[")
                    Console.Error.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Number |> color)
                    Console.Error.Write(sprintf "]%s" indentation)
                )
            }

    let private renderUnderline underline length outputType =
        match Verbosity.isNormal(), underline with
        | true, Some char ->
            let underline = char |> String.replicate length

            outputType
            |> render {
                Normal = fun _ -> printfn "%s" underline
                WithMarkup = fun _ -> Console.WriteLine(underline)
                WithType = fun t -> Console.WriteLine(underline, t |> color)
                Error = fun _ -> eprintfn "%s" underline
            }
        | _ -> ()

    module Markup =
        let hasMarkup (message: string) =
            message.Contains "<c:"

        type private MessageParts = (string * Color option) list

        let private addNotEmptyPart (parts: MessageParts) color part =
            if part <> ""
            then (part, color) :: parts
            else parts

        let internal parseMarkup (message: string): MessageParts =
            let rec parseMarkup (parts: MessageParts) (message: string) =
                if message |> hasMarkup then
                    match message.Split("<c", 2) with
                    | [| before; withMarkup |] ->
                        let parts = before |> addNotEmptyPart parts None

                        let (message, color) =
                            match withMarkup.Split(">", 2) with
                            | [| colorName; text |] ->
                                let color =
                                    colorName.TrimStart ':'
                                    |> Some
                                    |> TextWithMarkup
                                    |> color

                                text, Some color
                            | _ -> message, None

                        match message.Split("</c>", 2) with
                        | [| text; rest |] ->
                            let texts = text |> addNotEmptyPart parts color

                            if rest |> hasMarkup then parseMarkup texts rest
                            else rest |> addNotEmptyPart texts None

                        | _ -> message |> addNotEmptyPart parts None
                    | _ -> message |> addNotEmptyPart parts None
                else message |> addNotEmptyPart parts None

            message
            |> parseMarkup []
            |> List.rev

        let (|HasMarkup|_|) = function
            | message when message |> hasMarkup -> message |> parseMarkup |> Some
            | _ -> None

        let removeMarkup = function
            | HasMarkup texts -> texts |> List.map fst |> String.concat ""
            | text -> text

        let toMessage (parts: MessageParts) =
            parts
            |> List.map (fun (part, color) ->
                match color with
                | ColorName colorName -> sprintf "<c:%s>%s</c>" colorName part
                | _ -> part
            )
            |> String.concat ""

        let printWithMarkup message =
            let rec printMarkup: MessageParts -> unit = function
                | [] -> Console.WriteLine()
                | (text, color) :: others ->
                    match color with
                    | Some color -> Console.Write(text, color)
                    | _ -> Console.Write(text)

                    others |> printMarkup

            message
            |> parseMarkup
            |> printMarkup

    let internal block indentation allowDateTime outputType underline withNewLine (message: string) =
        if allowDateTime then
            outputType
            |> renderDateTime indentation

        if Verbosity.isNormal() then
            outputType
            |> render {
                Normal = fun _ -> printfn "%s" message
                WithType = fun t -> Console.WriteLine(message, t |> color)
                Error = fun _ -> eprintfn "%s" message
                WithMarkup = fun _ -> Markup.printWithMarkup message
            }

            let underlineLength =
                if Verbosity.isVerbose() && allowDateTime
                    then message.Length + indentation.Length + 21   // 21 is length of time string in [DD/MM/YYYY HH:MM:SS]
                    else message.Length

            outputType
            |> renderUnderline underline underlineLength

            if withNewLine then
                outputType
                |> render {
                    Normal = fun _ -> printfn ""
                    WithMarkup = fun _ -> Console.WriteLine()
                    WithType = fun _ -> Console.WriteLine()
                    Error = fun _ -> eprintfn ""
                }
