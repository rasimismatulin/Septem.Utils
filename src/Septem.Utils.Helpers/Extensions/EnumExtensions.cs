using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Septem.Utils.Helpers.Extensions;

public static class EnumExtensions
{
    public static string ToDescription(this Enum en)
    {
        var memInfo = en.GetType().GetMember(en.ToString());
        if (memInfo.Length == 0) 
            return en.ToString();

        var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attrs.Length > 0 ? 
            ((DescriptionAttribute)attrs[0]).Description : 
            en.ToString();
    }

    public static ICollection<string> GetDescriptions<TEnum>() =>
        (from Enum value in Enum.GetValues(typeof(TEnum)) select value.ToDescription()).ToList();
}