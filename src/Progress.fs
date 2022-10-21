namespace MF.ConsoleStyle

open System

type IProgress =
    inherit IDisposable

    abstract member Start: int -> unit
    abstract member Advance: unit -> unit
    abstract member Finish: unit -> unit

    abstract member SpawnChild: string * bool -> IProgress option

    /// Return true if progress bar is available.
    /// It won't tell if it has started or not.
    abstract member IsAvailable: unit -> bool

module internal Shell =
    open ShellProgressBar

    type private ShellProgress = ShellProgressBar.ProgressBar

    [<RequireQualifiedAccess>]
    module private ProgressOption =
        // fsharplint:disable
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
        // fsharplint:enable

    type ProgressBar(initialMessage) =
        let mutable progress = None

        member this.Start(total) =
            if progress.IsNone && this.IsAvailable() then
                progress <- Some (new ShellProgress(total, initialMessage, ProgressOption.options))

        member _.Advance() =
            match progress with
            | Some progressBar -> progressBar.Tick()
            | _ -> ()

        member _.Finish() =
            match progress with
            | Some progressBar ->
                progressBar.Message <- "Finished"
                progressBar.Dispose()
            | _ -> ()

        member _.SpawnChild(initialMessage, keep) =
            match progress with
            | Some progressBar ->
                new ChildProgressBar(progressBar, initialMessage, keep)
                :> IProgress
                |> Some
            | _ -> None

        member _.IsAvailable() = Console.WindowWidth > 0

        interface IProgress with
            member this.Start(total) = this.Start(total)
            member this.Advance() = this.Advance()
            member this.Finish() = this.Finish()
            member this.SpawnChild(message, keep) = this.SpawnChild(message, keep)
            member this.IsAvailable() = this.IsAvailable()

        interface IDisposable with
            member this.Dispose() = this.Finish()

    and ChildProgressBar(parent: ShellProgress, initialMessage: string, keep: bool) =
        let mutable progress = None

        member this.Start(total) =
            if progress.IsNone && this.IsAvailable() then
                progress <- Some (
                    if keep
                    then parent.Spawn(total, " " + initialMessage, ProgressOption.childOptionsToKeep)
                    else parent.Spawn(total, initialMessage, ProgressOption.childOptions)
                )

        member _.Advance() =
            match progress with
            | Some progressBar -> progressBar.Tick()
            | _ -> ()

        member _.Finish() =
            match progress with
            | Some progressBar ->
                progressBar.Message <- "Finished"
                progressBar.Dispose()
            | _ -> ()

        member _.SpawnChild(initialMessage, keep) = None
        member _.IsAvailable() = Console.WindowWidth > 0

        interface IProgress with
            member this.Start(total) = this.Start(total)
            member this.Advance() = this.Advance()
            member this.Finish() = this.Finish()
            member this.SpawnChild(message, keep) = this.SpawnChild(message, keep)
            member this.IsAvailable() = this.IsAvailable()

        interface IDisposable with
            member this.Dispose() = this.Finish()

type ProgressBar =
    private
    | Active of IProgress
    | Inactive

    with
        member this.Start(total) =
            match this with
            | Active progress -> progress.Start(total)
            | _ -> ()

        member this.Advance() =
            match this with
            | Active progress -> progress.Advance()
            | _ -> ()

        member this.Finish() =
            match this with
            | Active progress -> progress.Finish()
            | _ -> ()

        member this.Dispose() =
            match this with
            | Active progress -> progress.Dispose()
            | _ -> ()

        member this.SpawnChild(initialMessage, keep) =
            match this with
            | Active progress -> progress.SpawnChild(initialMessage, keep)
            | _ -> None

        member this.IsAvailable() =
            match this with
            | Active progress -> progress.IsAvailable()
            | _ -> false

        interface IProgress with
            member this.Start(total) = this.Start(total)
            member this.Advance() = this.Advance()
            member this.Finish() = this.Finish()
            member this.SpawnChild(message, keep) = this.SpawnChild(message, keep)
            member this.IsAvailable() = this.IsAvailable()

        interface IDisposable with
            member this.Dispose() = this.Finish()

[<RequireQualifiedAccess>]
module ProgressBar =
    let internal start (create: string -> IProgress) initialMessage total =
        let progress = create initialMessage

        if progress.IsAvailable() then
            progress.Start(total)
            Active progress
        else Inactive

    let internal startAsChild progress initialMessage total =
        match progress with
        | Active parent ->
            match parent.SpawnChild(initialMessage, false) with
            | Some progress when progress.IsAvailable() ->
                progress.Start(total)
                Active progress
            | _ -> Inactive
        | _ -> Inactive

    let internal startAsChildAndKeepIt progress initialMessage total =
        match progress with
        | Active parent ->
            match parent.SpawnChild(initialMessage, true) with
            | Some progress when progress.IsAvailable() ->
                progress.Start(total)
                Active progress
            | _ -> Inactive
        | _ -> Inactive

    let inactive = Inactive
