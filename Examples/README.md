This document contains several example programs, including detailed explanations, to act as a tutorial and showcase some of Retina's features. The source files for all programs are contained in this directory, so you can easily run the programs yourself. This document also includes links to an online test environment.

The explanations will assume at least a basic familiarity with regex. If you've never used regex before, the tutorial at [regular-expressions.info](https://www.regular-expressions.info/tutorial.html) is a great learning resource.

A note for readers with an interest in code golf: most of these programs are not as short as they could be. Instead, I've tried to use straightforward approaches and avoid unnecessary implicit syntax like unmatched brackets.

## Hello, World!

    K`Hello, World!

[Try it online!](https://tio.run/##K0otycxLNPz/3zvBIzUnJ19HITy/KCdF8f9/AA "Retina – Try It Online") The code can also be found in `hello-world.ret`.

Let's start with the simplest of all programs: the classic "Hello, World!". This program consists of a single *Constant* stage (indicated by the `K`), which discards the program's input and replaces it with the constant string `Hello, World!`. By default, Retina implicit prints the result of the last stage, so this is all we need to do.

## Converting matrix syntax

    m`^{{
    [
    m`}}$
    ]
    }, {
    ; 

[Try it online!](https://tio.run/##PY6xDsIwDER3f4UHRg9xEgqIT0FU7cDAUAbEZvnbgy8NDInuzs@5vB@f52vV1rZlNqMbbYv7ge7kwkZXbs1M3Slu4YIwC9c9qMKT8ElYKwahNYZacEYQHjbBHkMMJP8fCIc8hQKjZ@HLgEIhKl0C0l8TKnLaqzs9/qV9OdDJ/Qs "Retina – Try It Online") The code can also be found in `matrix-conversion.ret`.

This programs takes a bunch of 2D arrays using Mathematica's syntax (one per line), and converts them to MATLAB's syntax:

    Mathematica:  {{1, 2}, {3, 4}}
    MATLAB:       [1, 2; 3, 4]

First, we should split the program into its individual stages. Each line in a Retina program is called a *source*, and each stage is made up of one or more sources. This program doesn't contain any configuration that would affect stage types or the number of sources required for a stage, so we can follow the simple default rules:

- If we've only got one source left, that source forms a *Count* stage.
- Otherwise, this source and the next together form a *Replace* stage.

So, this program can be split into three Replace stages:

    m`^{ *{
    [

<!-- -->

    m`} *}$
    ]

<!-- -->

    } *, *{
    ; 

Each stage of a program modifies the *working string* (initially, this is the program's input), and passes the result on to the next stage.

The first source of each stage defines the regex we want to match and the second source contains the substitution. The backticks are used to separate a [*configuration*](https://github.com/m-ender/retina/wiki/The-Language#configuring-stages) from the regex (configurations always appear on the first source of a stage, if at all). These configurations are used to set various options for the stages. In this case, we use it to activate the *multiline* regex modifier, which makes `^` and `$` match at the beginning and end of lines (instead of just matching the beginning and end of the whole string).

With that in mind, the three replacements are very straightforward:

1. We first replace the two opening braces with a single opening bracket.
2. Then we replace the two closing braces with a single closing bracket.
3. And finally, we turn all the intermediate braces (which separate individual lines of the matrix) into semicolons.

The final value of the working string is printed automatically at the end of the program.

## Collatz

    /^1$/^+¶</[13579]$/(`.+
    $.(_3*$&*_)
    .+
    *
    )`__

[Try it online!](https://tio.run/##K0otycxLNPz/Xz/OUEU/TvvQNhv9aENjU3PLWBV9jQQ9bS4VPY14Yy0VNa14TS4gV4tLMyE@/v9/QyNjAA "Retina – Try It Online") The code can also be found in `collatz.ret`.

This program expects an integer as input and implements the iteration defined by the [Collatz conjecture](https://en.wikipedia.org/wiki/Collatz_conjecture):

- If **N** is even, map it to **N/2**.
- If **N** is odd, map it to **3N+1**.

We can use this program to look at some of Retina's control flow features. We'll also see how we can perform arithmetic in Retina (which only operates on strings).

First, we should split the program into its individual stages. Each line in a Retina program is called a *source*, and each stage is made up of one or more sources. This program doesn't contain any configuration that would affect stage types or the number of sources required for a stage, so we can follow the simple default rules: if we've only got one source left, that source forms a *Count* stage; otherwise, this source and the next together form a *Replace* stage. So, this program can be split into three stages:

    /^1$/^+¶</[13579]$/(`.+
    $.(_3*$&*_)

<!-- -->

    .+
    *

<!-- -->

    )`__

The first two are Replace stages and the last one is a Count stage. The `` ` `` on the first stage designates everything left of it as a [*configuration*](https://github.com/m-ender/retina/wiki/The-Language#configuring-stages). Configurations are used to a) ... well ... configure the current stage, but also b) to wrap one or more stages in a [*compound* stage](https://github.com/m-ender/retina/wiki/The-Language#compound-stages), which can provide additional behaviour – like control flow and output, for example.

This particular configuration string is quite a mouthful. It's best to a read a configuration from right to left, in chunks corresponding to compound stages:

- `/[13579]$/(`: The `(` immediately introduces a compound stage (specifically, a *group* stage), so there is no configuration for the Replace stage itself. Groups usually contain multiple stages, and continue until the matching `)`. In our program, that matching `)` is on the final stage, so this group contains all three stages of our program (yes, it also contains the stage that the `)` appears on, even though the `)` is visually before the stage itself).

  The group is configured by a single option, `/[13579]$/`, which is a regex that matches odd numbers. If we give a regex option to a group, it becomes an if/else construct: if the regex matches, the first child stage is executed. Otherwise, the remaining stages are executed. So for odd numbers, we execute the first Replace stage; and for even numbers, we execute the second Replace stage, followed by the Count stage.

- `¶<`: The `<` introduces the next compound stage (an *output* stage of the *pre-print* variety). This prints the *input* of its child stage before executing that child stage. As you'll see in a minute, we'll be running all of this in a loop. So the first time around, this output stage prints the program's input, and on subsequent loop iterations it will print the result of the previous iteration.

  The `¶` (a *pilcrow*) is a string option. Whenever you see a pilcrow in a Retina program, it's actually an alias for a good old linefeed (LF, 0x0A). The reason for this is that Retina uses linefeeds in the source code as the separator between sources, but sometimes you need actual linefeeds inside those sources. Hence, after the Retina splits the program's source file into lines/sources, it replaces all pilcrows with linefeeds.

  Linefeeds are a special kind of shorthand string option: normally, a string option uses double quotes, like `"foo"`. If the string only contains a single character, we can use a leading single quote instead, like `'!` as a shorthand for `"!"`. But if that single character happens to be a linefeed, we can even omit the single quote. So `¶` is identical to the option `"¶"`.

  When we give a string option to an output stage, this string is printed after the stage's usual output. In other words, all that the pilcrow does is to add a trailing linefeed to the output, so each step of the iteration appears on a separate line.

- `/^1$/^+`: The `+` introduces the final compound stage (a *loop* stage). This one has two options. Their order is irrelevant, so we go left to right here.

  `/^1$/` is another regex option, as you might expect. This regex, of course, matches only the exact string `1`. If you give a regex option to a loop stage, it becomes a while loop: as long as the regex matches, keep executing its child stage. But this is where the other option comes in.

  `^` is a generic option called "reverse". It is available on most types of stages, but what exactly it does, depends on the type of stage. In the case of loop stages, it inverts the logic of the while-conditional. In other words, since we're using both a regex option and the reverse option, this is actually an *until* loop: keep executing the child stage, until this regex matches the string.

So these three compound stages form the backbone of our program's structure: we repeatedly print the current string; then execute either the first or the other two stages, depending on the current integer's parity; and then we check whether we've reached `1`, otherwise we start from the top.

If you've paid attention, you might be wondering why the program also prints the final `1`, since that is never the input of a loop iteration (the loop stops once we hit `1`). Remember the implicit output we used in the ["Hello, World!"](#hello-world) program? Exactly the same thing is happening here: once we're done with the loop, the string ends up with the value `1` and the program ends. This final value is printed automatically.

Let's look at how we implement the actual arithmetic. Having taken care of all the configuration, the first stage (and the code for the odd case) is just:

    .+
    $.(_3*$&*_)

All atomic stages (except the constant stage, we've seen in the ["Hello, World!"](#hello-world) program) have a regex as their first source. If there's a second source, it will usually be a [substitution pattern](https://github.com/m-ender/retina/wiki/The-Language#substitution-syntax). This stage just replaces the entire string with the substitution pattern on the second line.

In general, the way we do integer arithmetic in Retina is by using [unary](https://en.wikipedia.org/wiki/Unary_numeral_system) representations. That is, we represent the natural number **n** as a string of **n** (usually identical) characters. String manipulation is quite effective at expressing arithmetic functions on unary numbers. The canonical character (or digit) for unary representations in Retina is the underscore, `_`, but you can use whatever you like.

This stage is run for odd integers, so what we need it to do is to triple the integer, and then add one. Here's how we do this.

Retina's substitution syntax is similar to those you may have seen in other programming languages (using `$n` to insert capturing groups, and `$&` for the entire match, for example), but it contains many unique additions. The two unique ones we're using here are the binary operator `*` (repeat) and the length measurement `$.(…)`.

So `*` is the repetition operator: `k*s` yields a string containing **k** copies of the string `s` (where `k` and `s` may themselves be complex substitution expressions). `*` is right-associative, so we should be looking at `$&*_` first. This uses the match (i.e. the current integer) as **k** and repeats the character `_`. In other words, this just gives us a unary representation of the current value **n**. Then we pass this to `3*…`, which repeats this whole string three times. We end up with a unary representation of **3n**. To add one, we simple prepend another underscore. So `_3*$&*_` gives us a unary representation of **3n+1**.

Now we just need to get back to decimal: converting from unary to decimal is as easy as counting the length of the unary representation. `$.(…)` does exactly that: it evaluates the `…` and then measures its length. The neat part is that this computation is lazy, so we never actually have this big unary string in memory: instead, the `$.(…)` operation *knows* what repetition does to the length of a string, and computes the resulting length directly whenever possible.

Lastly, we need to look at the even case, where we have two stages (a Replace and a Count stage). The bad news is, we can't easily do division with a similar lazy approach, so for this particular problem, we will always end up creating the big unary string after all, once we reach an even number. Nevertheless, dividing a value by two is easy enough in unary. First we convert **n** to unary:

    .+
    *

Now what's this, a lone `*` in a substitution? The `*` operator can be used with implicit operands if it appears at the boundary of the pattern, or a bracketed expression like `$(…)`, or right of an other repetition operator. Its default left-hand operand is `$&` and its default right-hand operand is `_`. So this substitution is equivalent to `$&*_`, which again expands **n** in unary, using underscores. The reason for this shorthand is that expanding entire matches in unary is a very common operation when trying to do arithmetic in Retina, so using a single `*` saves a lot of typing. And yes, we could have made use of this implicit syntax in the previous stage, but one thing at a time.

Dividing a unary number by a fixed integer is trivial, especially if want to convert the result to decimal anyway: to divide by **2**, for instance, all we need to do is count the number of matches of the regex `__` (containing two underscores). Since matches can't overlap, this just count how often **2** fits into **n**, implementing division in a very neat and visual way. Remember that the last stage is a Count stage? That's exactly what it does:

    __

And that's it for the even case: we use one stage to go to unary, and another to do the division and go back to decimal in one swipe.

## Fibonacci, take one

    K`0¶1
    {*\G0`
    )`(\d+)\n(\d+)
    $2$n$.($1*_$2*_)

[Try it online!](https://tio.run/##K0otycxLNPz/3zvB4NA2Q65qrRh3gwQuzQSNmBRtzZg8MMWlYqSSp6KnoWKoFa9ipBWv@f8/AA "Retina – Try It Online") The code can also be found in `fibonacci.ret`.

The [Fibonacci numbers](https://en.wikipedia.org/wiki/Fibonacci_number) are a famous sequence, where you start from two `1`s and repeatedly obtain the next term by adding the two most recent ones. The sequence starts with:

    0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, ...

This program continually prints out the number of the sequence. We will use this program to introduce a new stage type, a new type of option, show other ways to use linefeeds inside the program's sources and reinforce some of the ideas from the Collatz program.

Upper-case letters in configurations are used to specify the type of a stage. Most stage types generally use only one source (except the default Replace stage, and unless there are certain other options in the configuration). So this program is split into three stages as follows:

    K`0¶1

<!-- -->

    {*\G0`

<!-- -->

    )`(\d+)\n(\d+)
    $2$n$.($1*_$2*_)

You've already seen `K` in the ["Hello, World!"](#hello-world) example: it's a Constant stage, which discards its input and sets the working string to the constant given after the backtick. In this case, we initialise the working string to the value:

    0
    1

Since the next Fibonacci number always depends on the last two, we will keep track of two numbers, but always only print one of them. Linefeeds are often useful for separating multiple strings, because they are often treated specially by the regex, and Retina also has some stage types which work with lines.

For the second stage, there's a bit more going on in the configuration, so we should again separate it into chunks corresponding to compound stages, and go through them from right to left:

- `G0`: This configuration applies directly to the current atomic stage itself. `G` sets the stage type to *Grep*: the basic idea is to filter the lines of the working string, based on whether they contain a match of the stage's regex. In this case, the regex of the stage is empty, so it matches everywhere. So without anything else, this stage would do nothing at all, because all lines get at least one match.

  However, there's the second option, `0`. This is a [*limit*](https://github.com/m-ender/retina/wiki/The-Language#limits). These are used to select elements from certain lists. Much like the reverse option `^`, the exact meaning of the limit depends on the stage it is used in, and how many limits were already specified for that stage.

  In the case of atomic stages, the first limit is always used to filter the matches of the given regex. Limits are 0-based, so a limit of `0` refers to the first match. So while the empty regex might give a whole bunch of matches, we disregard all but the first one of them. That first match will always be at the beginning of the first line, so after applying the limit, all further lines no longer contain matches and are discarded by the Grep stage. So ``G1` `` is an easy way to keep only the first line of the current working string.
- `\`: This is a compound stage. In fact, it's another type of output stage: it prints the result of its child stage, followed by a trailing linefeed (so in this case, we don't need the explicit string option `¶`).
- `*`: Another compound stage: a dry run. You might be worried that the Grep stage just discarded on of the two numbers we need to continue our computation. By wrapping it in a dry run, we can restore the previous working string afterwards. A dry run executes its child stage, but doesn't actually affect the working string. As in this case, you will often find output stages inside dry run stages. This construction lets you print an intermediate result without actually messing up the working string.
- `{`: This character is actually a shorthand for `+(`, so it introduces two compound stages at once: a group which contains this and the next stage, and a loop which iterates the whole group.

  It may seem awkward that the group is opened with `{` but closed with `)`. Couldn't we use `}`? Well, `}` does in fact exist, but analogously to `{`, it's shorthand for `+)`. So if we had `{` *and* `}` that's identical to using both `+(` and `+)`, which would introduce two loop stages. Therefore, most of the time only one curly brace is used.

  You'll also notice that this loop stage doesn't have any configuration, as opposed to the one we used for the [Collatz](#collatz) program. Without any configuration, `+` represents a *convergence loop*. This loop continues until the child stage fails to modify the working string. In our program, each iteration will change the working string (because the numbers keep growing), so this loop never terminates.

The final stage is a regular replace stage again. It matches the two numbers with `(\d+)\n(\d+)`, capturing them in groups `1` and `2`. Note that inside a regex we can use `\n` for linefeeds, so we don't have to type the somewhat inconvenient pilcrow. This is then replaced with the substitution pattern `$2$n$.($1*_$2*_)`. You've already seen most of this in the Collatz program: the second integer becomes the first and we go through unary representations to add the two integers together to become the new second integer. In other words, `a b --> b (a+b)`. Note that inside a substitution pattern, we can use `$n` for linefeeds. It's only inside configurations and constant stages that we need to use the pilcrow.

Summing up, here is the overall structure of our program:

- We initialise the working string to the first two values.
- Then we start an infinite loop which...
  - Prints out the first number without modifying the working string.
  - And then computes the next number while getting rid of the one we just printed and no longer need.

## Fibonacci, take two

    K`1
    K`0
    !#2//+¶<`.+
    $.($&*_$-1*_)

https://tio.run/##K0otycxLNPz/3zvBkMs7wYBLX1/70DYbRWWjBD1tLhU9DRU1rXgVXUOteM3//wE The code can also be found in `fibonacci-history.ret`.

This program does the same as the previous one: it prints the Fibonacci numbers indefinitely, one number per line. But it uses a different approach to do so.

After the previous program, we could raise the question: is it possible to solve this without having to keep two numbers around in the working string? It's a bit inconvenient to have to filter out one of the numbers to print the current value. And of course the answer is, we can! Retina provides a way to refer back to the previous results of stages, which lets us access the value before the one we've currently got in the working string.

To do so, we need to back up one step in the sequence though: when we're at the first value `0` and want to go to the second value `1`, there is no penultimate value we could refer back to. However, if we start our Fibonacci sequence with an additional `1`, it's all fine:

    1, 0, 1, 1, 2, 3, 5, ...

We will start printing from the `0`.

The way this program works is that Retina keeps track of a so-called [history](#history). There's two parts to it, but the one we care about for now is the *result log*. By default, every stage in the program records its result in this log. We can then refer back to the **n**th most recent result in a substitution pattern with `$-n` (`n` here is 0-based). There are some compound stages, which don't record their results in the log: output stages and dry run stages without further configuration. The reason for this is that these can never modify the working string – you can get an output stage's result by looking at its child stage's result, and you can get a dry run's result by looking at the previous stage's result.

Let's look at this program and split it into stages:

    K`1

<!-- -->

    K`0

<!-- -->

    //+¶<!#2`.+
    $.($&*_$-1*_)

We start with two constant stages. We first set the working string to `1` and then to `0`. Yes, that means that the result from the first stage is discarded immediately, but it is still recorded in the result log which is where we need it.

The third stage is again a replace stage with a bunch of configuration:

- `!#2`: This is a global flag. It can really appear anywhere in any configuration, because it doesn't apply to any particular stage. Its purpose is to restrict the size of the result log: if we keep the result log on its default setting, every loop iteration will add an entry to it, and we'd eventually run out of memory much sooner than we might expect. We only ever really need the last two results, so we can use this option to restrict the result log's length to two entries. Whenever a third entry is added, the oldest one is discarded.

  You might wonder if the first Fibonacci program wouldn't suffer from this problem, as it also runs in an infinite loop. However, Retina is clever enough to disable the result log entirely if there's no chance of accessing it anyway (because no substitution pattern contains a reference to it).

  However, Retina cannot currently determine its size automatically, so when we do reference the result log, we will usually want to restrict its size to as many (or as few) entries as we need.
- `¶<`: You've seen this before in the [Collatz](#collatz) program: this prints the stage's input with a trailing linefeed before running the stage itself.
- `//+`: A while loop. Why can't we just use a single `+` as we did for the first Fibonacci program? Since we're now only keeping track of one number, there's one point in the sequence where the value of the working string won't change: the two consecutive `1`s after the initial zero. Since a single `+` is a convergence loop, it would stop at that point (it doesn't care about the fact that the iteration still has the side effect of modifying the result log).

  So to get a proper infinite loop, which continues even if the string doesn't change for an iteration, we turn it into a while loop with a regex option. Since the regex is empty, it always matches, so this is Retina's canonical infinite loop.

The atomic (Replace) stage itself replaces the entire current value with `$.($&*_$-1*_)`. This is similar to what we did for the first Fibonacci program, but this time we take the previous value from the result log with `$-1`.

## FizzBuzz

    100{.`^
    _
    *\(`^(_{3})+$
    Fizz,$&
    \b(_{5})+$
    Buzz
    ,_*

    _+
    $.&

[Try it online!](https://tio.run/##K0otycxLNPz/39DAQK86IY4rnksrRiMhTiO@2rhWU1uFyy2zqkpHRY0rJgkoZAoWciqtquLSidfi4orX5lLRU/v/HwA "Retina – Try It Online") The code can also be found in `fizzbuzz.ret`.

FizzBuzz is a classic problem for coding interviews: print the numbers from 1 to 100, but replace numbers divisible by **3** with `Fizz`, numbers divisible by **5** with `Buzz` and those divisible by both with `FizzBuzz`.

Let's start once more by splitting the source code into stages. This time we've only got replace stages, so it's simply:

    100{.`^
    _

<!-- -->

    *\(`^(_{3})+$
    Fizz,$&

<!-- --> 

    \b(_{5})+$
    Buzz

```
,_*

```

    _+
    $.&

The first stage contains a bit of configuration:

- `.`: This is a global option like `!#n`. It doesn't matter where it appears because it doesn't affect any stage in particular. This is the *silent* option, which disables Retina's implicit output at the end of the program. We're using it, because we'll have a loop that prints each of the 100 lines already, so there's nothing left to print at the end of the program (and as it happens, there will also be some junk on the working string that we don't want to print anyway).
- `100{`: You already know that this is short for `+(`. Let's talk about the group first: there's no closing `)` anywhere in program. This is perfectly fine: parentheses indicating groups don't have to be balanced. If there is no matching parenthesis, the group simply continues until the end of the program. An unmatched `(` represents the idea, "apply [this] to the remainder of the program", where [this] is any option or further compound stage applied to the group. It's also possible to use unmatched `)` (which would then start at the beginning of the program), but this is mostly useful for code golf, as it's usually less confusing to put any configuration at the *front* of a group.

  The `100` configures the loop stage: if we give an integer option to a loop, it restricts the number of iterations that will be performed. The loop is still a convergence loop, so it might stop early if the working string stops changing, but it will stop after at most 100 iterations.

The replace stage itself inserts an underscore at the beginning of the working string. We'll be using the working string as a *unary* loop counter, so this increments the loop counter. For the **i**th iteration, the working string will contain **i** underscores.

The second stage contains some more configuration:

- `(`: We wrap the remaining stages in another group.
- `\`: We wrap that group in an output stage, to print the result of each iteration with a trailing linefeed.
- `*`: And we wrap *that* in a dry run, so that we don't lose the loop counter in the working string after computing the output for the current line.

To turn the unary loop counter into the required result for the current line, we use the four replace stages:

    ^(_{3})+$
    Fizz,$&

Measuring divisibility by a fixed integer is trivial in unary: to check divisibility by 3, we make sure that the entire string can be matched by repeatedly matching exactly three underscores. If that's the case we insert a `Fizz,` in front of the loop counter. The purpose of the comma is to separate the two with a word boundary – we'll get rid of it in time.

    \b(_{5})+$
    Buzz

This is similar to the previous stage, but there are a couple of crucial differences: instead of anchoring the match to the beginning of the string, we anchor it to a word boundary. This is important in case we have already inserted a `Fizz,` (the underscores no longer make up the entire string but the comma now ensures that there's still a word boundary there).

We also don't reinsert the unary counter, because we no longer need it if the number is divisible by 5, and therefore we also don't need a comma for separation. Before we move on to the last two stages, let's consider the four cases we have now:

1. Numbers divisible *only* by 3 like **6** have been modified by the first stage of the loop:

       Fizz,______

2. Numbers divisible *only* by 5 like **5** itself have been modified by the second stage of the loop:

       Buzz

3. Numbers divisible by both 3 and 5 like **15** have been modified by both stages:

       Fizz,Buzz

4. Numbers which are divisible by neither 3 nor 5 like **4** have not been modified at all:

       ____

There's a few things we need to do to this to get the correct result: a) we need to get rid of those commas. b) we need to get rid of the underscores if there's a `Fizz` and c) we need to convert the case of only underscores to decimal. Luckily, we can do a) and b) at the same time with a very simple replacement:

```
,_*

```

This matches commas, as well as underscores after them if there are any, and removes them from the string. This way, we don't touch the underscores in numbers like **4**, because there's no preceding comma.

Now all that is left to do is to convert any remaining underscores to decimal:

    _+
    $.&

We do this by matching the underscores and replacing the match with its length. `$.&` is essentially equivalent `$.($&)`. This shorthand notation of inserting a `.` after the `$` to directly compute a single element's length works with most substitution elements like `$.1` or `$.-2`.

And that's it: the result of the iteration gets printed and we continue for another iteration until we've printed all 100 lines. There is no automatic output at the end of the program, because we've disabled it with `.`.

## Quine

    >>K`>>K`

[Try it online!](https://tio.run/##K0otycxLNPz/387OOwGE//8HAA "Retina – Try It Online") The code can also be found in `quine.ret`.

There isn't *much* to learn from this program, but quines are an interesting puzzle in most programming languages, so I felt a list of example programs couldn't be complete without one. And there are still two small points about Retina I can make with this program.

A [quine](https://en.wikipedia.org/wiki/Quine_(computing)) is a program which prints its own source code (usually without being allowed to read its own source code directly). It's usually nontrivial to write a quine, because any code required to print something needs to be printed as well. Retina's syntax rules make this a bit easier.

The first noteworthy thing about the program is that it contains two backticks: usually the configuration only continues until the first backtick (there are exceptions to this, for example you can use a backtick inside a string option, in which case this backtick would not terminate the configuration part of the source), so the second ``>>K` `` is just the stage's constant string.

Of course, this constant is just a repetition of the configuration itself. Let's look at that configuration:

- `K`: Marks the stage as a constant stage – we've seen those before. This means we discard the input and set the working string to ``>>K` ``.
- `>`: This is another output stage. It's actually the simplest form of output stage, which simply prints the result of its child stage without a trailing linefeed. In fact, you can think of `\` as a shorthand for `¶>`.

  Anyway, with this stage, we've printed the first half of the source code.
- `>`: This wraps everything in *another* output stage, so we'll just print the string again! And with that, we're already done. After printing the constant ``>>K` `` twice, we've exactly printed the source code.

But wait a minute. What about the implicit output at the end of the program? Well, we don't have any in this case. The reason for that is in the details of how Retina's implicit output is defined. Once Retina has parsed the entire program into a tree of (atomic and compound) stages, it looks at the very last stage (which may be either atomic or compound). If that stage *isn't* an output stage, or it's an output stage of the pre-print variety (`<`), it gets wrapped in a plain output stage implicitly.

The reason that we don't just wrap the final stage in an output stage regardless, is that this allows us to configure Retina's final output simply by making the stage explicit (which we can even do with a pre-configured output stage like `\`), without having to disable the implicit output with the `.` option.

This means that the second `>` actually replaces the implicit output, and we only get two copies of the constant string.

As quines are commonly viewed as puzzles, here is a little challenge: can you find a shorter quine? And can you find a shorter quine which contains two lines? Assume that the program's input is empty. (The quine given here would work for arbitrary input, because the constant stage discards it anyway.)