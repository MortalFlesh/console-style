Console style
=============

[![NuGet Version and Downloads count](https://buildstats.info/nuget/ConsoleStyle)](https://www.nuget.org/packages/ConsoleStyle)
[![Build Status](https://dev.azure.com/MortalFlesh/console-style/_apis/build/status/MortalFlesh.console-style)](https://dev.azure.com/MortalFlesh/console-style/_build/latest?definitionId=1)
[![Build Status](https://api.travis-ci.com/MortalFlesh/console-style.svg?branch=master)](https://travis-ci.com/MortalFlesh/console-style)

One of the most boring tasks when creating console commands is to deal with the styling of the command's input and output. Displaying titles and tables or asking questions to the user involves a lot of repetitive code.

This library is inspired by [SymfonyStyle](https://symfony.com/doc/current/console/style.html).

## Installation
```sh
dotnet add package ConsoleStyle
```

## Basic Usage
```fs
open MF.ConsoleStyle

Console.title "Hello World!"
```
for output:

    Hello world!
    ============

---
## Styling

### Input

#### Ask
```fs
let name = Console.ask "What's your name?"
let name = Console.askf "What's your %s?" "name"
```

Example:
> Note: question is yellow
```
What's your name? {stdin}
```

### Output - single

| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `mainTitle` | `Console.mainTitle "ConsoleStyle"` | cyan | _see output_ 👇 |
todo - add figlet/font
```
  _____                        __        ____  __           __
 / ___/ ___   ___   ___ ___   / / ___   / __/ / /_  __ __  / / ___
/ /__  / _ \ / _ \ (_-</ _ \ / / / -_) _\ \  / __/ / // / / / / -_)
\___/  \___//_//_//___/\___//_/  \__/ /___/  \__/  \_, / /_/  \__/
                                                  /___/

========================================================================

```
| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `title` | `Console.title "Title of foo"` | cyan | _see output_ 👇 |
| `titlef` | `Console.titlef "Title of %s" "foo"` | cyan | _see output_ 👇 |
```
Title of foo
============
```
| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `section` | `Console.secion "Section of foo"` | yellow | _see output_ 👇 |
| `sectionf` | `Console.sectionf "Section of %s" "foo"` | yellow | _see output_ 👇 |
```
Section of foo
--------------
```
| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `message` | `Console.message "a simple message"` | _default_ | |
| `messagef` | `Console.messagef "a simple %s" "message"` | _default_ | |
| `newLine` | `Console.newLine()` | _default_ | |
| `subTitle` | `Console.subTitle "Sub title"` | yellow | |
| `subTitlef` | `Console.subTitlef "Sub title of %s" "foo"` | yellow | |
| `error` | `Console.error "Something went wrong!"` | red | This output goes to `stderr` |
| `errorf` | `Console.errorf "Error: %s" "Some problem"` | red | This output goes to `stderr` |
| `success` | `Console.success "Done"` | green | |
| `successf` | `Console.successf "Success: %s" "OK"` | green | |
| `indentation` | `Console.indentation` | _default_ | _indentation of four spaces_ |
| `indent` | `Console.indent "Something indented"` | _default_ | _adds four spaces at the begining_ |

_NOTE_: all formatted (_*f_) functions, has more options - see [Formatting](#formatting)

### Output - many

| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `messages` | `Console.messages "-prefix-" ["line 1"; "line 2"]` | _default_ | _see output_ 👇 |
```
-prefix-line 1
-prefix-line 2
```
| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `options` | `Console.options "Foo options" [ ["first"; "desc 1"]; ["second"; "desc 2"] ]` | _default_ with yellow title | _see output_ 👇 |
```
Foo options
    - first   desc 1
    - second  desc 2
```
| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `simpleOptions` | `Console.simpleOptions "Foo options" [ ["first"; "desc 1"]; ["second", "desc 2"] ]` | Same as `options`, but without line prefix. _default_ with yellow title | _see output_ 👇 |
```
Foo options
    first   desc 1
    second  desc 2
```
| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `groupedOptions` | `Console.groupedOptions ":" "Grouped options" [ ["first"; "desc 1"]; ["group:first"; "desc"; "group 1"]; ["group:second"; "desc"; "group 2"]; ["second"; "desc 2"] ]` | Grouped options by their prefix, if there is any. _default_ with yellow title | _see output_ 👇 |
```
Grouped options
    first         desc 1
    second        desc 2
 group
    group:first   desc    group 1
    group:second  desc    group 2
```
| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `list` | `Console.list ["line 1"; "line 2"]` | _default_ | _see output_ 👇 |
```
 - line 1
 - line 2
```

### Formatting
Since string formatting with `sprintf` is handled by compiler and is not easy to reproduce, there are explicit functions for formatting 1, 2 and 3 parameters.
It is still **type safe** and **compiler friendly** (_only limitation is for number of parameters, but you can still simply use `sprintf` directly_).
```fs
Console.messagef  "Format %s parameter" "one"
Console.messagef2 "Format %s, %s parameter" "one" "two"
Console.messagef3 "Format %s, %s and %s parameter" "one" "two" "three"
Console.message   (sprintf "Format %s, %s, %s and %s parameter" "one" "two" "three" "more ...")
```
_NOTE_: Other functions allowing formatting works the same way.

### Markup in text
There is a special _tag_ for coloring a part of text (`<c:COLOR>text</c>`). It is not transformed in every function.

Functions allowing a markup in the text:
- `message` (_and all `messagef*` variants_)
- `list`
- `messages`
- `options`
- `simpleOptions`
- `groupedOptions`
- `table`
    - only in rows (_any markup will be removed from header_)

List of available colors:
- lightyellow, yellow, darkyellow
- lightred, red, darkred
- lightgreen, green, darkgreen
- lightcyan, cyan, darkcyan
- lightblue, blue, darkblue
- lightpink, pink, darkpink
- lightmagenta, magenta, darkmagenta
- purple
- lightgray, gray, darkgray
- black
- white

_NOTE_: Some of the color variants may be the same (_or just an alias_), this is the limitation of terminals.

_TIP_: Given color is normalized, so you can use `-` or `_` as separator or even use different case for colors.
(_Example: `darkblue` is same as `dark_blue`, `DARK--Blue`, `dark-blue`, ..._)

#### Usage
```fs
Console.message "Hello <c:green>world</c>!" // `Hello` and `!` will be in default color, and `world` will be green.
Console.message "<c:red>Hello</c> <c:green>world</c>!"  // Different color for every word.

Console.simpleOptions "Options:" [
    [ "option1"; "This is the <c:magenta>first</c> option"; "<c:yellow>[default: \"foo\"]</c>" ]
    [ "option2"; "This is the <c:magenta>second</c> option" ]
]
```

### Output complex components

#### Table
```fs
Console.table [ "FirstName"; "Surname" ] [
    [ "Jon"; "Snow" ]
    [ "Peter"; "Parker" ]
]
```
Output:
> Note: header is yellow
```
----------- ---------
 FirstName   Surname
----------- ---------
 Jon         Snow
 Peter       Parker
----------- ---------
```

### Progress bar
_For more see_ https://github.com/Mpdreamz/shellprogressbar

```fs
let total = 10
let progressBar = Console.progressStart "Starting..." total

for _ in 1 .. total do
    progressBar.Tick()

Console.progressFinish progressBar
```

## Verbosity
There are 5 levels of verbosity and every higher (_more verbose_) level will just add more information to previous - meaning, that `VeryVerbose` = `Normal` + `Verbose` + _More_ and so on (_except of `Quiet` which is oposite_)

    Level =
        | Quiet
        | Normal
        | Verbose
        | VeryVerbose
        | Debug

Default level is `Normal`

Some functions have a different output based on current verbosity level

On `Quiet` only input functions will show an output, no other output is shown (_some components are not even inicialized_)
- `ask` and `askf` function will show a _question_

From `Verbose` it shows date with time (`DD/MM/YYYY HH:MM:SS`) on the start of each line (_not for multiline outputs_)

Note: not every level adds information to output

### Usage
You can set a level of verbosity by
```fs
Verbosity.Verbose |> Console.setVerbosity
```

You can use a verbosity level in your application directly by
```fs
Console.isQuiet()
Console.isNormal()
Console.isVerbose()
Console.isVeryVerbose()
Console.isDebug()
```

### Example
As it was mentioned before, each level is addition to previous, so we have (except of `Quiet`)

| ⬇️ _function_ AND _level_ ➡️ | `Quiet`  | `Normal` | `Verbose` | `VeryVerbose` | `Debug` |
| ---                         | ---      | ---      | ---       | ---           | ---     |
| `isQuiet()`                 | **true** | false    | false     | false         | false   |
| `isNormal()`                | false    | **true** | **true**  | **true**      | **true**|
| `isVerbose()`               | false    | false    | **true**  | **true**      | **true**|
| `isVeryVerbose()`           | false    | false    | false     | **true**      | **true**|
| `isDebug()`                 | false    | false    | false     | false         | **true**|
