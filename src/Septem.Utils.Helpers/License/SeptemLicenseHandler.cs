using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Septem.Utils.Helpers.DeviceIdentification;
using Septem.Utils.Helpers.Extensions;

namespace Septem.Utils.Helpers.License;

public class SeptemLicenseHandler
{
    public static ILogger<SeptemLicenseHandler> Logger;

    public static string GenerateLicenseBase64String(SeptemLicense lic, byte[] certPrivateKeyData, SecureString certFilePwd)
    {
        try
        {
            var licenseObject = new XmlDocument();
            using (var writer = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(SeptemLicense), new[] { lic.GetType() });

                serializer.Serialize(writer, lic);

                licenseObject.LoadXml(writer.ToString());
            }

            var cert = new X509Certificate2(certPrivateKeyData, certFilePwd,
                X509KeyStorageFlags.MachineKeySet
                | X509KeyStorageFlags.PersistKeySet
                | X509KeyStorageFlags.Exportable);


            var rsaKey = (RSACng)cert.GetRSAPrivateKey();

            SignXml(licenseObject, rsaKey);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(licenseObject.OuterXml));
        }
        catch (Exception ex)
        {
            Logger.LogInformation(ex.GetMessagesAsString());
            return string.Empty;
        }
    }

    public static ValidatedSeptemLicense ParseLicenseFromBase64String(string licenseString, byte[] certPubKeyData, ClientDeviceInfo deviceInfo)
    {
        var license = new ValidatedSeptemLicense();

        if (string.IsNullOrWhiteSpace(licenseString))
        {
            license.ValidationCode = ValidationCode.EmptyLicense;
            license.ValidationMessage = $"{nameof(licenseString)} is Null or Empty";
            license.Status = SeptemLicenseStatus.Cracked;
            return license;
        }

        try
        {
            var cert = new X509Certificate2(certPubKeyData);

            var rsaKey = (RSACng)cert.GetRSAPublicKey();

            var xmlDoc = new XmlDocument { PreserveWhitespace = true };

            xmlDoc.LoadXml(Encoding.UTF8.GetString(Convert.FromBase64String(licenseString)));

            if (VerifyXml(xmlDoc, rsaKey))
            {
                var nodeList = xmlDoc.GetElementsByTagName("Signature");
                xmlDoc.DocumentElement?.RemoveChild(nodeList[0]);

                var licXml = xmlDoc.OuterXml;

                var serializer = new XmlSerializer(typeof(SeptemLicense));
                SeptemLicense lic;
                using (var reader = new StringReader(licXml))
                    lic = (SeptemLicense)serializer.Deserialize(reader);
                license = lic.Validate(deviceInfo.DeviceUid, deviceInfo.ClientUid);
                license.PublicKey = licenseString;
                return license;
            }
            license.ValidationCode = ValidationCode.InvalidLicense;
            license.ValidationMessage = "Can't verify XML";
            license.Status = SeptemLicenseStatus.Invalid;
            license.PublicKey = licenseString;
            return license;
        }
        catch (Exception ex)
        {
            license.ValidationCode = ValidationCode.Cracked;
            license.ValidationMessage = ex.Message;
            license.Status = SeptemLicenseStatus.Cracked;
            return license;
        }
    }

    private static void SignXml(XmlDocument xmlDoc, AsymmetricAlgorithm key)
    {
        if (xmlDoc == null)
            throw new ArgumentException("xmlDoc");
        if (key == null)
            throw new ArgumentException("Key");

        var signedXml = new SignedXml(xmlDoc) { SigningKey = key };
        var reference = new Reference { Uri = "" };
        var env = new XmlDsigEnvelopedSignatureTransform();

        reference.AddTransform(env);

        signedXml.AddReference(reference);
        signedXml.ComputeSignature();

        var xmlDigitalSignature = signedXml.GetXml();
        xmlDoc.DocumentElement?.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
    }

    private static bool VerifyXml(XmlDocument doc, AsymmetricAlgorithm key)
    {
        if (doc == null)
            throw new ArgumentException("Doc");
        if (key == null)
            throw new ArgumentException("Key");

        var signedXml = new SignedXml(doc);

        var nodeList = doc.GetElementsByTagName("Signature");

        if (nodeList.Count <= 0)
            throw new CryptographicException("Verification failed: No Signature was found in the document.");


        if (nodeList.Count >= 2)
            throw new CryptographicException("Verification failed: More that one signature was found for the document.");


        signedXml.LoadXml((XmlElement)nodeList[0]);

        return signedXml.CheckSignature(key);
    }

    public static string GenerateUid() => HardwareInfo.GenerateUid();

    public static bool ValidateUidFormat(string uid) => HardwareInfo.ValidateUidFormat(uid);
}