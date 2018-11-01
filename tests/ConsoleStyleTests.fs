namespace ConsoleStyle.Tests.Output

module OutputTest =
    open Expecto
    open ConsoleStyleTests.Helper.Output
    open MF.ConsoleStyle

    [<Tests>]
    let messageTest =
        testList "Message" [
            testCase "hello world" <| fun _ ->
                let result = withOutStr Console.message "Hello world"

                Expect.stringStarts "Hello world" (result) ""
        ]
