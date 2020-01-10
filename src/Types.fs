namespace MF.ConsoleStyle

type internal OutputType =
    | Title
    | SubTitle
    | TableHeader
    | Success
    | Error
    | Number
    | TextWithMarkup of string option
