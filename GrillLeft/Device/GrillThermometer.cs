using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;

namespace GrillLeft.Device
{
    internal class GrillThermometer
    {
        private readonly string Name;
        private readonly ulong Address;

        public static async Task<IList<GrillThermometer>> GetAllDevices()
        {
            var guid = new Guid("2899fe00-c277-48a8-91cb-b29ab0f01ac4");
            var services = await DeviceInformation.FindAllAsync(GattDeviceService.GetDeviceSelectorFromUuid(guid));
            var tasks = services.AsParallel()
                                .Select(async info => new GrillThermometer(info, await GattDeviceService.FromIdAsync(info.Id)));
            return await Task.WhenAll(tasks);
        }

        public GrillThermometer(DeviceInformation info, GattDeviceService service)
        {
            this.Name = info.Name;
            this.Address = service.Device.BluetoothAddress;
        }

        public override string ToString()
        {
            return $"{this.AddressString()} ({this.Name})";
        }

        private string AddressString()
        {
            var parts = BitConverter.GetBytes(Address).Reverse().Skip(2).Select(b => b.ToString("X2"));
            return String.Join(":", parts);
        }
    }
}