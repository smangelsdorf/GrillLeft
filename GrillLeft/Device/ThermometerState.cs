using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrillLeft.Device
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

        private readonly uint RawTemperature;

        internal ThermometerState(ThermometerChannel channel, byte[] bytes)
            : this(channel, bytes, DateTime.Now)
        {
        }

        internal ThermometerState(ThermometerChannel channel, byte[] bytes, DateTime time)
        {
            this.Channel = channel;
            this.Data = bytes;
            this.RawTemperature = (((uint)bytes[13]) << 8) + ((uint)bytes[12]);
            this.Time = time;
        }

        internal bool IsNullTemperature
        {
            get
            {
                return RawTemperature == 0x8FFF;
            }
        }

        internal double TemperatureValue
        {
            get
            {
                return ((double)RawTemperature) / 10d;
            }
        }

        internal String TemperatureString
        {
            get
            {
                if (IsNullTemperature)
                {
                    return "---";
                }
                else
                {
                    return String.Format("{0}", TemperatureValue);
                }
            }
        }
    }
}