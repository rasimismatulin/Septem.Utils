using System;
using System.Xml.Serialization;

namespace Septem.Utils.Helpers.License;

public class SeptemLicense
{
    [XmlIgnore]
    public string AppName { get; protected set; }

    [XmlElement("DeviceUid")]
    public string DeviceUid { get; set; }

    [XmlElement("ClientUid")]
    public Guid ClientUid { get; set; }

    [XmlElement("Type")]
    public SeptemLicenseType Type { get; set; }

    [XmlElement("CreateDateUtc")]
    public System.DateTime CreateDateUtc { get; set; }

    [XmlElement("StartDateUtc")]
    public System.DateTime StartDateUtc { get; set; }

    [XmlElement("EndDateUtc")]
    public System.DateTime? EndDateUtc { get; set; }

    public virtual ValidatedSeptemLicense Validate(string deviceUid, Guid clientUid)
    {
        var entity = new ValidatedSeptemLicense { LicenseEntity = this };
        var currentDate = System.DateTime.UtcNow;

        if (EndDateUtc.HasValue && EndDateUtc < currentDate)
        {
            entity.ValidationCode = ValidationCode.Expired;
            entity.ValidationDateUtc = EndDateUtc.Value;
            entity.ValidationMessage = $"The license deactivated at: {EndDateUtc.Value}";
            entity.Status = SeptemLicenseStatus.Invalid;
            return entity;
        }

        if (currentDate <= StartDateUtc)
        {
            entity.ValidationCode = ValidationCode.NotStarted;
            entity.ValidationDateUtc = StartDateUtc;
            entity.ValidationMessage = $"The license will be activated at: {StartDateUtc}";
            entity.Status = SeptemLicenseStatus.Invalid;
            return entity;
        }

        if (clientUid != ClientUid)
        {
            entity.ValidationCode = ValidationCode.InvalidClient;
            entity.ValidationMessage = "The license is NOT for this client!";
            entity.Status = SeptemLicenseStatus.Invalid;
            return entity;
        }

        switch (Type)
        {
            case SeptemLicenseType.Unknown:
            case SeptemLicenseType.Single:
                if (DeviceUid == deviceUid)
                {
                    entity.ValidationCode = ValidationCode.Valid;

                    if (EndDateUtc != null)
                        entity.ValidationDateUtc = EndDateUtc.Value;

                    entity.Status = SeptemLicenseStatus.Valid;
                    return entity;
                }
                else
                {
                    entity.ValidationCode = ValidationCode.InvalidDevice;
                    entity.ValidationMessage = "The license is NOT for this copy!";
                    entity.Status = SeptemLicenseStatus.Invalid;
                    return entity;
                }
            case SeptemLicenseType.Volume:
                entity.Status = SeptemLicenseStatus.Valid;
                return entity;
            default:
                entity.ValidationCode = ValidationCode.InvalidLicense;
                entity.ValidationMessage = "The license type is Unknown";
                entity.Status = SeptemLicenseStatus.Invalid;
                return entity;
        }
    }
}