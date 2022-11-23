using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Septem.Utils.Helpers.DeviceIdentification;

namespace Septem.Utils.Helpers.License;

public class LicenseChangedEventArgs
{
    public LicenseChangedEventArgs(ValidatedSeptemLicense oldLicense, ValidatedSeptemLicense newLicense)
    {
        OldLicense = oldLicense;
        NewLicense = newLicense;
    }

    public ValidatedSeptemLicense OldLicense { get; set; }
    public ValidatedSeptemLicense NewLicense { get; set; }
}

public class LicenseChecker
{
    public event EventHandler<LicenseChangedEventArgs> OnLicenseUpdated;

    private readonly ILogger<LicenseChecker> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly string _accessTokenRequestPayload;
    private readonly string _licenseDownloadRequestPayload;

    public ClientDeviceInfo ClientDeviceInfo { get; }

    private readonly byte[] _certPubicKeyData;
    private readonly string _licenseFilePath;
    private readonly string _licenseServerUrl;

    public string DeviceUid { get; }

    public ValidatedSeptemLicense CurrentLicense { get; private set; }

    public LicenseChecker(ILoggerFactory loggerFactory, Guid clientUid, string clientSecret, byte[] certPubicKeyData, string licenseFilePath, string licenseServerUrl)
    {
        _logger = loggerFactory.CreateLogger<LicenseChecker>();
        DeviceUid = HardwareInfo.GenerateUid();

        _certPubicKeyData = certPubicKeyData;
        _licenseFilePath = licenseFilePath;
        _licenseServerUrl = licenseServerUrl;

        _accessTokenRequestPayload = JsonSerializer.Serialize(new { clientUid, password = clientSecret });
        _licenseDownloadRequestPayload = JsonSerializer.Serialize(new { clientUid, deviceUid = DeviceUid });
        ClientDeviceInfo = new ClientDeviceInfo(clientUid, DeviceUid);
        CurrentLicense = GetFileLicense();
    }

    public async Task DownloadLicense(CancellationToken cancellationToken = default)
    {
        var serverLicense = await GetServerLicense(cancellationToken);

        if (serverLicense == null)
            return;

        if (Equals(CurrentLicense, serverLicense))
            return;

        _logger.LogInformation($"License updated. ServerLicense: {serverLicense}");
        OnLicenseUpdated?.Invoke(this, new LicenseChangedEventArgs(CurrentLicense, serverLicense));
        CurrentLicense = serverLicense;
        await File.WriteAllTextAsync(_licenseFilePath, CurrentLicense.PublicKey, cancellationToken);
    }


    private ValidatedSeptemLicense GetFileLicense()
    {
        try
        {
            if (!File.Exists(_licenseFilePath))
                return null;

            var licenseKey = File.ReadAllText(_licenseFilePath);
            if (string.IsNullOrWhiteSpace(licenseKey))
                return null;

            var fileLicense = SeptemLicenseHandler.ParseLicenseFromBase64String(licenseKey, _certPubicKeyData, ClientDeviceInfo);
            return fileLicense;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error read file license");
            return null;
        }
    }


    private async Task<ValidatedSeptemLicense> GetServerLicense(CancellationToken cancellationToken = default)
    {
        try
        {
            var accessToken = await GetJwtAccessToken(cancellationToken);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                _logger.LogError("Error download server license. Access token is null");
                return null;
            }

            var license = await DownloadLicenseString(accessToken, cancellationToken);
            return license;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error download server license");
            return null;
        }
    }

    private async Task<ValidatedSeptemLicense> DownloadLicenseString(string accessToken, CancellationToken cancellationToken)
    {
        var response = await SendHttp($"{_licenseServerUrl}/api/licenses/active", HttpMethod.Get, _licenseDownloadRequestPayload, accessToken, cancellationToken);
        var serverLicense = SeptemLicenseHandler.ParseLicenseFromBase64String(response, _certPubicKeyData, ClientDeviceInfo);
        return serverLicense;
    }

    private async Task<string> GetJwtAccessToken(CancellationToken cancellationToken)
    {
        var response = await SendHttp($"{_licenseServerUrl}/api/login", HttpMethod.Post, _accessTokenRequestPayload, default, cancellationToken);
        var jwtToken = JsonSerializer.Deserialize<JwtTokenResponse>(response, _jsonSerializerOptions);
        return jwtToken is { IsSuccess: true } ? jwtToken.AccessToken : null;
    }

    private async Task<string> SendHttp(string url, HttpMethod method, string payload, string accessToken = default, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        using var message = new HttpRequestMessage(method, url);
        message.Content = new StringContent(payload);
        message.Content.Headers.Remove("Content-Type");
        message.Content.Headers.Add("Content-Type", "application/json");
        if (!string.IsNullOrWhiteSpace(accessToken))
            message.Headers.Add("Authorization", $"Bearer {accessToken}");
        using var result = await client.SendAsync(message, CancellationToken.None);

        if (result.IsSuccessStatusCode)
            return await result.Content.ReadAsStringAsync(cancellationToken);

        _logger.LogError($"Invalid HTTP response code Url:{url}. Method:{method}. Payload:{payload}. Status code:{result.StatusCode}");
        return null;
    }
}