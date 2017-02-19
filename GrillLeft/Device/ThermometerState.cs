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
            One,
            Two
        }

        internal readonly ThermometerChannel Channel;

        private readonly byte[] Data;
        private readonly uint Temperature;

        internal ThermometerState(ThermometerChannel channel, byte[] bytes)
        {
            this.Channel = channel;
            this.Data = bytes;
            this.Temperature = (((uint)bytes[13]) << 8) + ((uint)bytes[12]);
        }

        internal String TemperatureString
        {
            get
            {
                if (Temperature == 0x8FFF)
                {
                    return "---";
                }
                else
                {
                    return String.Format("{0}", ((decimal)Temperature) / 10);
                }
            }
        }
    }
}