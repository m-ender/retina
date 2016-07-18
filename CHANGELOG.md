## 0.8.2

- New stage type: Deduplicate (`D`).
- General options: dry run with `*` (stage or group does not affect the string, but the result can be printed), implies `:`.
- Sort: default regex `.+`.
- Transliteration: new character classes `E` and `O` for even and odd digits.
- Match: only count/print unique matches (in order of first appearance) *after* applying limits with `@`.
- Replace: new substitution syntax. `$%_`, ``$%` `` and `$%'` now act like `$_`, ``$` `` and `$'` but are limited to the current line. Can be combined with `.` like ``$.%` ``.

## 0.8.1

- New stage type: Sort (`O`).

## 0.8.0

- General program structure: Change loops to generic groups, `+` to an option, and add `{` and `}` as shorthand for looped groups. This also means I've rewritten the configuration parser (which is now a lot more flexible for future extensions), and that configuration syntax has become a bit stricter.
- General options: Per-line mode with `%`.
- General options: Limits to restrict individual countable entities (usually matches) in all stage types.
- Transliteration: `_` can now be used to delete individual characters from matches.
- Split: Has been reimplemented instead of using `Regex.Split` which fixes some obscure bugs.
- Split: `-` omits captured groups from the resulting list.

## 0.7.3

- General options: `\` now implies `:`.
- Replace mode: `$#+` works now.
- Replace mode: `$*` now has implicit behaviour if not surrounded by other tokens. The preceding token defaults to $&, the following character to '1'.
- Replace mode: `$.1` and `$.{foo}` insert the length of the capture. Also works with ``$.` ``, `$.'`, `$._`, `$.&` and `$.+`.

## 0.7.2

- New general option: `\` suppresses a stage's trailing linefeed. Like `;` and `:`, it's relative position with respect to closing loops is relevant.
- Transliterate mode has been rewritten to allow for new features.
- Transliterate mode: ranges can now take escape sequences on either end.
- Transliterate mode: new character classes, `h`, `H` (hexadecimal digits), `l`, `L` (letters), `p` (printable ASCII).
- Transliterate mode: ranges and classes can be reversed by prepending `R`.
- Transliterate mode: the other set can be referenced once with `o`. If `o` is used in both sets, it will become a literal instead.
- Replace mode: `$#1` and `$#{foo}` to insert capture counts.
- Replace mode: `$*_` repeats the character `_` *n* times where *n* is the first decimal number in the result of the preceding token.

## 0.7.1

- Hotfix release: Mono could no longer read input from a pipe, which is fixed now.

## 0.7.0

- **Important change:** Single-file mode is now the default, so there is no more `-s` flag. Instead, the previous default behaviour can be recovered with the new `-m` flag.
- Retina now supports ISO 8859-1 encoded source files (in fact, it will assume ISO 8859-1 if the file is neither valid UTF-8 nor UTF-32).
- #11: Replace mode has been rewritten from scratch to avoid using `Regex.Replace`. This will allow many new features for the replacement syntax in the future.
- #22: Add `$n` as an escape sequence for linefeeds to replacement syntax.
- #29: Fixed a bug with the overlapping matches option `&`.
- #30: Don't block for input if invoked from the console. Input needs to be piped into the program now.
- #37: In single-file mode, pilcrows (`¶`) are replaced with linefeeds unless the `-P` flag is used.

## 0.6.1

- Add new stage type: Transliterate.

## 0.6.0

- Stage types can now be mixed, which (among other things) allows applying a 
  Match stage to the result of one or more Replace stages. An odd number of
  stages is now allowed and makes the last stage default to Match mode.
- #21: Improve defaults for Silent mode. One can now choose between making
  individual stages Silent (or not) or entire loops. By default only the
  last, outermost loop is not Silent, which means you always get only a
  single output regardless of whether the last stage is looped.

## 0.5.0

First numbered version. Notable changes over previous version:

- #19: Intermediate stages are silent by default.
- #15: Stages can be grouped into loops.
