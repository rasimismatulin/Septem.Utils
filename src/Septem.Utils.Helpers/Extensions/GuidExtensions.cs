using System;

namespace Septem.Utils.Helpers.Extensions
{
    public static class GuidExtensions
    {
        public static string Encode(this Guid guid) =>
            guid == default ? 
                default : 
                Convert.ToBase64String(guid.ToByteArray()).Replace("/", "-").Replace("+", "_").Replace("=", string.Empty);

        public static Guid Decode(this string guid) =>
            guid == default ? 
                default :  
                new Guid(Convert.FromBase64String(guid.Replace("-", "/").Replace("_", "+") + "=="));
    }
}
