namespace MF.ConsoleStyle

open System.Drawing
open Words

type Tab = {
    /// Text must not contain any new line \n
    Text: string
    /// Whether the value is set, all tabs in the row will have an extra line
    Value: string option
    Color: Color
}

[<RequireQualifiedAccess>]
module Tab =
    let ofString color text = { Text = text; Value = None; Color = color }
    let parseColor color text = { Text = text; Value = None; Color = Some color |> Color.parse |> Option.defaultValue Color.White }
    let create color text value = { Text = text; Value = Some value; Color = color }
    let text ({ Text = text }: Tab) = text
    let value ({ Value = value }: Tab) = value
    let hasValue ({ Value = value }: Tab) = value.IsSome
    let color ({ Color = color }: Tab) = color

    let internal words tab =
        [
            yield Word tab.Text

            match tab.Value with
            | Some value -> yield Word value
            | _ -> ()
        ]

[<RequireQualifiedAccess>]
module private Tabs =
    open WordLength.Operators

    /// 2 spaces at each side
    let private padding = WordLength 4

    let private line wordLength (color: string) =
        sprintf "<c:black|bg:%s>%s</c>" color (String.replicate wordLength " ")

    let private text (removeMarkup: string -> string) tabLength color text =
        let color = color |> Color.hash
        let textWithoutMarkup = text |> removeMarkup
        let colorize = sprintf "<c:black|bg:%s>%s</c>" color

        let spacesBefore, word, spacesAfter =
            if textWithoutMarkup.Length > tabLength then
                // todo<later> - for now the markup is stripped off when text is too long, so I don't need to solve markup cutting
                1, textWithoutMarkup[0 .. (tabLength - 2)], 1
            else
                let spacesBefore = (float tabLength - float textWithoutMarkup.Length) / 2. |> floor |> int
                let spacesAfter = tabLength - spacesBefore - textWithoutMarkup.Length
                spacesBefore, text, spacesAfter

        if word |> Markup.hasMarkup then
            sprintf "%s%s%s"
                (String.replicate spacesBefore " " |> colorize)
                word
                (String.replicate spacesAfter " " |> colorize)
        else
            sprintf "%s%s%s"
                (String.replicate spacesBefore " ")
                word
                (String.replicate spacesAfter " ")
            |> colorize

    let renderInLength removeMarkup render length (tabs: Tab list) =
        let colors = tabs |> List.map (Tab.color >> Color.hash)
        let line = colors |> List.map (line length)

        let withValues = tabs |> List.exists Tab.hasValue
        let text = text removeMarkup length

        [
            line
            tabs |> List.map (fun tab -> tab.Text |> text tab.Color)

            if withValues then
                tabs |> List.map (fun tab -> tab.Value |> Option.defaultValue "" |> text tab.Color)

            line
        ]
        |> List.map (String.concat "  ")
        |> List.iter render

    let private getMaxWordLength tabs =
        tabs
        |> List.collect Tab.words
        |> List.map Word.length
        |> List.max

    let render removeMarkup render tabs =
        let (WordLength length) = tabs |> getMaxWordLength <+> padding

        renderInLength removeMarkup render length tabs
