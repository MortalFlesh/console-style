namespace MF.ConsoleStyle

open System
open System.Threading.Tasks
open ShellProgressBar

type private ShellProgress = ShellProgressBar.ProgressBar

[<RequireQualifiedAccess>]
module private ProgressOption =
    // options.DenseProgressBar <- true    // one-line progress bar

    let options =
        ProgressBarOptions (
            ForegroundColor = ConsoleColor.Yellow,
            ForegroundColorDone = Nullable<ConsoleColor>(ConsoleColor.DarkGreen),

            BackgroundColor = Nullable<ConsoleColor>(ConsoleColor.DarkGray),

            DisplayTimeInRealTime = true,
            ProgressBarOnBottom = true
        )

    let childOptions =
        ProgressBarOptions (
            ForegroundColor = ConsoleColor.DarkYellow,
            ForegroundColorDone = Nullable<ConsoleColor>(ConsoleColor.DarkGreen),

            BackgroundColor = Nullable<ConsoleColor>(ConsoleColor.DarkGray),

            DisplayTimeInRealTime = true,
            ProgressBarOnBottom = true,
            CollapseWhenFinished = true
        )

    let childOptionsToKeep =
        ProgressBarOptions (
            ForegroundColor = ConsoleColor.DarkYellow,
            ForegroundColorDone = Nullable<ConsoleColor>(ConsoleColor.DarkGreen),

            BackgroundColor = Nullable<ConsoleColor>(ConsoleColor.DarkGray),

            DisplayTimeInRealTime = true,
            ProgressBarOnBottom = true,
            DenseProgressBar = true,
            CollapseWhenFinished = false
        )

type ProgressBar =
    private
    | Active of ShellProgress
    | ActiveChild of ChildProgressBar
    | Inactive

    with
        member this.Advance() =
            match this with
            | Active progressBar -> progressBar.Tick()
            | ActiveChild childProgress -> childProgress.Tick()
            | _ -> ()

        member this.Finish() =
            match this with
            | Active progressBar ->
                progressBar.Message <- "Finished"
                progressBar.Dispose()
            | _ -> ()

        interface IDisposable with
            member this.Dispose() = this.Finish()

[<RequireQualifiedAccess>]
module internal ProgressBar =
    let start initialMessage total =
        if Console.WindowWidth > 0 then new ShellProgress(total, initialMessage, ProgressOption.options) |> Active
        else Inactive

    let startAsChild progress initialMessage total =
        match progress with
        | Active progress when Console.WindowWidth > 0 -> progress.Spawn(total, initialMessage, ProgressOption.childOptions) |> ActiveChild
        | _ -> Inactive

    let startAsChildAndKeepIt progress initialMessage total =
        match progress with
        | Active progress when Console.WindowWidth > 0 -> progress.Spawn(total, " " + initialMessage, ProgressOption.childOptionsToKeep) |> ActiveChild
        | _ -> Inactive
