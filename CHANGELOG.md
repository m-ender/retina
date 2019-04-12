## 1.2.0

- Substitution syntax: added `$:&` and `$;&` to generate the match's index from the left and right, respectively. `<>[]` modifiers can be (ab)used for some off-by-one shenanigans.

## 1.1.1

This version switches from .NET Framework to .NET Core.

## 1.0.1

This version adds more fine-grained control over the history.

- The running result log isn't active unless at least one substitution in the program uses a `$-n` shorthand element (including `$-`) or a dynamic element `${…}`.
- The running result log's size can be limited with a new global flag `!#n`, where `n` indicates the number of entries. `!#` can be used for `!#0` to turn off the result log completely (this can be necessary if you need to use a dynamic element in a program that is supposed to run indefinitely without running out of memory).
- The `!,` option toggles whether any particular stage registers with the history (although it's still impossible to register output stages and dry run stages without conditional).
- The `!.` flag defaults all subsequent stages to *not* registering with the history (individual stages can be registered with `!.`). Using `!.` again, changes the default to registering again.

## 1.0.0

This version is almost a complete rewrite of Retina, which generalised and added a myriad of features to the language. Therefore, this changelog will necessarily be incomplete, and you won't get around learning about the new version by reading the new documentation on the wiki. However, I've tried to collect some of the changes here to give people with experience in Retina 0.8.2 a headstart.

- Semantics of atomic stages have been unified by defining a common preprocess to extract matches, separators and substitutions. Individual stages are defined in terms of the results of this preprocess. This makes many options available to all stages in a predictable way.
- Several new stage types were added: `P` (pad), `I` (match positions), `V` (reverse), `K` (constant).
- Sort stages were split into two stage types: `O` (regular sort) and `N` (numeric sort, formerly `O#`).
- An alternative type of transliteration stage was added, which performs a more general, cyclic style of transliteration (`Y`).
- Transliteration supports new character classes `V` and `v` for vowels, as well as an escape sequence for pilcrows.
- `M`atch stages were split into two types, `C`ount (formerly `M`) and `L`ist (formerly `M!`). `C` is the new default type for a stage on the last line of a program.
- The options `%` (per-line), `*` (dry run), `+` (loop) and output have been redefined as compound stages which form a wrapper around their child stage. This makes it easier to combine them and to reason about their interactions.
- Output was changed from `:` to `>`, and three alternative output stages exist as `<`, `;` and `\`.
- Several other compound stages were added for additional control flow, `&` (conditional), `~` (eval!), `_` (match mask, a generalised inverse to per-line stages).
- Existing compound stages were generalised with various options. For example, `+` can now represent various types of loops depending on its configuration, groups can select between its child stages based on a condition and `%` can use a custom line separator.
- Limit syntax was reworked completely. Limits are now similar to Python's indexing expressions (although they use commas instead of colons and have somewhat different semantics).
- Some stages can now be given strings or even regexes as options.
- Every stage can now optionally include a substitution (although a few don't make use of them).
- Stages can have multiple regexes (and corresponding substitutions).
- Stages can process the non-matches (called separators) instead of the matches in a string.
- It is now possible to find every substring that matches a given regex (not just one match per starting position).
- Substitution syntax was reworked completely and is now its own powerful minilanguage. The most important change for users familiar with previous version is that repetition is now *just* `*`, and `$*` is used to escape a literal asterisk. However, there are now also unary operators to perform simple string transformations right there in the substitution string, substitution elements can be selected dynamically and multiple substitution elements can be grouped together to be passed to operators as a single operand. It's even possible to access information from adjacent matches or separators inside a substitution pattern. Also, pilcrows are now escapable in substitution patterns.
- Options to introduce randomness were added wherever it made sense, so it's possible to pick random matches, shuffle lists of matches and characters, or pick one of multiple stages to execute at random, among other things.
- All intermediate stage results are now recorded in a *history*, which can be accessed in substitution patterns, to refer back to earlier strings. In a way, this lets you create variables by generating the desired string inside a dry run and then referring back to it using the history.
- Stages with list-like output (`L`ist, `S`plit, Pos`I`tions, `G`rep and `A`ntiGrep) can now configure the prefix, delimiters and suffix of the list format.
- And some more which I probably forgot about...

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
