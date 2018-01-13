using Retina.Replace.Nodes;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Retina.Replace
{
    class Parser
    {
        class NoElementException : Exception { }

        class ParserError : Exception
        {
            public ParserError() { }
            public ParserError(string message) : base(message) { }
            public ParserError(string message, Exception innerException) : base(message, innerException) { }
        }

        private bool CyclicMatches;

        private List<Token> Tokens;
        private int Current;

        private History History;

        public Parser(History history)
        {
            History = history;
        }
        public Parser(History history, bool cyclicMatches) : this(history)
        {
            CyclicMatches = cyclicMatches;
        }

        public Node ParseReplacement(string replacement)
        {
            Tokenize(replacement);

            return ParseTokens();
        }

        private void Tokenize(string replacement)
        {
            Tokens = new List<Token>();

            var tokenizer = new Regex(@"\G( # Use \G to ensure that the tokens cover the entire string.
              (?<repeat>[*])           # The binary repetition operator.
            |
              (?<elementClose>})       # } closes a dynamic element.
            |
              (?<concatClose>[)])      # ) closes a concatenation element.
            |
              [$]                      # All other elements start with a $.
              (
                (?<elementOpen>{)      # ${ opens a dynamic element.
              |
                (?<concatOpen>         # $.( or $( opens a concatenation element.
                  [.]?
                  [(]
                )
              |
                (?<unary>[\\lLuUT^])   # $^, $\, $l, $L, $u, $U, $T are unary operators.
              |
                (?<escape>             # $*, $$, $), $}, $n, $¶ are escape sequences of special characters.
                  [*$)}n\n]
                )
              |
                (?<element>            # Shorthands for various common elements.
                  [.]?                 # An optional length modifier is always available.
                  (
                    (?<numbered>       # $n are numbered groups.
                      [<>[\]]?         # Pull the value from an adjacent separator or match.
                      [#?]?            # Capture count or random capture.
                      (?:\d+|&)        # & is an alias for 0.
                    )
                  |
                    (?<context>        # $`, $' and $_ are context elements.
                      [<>[\]]?         # Pull the value from an adjacent separator or match.
                      %?               # Stop at the nearest linefeed.
                      [`'=""]
                    )
                  |
                    (?<history>        # $-n, $+n are history elements.
                      [-+]\d*
                    )
                  )
                )
              )
            |
              (?<literal>
                \d+                    # Numbers are literals but are treated as single tokens for the purpose of others
                                       # like $*.
              |
                .                      # Anything else is a literal (including $ that don't introduce a valid token).
              )
            )", RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

            MatchCollection tokenMatches = tokenizer.Matches(replacement);

            foreach (Match t in tokenMatches)
            {
                string source = t.Value;
                TokenType type;

                if (t.Groups["literal"].Success)
                    type = TokenType.Literal;
                else if (t.Groups["repeat"].Success)
                    type = TokenType.Repeat;
                else if (t.Groups["elementOpen"].Success)
                    type = TokenType.ElementOpen;
                else if (t.Groups["elementClose"].Success)
                    type = TokenType.ElementClose;
                else if (t.Groups["concatOpen"].Success)
                    type = TokenType.ConcatOpen;
                else if (t.Groups["concatClose"].Success)
                    type = TokenType.ConcatClose;
                else if (t.Groups["element"].Success)
                    type = TokenType.Shorthand;
                else if (t.Groups["unary"].Success)
                    type = TokenType.Unary;
                else if (t.Groups["escape"].Success)
                    type = TokenType.Escape;
                else
                    throw new ParserError("This shouldn't happen...");

                Tokens.Add(new Token(type, source));
            }

            Tokens.Add(new Token(TokenType.EOF, ""));
        }


        private Node ParseTokens()
        {
            return ParseTopLevel();
        }

        private Node ParseTopLevel()
        {
            var nodes = new List<Node>();
            while (!IsAtEnd())
            {
                if (Match(TokenType.ElementClose, TokenType.ConcatClose))
                    nodes.Add(new Literal(Previous().Source));
                else
                    nodes.Add(ParseUnary());
            }

            return new Concatenation(nodes);
        }

        private Node ParseUnary()
        {
            if (Match(TokenType.Unary))
            {
                Token op = Previous();
                Node argument;
                try { argument = ParseUnary(); }
                catch (NoElementException) { argument = new DynamicElement("&", History, false); }

                switch (op.Source[1])
                {
                case '^': return new Reverse(argument);
                case '\\': return new RegexEscape(argument);
                case 'l': return new LowerCaseHead(argument);
                case 'L': return new LowerCaseAll(argument);
                case 'u': return new UpperCaseHead(argument);
                case 'U': return new UpperCaseAll(argument);
                case 'T': return new TitleCaseAll(argument);
                default: throw new ParserError("Unknown unary operator encountered.");
                }
            }
            else
                return ParseRepeat();
        }

        private Node ParseRepeat()
        {
            try
            {
                Node leftArgument = ParseElement();
                if (Match(TokenType.Repeat))
                {
                    Node rightArgument;
                    try { rightArgument = ParseUnary(); }
                    catch (NoElementException) { rightArgument = new Literal("_"); }
                    return new Repetition(leftArgument, rightArgument);
                }

                return leftArgument;
            }
            catch (NoElementException e)
            {
                // We only end up here if the parsing attempt for leftArgument failed.
                if (Match(TokenType.Repeat))
                {
                    Node leftArgument = new DynamicElement("&", History, false);
                    Node rightArgument;
                    try { rightArgument = ParseUnary(); }
                    catch (NoElementException) { rightArgument = new Literal("_"); }
                    return new Repetition(leftArgument, rightArgument);
                }

                // If there is no "*", we propagate the exception outwards.
                throw e;
            }
        }

        private Node ParseElement()
        {
            if (Match(TokenType.Shorthand))
            {
                // Drop the leading "$".
                string shorthand = Previous().Source.Substring(1);
                return new DynamicElement(shorthand, History, CyclicMatches);
            }

            if (Match(TokenType.Escape))
            {
                char source = Previous().Source[1];

                switch (source)
                {
                case 'n': return new Literal("\n");
                case '\n': return new Literal("¶");
                default: return new Literal(source.ToString());
                }
            }

            if (Match(TokenType.Literal))
                return new Literal(Previous().Source);

            if (Match(TokenType.ElementOpen))
            {
                Node inner = ParseConcatenation();
                // Consume the closing } if there is one.
                Match(TokenType.ElementClose);

                return new DynamicElement(inner, History, CyclicMatches);
            }

            if (Match(TokenType.ConcatOpen))
            {
                bool length = Previous().Source[1] == '.';
                Node inner = ParseConcatenation();
                // Consume the closing } if there is one.
                Match(TokenType.ConcatClose);

                if (length)
                    return new Length(inner);
                else
                    return inner;
            }

            throw new NoElementException();
        }

        private Node ParseConcatenation()
        {
            var nodes = new List<Node>();
            while (!IsAtEnd())
            {
                try { nodes.Add(ParseUnary()); }
                catch (NoElementException) { break; }
            }

            return new Concatenation(nodes);
        }


        // Helper methods

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;

            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) ++Current;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek()
        {
            return Tokens[Current];
        }

        private Token Previous()
        {
            return Tokens[Current - 1];
        }
    }
}
