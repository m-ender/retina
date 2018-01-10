namespace Retina.Replace
{
    class Token
    {
        public TokenType Type { get; set; }
        public string Source { get; set; }

        public Token(TokenType type, string source)
        {
            Type = type;
            Source = source;
        }
    }

    enum TokenType
    {
        Literal = 0,
        Repeat,
        ElementOpen,
        ConcatOpen,
        ElementClose,
        ConcatClose,
        Unary,
        Escape,
        Shorthand,
        EOF,
    }
}
