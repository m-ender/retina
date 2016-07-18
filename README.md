# Retina

Retina is a regex-based recreational programming language. Its main feature is taking some text via standard input and repeatedly applying regex operations to it (e.g. matching, splitting, and most of all replacing). Under the hood, it uses .NET's regex engine, which means that both the .NET flavour and the ECMAScript flavour are available.

Retina was mainly developed for [Code golf](https://en.wikipedia.org/wiki/Code_golf) which may explain its very terse configuration syntax and some weird design decisions.

## Running Retina

There is an up-to-date .NET binary of Retina in the root directory of the repository. Alternatively, you can build it yourself from the C# sources. The code requires .NET 4.5. I do not regularly test Retina with Mono, but previous versions have worked without problems.

Source files can simply be passed as command-line arguments. For details of ways to invoke Retina [see the docs](https://github.com/mbuettner/retina/wiki/The-Language#basics).

## How does it work?

Full documentation of the language **[can be found in the Wiki](https://github.com/mbuettner/retina/wiki/The-Language)**. It might also be worth having a look at the **[changelog](https://github.com/mbuettner/retina/blob/master/CHANGELOG.md)**.
