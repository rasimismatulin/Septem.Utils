using System.ComponentModel;

namespace Septem.Utils.Helpers.ActionInvoke.Collection.Enums;

public enum SearchOptions
{
    [Description("Equals")] Equals,
    [Description("Like")] Like,
    [Description("Contains")] Contains,
}