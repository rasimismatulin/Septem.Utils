using System;

namespace Septem.Utils.Helpers.License;

public class ValidatedSeptemLicense
{
    public string PublicKey { get; set; }

    public SeptemLicense LicenseEntity { get; set; }

    public SeptemLicenseStatus Status { get; set; }

    public string ValidationCode { get; set; }

    public DateTime? ValidationDateUtc { get; set; }

    public string ValidationMessage { get; set; }

    public override string ToString()
    {
        return ValidationMessage;
    }

    public override bool Equals(object other)
    {
        return other switch
        {
            null => false,
            ValidatedSeptemLicense license => Equals(license),
            _ => ReferenceEquals(this, other)
        };
    }

    protected bool Equals(ValidatedSeptemLicense other)
    {
        return PublicKey == other.PublicKey;
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return (PublicKey != null ? PublicKey.GetHashCode() : 0);
    }
}