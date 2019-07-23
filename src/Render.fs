namespace MF.ConsoleStyle

module private Render =
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

    let color = function
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

    let rec private printWithMarkup (message: string) =
        if message.Contains("<c:") then
            match message.Split("<c", 2) with
            | [| before; withMarkup |] ->
                Console.Write(before)

                let (messageColor, message) =
                    match withMarkup.Split(">", 2) with
                    | [| color; text |] -> (Some (color.TrimStart ':'), text)
                    | _ -> (None, message)

                match message.Split("</c>", 2) with
                | [| textToColor; rest |] ->
                    Console.Write(textToColor, TextWithMarkup messageColor |> color)

                    if rest.Contains "<c:" then printWithMarkup rest
                    else Console.WriteLine(rest)

                | _ -> Console.WriteLine(message)
            | _ -> Console.WriteLine(message)
        else Console.WriteLine(message)

    let block indentation allowDateTime outputType underline withNewLine (message: string) =
        if allowDateTime then
            outputType
            |> renderDateTime indentation

        if Verbosity.isNormal() then
            outputType
            |> render {
                Normal = fun _ -> printfn "%s" message
                WithType = fun t -> Console.WriteLine(message, t |> color)
                Error = fun _ -> eprintfn "%s" message
                WithMarkup = fun _ -> printWithMarkup message
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
