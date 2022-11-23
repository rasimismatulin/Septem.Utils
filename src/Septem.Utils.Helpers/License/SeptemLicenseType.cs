using System.ComponentModel;

namespace Septem.Utils.Helpers.License;

public enum SeptemLicenseType
{
    [Description("Unknown")]
    Unknown = 0,
    [Description("Single")]
    Single = 1,
    [Description("Volume")]
    Volume = 2
}