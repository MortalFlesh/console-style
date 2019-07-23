namespace MF.ConsoleStyle

type private OutputType =
    | Title
    | SubTitle
    | TableHeader
    | Success
    | Error
    | Number
    | TextWithMarkup of string option
