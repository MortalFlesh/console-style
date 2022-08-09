namespace MF.ConsoleStyle

module internal Logging =
    open System
    open Microsoft.Extensions.Logging

    let private normalizeString (string: string) =
        string.Replace(" ", "").ToLowerInvariant()

    [<RequireQualifiedAccess>]
    module LogLevel =
        let parse = normalizeString >> function
            | "trace" | "vvv" -> LogLevel.Trace
            | "debug" | "vv" -> LogLevel.Debug
            | "information" | "v" | "normal" -> LogLevel.Information
            | "warning" -> LogLevel.Warning
            | "error" -> LogLevel.Error
            | "critical" -> LogLevel.Critical
            | "quiet" | "q" | _ -> LogLevel.None

    [<RequireQualifiedAccess>]
    module LoggerFactory =
        open NReco.Logging.File

        let create command level =
            LoggerFactory.Create(fun builder ->
                builder
                    .SetMinimumLevel(level)
                    .AddFile(
                        (command |> normalizeString |> sprintf "logs/log_%s_{0:yyyy}-{0:MM}-{0:dd}.log"),
                        fun c ->
                            c.FormatLogFileName <- fun name -> String.Format(name, DateTime.UtcNow)
                            c.Append <- true
                            c.MinLevel <- LogLevel.Trace
                    )
                |> ignore
            )

        let loggerFactory = create "debug" LogLevel.Trace
