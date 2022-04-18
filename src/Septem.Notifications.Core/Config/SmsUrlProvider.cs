using System;
using System.Net;

namespace Septem.Notifications.Core.Config;

public class SmsUrlProvider
{
    private readonly string _messagingUrl;

    public SmsUrlProvider(string apiEndpoint, string userName, string password, string senderName)
    {
        apiEndpoint = string.IsNullOrWhiteSpace(apiEndpoint) ? throw new ArgumentNullException(nameof(apiEndpoint)) : ClearTrailingBackSlash(apiEndpoint);
        userName = string.IsNullOrWhiteSpace(userName) ? throw new ArgumentNullException(nameof(userName)) : WebUtility.UrlEncode(userName);
        password = string.IsNullOrWhiteSpace(password) ? throw new ArgumentNullException(nameof(password)) : WebUtility.UrlEncode(password);
        senderName = string.IsNullOrWhiteSpace(senderName) ? throw new ArgumentNullException(nameof(senderName)) : WebUtility.UrlEncode(senderName);

        _messagingUrl = $"{ClearTrailingBackSlash(apiEndpoint)}?user={userName}&password={password}&from={senderName}";
    }

    private static string ClearTrailingBackSlash(string endpoint)
    {
        if (!endpoint.EndsWith("/"))
            return endpoint;

        return endpoint.Remove(endpoint.Length - 1);
    }

    public string CreateMessageUrl(string number, string message)
    {
        message = FixUnicodeSymbols(message);
        var urlEncodedNumber = string.IsNullOrWhiteSpace(number) ? throw new ArgumentNullException(nameof(number)) : WebUtility.UrlEncode(number);
        var urlEncodedMessage = string.IsNullOrWhiteSpace(message) ? throw new ArgumentNullException(nameof(message)) : WebUtility.UrlEncode(message);

        return $"{_messagingUrl}&gsm={urlEncodedNumber}&text={urlEncodedMessage}";
    }

    private string FixUnicodeSymbols(string message)
    {
        return message.ReplaceCase("ə", "e")
            .ReplaceCase("ı", "i")
            .ReplaceCase("ö", "o")
            .ReplaceCase("ü", "u")
            .ReplaceCase("ş", "sh")
            .ReplaceCase("ç", "c")
            .ReplaceCase("ğ", "g");
    }
}