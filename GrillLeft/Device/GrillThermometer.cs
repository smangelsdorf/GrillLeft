using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace GrillLeft.Device
{
    internal class GrillThermometer
    {
        private static Guid SERVICE_GUID = new Guid("2899fe00-c277-48a8-91cb-b29ab0f01ac4");
        private static Guid CHANNEL_ONE_GUID = new Guid("28998e10-c277-48a8-91cb-b29ab0f01ac4");
        private static Guid CHANNEL_TWO_GUID = new Guid("28998e11-c277-48a8-91cb-b29ab0f01ac4");

        private readonly string Name;
        private readonly ulong Address;
        private readonly GattCharacteristic[] Characteristics;

        internal ThermometerStateObservable Observable { get; private set; }

        public static async Task<IList<GrillThermometer>> GetAllDevices()
        {
            var selector = GattDeviceService.GetDeviceSelectorFromUuid(SERVICE_GUID);
            var services = await DeviceInformation.FindAllAsync(selector);
            var tasks = services.AsParallel()
                                .Select(async info => new GrillThermometer(info, await GattDeviceService.FromIdAsync(info.Id)));
            return await Task.WhenAll(tasks);
        }

        public GrillThermometer(DeviceInformation info, GattDeviceService service)
        {
            this.Name = info.Name;
            this.Address = service.Device.BluetoothAddress;
            this.Characteristics = service.GetAllCharacteristics()
                                          .Where(ch => ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
                                          .ToArray();

            this.Observable = new ThermometerStateObservable();
        }

        private void ReceiveCharacteristicValue(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var bytes = new byte[args.CharacteristicValue.Length];
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(bytes);

            if (sender.Uuid == CHANNEL_ONE_GUID)
            {
                Observable.Emit(new ThermometerState(ThermometerState.ThermometerChannel.One, bytes));
            }
            else if (sender.Uuid == CHANNEL_TWO_GUID)
            {
                Observable.Emit(new ThermometerState(ThermometerState.ThermometerChannel.Two, bytes));
            }
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

        public async void Listen()
        {
            foreach (var ch in Characteristics)
            {
                Console.WriteLine(ch.Uuid);
                ch.ValueChanged += ReceiveCharacteristicValue;
            }

            var tasks = Characteristics.Select(async ch => await ch.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate));
            foreach (var status in await Task.WhenAll(tasks))
            {
                Console.WriteLine("{0}", status.HasFlag(GattCommunicationStatus.Success));
            }
            Console.WriteLine("Done");
        }
    }
}