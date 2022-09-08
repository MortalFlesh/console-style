namespace MF.ConsoleStyle

type Underline = Underline of string
type Indentation = Indentation of string
type ShowDateTime =
    | ShowDateTime
    | ShowDateTimeAs of format: string
    | ShowDateTimeFrom of Verbosity.Level
    | ShowDateTimeFromAs of Verbosity.Level * format: string

type AddNewLine =
    | NewLine
    | NewLines of int

type TagName = TagName of string

type CustomTag = {
    Tag: TagName
    Markup: MarkupString
}

[<RequireQualifiedAccess>]
module TagName =
    let internal asStart (TagName tag) = $"<{tag}>"
    let internal asEnd (TagName tag) = $"</{tag}>"

[<RequireQualifiedAccess>]
module CustomTag =
    let createWithMarkup tag markup =
        {
            Tag = tag
            Markup = MarkupString (markup |> Markup.asString)
        }

    /// Markup could contain either full markup representation "<c:FOREGROUND|bg:BACKGROUND|MODIFIER>" or just a markup ":FOREGROUND|bg:BACKGROUND|MODIFIER"
    let createAndParseMarkup (TagName tag) markup =
        match tag, markup with
        | null, _ | "", _ -> Result.Error "Empty tag"
        | _, null | _, "" -> Result.Error "Empty markup"
        | tag, markup -> Ok { Tag = TagName tag; Markup = MarkupString.create markup }

    let internal apply (text: string) (customTag: CustomTag) =
        let tagStart = customTag.Tag |> TagName.asStart

        if text.Contains(tagStart) |> not then text
        else
            let tagEnd = customTag.Tag |> TagName.asEnd
            let markup = customTag.Markup |> MarkupString.value

            text
                .Replace(tagStart, markup)
                .Replace(tagEnd, "</c>")

type Style = {
    Underline: Underline option
    Indentation: Indentation option
    ShowDateTime: ShowDateTime option
    NewLine: AddNewLine option
    DateTimeFormat: string option
    CustomTags: CustomTag list
}

[<RequireQualifiedAccess>]
module internal Style =
    let [<Literal>] DefaultIndentation = "    "

    let empty = {
        Underline = None
        Indentation = None
        ShowDateTime = None
        NewLine = None
        DateTimeFormat = None
        CustomTags = []
    }

    let (|IsIndentated|_|) = function
        | { Indentation = Some (Indentation value) } -> Some value
        | _ -> None

    let (|HasShowDateTime|_|) = function
        | { ShowDateTime = Some showDateTime } -> Some showDateTime
        | _ -> None

    let (|ShowUnderline|_|) = function
        | { Underline = Some (Underline style) } -> Some style
        | _ -> None

    let (|HasNewLine|_|) = function
        | { NewLine = Some newLine } -> Some newLine
        | _ -> None

    let applyCustomTags style text =
        style.CustomTags
        |> List.fold CustomTag.apply text

    let removeMarkup style text =
        applyCustomTags style text
        |> Markup.removeMarkup

    [<RequireQualifiedAccess>]
    module Message =
        let ofString message =
            let hasMarkup = message |> Markup.hasMarkup
            {
                Text = message
                Length = message.Length
                HasMarkup = hasMarkup
                LengthWithoutMarkup = if hasMarkup then (message |> Markup.removeMarkup).Length else message.Length
            }

        let applyCustomTags style (message: Message) =
            message.Text
            |> applyCustomTags style
            |> ofString

