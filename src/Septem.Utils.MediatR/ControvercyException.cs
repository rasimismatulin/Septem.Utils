using System;
using System.Globalization;
using System.Resources;
using System.Runtime.Serialization;

namespace Septem.Utils.MediatR;

[Serializable]
public class ControversyException : Exception
{
    public static ResourceManager IssueResourceManager;

    public ControversyException()
    {
    }

    public ControversyException(string message, CultureInfo cultureInfo, params object[] arguments) 
        : base(string.Format(IssueResourceManager.GetString(message, cultureInfo) ?? "Not localized message", arguments))
    {
            
    }

    public ControversyException(string message, Exception inner, CultureInfo cultureInfo, params object[] arguments) :
        base(string.Format(IssueResourceManager.GetString(message, cultureInfo) ?? "Not localized message", arguments), inner)
    {
    }

    protected ControversyException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}