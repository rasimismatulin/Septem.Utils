using System;

namespace Septem.Utils.Helpers.License;

public class ClientDeviceInfo
{
    public ClientDeviceInfo(Guid clientUid, string deviceUid)
    {
        ClientUid = clientUid;
        DeviceUid = deviceUid;
    }

    public Guid ClientUid { get; set; }

    public string DeviceUid { get; set; }
}