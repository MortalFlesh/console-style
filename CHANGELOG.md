# Changelog

<!-- There is always Unreleased section on the top. Subsections (Add, Changed, Fix, Removed) should be Add as needed. -->
## Unreleased

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
- Update for dotnet core 2.2
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
