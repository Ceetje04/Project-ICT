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
            foreach (string s in SerialPort.GetPortNames()) // Zoek de beschikbare poorten.
            {
                cbxComPorts.Items.Add(s);
            }
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

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Lees alle tekst in, tot je een 'nieuwe lijn symbool' binnenkrijgt.
            // New line = '\n' = ASCII-waarde 10 = ALT 10. 
            string receivedText = _serialPort.ReadLine();

            //// Geef de ontvangen data door aan een method die op de UI thread loopt.
            //// Doe dat via een Action delegate... Delegates en Events zullen 
            //// in detail behandeld worden in het vak OOP.
            Dispatcher.Invoke(new Action<string>(UpdateLabelData), receivedText);
            split_string(receivedText);
        }

        // 1ste manier met split string
        public void split_string(string text)
        {
            string arduino = text;
            string[] var = arduino.Split(";"); // Zoekt de drie puntkomma's en zet ze om in drie strings.

            string temperatuur = var[0];
            UpdateLabelTemp(float.Parse(temperatuur, System.Globalization.CultureInfo.InvariantCulture));
            
            string vochtigheid = var[1];
            UpdateLabelVocht(float.Parse(vochtigheid, System.Globalization.CultureInfo.InvariantCulture));

            string luchtdruk = var[2];
            UpdateLabelDruk(float.Parse(luchtdruk, System.Globalization.CultureInfo.InvariantCulture));
        }

        private void UpdateLabelData(string text)
        {
            lblData.Content = text;
        }

        private void UpdateLabelTemp(double temperatuur) //Dit werkt niet omdat temperatuur een string is.
        {

            Task.Run(() => this.Dispatcher.Invoke(() =>
            {
                lblTemp.Content = ($"{temperatuur} °C");
            }));

            if (temperatuur < 15)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblTempAlarm.Content = "Opgepast voor lage temperatuur!";
                    lblTempAlarm.Background = new SolidColorBrush(Colors.Blue);
                });
            }
            if (temperatuur > 25)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblTempAlarm.Content = "Opgepast voor hoge temperatuur!";
                    lblTempAlarm.Background = new SolidColorBrush(Colors.Red);
                });
            }
            if (temperatuur > 15 && temperatuur < 25)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblTempAlarm.Content = "Goede temperatuur!";
                    lblTempAlarm.Background = new SolidColorBrush(Colors.Green);
                });
            }
        }

        private void UpdateLabelVocht(float vochtigheid)
        {
            Convert.ToDouble(vochtigheid);

            Task.Run(() => this.Dispatcher.Invoke(() =>
            {
                lblVocht.Content = ($"{vochtigheid} %");
            }));

            if (vochtigheid < 30)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblDrukAlarm.Content = "opgepast voor lage luchtvochtigeid!";
                    lblDrukAlarm.Background = new SolidColorBrush(Colors.Red);
                });
            }
            if (vochtigheid > 70)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblDrukAlarm.Content = "opgepast voor hoge luchtvochtigheid!";
                    lblDrukAlarm.Background = new SolidColorBrush(Colors.Red);
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblDrukAlarm.Content = "Goede luchtvochtigheid!";
                    lblDrukAlarm.Background = new SolidColorBrush(Colors.Green);
                });
            }
        }

        private void UpdateLabelDruk(double luchtdruk)
        {
            Convert.ToDouble(luchtdruk);

            Task.Run(() => this.Dispatcher.Invoke(() =>
            {
                lblDruk.Content = ($"{luchtdruk:f2} hPa");
            }));

            if (luchtdruk < 950)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblDrukAlarm.Content = "opgepast voor lage luchtdruk!";
                    lblDrukAlarm.Background = new SolidColorBrush(Colors.Red);
                });
            }
            if (luchtdruk > 1060)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblDrukAlarm.Content = "opgepast voor hoge luchtdruk!";
                    lblDrukAlarm.Background = new SolidColorBrush(Colors.Red);
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblDrukAlarm.Content = "Goede luchtdruk!";
                    lblDrukAlarm.Background = new SolidColorBrush(Colors.Green);
                });
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Dispose();
            }
        }

        // Sluit de window wanneer op de afsluitknop gedrukt wordt.
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
