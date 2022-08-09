namespace MF.ConsoleStyle

module Verbosity =
    type Level =
        | Quiet
        | Normal
        | Verbose
        | VeryVerbose
        | Debug

    let internal isQuiet = function
        | Quiet -> true
        | Normal
        | Verbose
        | VeryVerbose
        | Debug -> false

    let internal isNormal = function
        | Quiet -> false
        | Normal
        | Verbose
        | VeryVerbose
        | Debug -> true

    let internal isVerbose = function
        | Quiet
        | Normal -> false
        | Verbose
        | VeryVerbose
        | Debug -> true

    let internal isVeryVerbose = function
        | Quiet
        | Normal
        | Verbose -> false
        | VeryVerbose
        | Debug -> true

    let internal isDebug = function
        | Quiet
        | Normal
        | Verbose
        | VeryVerbose -> false
        | Debug -> true

    let internal isSameOrAbove verbosity = function
        | Quiet -> isQuiet verbosity
        | Normal -> isNormal verbosity
        | Verbose -> isVerbose verbosity
        | VeryVerbose -> isVeryVerbose verbosity
        | Debug -> isDebug verbosity
