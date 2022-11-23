
namespace Septem.Utils.Helpers.Extensions;

public static class StringExtensions
{
    public static string ReplaceCase(this string str, string c1, string c2) =>
        str.Replace(c1.ToLower(), c2.ToLower()).Replace(c1.ToUpper(), c2.ToUpper());
}