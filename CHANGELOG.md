# Changelog

<!-- There is always Unreleased section on the top. Subsections (Add, Changed, Fix, Removed) should be Add as needed. -->
## Unreleased
- Allow to use custom progress bar instance
    - Add `IProgress` interface for a progress bar
    - Add `ConsoleStyle` method
        - `CreateProgressWith`
        - `ProgressStartDefault`
- Allow to create inactive progress bar
    - Add `ProgressBar.inactive` function
- Add custom tag
    - `number` to style numbers
    - `u` for underline
    - `b` for bold
    - `i` for italic

## 4.1.0 - 2022-10-19
- Show errors with more then 2 lines just in red foreground, without a block

## 4.0.0 - 2022-10-19
- [**BC**] Require Fully Qualified access for `OutputType`

## 3.0.0 - 2022-10-05
- [**BC**] Use net 6.0
- Update dependencies
- [**BC**] Remove `CompiledName` attribute
- Add Outputs
    - `Buffer`
    - `Console`
    - `Print`
- [**BC**] Make `ConsoleStyle` as instance, not as a global functions
- Add `Font` to `MainTitle`
- Add `Style` settings
- Add output functions
    - `Note`
    - `Tabs`
    - `Warning`
    - `WriteLine`
- Enhance markup (colors, underline, bold, italic, ...)
- Progress bar
    - Add `ProgressBar` type to implement `IDisposable` instead of using `option`
    - [**BC**] Change `ProgressBar` functions to use `ProgressBar` type
        - `Console.progressStart`
        - `Console.progressAdvance`
        - `Console.progressFinish`
    - Add `ProgressBar` children

## 2.0.0 - 2020-01-13
- Update dependencies
- [**BC**] Require .net core `^3.1`
- Add `AssemblyInfo`
- [**BC**] Table requires `list` instead of `seq`
- Allow markup in Tables
- Ignore markup in Table header
- [**BC**] Options requires `list` instead of `seq`
- [**BC**] Change `string * string` to `string list` in functions:
    - `options`
    - `simpleOptions`
    - `groupedOptions`

## 1.4.1 - 2019-07-31
- Fix markup in grouped options.

## 1.4.0 - 2019-07-24
- Fix error when empty options (_sequence_) is given.
- Allow simple _markup_ for coloring part of text in functions:
    - `message` (_and all variants with formatting_)
    - `list`
    - `messages`
    - `options`
    - `simpleOptions`
    - `groupedOptions`

## 1.3.0 - 2019-07-10
- Use paket for dependency management
- Update for .net core 2.2
- Add Lint
- Add `simpleOptions` function to show `options` without any line prefix
- Add `groupedOptions` function to show `options` grouped by prefix

## 1.2.0 - 2018-12-07
- Print error output as standard error output
- Add other output functions with formatting of more parameters

## 1.1.0 - 2018-11-21
- Add verbosity

## 1.0.0 - 2018-11-07
- Initial implementation
