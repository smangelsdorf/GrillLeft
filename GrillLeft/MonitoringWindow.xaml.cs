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
        private GrillThermometer grillThermometer;

        public MonitoringWindow()
        {
            InitializeComponent();
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
            }
        }

        private async void ProbeTemperatures()
        {
            for (;;)
            {
                //
            }
        }
    }
}