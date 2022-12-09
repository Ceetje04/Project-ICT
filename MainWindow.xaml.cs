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
using System.IO.Ports;

namespace Project_ICT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort _serialPort;

        public MainWindow()
        {
            InitializeComponent();

            _serialPort = new SerialPort();

            // Komt er data binnen op de seriële poort, vang ze op...
            _serialPort.DataReceived += _serialPort_DataReceived;

            cbxComPorts.Items.Add("None");
            foreach (string s in SerialPort.GetPortNames())
            {
                cbxComPorts.Items.Add(s);
            }
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Lees alle tekst in, tot je een 'nieuwe lijn symbool' binnenkrijgt.
            // New line = '\n' = ASCII-waarde 10 = ALT 10. 
            string receivedText = _serialPort.ReadLine();

            // Geef de ontvangen data door aan een method die op de UI thread loopt.
            // Doe dat via een Action delegate... Delegates en Events zullen 
            // in detail behandeld worden in het vak OOP.
            Dispatcher.Invoke(new Action<string>(UpdateLabel), receivedText);
        }

        private void cbxComPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();

                if (cbxComPorts.SelectedItem.ToString() != "None")
                {
                    _serialPort.PortName = cbxComPorts.SelectedItem.ToString();
                    _serialPort.Open();
                }
            }
        }

        private void UpdateLabel(string text)
        {
            lblData.Content = text;
        }
        private void split_string()
        { }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
