namespace MF.ConsoleStyle

open System

type Underline = Underline of string
type Indentation = Indentation of string

[<RequireQualifiedAccess>]
module Underline =
    let internal (|IsSet|_|) = function
        | Underline empty when empty |> String.IsNullOrWhiteSpace -> None
        | underline -> Some underline

    let internal inLength length (Underline underline) =
        (underline |> String.replicate length).Substring(0, length)

[<RequireQualifiedAccess>]
module Indentation =
    let value (Indentation indentation) = indentation

type ShowDateTime =
    | NoDateTime
    | ShowDateTimeAs of format: string
    | ShowDateTimeFor of Map<Verbosity.Level, string>

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
    ShowDateTime: ShowDateTime
    MainTitleUnderline: Underline
    TitleUnderline: Underline
    SubTitleUnderline: Underline
    Indentation: Indentation
    CustomTags: CustomTag list
}

[<RequireQualifiedAccess>]
module Style =
    let [<Literal>] DefaultIndentation = "    "

    let defaults = {
        ShowDateTime =
            [
                Verbosity.VeryVerbose, "HH:mm:ss"
                Verbosity.Debug, "yyyy-MM-dd HH:mm:ss"
            ]
            |> Map.ofList
            |> ShowDateTimeFor
        MainTitleUnderline = Underline "-="
        TitleUnderline = Underline "="
        SubTitleUnderline = Underline "-"
        Indentation = Indentation DefaultIndentation
        CustomTags = []
    }

    let internal applyCustomTags style text =
        style.CustomTags
        |> List.fold CustomTag.apply text

    let removeMarkup style text =
        applyCustomTags style text
        |> Markup.removeMarkup

    [<RequireQualifiedAccess>]
    module internal Message =
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

        let indent (Indentation indentation) (message: Message) =
            {
                message with
                    Text = sprintf "%s%s" indentation message.Text
                    Length = message.Length + indentation.Length
                    LengthWithoutMarkup = message.LengthWithoutMarkup + indentation.Length
            }
