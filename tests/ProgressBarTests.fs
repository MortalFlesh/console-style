namespace ConsoleStyle.Tests.ProgressBar

module ProgressBarTest =
    open MF.ConsoleStyle

    let prepare verbosity = 
        // todo 
        (* match verbosity with
        | Some verbosity ->
            printfn "Verbosity: %A" verbosity
            verbosity |> Console.setVerbosity
        | _ ->
            printfn "Verbosity: Default"

        // verbosity
        printfn "Is quiet %s" (if Console.isQuiet() then "yes" else "no")
        printfn "Is normal %s" (if Console.isNormal() then "yes" else "no")
        printfn "Is verbose %s" (if Console.isVerbose() then "yes" else "no")
        printfn "Is very verbose %s" (if Console.isVeryVerbose() then "yes" else "no")
        printfn "Is debug %s" (if Console.isDebug() then "yes" else "no")

        // progress bar
        let total = 10
        let progressBar = Console.progressStart "Starting..." total
        for _ in 1 .. total do
            progressBar |> Console.progressAdvance
        Console.progressFinish progressBar *)

        0
