module ConsoleStyle.Tests.BufferedOutput

open Expecto
open MF.ConsoleStyle

[<Tests>]
let bufferedOutputTests =
    testList "Buffered output tests" [
        use consoleOutput =
            new Output.BufferOutput(
                verbosity = Verbosity.Normal
            )

        let style = {
            Style.defaults with
                CustomTags = [
                    { Tag = TagName "error"; Markup = MarkupString ":red|bg:black|r" }
                ]
        }

        let console = ConsoleStyle(consoleOutput, style)

        testCase "should buffer the output" <| fun _ ->
            console.Write("Hello %s", "World")
            let output = consoleOutput.Fetch()
            Expect.equal output "Hello World" "Fetched hello world."

        testCase "should buffer the output and fetch multiple times" <| fun _ ->
            let output = [
                for i in 1 .. 5 do
                    console.Write("Loop %d", i)
                    yield consoleOutput.Fetch()
            ]

            let expected = [
                "Loop 1"
                "Loop 2"
                "Loop 3"
                "Loop 4"
                "Loop 5"
            ]

            Expect.equal output expected "Fetched buffer loops."

        testCase "should buffer the output with colors" <| fun _ ->
            console.Write("Jon <c:%s>%s</c>", "cyan", "Snow")
            let output = consoleOutput.Fetch()
            Expect.equal output "Jon \u001b[38;2;0;255;255mSnow\u001b[0m" "Fetched Jon Snow with formatted color."

        testCase "should buffer the output with colors and background" <| fun _ ->
            let parts =
                [
                    "<c:|bg:yellow>Hello</c>", "\u001b[48;2;255;255;0mHello\u001b[0m"
                    "<c:white|bg:#D20000>world</c>", "\u001b[38;2;255;255;255;48;2;210;0;0mworld\u001b[0m"
                    "<c:#DEB887FF>and</c>", "\u001b[38;2;222;184;135mand\u001b[0m"
                    "everyone", "everyone"
                    "<c:black|bg:#43ff64d9>else</c>.", "\u001b[38;2;0;0;0;48;2;67;255;100melse\u001b[0m."
                ]

            let word = parts |> List.map fst |> String.concat " "
            let expected = parts |> List.map snd |> String.concat " "

            console.Write(word)
            let output = consoleOutput.Fetch()
            Expect.equal output expected "Fetched hello world with formatted color and backgrounds."

        testCase "should buffer the output with colors and background and get just text" <| fun _ ->
            let parts =
                [
                    "<c:|bg:yellow>Hello</c>", "Hello"
                    "<c:white|bg:#D20000>world</c>", "world"
                    "<c:#DEB887FF>and</c>", "and"
                    "everyone", "everyone"
                    "<c:black|bg:#43ff64d9>else</c>.", "else."
                ]

            let word = parts |> List.map fst |> String.concat " "
            let expected = parts |> List.map snd |> String.concat " "

            let wordWithoutMarkup = word |> Markup.removeMarkup
            Expect.equal wordWithoutMarkup expected "Word without markup should be the same as the expected output"

            console.Write(word)
            let output = consoleOutput.Fetch() |> Markup.removeMarkup
            Expect.equal output expected "Fetched output should be same with removed markup (even after rendering)."

        testCase "should buffer the output with custom tag" <| fun _ ->
            let word = "This is custom <error>error message</error>."
            let expected = "This is custom \u001b[7;38;2;255;0;0;48;2;0;0;0merror message\u001b[0m."
            let expectedAppliedCustomTag = "This is custom <c:red|bg:black|r>error message</c>."
            let expectedNoMarkup = "This is custom error message."

            let wordWithoutMarkup = word |> Style.removeMarkup style
            Expect.equal wordWithoutMarkup expectedNoMarkup "Word without markup should be the same as the expected output"

            let wordWithoutCustomTags = word |> Style.applyCustomTags style
            Expect.equal wordWithoutCustomTags expectedAppliedCustomTag "Word should have custom tag replaced by a markup"

            console.Write(word)
            let outputWithMarkup = consoleOutput.Fetch()
            Expect.equal outputWithMarkup expected "Fetched output should be formatted."

            let output = outputWithMarkup |> Markup.removeMarkup
            Expect.equal output expectedNoMarkup "Fetched output should be same with removed markup (even after rendering)."
    ]
