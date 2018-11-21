namespace ConsoleStyle.Tests.Output

module OutputTest =
    open MF.ConsoleStyle

    let prepare verbosity = 
        match verbosity with
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

        0
