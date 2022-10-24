module ConsoleStyle.Tests.StyleTests

open Expecto
open MF.ConsoleStyle

type TestCase = {
    Description: string
    Style: Style
    Render: ConsoleStyle -> unit
    Expected: string list
}

let style = Style.defaults

let provideStyles: TestCase list = [
    {
        Description = "title - default"
        Style = style
        Render = fun console -> console.Title "Default Title"
        Expected = [
            "Default Title"
            "============="
            ""
            ""
        ]
    }
    {
        Description = "title - special underline"
        Style = { style with TitleUnderline = Underline "<>" }
        Render = fun console -> console.Title "Fency Title"
        Expected = [
            "Fency Title"
            "<><><><><><"
            ""
            ""
        ]
    }

    {
        Description = "sub-title - default"
        Style = style
        Render = fun console -> console.SubTitle "Default SubTitle"
        Expected = [
            "Default SubTitle"
            ""
        ]
    }
    {
        Description = "sub-title - special underline"
        Style = { style with TitleUnderline = Underline "<>" }
        Render = fun console -> console.SubTitle "Fency SubTitle"
        Expected = [
            "Fency SubTitle"
            ""
        ]
    }

    {
        Description = "section - default"
        Style = style
        Render = fun console -> console.Section "Default Section"
        Expected = [
            "Default Section"
            "---------------"
            ""
            ""
        ]
    }
    {
        Description = "section - special underline"
        Style = { style with SectionUnderline = Underline ".:.." }
        Render = fun console -> console.Section "Fency Section"
        Expected = [
            "Fency Section"
            ".:...:...:..."
            ""
            ""
        ]
    }

    {
        Description = "Show DateTime with custom indentation"
        Style =
            { style with
                ShowDateTime = ShowDateTimeFor (Map.ofList [
                    Verbosity.Normal, "dd.MM.yyyy THH:mm"
                ])
                Indentation = Indentation ".."
            }
        Render = fun console -> console.Message "Message"
        Expected = [
            sprintf "[%s]..%s" (System.DateTime.Now.ToString("dd.MM.yyyy THH:mm")) "Message"
            ""
        ]
    }

    {
        Description = "Success with custom block length"
        Style = { style with BlockLength = 20 }
        Render = fun console -> console.Success "Message"
        Expected = [
            "                    "
            " ✅ Message         "
            "                    "
            ""
            ""
        ]
    }
    {
        Description = "Warning with custom block length"
        Style = { style with BlockLength = 20 }
        Render = fun console -> console.Warning "Message"
        Expected = [
            "                    "
            " ⚠️  Message         "
            "                    "
            ""
            ""
        ]
    }
    {
        Description = "Custom tag - number"
        Style = style
        Render = fun console -> console.Message "This is <number>42</number>"
        Expected = [
            "This is 42"
            ""
        ]
    }
    {
        Description = "Custom tag - underline"
        Style = style
        Render = fun console -> console.Message "This is <u>underlined</u>"
        Expected = [
            "This is underlined"
            ""
        ]
    }
    {
        Description = "Custom tag - bold"
        Style = style
        Render = fun console -> console.Message "This is <b>bold</b>"
        Expected = [
            "This is bold"
            ""
        ]
    }
    {
        Description = "Custom tag - italic"
        Style = style
        Render = fun console -> console.Message "This is <i>italic</i>"
        Expected = [
            "This is italic"
            ""
        ]
    }
]

[<Tests>]
let styleTestsTests =
    testList "Style tests" [
        let linesWithoutMarkup style string =
            (string |> Style.removeMarkup style).Split("\n") |> Seq.toList

        yield!
            provideStyles
            |> List.map (fun tc ->
                testCase $"should buffer the output with {tc.Description} (without markup)" <| fun _ ->
                    use consoleOutput = new Output.BufferOutput(verbosity = Verbosity.Normal)

                    ConsoleStyle(consoleOutput, tc.Style)
                    |> tc.Render

                    let output = consoleOutput.Fetch() |> linesWithoutMarkup tc.Style

                    Expect.equal output tc.Expected tc.Description
            )
    ]
