using GrillLeft.Data;
using GrillLeft.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GrillLeft
{
    /// <summary>
    /// Interaction logic for MonitoringWindow.xaml
    /// </summary>
    public partial class MonitoringWindow : Window
    {
        private class Observer : IObserver<ThermometerState>
        {
            private readonly MonitoringWindow window;
            private readonly ConnectionManager connectionManager;

            public Observer(MonitoringWindow window, ConnectionManager connectionManager)
            {
                this.window = window;
                this.connectionManager = connectionManager;
            }

            public void OnNext(ThermometerState value)
            {
                connectionManager.RecordReading(value);

                switch (value.Channel)
                {
                    case ThermometerState.ThermometerChannel.One:
                        window.dataLeft.ThermometerState = value;
                        break;

                    case ThermometerState.ThermometerChannel.Two:
                        window.dataRight.ThermometerState = value;
                        break;
                }
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            public void OnCompleted()
            {
                throw new NotImplementedException();
            }
        }

        private GrillThermometer grillThermometer;
        private IDisposable subscription;
        private ConnectionManager connectionManager;

        public MonitoringWindow()
        {
            InitializeComponent();
            this.connectionManager = new ConnectionManager();
        }

        internal GrillThermometer GrillThermometer
        {
            get
            {
                return grillThermometer;
            }

            set
            {
                grillThermometer = value;
                grillThermometer.Listen();
                subscription = grillThermometer.Observable.Subscribe(new Observer(this, connectionManager));
            }
        }
    }
}