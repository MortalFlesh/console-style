#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.create "Clean" (fun _ ->
    !! "./**/bin"
    ++ "./**/obj"
    |> Shell.cleanDirs 
)

Target.create "Build" (fun _ ->
    !! "./**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

"Clean"
    ==> "Build"

Target.runOrDefault "Build"