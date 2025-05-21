using System.Collections.Generic;
using System.Drawing;

namespace Septem.Notifications.Abstractions;

public class FcmConfiguration
{
    private readonly Dictionary<string, string> _internalData = new();

    public string InstanceName { get; set; }

    public int? BadgeCount { get; set; }

    public IReadOnlyDictionary<string, string> Data => _internalData;

    public string Color { get; set; }


    public FcmConfiguration SetColor(Color color)
    {
        Color = HexConverter(color);
        return this;
    }

    public FcmConfiguration SetInstanceName(string instanceName)
    {
        InstanceName = instanceName;
        return this;
    }

    public FcmConfiguration SetBadgeCount(int badgeCount)
    {
        BadgeCount = badgeCount;
        return this;
    }

    public FcmConfiguration AddData(string key, string value)
    {
        _internalData[key] = value;
        return this;
    }

    private static string HexConverter(Color c)
    {
        return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
    }
}