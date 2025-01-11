namespace Core.Strategies;

public class StringSplitStrategy : ISearchTermCounterStrategy
{
    public int Count(string input, string pattern) => input.Split([pattern], StringSplitOptions.None).Length - 1;
}