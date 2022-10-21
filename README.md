Console style
=============

[![NuGet Version and Downloads count](https://buildstats.info/nuget/ConsoleStyle)](https://www.nuget.org/packages/ConsoleStyle)
[![Check](https://github.com/MortalFlesh/console-style/actions/workflows/checks.yaml/badge.svg)](https://github.com/MortalFlesh/console-style/actions/workflows/checks.yaml)

> One of the most boring tasks when creating console commands is to deal with the styling of the command's input and output. Displaying titles and tables or asking questions to the user involves a lot of repetitive code.

> This library is inspired by [SymfonyStyle](https://symfony.com/doc/current/console/style.html).

## Installation
```sh
dotnet add package ConsoleStyle
```

## Basic Usage
```fs
open MF.ConsoleStyle

let console = ConsoleStyle()

console.Title "Hello World!"
```
for output:

    Hello world!
    ============

---

### Input

#### Ask
```fs
let name = console.Ask "What's your name?"
let name = console.Ask("What's your %s?", "name")
```

Example:
```
What's your name? {stdin}
```

### Output - single

| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `mainTitle` | `console.MainTitle "ConsoleStyle"` | cyan | _see output_ 👇 |

```
  _____                        __        ____  __           __
 / ___/ ___   ___   ___ ___   / / ___   / __/ / /_  __ __  / / ___
/ /__  / _ \ / _ \ (_-</ _ \ / / / -_) _\ \  / __/ / // / / / / -_)
\___/  \___//_//_//___/\___//_/  \__/ /___/  \__/  \_, / /_/  \__/
                                                  /___/

-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

```
| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `title` | `console.Title "Title of foo"` | cyan | _see output_ 👇 |
```
Title of foo
============
```
| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `section` | `console.Secion "Section of foo"` | dark-yellow | _see output_ 👇 |
```
Section of foo
--------------
```
| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `message` | `console.Message "a simple message"` | _default_ | |
| `newLine` | `console.NewLine()` | _default_ | |
| `subTitle` | `console.SubTitle "Sub title"` | yellow | _Text is natively underlined_ |
| `error` | `console.Error "Something went wrong!"` | red | This output goes to the `stderr` |
| `success` | `console.Success "Done"` | green | |
| `indent` | `console.Indent "Something indented"` | _default_ | _adds spaces at the beginning_ |

**NOTE**: most of the functions allows formatted variant with up to 5 args - see [Formatting](#formatting)

### Output - many

| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `messages` | `console.Messages "-prefix-" ["line 1"; "line 2"]` | _default_ | _see output_ 👇 |
```
-prefix-line 1
-prefix-line 2
```
| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `options` | `console.Options "Foo options" [ ["first"; "desc 1"]; ["second"; "desc 2"] ]` | _default_ with yellow title | _see output_ 👇 |
```
Foo options
    - first   desc 1
    - second  desc 2
```
| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `simpleOptions` | `console.SimpleOptions "Foo options" [ ["first"; "desc 1"]; ["second", "desc 2"] ]` | Same as `options`, but without line prefix. _default_ with yellow title | _see output_ 👇 |
```
Foo options
    first   desc 1
    second  desc 2
```
| Function | example | color | note |
| ---      | ---     | ---   | ---      |
| `groupedOptions` | `console.GroupedOptions ":" "Grouped options" [ ["first"; "desc 1"]; ["group:first"; "desc"; "group 1"]; ["group:second"; "desc"; "group 2"]; ["second"; "desc 2"] ]` | Grouped options by their prefix, if there is any. _default_ with yellow title | _see output_ 👇 |
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
| `list` | `console.List ["line 1"; "line 2"]` | _default_ | _see output_ 👇 |
```
 - line 1
 - line 2
```

### Formatting
Since string formatting with `sprintf` is handled by compiler and is not easy to reproduce, there are explicit functions for formatting up to 5 parameters.
It is still **type safe** and **compiler friendly** (_only limitation is for number of parameters, but you can still simply use `sprintf` directly_).
```fs
console.Message("Format %s parameter", "one")
console.Message("Format %s, %s parameter", "one", "two")
console.Message("Format %s, %d and %d parameter", "one", 2, 3)
console.Message(sprintf "Format %s, %s, %s and %s parameter" "one" "two" "three" "more ...")
```
**NOTE**: Other functions allowing formatting works the same way.

### Markup in text
There is a special _tag_ for formatting a part of text (`<c:COLOR>text</c>`).

Formatting -> Usage
- Bold           `<c:|b>Bold</c>`
- Dim            `<c:|d>Dim</c>`
- Italic         `<c:|i>Italic</c>`
- Underline      `<c:|u>Underline</c>`
- Reverse        `<c:|r>Reverse</c>`
- StrikeThrough  `<c:|s>StrikeThrough</c>`
- Foreground     `<c:COLOR>Colored</c>`
- Background     `<c:|bg:COLOR>Colored</c>`

Combining formatting:
- minimal markup is `<c:>text</c>`
- colors `<c:black|bg:red>text with black foreground and red background</c>`
- bold and underlined `<c:|bu>Bold and underlined</c>`
- all formatting options `<c:#D20000|bg:blue|bdiurs>Over formatted</c>`

#### Available colors:
Named colors

![Colors](example/rendered/color-groups.png?raw=true "Colors")

_TIP_: Given color is normalized, so you can use `-` or `_` as separator or even use different case for colors.
(_Example: `darkblue` is same as `dark_blue`, `DARK--Blue`, `dark-blue`, ..._)

You can also use RGB and RGBA color codes

![More Colors](example/rendered/colors.png?raw=true "More Colors")

**NOTE**: You can use colors as both foreground and background for a text.

#### Usage
```fs
console.Message "Hello <c:green>world</c>!" // `Hello` and `!` will be in default color, and `world` will be green.
console.Message "<c:red>Hello</c> <c:green>world</c>!"  // Different color for every word.

console.SimpleOptions "Options:" [
    [ "option1"; "This is the <c:magenta>first</c> option"; "<c:yellow>[default: \"foo\"]</c>" ]
    [ "option2"; "This is the <c:magenta>second</c> option" ]
]
```

### Output complex components

#### Table
```fs
console.Table [ "FirstName"; "Surname" ] [
    [ "Jon"; "Snow" ]
    [ "Peter"; "Parker" ]
]
```
Output:
```
----------- ---------
 FirstName   Surname
----------- ---------
 Jon         Snow
 Peter       Parker
----------- ---------
```

#### Tabs
> See in the [example](/example/) dir

![Tabs](example/rendered/tabs.png?raw=true "Tabs")

```fs
// First line
[ "red"; "green"; "yellow"; "blue"; "purple"; "orange"; "gray" ]
|> List.map (fun color -> { Tab.parseColor color "Sample" with Value = Some color })
|> console.Tabs

// Second line
[ "#ed1017"; "#67c355"; "#f3d22b"; "#1996f0"; "#9064cb"; "#ff9603"; "#babab8" ]
|> List.map (fun color -> { Tab.parseColor color "Sample" with Value = Some color })
|> fun tabs -> console.Tabs(tabs, 10)

// Third line
[
    "#ed1017", "#9e2e22"
    "#67c355", "#087a3f"
    "#f3d22b", "#faa727"
    "#1996f0", "#0278be"
    "#9064cb", "#6a3390"
    "#ff9603", "#faa727"
    "#babab8", "#333333"
]
|> List.mapi (fun i (color, darker) -> {
    Tab.parseColor color (sprintf "<c:dark-gray|bg:%s|ub>Metric</c>" color)
        with Value = Some <| sprintf "<c:magenta|bg:%s> %02d </c><c:|bg:%s>%% </c>" darker (i * 10) darker
    }
)
|> fun tabs -> console.Tabs(tabs, 10)
```

### Progress bar
> For more info see https://github.com/Mpdreamz/shellprogressbar

```fs
let total = 10
let progressBar = console.ProgressStart "Starting..." total

for _ in 1 .. total do
    console.ProgressAdvance progressBar

console.ProgressFinish progressBar
```

**TIP**: For more examples (_async, with children, etc_) see the `example/Program.fs`

## Styling
There is a `Style` settings where you can set up some attributes
```fs
type Style = {
    /// Whether and how to show a date time
    ShowDateTime: ShowDateTime

    /// Underline used for main title
    MainTitleUnderline: Underline

    /// Underline used for title
    TitleUnderline: Underline

    /// Underline used for section
    SectionUnderline: Underline

    /// Indentation used in many places (options, date time, ...)
    Indentation: Indentation

    /// Block length is shared for all blocks (success, warning, ...)
    BlockLength: int

    /// Custom tags, available in the markup
    CustomTags: CustomTag list
}

let console = ConsoleStyle(style)
```

**NOTE**: There is also a default styles which you can just override
```fs
let enhancedStyle = { Style.defaults with Indentation = Indentation "  " }
```

### Custom tags
> See more in the [example](/example/) dir

![Custom Tags](example/rendered/custom-tags.png?raw=true "Custom Tags")

```fs
let style = {
    Style.defaults with
        CustomTags = [
            CustomTag.createWithMarkup (TagName "customTag") {
                Bold = true
                Dim = true
                Italic = true
                Underline = true
                Reverse = true
                StrikeThrough = true
                Foreground = Some "black"
                Background = Some "red"
            }
            CustomTag.createAndParseMarkup (TagName "service") ":#0b6695|bg:#f79333|u" |> orFail
            {
                Tag = TagName "name"
                Markup = MarkupString.create "#23ae91"
            }
        ]
}

let console = ConsoleStyle(style)

console.Section "Custom tags"
console.Message ("Now the <customTag>custom tags</customTag> example")
console.Table [ "Tag"; "Value" ] [
    [ "service"; "<service>domain-context</service>" ]
    [ "name"; "<name>Jon Snow</name>" ]
]
```

## Output
There are multiple outputs available
- Console - prints output with `Console.Write` functions (**Default**)
- Print - prints output with `printf` and `eprintf` functions
- Buffered - buffer every write into string and offers it on `Fetch` method
- Stream - writes to the given `System.IO.Stream`
- _Combined_ - combination of other outputs

### Usage
```fs
use bufferedOutput = Output.BufferOutput(Verbosity.Normal)
let console = ConsoleStyle(bufferedOutput)

console.Message("Hello")

let content: string = bufferedOutput.Fetch()
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
- `ask` function will show a _question_

From `VeryVerbose` it shows time (and date on `Debug`) on the start of each line (_not for multiline outputs_)

**Note**: not every level adds information to output

### Usage
You can set a level of verbosity by
```fs
let console = ConsoleStyle(Verbosity.Verbose)

// or change it on the fly
console.Verbosity <- Verbosity.Verbose
```

You can use a verbosity level in your application directly by
```fs
console.IsQuiet()
console.IsNormal()
console.IsVerbose()
console.IsVeryVerbose()
console.IsDebug()
```

### Example
As it was mentioned before, each level is addition to previous, so we have (except of `Quiet`)

| ⬇️ _function_ AND _level_ ➡️ | `Quiet`  | `Normal` | `Verbose` | `VeryVerbose` | `Debug` |
| ---                         | ---      | ---      | ---       | ---           | ---     |
| `IsQuiet()`                 | **true** | false    | false     | false         | false   |
| `IsNormal()`                | false    | **true** | **true**  | **true**      | **true**|
| `IsVerbose()`               | false    | false    | **true**  | **true**      | **true**|
| `IsVeryVerbose()`           | false    | false    | false     | **true**      | **true**|
| `IsDebug()`                 | false    | false    | false     | false         | **true**|
