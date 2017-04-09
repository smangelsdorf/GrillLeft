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

        public string TemperatureString
        {
            get
            {
                if (ThermometerState == null)
                {
                    return "---";
                }

                return ThermometerState.TemperatureString;
            }
        }

        public string TargetTemperatureString
        {
            get
            {
                if (ThermometerState == null)
                {
                    return "Target: ---";
                }

                return $"Target: {ThermometerState.TargetTemperatureString}";
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

                return ThermometerState.RawDataString;
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