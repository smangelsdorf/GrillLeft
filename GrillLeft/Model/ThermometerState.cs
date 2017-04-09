using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrillLeft.Model
{
    internal class ThermometerState
    {
        internal enum ThermometerChannel
        {
            One = 1,
            Two = 2
        }

        internal readonly ThermometerChannel Channel;
        internal readonly DateTime Time;
        internal readonly byte[] Data;

        internal ThermometerState(ThermometerChannel channel, byte[] bytes)
            : this(channel, bytes, DateTime.Now)
        {
        }

        internal ThermometerState(ThermometerChannel channel, byte[] bytes, DateTime time)
        {
            this.Channel = channel;
            this.Data = bytes;
            this.Time = time;
        }

        private uint RawTemperatureFromBytes(byte high, byte low)
        {
            return (((uint)high) << 8) + ((uint)low);
        }

        private String TemperatureStringFromRawTemperature(uint raw)
        {
            if (raw == 0x8FFF)
            {
                return "---";
            }
            else
            {
                return String.Format("{0:0.0}", raw / 10d);
            }
        }

        public uint RawTemperature
        {
            get
            {
                return RawTemperatureFromBytes(Data[13], Data[12]);
            }
        }

        public uint RawTargetTemperature
        {
            get
            {
                return RawTemperatureFromBytes(Data[11], Data[10]);
            }
        }

        public double TemperatureValue
        {
            get
            {
                return ((double)RawTemperature) / 10d;
            }
        }

        public string TemperatureString
        {
            get
            {
                return TemperatureStringFromRawTemperature(RawTemperature);
            }
        }

        public string TargetTemperatureString
        {
            get
            {
                return TemperatureStringFromRawTemperature(RawTargetTemperature);
            }
        }

        public string RawDataString
        {
            get
            {
                var components = Data.Select(b => String.Format("{0:X2}", b));
                return String.Join(":", components.Take(8)) + "\r\n" + String.Join(":", components.Skip(8));
            }
        }
    }
}