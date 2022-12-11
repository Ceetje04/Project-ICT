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

        //// 1ste manier met split string
        //public void split_string(string text)
        //{
        //    string arduino = text;
        //    string[] var = arduino.Split(new char[] {';'}, 3); // Zoekt de drie puntkomma's en zet ze om in drie strings.

        //    string temperatuur = var[0];
        //    UpdateLabelTemp(temperatuur);

        //    string vochtigheid = var[1];
        //    UpdateLabelVocht(vochtigheid);

        //    string luchtdruk = var[2];
        //    UpdateLabelDruk(luchtdruk);

        //}

        // 2e manier met substring.
        public void split_string(string text)
        {
            string arduino = text;
            int index = arduino.IndexOf(';') + 1; // Zoekt naar de eerste puntkomma.

            string temperatuur = arduino.Substring(index, arduino.Length - index);
            UpdateLabelTemp(temperatuur); // Temperatuur moet geconverteerd worden naar double.

            string vochtigheid = arduino.Substring(index, arduino.Length - index);
            UpdateLabelVocht(vochtigheid);

            string luchtdruk = arduino.Substring(index, arduino.Length - index);
            UpdateLabelDruk(luchtdruk);
        }

        private void UpdateLabelData(string text)
        {
            lblData.Content = text;
        }


        private void UpdateLabelTemp(double temperatuur) //Dit werkt niet omdat temperatuur een string is.
        {
            temperatuur = Convert.ToDouble(temperatuur);
            lblTemp.Content = ($"Temperatuur: {temperatuur :D2} °C");

            if (temperatuur < 15)
            {
                lblDrukAlarm.Content = "opgepast voor lage temperatuur!";
                lblDrukAlarm.Background = new SolidColorBrush(Colors.Red);
            }
            if (temperatuur > 25)
            {
                lblDrukAlarm.Content = "opgepast voor hoge temperatuur!";
                lblDrukAlarm.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                lblDrukAlarm.Content = "Goede temperatuur!";
                lblDrukAlarm.Background = new SolidColorBrush(Colors.Green);
            }
        }


        private void UpdateLabelVocht(double vochtigheid)
        {
            vochtigheid = Convert.ToDouble(vochtigheid);
            lblVocht.Content = ($"Vochtigheid: {vochtigheid:D2} %");

            if (vochtigheid < 30)
            {
                lblDrukAlarm.Content = "opgepast voor lage luchtvochtigeid!";
                lblDrukAlarm.Background = new SolidColorBrush(Colors.Red);
            }
            if (vochtigheid > 70)
            {
                lblDrukAlarm.Content = "opgepast voor hoge luchtvochtigheid!";
                lblDrukAlarm.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                lblDrukAlarm.Content = "Goede luchtvochtigheid!";
                lblDrukAlarm.Background = new SolidColorBrush(Colors.Green);
            }
        }

        private void UpdateLabelDruk(double luchtdruk)
        {
            luchtdruk = Convert.ToDouble(luchtdruk);
            lblDruk.Content = ($"Luchtdruk: {luchtdruk:D2} hPa");

            if(luchtdruk < 950)
            {
                lblDrukAlarm.Content = "opgepast voor lage luchtdruk!";
                lblDrukAlarm.Background = new SolidColorBrush(Colors.Red);
            }
            if(luchtdruk > 1060)
            {
                lblDrukAlarm.Content = "opgepast voor hoge luchtdruk!";
                lblDrukAlarm.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                lblDrukAlarm.Content = "Goede luchtdruk!";
                lblDrukAlarm.Background = new SolidColorBrush(Colors.Green);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        // Sluit de window wanneer op de afsluitknop gedrukt wordt.
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
