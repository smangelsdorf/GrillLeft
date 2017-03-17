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

namespace GrillLeft
{
    /// <summary>
    /// Interaction logic for TemperatureGraph.xaml
    /// </summary>
    public partial class TemperatureGraph : UserControl
    {
        internal readonly TemperatureSeriesViewModel ViewModel;

        public TemperatureGraph()
        {
            ViewModel = new TemperatureSeriesViewModel(
                (a) => Dispatcher.Invoke(a),
                () => chart.Update()
            );

            DataContext = ViewModel;

            InitializeComponent();
        }
    }
}