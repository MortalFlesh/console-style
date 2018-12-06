namespace MF.ConsoleStyle

module private Render =
    open System
    open System.Drawing
    open Colorful
    open Types

    type private Render = {
        Normal: unit -> unit
        WithType: OutputType -> unit
        Error: unit -> unit
    }

    let color = function
        | Title -> Color.Cyan
        | SubTitle -> Color.Yellow
        | TableHeader -> Color.DarkGoldenrod
        | Success -> Color.LimeGreen
        | Error -> Color.Red
        | Number -> Color.Magenta

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
            | _ -> configuration.WithType t
        | _ -> configuration.Normal()

    let private renderDateTime indentation outputType =
        match Verbosity.isVerbose() with
        | true ->
            let renderNormalDateTime _ =
                Console.Write("[")
                Console.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Number |> color)
                Console.Write(sprintf "]%s" indentation)

            outputType
            |> render {
                Normal = renderNormalDateTime
                WithType = renderNormalDateTime
                Error = (fun _ ->
                    Console.Error.Write("[")
                    Console.Error.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Number |> color)
                    Console.Error.Write(sprintf "]%s" indentation)
                )
            }
        | _ -> ()

    let private renderUnderline underline length outputType =
        match Verbosity.isNormal(), underline with
        | true, Some char ->
            let underline = char |> String.replicate length

            outputType
            |> render {
                Normal = fun _ -> printfn "%s" underline
                WithType = fun t -> Console.WriteLine(underline, t |> color)
                Error = fun _ -> eprintfn "%s" underline
            }
        | _ -> ()

    let block indentation outputType underline withNewLine (message: string) =
        outputType
        |> renderDateTime indentation

        match Verbosity.isNormal() with
        | true ->
            outputType
            |> render {
                Normal = fun _ -> printfn "%s" message
                WithType = fun t -> Console.WriteLine(message, t |> color)
                Error = fun _ -> eprintfn "%s" message
            }

            let underlineLength =
                if Verbosity.isVerbose()
                    then message.Length + indentation.Length + 21   // 21 is length of time string in [DD/MM/YYYY HH:MM:SS]
                    else message.Length

            outputType
            |> renderUnderline underline underlineLength

            if withNewLine then
                outputType
                |> render {
                    Normal = fun _ -> printfn ""
                    WithType = fun _ -> printfn ""
                    Error = fun _ -> eprintfn ""
                }
        | _ -> ()
