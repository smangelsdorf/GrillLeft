using GrillLeft.Device;
using GrillLeft.Model;
using GrillLeft.ViewModel;
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
using static GrillLeft.Model.ThermometerState;

namespace GrillLeft
{
    /// <summary>
    /// Interaction logic for ThermometerData.xaml
    /// </summary>
    public partial class ThermometerData : UserControl
    {
        internal ThermometerDataViewModel ViewModel;

        internal String ChannelHeading
        {
            set
            {
                this.channelLabel.Content = value;
            }
        }

        public ThermometerData()
        {
            ViewModel = new ThermometerDataViewModel();

            DataContext = ViewModel;

            InitializeComponent();
        }
    }
}