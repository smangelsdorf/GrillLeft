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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GrillLeft
{
    /// <summary>
    /// Interaction logic for ThermometerData.xaml
    /// </summary>
    public partial class ThermometerData : UserControl
    {
        private ThermometerState CurrentThermometerState;

        internal ThermometerState ThermometerState
        {
            get
            {
                return CurrentThermometerState;
            }
            set
            {
                CurrentThermometerState = value;
                Dispatcher.Invoke(() =>
                {
                    temperatureLabel.Content = value.TemperatureString;
                });
            }
        }

        public ThermometerData()
        {
            InitializeComponent();
        }
    }
}