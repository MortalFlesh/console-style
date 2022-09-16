namespace MF.ConsoleStyle.Output

open MF.ConsoleStyle

type private SystemConsole = System.Console

type PrintOutput (verbosity) =
    let mutable verbosity: Verbosity.Level = verbosity

    interface IOutput with
        // Verbosity
        member __.Verbosity
            with get() = verbosity
            and set (level) = verbosity <- level

        member _.IsQuiet() = Verbosity.isQuiet verbosity
        member _.IsNormal() = Verbosity.isNormal verbosity
        member _.IsVerbose() = Verbosity.isVerbose verbosity
        member _.IsVeryVerbose() = Verbosity.isVeryVerbose verbosity
        member _.IsDebug() = Verbosity.isDebug verbosity

        // Output
        member _.Write(message) =
            printf "%s" message

        member _.WriteLine(message) =
            printfn "%s" message

        member _.WriteError(message) =
            eprintf "%s" message

        member _.WriteErrorLine(message) =
            eprintfn "%s" message

        (* member this.WriteBig style (Message message) =
            let output = this :> IOutput
            if output.IsNormal() then
                let underline =
                    match style with
                    | Style.ShowUnderline underline -> underline
                    | _ -> "="

                Console.WriteAscii(message, OutputType.color Title)
                Console.WriteLine(String.replicate (message.Length * 6) underline, OutputType.color Title)

                match style with
                | Style.HasNewLine NewLine -> Console.WriteLine()
                | Style.HasNewLine (NewLines i) -> for _ in 0 .. (i - 1) do Console.WriteLine()
                | _ -> () *)
