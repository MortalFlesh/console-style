// ========================================================================================================
// === F# / Project fake build ==================================================================== 1.2.0 =
// --------------------------------------------------------------------------------------------------------
// Options:
//  - no-clean   - disables clean of dirs in the first step (required on CI)
//  - no-lint    - lint will be executed, but the result is not validated
// ========================================================================================================

open Fake.Core
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators

open ProjectBuild
open Utils

[<EntryPoint>]
let main args =
    args |> Args.init

    Targets.init {
        Project = {
            Name = "MF/ConsoleStyle"
            Summary = "Library to help with basic Console I/O"
            Git = Git.init ()
        }
        Specs = Spec.defaultLibrary
    }

    args |> Args.run
