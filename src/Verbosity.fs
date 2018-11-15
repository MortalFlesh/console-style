namespace MF.ConsoleStyle

module Verbosity =
    type Level =
        | Quiet
        | Normal
        | Verbose
        | VeryVerbose
        | Debug

    let mutable private currentLevel = Normal

    let internal setVerbosity level =
        currentLevel <- level

    let internal isQuiet () =
        match currentLevel with
        | Quiet -> true
        | Normal
        | Verbose
        | VeryVerbose
        | Debug -> false

    let internal isNormal () =
        match currentLevel with
        | Quiet -> false
        | Normal
        | Verbose
        | VeryVerbose
        | Debug -> true

    let internal isVerbose () =
        match currentLevel with
        | Quiet
        | Normal -> false
        | Verbose
        | VeryVerbose
        | Debug -> true

    let internal isVeryVerbose () =
        match currentLevel with
        | Quiet
        | Normal
        | Verbose -> false
        | VeryVerbose
        | Debug -> true

    let internal isDebug () =
        match currentLevel with
        | Quiet
        | Normal
        | Verbose
        | VeryVerbose -> false
        | Debug -> true
