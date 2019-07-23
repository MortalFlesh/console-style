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
        Console.mainTitle "This is mainTitle!"
        Console.mainTitlef "Formatted mainTitle %s!" "F#"
        Console.mainTitlef2 "Formatted mainTitle %s and %i!" "F#" 42
        Console.mainTitlef3 "Formatted mainTitle %s with %s and %i!" "foo" "bar" 42

        Console.title "This is title!"
        Console.titlef "Formatted title %s!" "F#"
        Console.titlef2 "Formatted title %s and %i!" "F#" 42
        Console.titlef3 "Formatted title %s with %s and %i!" "foo" "bar" 42

        Console.section "This is section!"
        Console.sectionf "Formatted section %s!" "F#"
        Console.sectionf2 "Formatted section %s and %i!" "F#" 42
        Console.sectionf3 "Formatted section %s with %s and %i!" "foo" "bar" 42

        Console.subTitle "This is subTitle!"
        Console.subTitlef "Formatted subTitle %s!" "F#"
        Console.subTitlef2 "Formatted subTitle %s and %i!" "F#" 42
        Console.subTitlef3 "Formatted subTitle %s with %s and %i!" "foo" "bar" 42

        Console.message "This is <c:green>message</c>!"
        Console.messagef "Formatted <c:green>message</c> %s!" "F#"
        Console.messagef2 "Formatted <c:green>message</c> %s and %i!" "F#" 42
        Console.messagef3 "Formatted <c:green>message</c> %s with %s and %i!" "foo" "bar" 42

        Console.error "This is error!"
        Console.errorf "Formatted error %s!" "F#"
        Console.errorf2 "Formatted error %s and %i!" "F#" 42
        Console.errorf3 "Formatted error %s with %s and %i!" "foo" "bar" 42

        Console.success "This is success!"
        Console.successf "Formatted success %s!" "F#"
        Console.successf2 "Formatted success %s and %i!" "F#" 42
        Console.successf3 "Formatted success %s with %s and %i!" "foo" "bar" 42

        "Indented message" |> Console.indent |> Console.message

        Console.newLine()

        // output many
        Console.messages "prefix" ["<c:yellow>line 1</c>"; "line 2"]
        Console.options "Foo options" [("first", "Description of the <c:blue>1st</c>"); ("second", "Description of the 2nd")]
        Console.simpleOptions "Foo simple options" [("first", "Description of the <c:blue>1st</c>"); ("second", "Description of the 2nd")]
        Console.groupedOptions ":" "Grouped options" [("first", "desc <c:darkgreen>1</c>"); ("group:first", "desc group 1"); ("group:second", "desc group 2"); ("second", "desc 2")]
        Console.list [
            "<c:yellow>line 1"  // missing end tag
            "<c:>line 2</c>"    // missing color
        ]

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
