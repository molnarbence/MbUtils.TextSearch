﻿namespace MbUtils.TextSearch.Business;

public class StringSplitStrategy(string pattern) : ISearchTermCounterStrategy
{
    public int Count(string input) => input.Split([pattern], StringSplitOptions.None).Length - 1;
}