module ConsoleStyle.Tests.Markup

open Expecto
open System.Drawing
open MF.ConsoleStyle

[<Tests>]
let parseColorsForMarkupTest =
    testList "Parse markup" [
        yield!
            [
                // Empty
                {| Markup = ""; Expected = Markup.empty; Description = "Empty string" |}
                {| Markup = "abc"; Expected = Markup.empty; Description = "Random string" |}
                {| Markup = ":|bg:"; Expected = Markup.empty; Description = "Empty foreground and background" |}

                // Foreground only
                {| Markup = ":red"; Expected = { Markup.empty with Foreground = Some "red" }; Description = "Foreground only - red" |}
                {| Markup = ":red|"; Expected = { Markup.empty with Foreground = Some "red" }; Description = "Foreground only - red with pipe" |}
                {| Markup = ":red||"; Expected = { Markup.empty with Foreground = Some "red" }; Description = "Foreground only - red with pipes" |}
                {| Markup = ":red|bg:|"; Expected = { Markup.empty with Foreground = Some "red" }; Description = "Foreground only - red with empty bg and modifiers" |}
                {| Markup = ":#D20000"; Expected = { Markup.empty with Foreground = Some "#D20000" }; Description = "Foreground only - red as RGB" |}
                {| Markup = ":#D20000FF"; Expected = { Markup.empty with Foreground = Some "#D20000FF" }; Description = "Foreground only - red as RGBA" |}

                // Background only
                {| Markup = ":|bg:dark-green"; Expected = { Markup.empty with Background = Some "dark-green" }; Description = "No foreground with green background" |}
                {| Markup = ":|bg:dark-green|"; Expected = { Markup.empty with Background = Some "dark-green" }; Description = "No foreground with green background with pipe" |}

                // Foreground and background
                {| Markup = ":white|bg:green"; Expected = { Markup.empty with Foreground = Some "white"; Background = Some "green" }; Description = "White text with green background" |}
                {| Markup = ":#000000|bg:#00FF00"; Expected = { Markup.empty with Foreground = Some "#000000"; Background = Some "#00FF00" }; Description = "White text with green background by hash" |}
                {| Markup = ":#000000|bg:#00FF00|"; Expected = { Markup.empty with Foreground = Some "#000000"; Background = Some "#00FF00" }; Description = "White text with green background by hash with empty pipe" |}

                // Modifiers only
                {| Markup = ":|ui"; Expected = { Markup.empty with Underline = true; Italic = true }; Description = "Short - No colors just underline and italic" |}
                {| Markup = ":||ui"; Expected = { Markup.empty with Underline = true; Italic = true }; Description = "No colors just underline and italic" |}
                {| Markup = ":|bg:|s"; Expected = { Markup.empty with Strikethrough = true }; Description = "No colors just modifiers" |}

                // Colors + modifiers
                {| Markup = ":red|r"; Expected = { Markup.empty with Foreground = Some "red"; Reverse = true }; Description = "Foreground color and modifier without background" |}
                {| Markup = ":red||r"; Expected = { Markup.empty with Foreground = Some "red"; Reverse = true }; Description = "Foreground color and modifier" |}
                {| Markup = ":red|bg:|r"; Expected = { Markup.empty with Foreground = Some "red"; Reverse = true }; Description = "Foreground color and modifier with empty bg" |}
                {| Markup = ":|bg:red|r"; Expected = { Markup.empty with Background = Some "red"; Reverse = true }; Description = "Background color and modifier" |}

                // Full
                {| Markup = ":red|bg:white|r"; Expected = { Markup.empty with Foreground = Some "red"; Background = Some "white"; Reverse = true }; Description = "Colors and modifier" |}
                {|
                    Markup = ":red|bg:green|bdiurs"
                    Expected = {
                        Bold = true
                        Dim = true
                        Italic = true
                        Underline = true
                        Reverse = true
                        Strikethrough = true
                        Foreground = Some "red"
                        Background = Some "green"
                    }
                    Description = "Full markup"
                |}
            ]
            |> List.map (fun case ->
                testCase $"should parse {case.Description}" <| fun _ ->
                    let actual = Markup.parse case.Markup
                    Expect.equal actual case.Expected case.Description
            )
    ]

[<Tests>]
let formatMarkupAsStringTest =
    testList "Markup as string" [
        yield!
            [
                // Empty
                {| Markup = Markup.empty; Expected = ":"; Description = "Empty markup" |}

                // Foreground only
                {| Markup = { Markup.empty with Foreground = Some "red" }; Expected = ":red"; Description = "Foreground only - red" |}
                {| Markup = { Markup.empty with Foreground = Some "#D20000" }; Expected = ":#D20000"; Description = "Foreground only - red as RGB" |}
                {| Markup = { Markup.empty with Foreground = Some "#D20000FF" }; Expected = ":#D20000FF"; Description = "Foreground only - red as RGBA" |}

                // Background only
                {| Markup = { Markup.empty with Background = Some "dark-green" }; Expected = ":|bg:dark-green"; Description = "No foreground with green background" |}

                // Foreground and background
                {| Markup = { Markup.empty with Foreground = Some "white"; Background = Some "green" }; Expected = ":white|bg:green"; Description = "White text with green background" |}
                {| Markup = { Markup.empty with Foreground = Some "#000000"; Background = Some "#00FF00" }; Expected = ":#000000|bg:#00FF00"; Description = "White text with green background by hash" |}

                // Modifiers only
                {| Markup = { Markup.empty with Underline = true; Italic = true }; Expected = ":|iu"; Description = "No colors just underline and italic" |}
                {| Markup = { Markup.empty with Strikethrough = true }; Expected = ":|s"; Description = "No colors just modifier" |}

                // Colors + modifiers
                {| Markup = { Markup.empty with Foreground = Some "red"; Reverse = true }; Expected = ":red|r"; Description = "Foreground color and modifier without background" |}
                {| Markup = { Markup.empty with Background = Some "red"; Reverse = true }; Expected = ":|bg:red|r"; Description = "Background color and modifier" |}

                // Full
                {| Markup = { Markup.empty with Foreground = Some "red"; Background = Some "white"; Reverse = true }; Expected = ":red|bg:white|r"; Description = "Colors and modifier" |}
                {|
                    Markup = {
                        Bold = true
                        Dim = true
                        Italic = true
                        Underline = true
                        Reverse = true
                        Strikethrough = true
                        Foreground = Some "red"
                        Background = Some "green"
                    }
                    Expected = ":red|bg:green|bdiurs"
                    Description = "Full markup"
                |}
            ]
            |> List.map (fun case ->
                testCase $"should format {case.Description}" <| fun _ ->
                    let actual = Markup.asString case.Markup
                    Expect.equal actual case.Expected case.Description
            )
    ]

[<Tests>]
let parseMarkupTest =
    [
        "Empty string", "", []
        "No markup", "Hello World", [ { Text = "Hello World"; Markup = Markup.empty } ]
        "Green World", "Hello <c:green>World</c>", [ { Text = "Hello "; Markup = Markup.empty }; { Text = "World"; Markup = { Markup.empty with Foreground = Some "green" }} ]

        "Hello World with foreground and background",
        "<c:|bg:#D20000FF>Hello</c> <c:white|bg:green>World</c><c:yellow|bg:>!</c>",
        [
            { Text = "Hello"; Markup = { Markup.empty with Background = Some "#D20000FF" }}
            { Text = " "; Markup = Markup.empty }
            { Text = "World"; Markup = { Markup.empty with Foreground = Some "white"; Background = Some "green" }}
            { Text = "!"; Markup = { Markup.empty with Foreground = Some "yellow" }}
        ]

        "Formatted Word (Table.Row.Column)",
        sprintf " %-*s" ((* column length *)10 - (* leading space *)1) "Word",
        [ { Text = " Word     "; Markup = Markup.empty } ]

        "Formatted Word With Color (Table.Row.Column)",
        sprintf " %-*s" ((* column length *)10 - (* leading space *)1 + (* markup length *)11) "<c:red>Word</c>",
        [
            { Text = " "; Markup = Markup.empty }
            { Text = "Word"; Markup = { Markup.empty with Foreground = Some "red" }}
            { Text = "     "; Markup = Markup.empty }
        ]
    ]
    |> List.map (fun (case, string, expected) ->
        testCase case <| fun _ ->
            let result = string |> Markup.parseMarkup

            sprintf "Parse markup of %A" string
            |> Expect.equal result expected
    )
    |> testList "Check parsing markup"
