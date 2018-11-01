module ConsoleStyleTests.Helper.Output

open System
open System.Text
open System.IO

type private OutStr(sb:StringBuilder, orig:TextWriter) =
    inherit TextWriter()
    override x.Encoding = stdout.Encoding
    override x.Write (s:string) = sb.Append s |> ignore; orig.Write s
    override x.WriteLine (s:string) = sb.AppendLine s |> ignore; orig.WriteLine s
    override x.WriteLine() = sb.AppendLine() |> ignore; orig.WriteLine()
    member x.Value with get() = sb.ToString().TrimEnd()
    static member Create() =
        let orig = stdout
        let out = new OutStr(new StringBuilder(), orig)
        Console.SetOut(out)
        out
    interface IDisposable with member x.Dispose() = Console.SetOut(orig)

let withOutStr f a =
    use out = OutStr.Create()
    a |> f
    out.Value
