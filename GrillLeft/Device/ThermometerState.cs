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

        private readonly ThermometerChannel Channel;

        internal ThermometerState(ThermometerChannel channel, byte[] bytes)
        {
            this.Channel = channel;
        }
    }
}