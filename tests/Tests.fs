namespace ConsoleStyle.Tests

module Tests =
    open Expecto
    open MF.ConsoleStyle

    let runTest() = 
        // output single
        Console.mainTitle "ConsoleStyle"
        Console.mainTitlef "Hello World from %s!" "F#"
        Console.title "Hello World!"
        Console.titlef "Hello World from %s!" "F#"
        Console.section "Hello World!"
        Console.sectionf "Hello World from %s!" "F#"
        Console.subTitle "Hello World!"
        Console.subTitlef "Hello World from %s!" "F#"
        Console.message "Hello World!"
        Console.messagef "Hello World from %s!" "F#"
        Console.error "Hello World!"
        Console.errorf "Hello World from %s!" "F#"
        Console.success "Hello World!"
        Console.successf "Hello World from %s!" "F#"
        "Indented foo" |> Console.indent |> Console.message

        // output many
        Console.messages "prefix" ["line 1"; "line 2"]
        Console.options "Foo options" [("first", "Description of the 1st"); ("second", "Description of the 2nd")]
        Console.optionsf "%s options" "foo" [("first", "desc 1"); ("second", "desc 2")]
        Console.list ["line 1"; "line 2"]

        // components
        // table
        Console.table ["FirstName"; "Surname"] [
            ["Jon"; "Snow"]
            ["Peter"; "Parker"]
        ]
        Console.table [] [
            ["Jon"; "Snow"]
            ["Peter"; "Parker"]
        ]
        Console.table ["FirstName"; "Surname"] []
        Console.table [] []

        // todo - temporary disabled - because it is not working correctly on azure pipelines
        // progress bar
        let total = 10
        let progressBar = Console.progressStart "Starting..." total
        for _ in 1 .. total do
            progressBar.Tick()
        Console.progressFinish progressBar
        0

    [<EntryPoint>]
    let main argv =
        match argv with
        | [|"prepare"|] -> runTest()
        | _ -> Tests.runTestsInAssembly defaultConfig argv
