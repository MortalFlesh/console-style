Console style
=============

[![Build Status](https://dev.azure.com/MortalFlesh/console-style/_apis/build/status/MortalFlesh.console-style)](https://dev.azure.com/MortalFlesh/console-style/_build/latest?definitionId=1)
[![Build Status](https://api.travis-ci.com/MortalFlesh/console-style.svg?branch=master)](https://travis-ci.com/MortalFlesh/console-style)

One of the most boring tasks when creating console commands is to deal with the styling of the command's input and output. Displaying titles and tables or asking questions to the user involves a lot of repetitive code.

This library is inspired by [SymfonyStyle](https://symfony.com/doc/current/console/style.html).

## Installation
```sh

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

| Function | example | color | example |
| ---      | ---     | ---   | ---      |
| `mainTitle` | `Console.mainTitle "ConsoleStyle"` | cyan | _see example_ 👇 |
| `mainTitlef` | `Console.mainTitlef "Console%" "Style"` | cyan | _see example_ 👇 |
```
  _____                        __        ____  __           __
 / ___/ ___   ___   ___ ___   / / ___   / __/ / /_  __ __  / / ___
/ /__  / _ \ / _ \ (_-</ _ \ / / / -_) _\ \  / __/ / // / / / / -_)
\___/  \___//_//_//___/\___//_/  \__/ /___/  \__/  \_, / /_/  \__/
                                                  /___/

========================================================================

```
| Function | example | color | example |
| ---      | ---     | ---   | ---      |
| `title` | `Console.title "Title"` | cyan | _see example_ 👇 |
| `titlef` | `Console.titlef "Title of %s" "foo"` | cyan | _see example_ 👇 |
```
Title of foo
============
```
| Function | example | color | example |
| ---      | ---     | ---   | ---      |
| `section` | `Console.secion "Section of foo"` | yellow | _see example_ 👇 |
| `sectionf` | `Console.sectionf "Section of %s" "foo"` | yellow | _see example_ 👇 |
```
Section of foo
--------------
```
| Function | example | color | example |
| ---      | ---     | ---   | ---      |
| `message` | `Console.message "a simple message"` | _default_ | |
| `messagef` | `Console.messagef "a simple %s" "message"` | _default_ | |
| `newLine` | `Console.newLine()` | _default_ | |
| `subTitle` | `Console.subTitle "Sub title"` | yellow | |
| `subTitlef` | `Console.subTitlef "Sub title of %s" "foo"` | yellow | |
| `error` | `Console.error "Something went wrong!"` | red | |
| `errorf` | `Console.errorf "Error: %s" "Some problem"` | red | |
| `success` | `Console.success "Done"` | green | |
| `successf` | `Console.successf "Success: %s" "OK"` | green | |
| `indentation` | `Console.indentation` | _default_ | _indentation of four spaces_ |
| `indent` | `Console.indent "Something indented"` | _default_ | _adds four spaces at the begining_ |
| `indentf` | `Console.indent "%s indented" "Something"` | _default_ | _adds four spaces at the begining_ |

### Output - many

| Function | example | color | example |
| ---      | ---     | ---   | ---      |
| `messages` | `Console.messages "-prefix-" ["line 1"; "line 2"]` | _default_ | _see example_ 👇 |
```
-prefix-line 1
-prefix-line 2
```
| Function | example | color | example |
| ---      | ---     | ---   | ---      |
| `options` | `Console.options "Foo options" [("first", "desc 1"); ("second", "desc 2")]` | _default_ with yellow title | _see example_ 👇 |
| `optionsf` | `Console.optionsf "%s options" "Foo" [("first", "desc 1"); ("second", "desc 2")]` | _default_ with yellow title | _see example_ 👇 |
```
Foo options
    - first   desc 1
    - second  desc 2
```
| Function | example | color | example |
| ---      | ---     | ---   | ---      |
| `list` | `Console.list ["line 1"; "line 2"]` | _default_ | _see example_ 👇 |
```
 - line 1
 - line 2
```

### Output complex components

#### Table
```fs
Console.table ["FirstName"; "Surname"] [
    ["Jon"; "Snow"]
    ["Peter"; "Parker"]
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
