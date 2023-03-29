using System;
using System.Linq;

namespace Septem.Utils.Helpers.CodeGeneration;

public static class RandomStringGenerator
{
    private static readonly Random Random = new();
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_";

    public static string Get(int length)
    {
        return new string(Enumerable.Repeat(Chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}