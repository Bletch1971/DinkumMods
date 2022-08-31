using System;

namespace AlwaysActiveBooks
{
    [Flags]
    internal enum BookType
    {
        Manual = 0,
        Bug = 1,
        Fish = 2,
        Both = Bug | Fish,
    }
}
