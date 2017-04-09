using GrillLeft.Model;
using GrillLeft.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GrillLeft.Model.ThermometerState;

namespace GrillLeftTests.ViewModel
{
    [TestClass()]
    public class ThermometerDataViewModelTests
    {
        [TestMethod()]
        public void TemperatureStringTest()
        {
            var data = new byte[] {
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };

            var vm = new ThermometerDataViewModel();
            vm.ThermometerState = new ThermometerState(ThermometerChannel.One, (byte[])data.Clone());

            Assert.AreEqual("0.0", vm.TemperatureString);

            data[13] = 0x0A;
            data[12] = 0x15;
            vm.ThermometerState = new ThermometerState(ThermometerChannel.One, (byte[])data.Clone());

            Assert.AreEqual("258.1", vm.TemperatureString);

            data[13] = 0x0C;
            data[12] = 0x12;
            vm.ThermometerState = new ThermometerState(ThermometerChannel.One, (byte[])data.Clone());

            Assert.AreEqual("309.0", vm.TemperatureString);

            data[13] = 0x8F;
            data[12] = 0xFF;
            vm.ThermometerState = new ThermometerState(ThermometerChannel.One, (byte[])data.Clone());
            Assert.AreEqual("---", vm.TemperatureString);
        }

        [TestMethod()]
        public void TargetTemperatureStringTest()
        {
            var data = new byte[] {
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };

            var vm = new ThermometerDataViewModel();
            vm.ThermometerState = new ThermometerState(ThermometerChannel.One, (byte[])data.Clone());

            Assert.AreEqual("0.0", vm.TargetTemperatureString);

            data[11] = 0x0A;
            data[10] = 0x15;
            vm.ThermometerState = new ThermometerState(ThermometerChannel.One, (byte[])data.Clone());

            Assert.AreEqual("258.1", vm.TargetTemperatureString);

            data[11] = 0x0C;
            data[10] = 0x12;
            vm.ThermometerState = new ThermometerState(ThermometerChannel.One, (byte[])data.Clone());

            Assert.AreEqual("309.0", vm.TargetTemperatureString);

            data[11] = 0x8F;
            data[10] = 0xFF;
            vm.ThermometerState = new ThermometerState(ThermometerChannel.One, (byte[])data.Clone());
            Assert.AreEqual("---", vm.TargetTemperatureString);
        }

        [TestMethod()]
        public void RawDataStringTest()
        {
            var data = new byte[] {
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };

            var vm = new ThermometerDataViewModel();
            vm.ThermometerState = new ThermometerState(ThermometerChannel.One, (byte[])data.Clone());

            Assert.AreEqual("00:00:00:00:00:00:00:00\r\n00:00:00:00:00:00:00:00", vm.RawDataString);

            data[13] = 0x0A;
            data[12] = 0x15;
            vm.ThermometerState = new ThermometerState(ThermometerChannel.One, (byte[])data.Clone());

            Assert.AreEqual("00:00:00:00:00:00:00:00\r\n00:00:00:00:15:0A:00:00", vm.RawDataString);

            for (byte i = 0; i < data.Length; ++i)
            {
                data[i] = i;
            }

            vm.ThermometerState = new ThermometerState(ThermometerChannel.One, (byte[])data.Clone());

            Assert.AreEqual("00:01:02:03:04:05:06:07\r\n08:09:0A:0B:0C:0D:0E:0F", vm.RawDataString);

            for (byte i = 0; i < data.Length; ++i)
            {
                data[i] = (byte)(i << 4);
            }

            vm.ThermometerState = new ThermometerState(ThermometerChannel.One, (byte[])data.Clone());

            Assert.AreEqual("00:10:20:30:40:50:60:70\r\n80:90:A0:B0:C0:D0:E0:F0", vm.RawDataString);

            for (byte i = 0; i < data.Length; ++i)
            {
                data[i] = (byte)((i << 4) + i);
            }

            vm.ThermometerState = new ThermometerState(ThermometerChannel.One, (byte[])data.Clone());

            Assert.AreEqual("00:11:22:33:44:55:66:77\r\n88:99:AA:BB:CC:DD:EE:FF", vm.RawDataString);
        }

        [TestMethod()]
        public void NotifyPropertyChangedTest()
        {
            var data = new byte[] {
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };

            var notifications = 0;
            PropertyChangedEventHandler receiver = (sender, e) =>
            {
                notifications++;
            };

            var vm = new ThermometerDataViewModel();
            var state = new ThermometerState(ThermometerChannel.One, (byte[])data.Clone());

            INotifyPropertyChanged notifier = vm;
            notifier.PropertyChanged += receiver;

            Assert.AreEqual(0, notifications);
            vm.ThermometerState = state;
            Assert.AreEqual(1, notifications);
            vm.ThermometerState = state;
            Assert.AreEqual(2, notifications);
            vm.ThermometerState = state;
            Assert.AreEqual(3, notifications);

            notifier.PropertyChanged -= receiver;

            vm.ThermometerState = state;
            Assert.AreEqual(3, notifications);
            vm.ThermometerState = state;
            Assert.AreEqual(3, notifications);

            notifier.PropertyChanged += receiver;
            vm.ThermometerState = state;
            Assert.AreEqual(4, notifications);
            vm.ThermometerState = state;
            Assert.AreEqual(5, notifications);
        }
    }
}