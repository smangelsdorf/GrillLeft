using GrillLeft.Device;
using System;
using System.Collections;
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
using Windows.Foundation;

namespace GrillLeft
{
    public partial class BluetoothDevicesWindow : Window
    {
        public BluetoothDevicesWindow()
        {
            InitializeComponent();
            LoadDevices();
        }

        private async void LoadDevices()
        {
            deviceListBox.ItemsSource = new ArrayList();
            loadingLabel.Visibility = Visibility.Visible;

            var devices = await GrillThermometer.GetAllDevices();
            deviceListBox.ItemsSource = devices;

            loadingLabel.Visibility = Visibility.Hidden;
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadDevices();
        }

        private void deviceListBox_SelectionChanged(object senderObj, SelectionChangedEventArgs e)
        {
            var sender = senderObj as ListBox;
            continueButton.IsEnabled = (sender?.SelectedItem != null);
        }

        private void continueButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new MonitoringWindow();
            window.GrillThermometer = deviceListBox.SelectedItem as GrillThermometer;
            window.Show();

            Close();
        }
    }
}