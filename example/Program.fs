// Learn more about F# at http://fsharp.org

let orFail = function
    | Ok value -> value
    | Error error -> failwithf "Error: %A" error

open System
open System.IO
open MF.ConsoleStyle

module Progress =
    type Progress (isEnabled, output: ConsoleStyle, name: string) =
        let mutable progressBar: ProgressBar option = None

        member _.IsAvailable() = true

        member _.Start(total: int) =
            progressBar <-
                if isEnabled && (not <| output.IsDebug())
                    then
                        output.ProgressStartDefault(name, total)
                        |> Some
                    else
                        output.Message $"<c:dark-yellow>[Debug] Progress for \"{name}\" for (</c><c:magenta>{total}</c><c:dark-yellow>) is disabled</c>"
                        None

        member _.Advance() =
            progressBar |> Option.iter output.ProgressAdvance
            if output.IsDebug() then output.Message $"  ├──> <c:gray>[Debug] Progress advanced</c>"

        member _.Finish() =
            if output.IsDebug() then output.Message $"  └──> <c:dark-yellow>[Debug] Progress finished</c>\n"
            progressBar |> Option.iter output.ProgressFinish
            progressBar <- None

        member _.SpawnChild(message, keep) =
            match progressBar with
            | Some progres -> progres.SpawnChild(message, keep)
            | _ -> None

        interface IProgress with
            member this.Start(total) = this.Start(total)
            member this.Advance() = this.Advance()
            member this.Finish() = this.Finish()
            member this.SpawnChild(message, keep) = this.SpawnChild(message, keep)
            member this.IsAvailable() = this.IsAvailable()

        interface IDisposable with
            member this.Dispose() = this.Finish()

let showConsoleExample (console: ConsoleStyle) =
    // console.Verbosity <- Verbosity.Level.VeryVerbose
    console.Title "Simple output"
    console.MainTitle "ConsoleStyle"

    let font = Colorful.FigletFont.Load("chunky.flf")
    console.MainTitle("Title with font", font)

    let figlet = Colorful.Figlet(font)
    console.MainTitle("Figlet title", figlet)

    let answer = "World"
    // let answer = console.Ask "What is your name?"
    // console.Message("Thats <c:magenta>your</c> answer: <c:cyan>%A</c>", answer)

    console.Message("<c:|bg:yellow>Hello</c> <c:white|bg:#D20000>%s</c> <c:#DEB887FF>and</c> everyone <c:black|bg:#43ff64d9>else</c>.", answer)
    console.NewLine()

    console.Title("Multiple messages output")

    console.GroupedOptions ":" "Available commands:" [
        [ "list"; "Lists commands" ]
        [ "<c:blue>deployment:list</c>"; "Lists environment" ]
        [ "deployment:release"; "Release a package" ]
        [ "<c:blue>debug</c>:<c:dark-pink>configuration</c>"; "Dumps configuration" ]
        [ "<c:red>test</c>:<c:pink>d</c>"; "D" ]
        [ "<c:dark-red>test</c>:<c:yellow>c</c>"; "C" ]
        [ "<c:gray>test</c>:<c:purple>a</c>"; "a" ]
        [ "<c:white>test</c>:<c:black|bg:gray>b</c>"; "B" ]
    ]

    console.SimpleOptions "Options:" [
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c:yellow>[default: \"./config.yaml\"]</c>" ]
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c:yellow>[default: \"./config.yaml\"] (missing end tag)" ]
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c>[default: \"./config.yaml\"] (undefined color)" ]
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c:>[default: \"./config.yaml\"] (undefined color)" ]
        [ "-c, --config=CONFIG"; "Path to deploy <c:dark-blue>config</c> <c[default: \"./config.yaml\"] (incomplete tag)" ]
        [ "    --message";       "Some message" ]
        [ "    --parts";         "Required parts <c:yellow>[default: [\"foo\"; \"bar\"]]</c> <c:blue>(multiple values allowed)</c>" ]
    ]

    let showProgress name (console: ConsoleStyle) =
        console.Section $"Progress bar - {name}"

        let subTask progressBar i start =
            let partial = 5
            use subTaskProgress = partial |> start progressBar $"Subtask {i} ..."
            for _ in 1 .. partial do
                System.Threading.Thread.Sleep 200
                subTaskProgress |> console.ProgressAdvance

        let total = 10
        let progressBar = console.ProgressStart "Starting ..." total
        for i in 1 .. total do
            System.Threading.Thread.Sleep 100
            subTask progressBar i console.ProgressStartChild
            //subTask progressBar i console.ProgressStartChildAndKeepIt
            System.Threading.Thread.Sleep 100

            progressBar |> console.ProgressAdvance
        console.ProgressFinish progressBar

    let showAsyncProgress name (console: ConsoleStyle) =
        console.Section $"Progress bar <Async> - {name}"

        let debug i message =
            if console.IsDebug() then console.Message("<c:cyan>[Async][</c><c:magenta>%02i</c><c:cyan>]</c> %s", i, message)

        let jobs = [ 1 .. 10 ]
        let main = console.ProgressStart "Main" jobs.Length

        let jobs =
            jobs
            |> List.map (fun i -> async {
                debug i "Start ..."
                let subTasks = [ 1 .. 10 ]
                use current = console.ProgressStartChild main $"Work [{i}]" subTasks.Length
                do! Async.Sleep 200
                let random = Random()

                for j in subTasks do
                    debug i $"A[{i}] - work [{j}]"
                    do! Async.Sleep (200 * random.Next(0, 4))
                    current |> console.ProgressAdvance

                do! Async.Sleep 200
                debug i "Done"
                main |> console.ProgressAdvance
            })

        jobs
        |> Async.Parallel
        |> Async.Ignore
        |> Async.RunSynchronously

        main |> console.ProgressFinish

        console.Success "Async progress done"

    console |> showProgress "default"

    console.CreateProgressWith (fun message -> new Progress.Progress(true, console, message))
    console |> showProgress "custom - enabled"

    console.CreateProgressWith (fun message -> new Progress.Progress(false, console, message))
    console |> showProgress "custom - disbled"

    let verbosity = console.Verbosity
    console.Verbosity <- Verbosity.Debug
    console.CreateProgressWith (fun message -> new Progress.Progress(true, console, message))
    console |> showProgress "custom - enabled with debug"

    console.CreateProgressWith (fun message -> new Progress.Progress(false, console, message))
    console |> showProgress "custom - disbled with debug"
    console.Verbosity <- verbosity

    let verbosity = console.Verbosity
    console.Verbosity <- Verbosity.Debug
    console.CreateProgressWith (fun message -> new Progress.Progress(true, console, message))
    console |> showAsyncProgress "default"
    console.Verbosity <- verbosity

    console.Title "Color Example"

    console.Section("Spectrum with %s and %s colors", "<c:dark-yellow>foreground</c>", "<c:black|bg:dark-yellow>background</c>")
    let spectrum = [
        "#124542", "a"
        "#185C58", "b"
        "#1E736E", "c"
        "#248A84", "d"
        "#20B2AA", "e"
        "#3FBDB6", "f"
        "#5EC8C2", "g"
        "#7DD3CE", "i"
        "#9CDEDA", "j"
        "#BBE9E6", "k"
    ]
    console.SubTitle("Spectrum with foreground")
    spectrum
    |> List.iter (fun (color, letter) -> console.Write("<c:%s>%s</c>", color, letter))
    |> console.NewLine
    |> console.NewLine

    console.SubTitle("Spectrum with background")
    spectrum
    |> List.iter (fun (color, letter) -> console.Write("<c:black|bg:%s>%s</c>", color, letter))
    |> console.NewLine
    |> console.NewLine

    [
        "yellow", [
            "light-yellow"
            "yellow"
            "dark-yellow"
        ]

        "orange", [
            "light-orange"
            "orange"
            "dark-orange"
        ]

        "red", [
            "lightred"
            "red"
            "darkred"
        ]

        "green", [
            "lightgreen"
            "green"
            "darkgreen"
        ]

        "cyan", [
            "Light-cyan"
            "cyan"
            "darkcyan"
        ]

        "blue", [
            "Light-Blue"
            "blue"
            "darkblue"
        ]

        "magenta", [
            "light-magenta"
            "magenta"
            "dark-magenta"
        ]

        "pink", [
            "light-pink"
            "pink"
            "dark-pink"
        ]

        "purple", [
            "light-purple"
            "purple"
            "dark-purple"
        ]

        "gray", [
            "lightgray"
            "gray"
            "darkGray"
        ]

        "black & white", [
            "black"
            "white"
        ]
    ]
    |> List.map (fun (colorGroup, colors) ->
        let colorize color = sprintf "<c:%s>%s</c>" color color

        [
            colorize colorGroup
            colors |> List.map colorize |> String.concat ", "
        ]
    )
    |> console.Table [ "Color Group"; "Colors" ]

    [ "red"; "green"; "yellow"; "blue"; "purple"; "orange"; "gray" ]
    |> List.map (fun color -> { Tab.parseColor color "Sample" with Value = Some color })
    |> console.Tabs

    [ "#ed1017"; "#67c355"; "#f3d22b"; "#1996f0"; "#9064cb"; "#ff9603"; "#babab8" ]
    |> List.map (fun color -> { Tab.parseColor color "Sample" with Value = Some color })
    |> fun tabs -> console.Tabs(tabs, 10)

    [
        "#ed1017", "#9e2e22"
        "#67c355", "#087a3f"
        "#f3d22b", "#faa727"
        "#1996f0", "#0278be"
        "#9064cb", "#6a3390"
        "#ff9603", "#faa727"
        "#babab8", "#333333"
    ]
    |> List.mapi (fun i (color, darker) -> {
        Tab.parseColor color (sprintf "<c:dark-gray|bg:%s|ub>Metric</c>" color)
            with Value = Some <| sprintf "<c:magenta|bg:%s> %02d </c><c:|bg:%s>%% </c>" darker (i * 10) darker
        }
    )
    |> fun tabs -> console.Tabs(tabs, 10)

    let colorSquare color =
        sprintf "<c:|bg:#%s>    </c>" color

    let colors = [
    //     1         2         3         4         5         6         7         8         9         10        11        12        13        14        15        16        17        18        19        20
        "ffc20f"; "faa727"; "f79333"; "f46f2c"; "f04d2d"; "cb3430"; "9e2e22"; "d71f43"; "ee2866"; "ef4b7e"; "ce1984"; "ad308c"; "ce4998"; "c8619e"; "bf71aa"; "c689a7"; "9a769c"; "976aad"; "804b9d"; "6a3390"
        "a5cf4f"; "8cc747"; "2faa4f"; "2d9848"; "087a3f"; "096232"; "037957"; "119b7a"; "23ae91"; "16baaf"; "00aea5"; "008886"; "07636e"; "0c4d77"; "0b6695"; "0278be"; "078dca"; "01a8dd"; "09b1e2"; "3fc8f4"
    ]

    colors
    |> List.map colorSquare
    |> List.splitInto 2
    |> List.collect (fun line -> [ line; line ])
    |> List.iter (String.concat "" >> console.WriteLine)
    |> console.NewLine

    let colorLetter (i: int) color =
        sprintf "<c:#%s> %s%s </c>" color (System.Convert.ToChar(i + 65) |> string) (System.Convert.ToChar(i + 97) |> string)

    colors
    |> List.splitInto 2
    |> List.map (List.mapi colorLetter)
    |> List.iter (String.concat "" >> console.WriteLine)
    |> console.NewLine

    console.Section "Custom tags"
    console.SubTitle "Defaults"
    console.Message " - this is <number>42</number>"
    console.Message " - this is <u>underlined</u>"
    console.Message " - this is <b>bold</b>"
    console.Message " - this is <i>italic</i>"

    console.Message ("Now the <customTag>custom tags</customTag> example")
    console.Table [ "Tag"; "Value" ] [
        [ "service"; "<service>domain-context</service>" ]
        [ "name"; "<name>Jon Snow</name>" ]
    ]

    console.Error "Error message"
    console.Error "Error\nmessage"
    console.Error("Error %s", "<c:black|bg:magenta>in</c>-<c:white|bg:dark-cyan>style</c>")
    console.Error(String.concat "\n\n" ["error"; "with"; "multiple"; "lines"])

    console.Warning "Warning message"
    console.Warning "Warning\nmessage"
    console.Warning("Warning %s", "<c:black|bg:magenta>in</c>-<c:white|bg:dark-cyan>style</c>")

    console.Success "Done"
    console.Success "Done\non more lines"
    console.Success("Done %s", "<c:black|bg:magenta>in</c>-<c:white|bg:dark-cyan>style</c>")

    console.NewLine()

open Output.CombinedOutput.Operators

[<EntryPoint>]
let main argv =
    let verbosity = Verbosity.Normal
    let consoleOutput = Output.ConsoleOutput(verbosity)
    use bufferOutput = new Output.BufferOutput(verbosity)

    use fileStream = new FileStream("outputStream.txt", System.IO.FileMode.Create)
    use fileStreamOutput = new Output.StreamOutput(verbosity, fileStream)

    use memoryStream = new MemoryStream()
    use buffer = new BufferedStream(memoryStream)
    use bufferStreamOutput = new Output.StreamOutput(verbosity, buffer)

    let style = {
        Style.defaults with
            CustomTags = Style.defaults.CustomTags @ [
                CustomTag.createWithMarkup (TagName "customTag") {
                    Bold = true
                    Dim = true
                    Italic = true
                    Underline = true
                    Reverse = true
                    StrikeThrough = true
                    Foreground = Some "black"
                    Background = Some "red"
                }
                CustomTag.createAndParseMarkup (TagName "service") ":#0b6695|bg:#f79333|u" |> orFail
                {
                    Tag = TagName "name"
                    Markup = MarkupString.create "#23ae91"
                }
            ]
    }

    let output = consoleOutput <+> bufferOutput <+> fileStreamOutput <+> bufferStreamOutput

    let console = ConsoleStyle(output, style)
    showConsoleExample console

    let output = bufferOutput.Fetch()
    System.IO.File.WriteAllText("output.txt", (output |> console.RemoveMarkup))

    printf "---\nOutput buffer stream\n---\n"
    use r = new StreamReader(buffer)
    printfn "%s" <| r.ReadToEnd()
    0
