# Retina

## What is Retina?

Retina is a regex-based programming language. It's main feature is taking some text via standard input and repeatedly applying regex operations to it (e.g. matching, splitting, and most of all replacing). Under the hood, it uses .NET's regex engine, which means that both the .NET flavour and the ECMAScript flavour are available.

Retina was mainly developed for [Code golf](https://en.wikipedia.org/wiki/Code_golf) which may explain its very terse configuration syntax and some weird design decisions.

## Running Retina

There is an up-to-date Windows binary of Retina in the root directory of the repository. Alternatively, you can build it yourself from the C# sources. I do not regularly test Retina with Mono, but previous versions have worked without problems.

## How does it work?

Each program is grouped into *stages*. A stage consists of one or two parts depending on its type (or *mode*). Each stage transforms its input (string) to an output string which is passed to the next stage. The first stage reads from the standard input stream. Each stage may optionally print its result to the standard output stream in addition to passing it on. By default, only the very last stage prints its result. Stages can also be grouped into loops.

By default, Retina reads a single source file, where each line is a separate "part" (so each stage is one or two lines):

    Retina ./program.ret

**Important:** Retina expects the file to use Unix-style line endings (a single linefeed character, 0x0A). If the file uses Windows-style line endings (`\r\n`, carriage return 0x0D followed by linefeed), the carriage returns will be contained at the ends of the parts.

Note that this means that no part can contain a linefeed character. However, escape sequences exist (`\n` or `$n` depending on whether you're in a regex or a substitution). If you actually want to use multiple lines for each parts (e.g. to split a complicated regex over multiple lines in free-spacing mode), you can use the `-m` flag. In this case, each part is either given as a separate file, or directly on the command line, by using the `-e` flag. So all of the following are valid invocations:

    Retina -m ./pattern.rgx
    Retina -m -e "foo.*"
    Retina -m ./pattern.rgx ./replacement.rpl
    Retina -m ./pattern.rgx -e "bar"
    Retina -m -e "foo.*" ./replacement.rpl
    Retina -m -e "foo.*" -e "bar"
    Retina -m ./pattern1.rgx ./replacement1.rpl ./pattern2.rgx
    Retina -m ./pattern1.rgx -e "bar" -e "foo*"

### The Pattern

Regardless of whether a stage consists of one or two parts, the first one will be the *pattern*. First and foremost, this will contain the regex to be used. However, if the file contains at least one backtick (`` ` ``), the code before the first backtick will be used to configure the stage - let's call this the *configuration string*. As an example, the pattern file

    _Ss`a.

configures Retina with `_Ss` (more on that later) and defines the regex as `a.`. Any further backticks are simply part of the regex. If you want to use backticks in your regex, but do not want to configure Retina, just start your pattern file with a single backtick (which translates to an empty configuration).

Most types of stages only use a single file.

### The Replacement

If the stage operates in Replace mode, it consists of two parts, where the first is a pattern as described above, and the second is the replacement string. You can use all the usual references to capturing groups like `$n`. If no replacement part is supplied (but Replace mode is enforced), the replacement string is assumed to be the empty string.

### The Configuration String

Currently, the configuration string simply is a (mostly) unordered bunch of characters, which switches between different options, modes and flags. This may get more complicated in the future. If multiple conflicting options are used, the latter option will override the former.

Some characters are available in all modes, some only in specific ones. Mode-specific options are denoted by non-alphanumeric characters and are mentioned below when the individual modes are discussed.

#### Regex Modifiers

[All regex modifiers available in .NET](https://msdn.microsoft.com/en-us/library/system.text.regularexpressions.regexoptions%28v=vs.110%29.aspx) (through `RegexOptions`), except `Compiled` are available through the configuration string. This means you don't have use inline modifiers like `(?m)` in the regex (although you can). All regex modifiers are available through lower case letters:

- `c`: Is for `CultureInvariant`. Quoting MSDN: "Specifies that cultural differences in language is ignored. For more information, see the "Comparison Using the Invariant Culture" section in the Regular Expression Options topic."
- `e`: Activates `ECMAScript` mode and essentially changes the regex flavour. Some of the other modifiers don't work in combination with this.
- `i`: Makes the pattern case-insensitive.
- `m`: Activates `Multiline` mode, which makes `^` and `$` match the beginning and end of lines, respectively, in addition to the beginning and end of the entire input.
- `n`: Activates `ExplicitCapture` mode. Quoting MSDN: "Specifies that the only valid captures are explicitly named or numbered groups of the form (?<name>…). This allows unnamed parentheses to act as noncapturing groups without the syntactic clumsiness of the expression (?:…). For more information, see the "Explicit Captures Only" section in the Regular Expression Options topic."
- `r`: Activates `RightToLeft` mode. For more information [see MSDN](https://msdn.microsoft.com/en-us/library/yd1hzczs(v=vs.110).aspx#RightToLeft).
- `s`: Activates `Singleline` mode, which makes `.` match newline characters.
- `x`: Activates free-spacing mode, in which all whitespace in the pattern is ignored (unless escaped), and single-line comments can be written with `#`.

#### Mode Selection

The default mode for a stage depends on whether there are any further parts in the program after the current *pattern*: if this pattern is the last part of the program, the stage defaults to Match mode. Otherwise it defaults to Replace mode. The following upper-case letters in the configuration string can override these defaults:

- `M`: Match mode.
- `R`: Replace mode
- `S`: Split mode.
- `G`: Grep mode.
- `A`: AntiGrep mode.
- `T`: Transliterate mode.

#### General Options

The following options apply to all modes:

- `(` and `)`: `(` opens a loop and `)` closes a loop. All stages between `(` and `)` (inclusive) will be repeated in a loop until an iteration doesn't change the result. Note that `(` and `)` may appear in the same stage, looping only that stage. Also, the order of `(` and `)` within a single stage is irrelevant - they are always treated as if all `(` appear before all `)`. Furthermore, an unmatched `)` assumes a `(` in the first stage, and an unmatched `(` assumes a `)` in the last stage. Loops can be nested. These options makes Retina [Turing-complete](http://en.wikipedia.org/wiki/Turing_completeness) (see below for details).
- `+`: Short-hand for `()`.
- `;` and `:`: Turn Silent mode off and on, respectively (this determines whether the result of a stage or loop is printed to the standard output stream). Every stage *and* every loop has a separate silent flag. All but the last outermost loop are silent by default. If a loop has been closed with `)` or `+` in the current configuration string *before* the `;` or `:`, the silent mode of that (last closed) loop is affected. Otherwise, the stage's own silent mode is affected. This means that `(:)` defines a stage which is looped and prints its result on each iteratio, but `()+` is a stage which is looped but only prints the result once the loop terminates. This is currently the only case where the order of options in the configuration string is not arbitrary.

## Operation Modes

As outlined above, Retina currently supports 6 different operation modes: Match, Replace, Split, Grep, AntiGrep, Transliterate.

### Match Mode

This mode takes the regex with its modifiers and applies it to the input. By default, the result is the number of matches.

Match mode currently supports the following options:

- `!`: Instead of the number of matches, the result is a newline-separated list of all matches.
- `&`: Consider overlapping matches. Normally, the regex engine will start looking for the next match after the *end* of the previous match. With this mode, Retina will instead look for the next match after the *beginning* of the previous match. Note that this will still not take into account overlapping matches which start at the same position (but end at different positions.)

Ultimately, this mode will probably receive the most elaborate configuration scheme, in order to print capturing groups or other information about the match.

### Replace Mode

Replace mode does what it says on the tin: it replaces all matches of the regex in the input with the replacement string, and returns the result. Replace mode currently doesn't have any dedicated options, but will probably get at least one more option in the future: limiting the number of replacements done per iteration (e.g. replace only the first match).

Replace mode does not use .NET's built-in `Regex.Replace`, but a custom implementation instead. All of the substitution elements `$...` that are valid in .NET also work in Replace mode. However, Retina understands the following additional substitution elements:

- `$n`: Is an escape sequence for a linefeed character (0x0A), much like `\n` inside a regex.

### Split Mode

This passes the regex on to `Regex.Split`. The result of `Regex.Split` separated by newlines is the result of this stage. This means that you can use capturing groups to include parts of the matches in the output.

Split mode comes with one additional option:

- `_`: Filter all empty strings out of the result before printing.

I might add more configuration options in order to control the output format or to limit the number of splits in the future.

### Grep and AntiGrep Mode

Grep mode makes Retina assume [grep's](http://en.wikipedia.org/wiki/Grep) basic mode of operation: the input is split into individual lines, the regex is matched against each line, and the result consists of all lines that yielded a match.

AntiGrep mode is almost the same, except that it prints all lines which *didn't* yield a match.

### Transliterate Mode

This mode is rather versatile mode, which is mainly intended for substituting individual characters for others. Its format is slightly different from the other modes, in that it consists of up to four segments on a single line. All of the following are valid Transliterate stages:

    config`from
    config`from`to
    config`from`to`regex

Here, `config` is just the regular configuration string, which needs to contain `T` to select this mode. If `to` is not given it is assumed to be an empty string. If `regex` is not given *or* empty it is assumed to be `[\s\S]+`, which always matches the entire input.

First, both `from` and `to` are expanded to lists of characters, using the following rules (reading left to right):

- The following escape sequences are known (only for convenience - you can also embed the characters directly in the source code): `\a` (bell, 0x07), `\b` (backspace, 0x08), `\f` (form feed, `0x0C`), `\n` (line feed, `0x0A`), `\r` (carriage return, `0x0D`), `\t` (tab, `0x09`), `\v` (vertical tab, `0x0B`). If these are encountered, they are added to the character list.
- If a `\` is encountered which is not followed by one of these characters, the next character itself is added to the list (this let's you add, e.g., `` ` `` to the list). If there is a single `\` left at the end of the stage, it is also added to the list literally.
- If a `-` is encountered before the end of the stage and a single character was added to the list just before this, it denotes a range of characters. Ranges can be ascending or descending (they can also be degenerate, e.g. `a-a`). Note that the `-` also acts as an escape for the next character, which means that ``A-` `` is a range and does *not* end the current part. However, the above special escape sequences are not known, so they cannot be used at the right end of a range: `a-\n` would add a range from `a` to `\` and the single character `n`. However `\n-a` would be a range from the line feed (0x0A) to `a`.
- `d` is equivalent to `0-9`.
- `w` is equivalent to `_0-9A-Za-z`.
- In any case not covered above, the encountered character is just added to the list literally.

If the expanded `to` list is shorter than the `from` list, it will be padded to the same length by repeating its last character. That is, the following specifications are equivalent:

    T`d`abc
    T`0123456789`abcccccccc

After this preprocessing has been done, the regex is applied to the input string. Parts which aren't matched are left unchanged, but in the matches, every character found in `from` is replaced by the character at the same position in `to`. Characters not found in `from` are left unchanged as well. If `to` is empty, the characters in `from` are removed from the matches instead.

This mode allows simple character transformations which would otherwise require listing every character separately. Some examples:

    Tx`A-Za-z`a-zA-Z  # Swap the case of all ASCII letters in the input.
    Tx`w`_da-zA-Z     # Same, but three characters shorter.
    Tx`a-z`A-Z`\b.    # Capitalise the first letter of each word.
    Tx`A-Z`N-ZA-M     # ROT-13.
    Tx`A-Z`Z-A        # Atbash cypher.

Transliterate mode currently doesn't have any dedicated options.

## Retina is Turing-complete

While the .NET-flavour itself is *just* short of being Turing-complete (as far as I know), Retina is, thanks to repeated Replace mode via the `+` option. As an example, I've implemented a [2-Tag System](http://en.wikipedia.org/wiki/Tag_system) "interpreter" in Retina.

Pattern file:

    +`^(.).(\w*)(?=\|.*\1>(\w*))|^(?<2>\w+).*

Replacement file:

    $2$3

With this Retina code in place, an arbitrary tag system can be supplied via STDIN, as long as its alphabet consists only of alphanumerical characters and underscores (this limitation could easily be removed). The input must consist of the initial word, as well as the set of production rules available for the tag system. For instance, [the first example system given on Wikipedia](http://en.wikipedia.org/wiki/Tag_system#Example:_A_simple_2-tag_illustration), would be written as

    baa|a>ccbaH,b>cca,c>cc

Note that the pattern is hardcoded to 2-tag systems, although it would also be possible to generalise this to `n`-tag systems, where `n` could be encoded in unary in the input string.
