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
        Console.mainTitlef "Formatted mainTitle %s!" "F#"
        Console.title "Title!"
        Console.titlef "Formatted title %s!" "F#"
        Console.section "Section!"
        Console.sectionf "Formatted section %s!" "F#"
        Console.subTitle "Subtitle!"
        Console.subTitlef "Formatted subtitle %s!" "F#"
        Console.message "Message!"
        Console.messagef "Formatted message %s!" "F#"
        Console.error "Error!"
        Console.errorf "Formatted error %s!" "F#"
        Console.success "Success!"
        Console.successf "Formatted success %s!" "F#"
        "Indented message" |> Console.indent |> Console.message

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
