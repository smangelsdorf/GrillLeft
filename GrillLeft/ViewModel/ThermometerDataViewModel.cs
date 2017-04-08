using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrillLeft.Model;
using System.ComponentModel;

namespace GrillLeft.ViewModel
{
    internal class ThermometerDataViewModel : INotifyPropertyChanged
    {
        private List<PropertyChangedEventHandler> handlers;
        private ThermometerState state;

        internal ThermometerDataViewModel()
        {
            this.handlers = new List<PropertyChangedEventHandler>();
        }

        public ThermometerState ThermometerState
        {
            get
            {
                return state;
            }

            internal set
            {
                Console.WriteLine("got state");
                state = value;
                foreach (var handler in handlers)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(""));
                }
            }
        }

        public uint RawTemperature
        {
            get
            {
                var data = ThermometerState.Data;
                return (((uint)data[13]) << 8) + ((uint)data[12]);
            }
        }

        public double TemperatureValue
        {
            get
            {
                return ((double)RawTemperature) / 10d;
            }
        }

        public bool IsNullTemperature
        {
            get
            {
                return ThermometerState == null || RawTemperature == 0x8FFF;
            }
        }

        public string TemperatureString
        {
            get
            {
                if (IsNullTemperature)
                {
                    return "---";
                }
                else
                {
                    return String.Format("{0:0.0}", TemperatureValue);
                }
            }
        }

        public string RawDataString
        {
            get
            {
                if (ThermometerState == null)
                {
                    return "(no data yet)";
                }

                var components = ThermometerState.Data.Select(b => String.Format("{0:X2}", b));
                return String.Join(":", components.Take(8)) + "\r\n" + String.Join(":", components.Skip(8));
            }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                handlers.Add(value);
            }

            remove
            {
                handlers.Remove(value);
            }
        }
    }
}