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
using System.Threading;

namespace Project_ICT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort _serialPort;

        string _temperatuur;
        string _vochtigheid;
        string _luchtdruk;

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

        public void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //// Lees alle tekst in, tot je een 'nieuwe lijn symbool' binnenkrijgt.
            //// New line = '\n' = ASCII-waarde 10 = ALT 10. 
            string receivedText = _serialPort.ReadLine();

            ////// Geef de ontvangen data door aan een method die op de UI thread loopt.
            ////// Doe dat via een Action delegate... Delegates en Events zullen 
            ////// in detail behandeld worden in het vak OOP.
            Dispatcher.Invoke(new Action<string>(UpdateLabelData), receivedText);

            Split_Data split_data = new Split_Data(); // Nieuwe klasse.

            split_data.split_string(receivedText, ref _temperatuur, ref _vochtigheid, ref _luchtdruk); // Het splitsen van de string gebeurt in de klasse. Temperatuur kunnnen in de klasse en in de main aangepast worden door te werken met passing reference.

            UpdateLabelTemp(float.Parse(_temperatuur, System.Globalization.CultureInfo.InvariantCulture)); // Ga naar de method voor het aanpassen van de temperatuur. Temperatuur wordt omgezet met een float.
            UpdateLabelVocht(float.Parse(_vochtigheid, System.Globalization.CultureInfo.InvariantCulture)); // Ga naar de method voor het aanpassen van de vochtigheid. Vochtigheid wordt omgezet naar een float.
            UpdateLabelDruk(float.Parse(_luchtdruk, System.Globalization.CultureInfo.InvariantCulture)); // Ga naar de method voor het aanpassen van de luchtdruk. Luchtdruk wordt omgezet naar een float.
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if ((_serialPort.IsOpen) && (_serialPort != null))
            {
                COM.Visibility = Visibility.Hidden;
                Weerstation.Visibility = Visibility.Visible;
            }

            else
            {
                MessageBox.Show("Selecteer eerst een COM-poort!");
            }
        }

        //// 1ste manier met split string
        //public void split_string(string text)
        //{
        //    //string arduino = text;
        //    //string[] var = arduino.Split(";"); // Zoekt de drie puntkomma's en zet ze om in drie strings.

        //    //string temperatuur = var[0];
        //    //UpdateLabelTemp(float.Parse(temperatuur, System.Globalization.CultureInfo.InvariantCulture));

        //    //string vochtigheid = var[1];
        //    //UpdateLabelVocht(float.Parse(vochtigheid, System.Globalization.CultureInfo.InvariantCulture));

        //    //string luchtdruk = var[2];
        //    //UpdateLabelDruk(float.Parse(luchtdruk, System.Globalization.CultureInfo.InvariantCulture));
        //}

        public void UpdateLabelData(string text)
        {
            lblData.Content = text;
        }

        public void UpdateLabelTemp(double temperatuur) //Dit werkt niet omdat temperatuur een string is.
        {

            Task.Run(() => this.Dispatcher.Invoke(() => // Zorgt dat dit deel programma prioritair wordt uitgevoerd op de treat.
            {
                lblTemp.Content = ($"{temperatuur:f2} °C");
            }));

            if (temperatuur < 15)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblTempAlarm.Content = "Opgepast voor lage temperatuur!";
                    lblTempAlarm.Background = new SolidColorBrush(Colors.Blue);
                    imgTemp.Source = new BitmapImage(new Uri("/koude_temperatuur.jpg", UriKind.Relative));
                });
            }
            if (temperatuur > 25)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblTempAlarm.Content = "Opgepast voor hoge temperatuur!";
                    lblTempAlarm.Background = new SolidColorBrush(Colors.Red);
                    imgTemp.Source = new BitmapImage(new Uri("/warme_temperatuur.jpg", UriKind.Relative));
                });
            }
            if (temperatuur > 15 && temperatuur < 25)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblTempAlarm.Content = "Goede temperatuur!";
                    lblTempAlarm.Background = new SolidColorBrush(Colors.Green);
                    imgTemp.Source = new BitmapImage(new Uri("/goede_temperatuur.jpg", UriKind.Relative));
                });
            }
        }

        public void UpdateLabelVocht(float vochtigheid)
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
                    imgVocht.Source = new BitmapImage(new Uri("/lage_luchtvochtigheid.jpg", UriKind.Relative));
                });
            }
            if (vochtigheid > 70)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblDrukAlarm.Content = "opgepast voor hoge luchtvochtigheid!";
                    lblDrukAlarm.Background = new SolidColorBrush(Colors.Red);
                    imgVocht.Source = new BitmapImage(new Uri("/hoge_luchtvochtigheid.jpeg", UriKind.Relative));
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblDrukAlarm.Content = "Goede luchtvochtigheid!";
                    lblDrukAlarm.Background = new SolidColorBrush(Colors.Green);
                    imgVocht.Source = new BitmapImage(new Uri("/goede_luchtvochtigheid.jpg", UriKind.Relative));
                });
            }
        }

        public void UpdateLabelDruk(double luchtdruk)
        {
            Convert.ToDouble(luchtdruk);

            Task.Run(() => this.Dispatcher.Invoke(() =>
            {
                lblDruk.Content = ($"{luchtdruk:f2} hPa");
            }));

            if (luchtdruk < 970)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblDrukAlarm.Content = "Lage luchtdruk!";
                    lblDrukAlarm.Background = new SolidColorBrush(Colors.Red);
                    imgDruk.Source = new BitmapImage(new Uri("/lage_luchtdruk.gif", UriKind.Relative));
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblDrukAlarm.Content = "Hoge luchtdruk!";
                    lblDrukAlarm.Background = new SolidColorBrush(Colors.Red);
                    imgDruk.Source = new BitmapImage(new Uri("/hoge_luchtdruk.gif", UriKind.Relative));
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

        private void cbRGB_Click(object sender, RoutedEventArgs e)
        {
            if (cbRGB.IsChecked == true)
            {
                Weerstation.Background = Brushes.SkyBlue;
                lblWeerstation.Foreground = Brushes.Red;
                gbData.Foreground = Brushes.Green;
                gbData.Background = Brushes.Yellow;
                lblData.Foreground = Brushes.Red;
                gbTemp.Foreground = Brushes.Green;
                gbTemp.Background = Brushes.Yellow;
                lblCTemp.Foreground = Brushes.Red;
                lblTemp.Foreground = Brushes.Red;
                gbVocht.Foreground = Brushes.Green;
                gbVocht.Background = Brushes.Yellow;
                lblCVocht.Foreground = Brushes.Red;
                lblVocht.Foreground = Brushes.Red;
                gbDruk.Foreground = Brushes.Green;
                gbDruk.Background = Brushes.Yellow;
                lblCDruk.Foreground = Brushes.Red;
                lblDruk.Foreground = Brushes.Red;
                cbRGB.Foreground = Brushes.Red;
                cbRGB.Background = Brushes.Yellow;
                Close_Weerstation.Foreground = Brushes.Red;
                Close_Weerstation.Background = Brushes.Yellow;
            }

            else
            {
                Weerstation.Background = Brushes.LightGray;
                lblWeerstation.Foreground = Brushes.Black;
                gbData.Foreground = Brushes.Black;
                gbData.Background = Brushes.LightGray;
                lblData.Foreground = Brushes.Black;
                gbTemp.Foreground = Brushes.Black;
                gbTemp.Background = Brushes.LightGray;
                lblCTemp.Foreground = Brushes.Black;
                lblTemp.Foreground = Brushes.Black;
                gbVocht.Foreground = Brushes.Black;
                gbVocht.Background = Brushes.LightGray;
                lblCVocht.Foreground = Brushes.Black;
                lblVocht.Foreground = Brushes.Black;
                gbDruk.Foreground = Brushes.Black;
                gbDruk.Background = Brushes.LightGray;
                lblCDruk.Foreground = Brushes.Black;
                lblDruk.Foreground = Brushes.Black;
                cbRGB.Foreground = Brushes.Black;
                cbRGB.Background = Brushes.White;
                Close_Weerstation.Foreground = Brushes.Black;
                Close_Weerstation.Background = Brushes.LightGray;
            }
        }
    }
}
