using System;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace Septem.Utils.Helpers.DeviceIdentification;

public class HardwareInfo
{
    private static string ManagementObjectSearch(string query, string key)
    {
        try
        {
#pragma warning disable CA1416
            var mObj = new ManagementObjectSearcher(query);
            var objList = mObj.Get();
            var keyValue = string.Empty;
            foreach (var o in objList)
            {
                var mo = (ManagementObject)o;
                keyValue = mo[key].ToString();
                break;
            }
            return keyValue;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string GetDiskVolumeSerialNumber() =>
        ManagementObjectSearch("SELECT VolumeSerialNumber FROM win32_logicaldisk", "VolumeSerialNumber");

    private static string GetProcessorId() =>
        ManagementObjectSearch("SELECT ProcessorId FROM Win32_processor", "ProcessorId");

    private static string GetMotherboardId() =>
        ManagementObjectSearch("SELECT SerialNumber FROM Win32_BaseBoard", "SerialNumber");

    private static string GetMacAddress()
    {
        var networkServices = NetworkInterface.GetAllNetworkInterfaces()
            .Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            .ToArray();

        var settingName = "Machine_NetworkAddress_List";
        var networks = IsolatedStorageSettings.GetSetting(settingName);

        if (string.IsNullOrWhiteSpace(networks))
        {
            networks = string.Join(",", networkServices.Select(x => x.Id));
            IsolatedStorageSettings.SaveSettings(settingName, networks);
        }

        var existsIds = networks;
        var macAddress = string.Join(",", networkServices
            .Where(x => existsIds.Contains(x.Id))
            .OrderBy(x => x.Id)
            .Select(nic => $"{nic.Id}:{nic.GetPhysicalAddress()}".ToString())
            .ToArray());

        //If no mac address existed rewrite mac addresses
        if (string.IsNullOrWhiteSpace(macAddress))
        {
            networks = string.Join(",", networkServices.Select(x => x.Id));
            IsolatedStorageSettings.SaveSettings(settingName, networks);

            existsIds = networks;
            macAddress = string.Join(",", networkServices
                .Where(x => existsIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(nic => $"{nic.Id}:{nic.GetPhysicalAddress()}".ToString())
                .ToArray());
        }

        return macAddress;
    }


    private static string _deviceUid;

    public static string GenerateUid()
    {
        if (string.IsNullOrWhiteSpace(_deviceUid))
        {
            var id = string.Join(" - ", $"[ProcessorId: {GetProcessorId()}]",
                $"[MotherboardId: {GetMotherboardId()}]",
                $"[DiskSerial: {GetDiskVolumeSerialNumber()}]" +
                $"[Mac Address: {GetMacAddress()}]");

            var byteIds = Encoding.UTF8.GetBytes(id);

            var md5 = MD5.Create();
            var checksum = md5.ComputeHash(byteIds);

            var part1Id = Base36.Encode(BitConverter.ToUInt32(checksum, 0));
            var part2Id = Base36.Encode(BitConverter.ToUInt32(checksum, 4));
            var part3Id = Base36.Encode(BitConverter.ToUInt32(checksum, 8));
            var part4Id = Base36.Encode(BitConverter.ToUInt32(checksum, 12));

            _deviceUid = $"{part1Id}-{part2Id}-{part3Id}-{part4Id}";
        }

        return _deviceUid;
    }

    public static byte[] GetUidInBytes(string uid)
    {
        var ids = uid.Split('-');

        if (ids.Length != 4)
            throw new ArgumentException("Wrong UID");

        var value = new byte[16];

        Buffer.BlockCopy(BitConverter.GetBytes(Base36.Decode(ids[0])), 0, value, 0, 8);
        Buffer.BlockCopy(BitConverter.GetBytes(Base36.Decode(ids[1])), 0, value, 8, 8);
        Buffer.BlockCopy(BitConverter.GetBytes(Base36.Decode(ids[2])), 0, value, 16, 8);
        Buffer.BlockCopy(BitConverter.GetBytes(Base36.Decode(ids[3])), 0, value, 24, 8);

        return value;
    }

    public static bool ValidateUidFormat(string uid) =>
        !string.IsNullOrWhiteSpace(uid) && uid.Split('-').Length == 4;
}