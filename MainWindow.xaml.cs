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

            // Maak een nieuwe serialport aan.
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
                // Start de seriele poort als hij beschikbaar is.
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
            // Lees alle tekst in, tot je een 'nieuwe lijn symbool' binnenkrijgt.
            // New line = '\n' = ASCII-waarde 10 = ALT 10.
            string receivedText = _serialPort.ReadLine();

            // Geef de ontvangen data door aan een method die op de UI thread loopt.
            // Doe dat via een Action delegate... Delegates en Events zullen 
            // in detail behandeld worden in het vak OOP.
            Dispatcher.Invoke(new Action<string>(UpdateLabelData), receivedText);

            // Aanmaken van een nieuwe klasse.
            Split_Data split_data = new Split_Data();

            // Het splitsen van de string gebeurt in de klasse split_data.
            // Temperatuur kunnnen in de klasse en in de main aangepast worden door te werken met passing reference.
            split_data.split_string(receivedText, ref _temperatuur, ref _vochtigheid, ref _luchtdruk);

            // Ga naar de method voor het aanpassen van de temperatuur, vochtigheid...
            // De variabele wordt telkens omgezet naar een float voordat deze meegegeven wordt naar de method.
            // De cultuur wordt aangepast zodat de punten vanuit de string komma's zullen zijn op de labels.
            UpdateLabelTemp(float.Parse(_temperatuur, System.Globalization.CultureInfo.InvariantCulture));
            UpdateLabelVocht(float.Parse(_vochtigheid, System.Globalization.CultureInfo.InvariantCulture));
            UpdateLabelDruk(float.Parse(_luchtdruk, System.Globalization.CultureInfo.InvariantCulture));
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // Als de seriale poort beschikbaar is en wordt data verstuurd dan mag de tweede window geopend worden.
            if ((_serialPort.IsOpen) && (_serialPort != null))
            {
                COM.Visibility = Visibility.Hidden; // Hierop staan alle elementen voor het selecteren van een poort, dit wordt nu verborgen.
                Weerstation.Visibility = Visibility.Visible; // Hierop stan elementen voor aflezen van de meting, dit wordt nu getoond.
            }

            else // Geef een waarschuwing wanneer er geen COM-poort geselecteerd is.
            {
                MessageBox.Show("Selecteer eerst een COM-poort!");
            }
        }

        // Deze method wordt opgeroepen om het label met de volledige string aan te passen.
        public void UpdateLabelData(string text)
        {
            // Dit kan handig voor de gebruiker om te kijken of de string wel goed ontvangen wordt.
            lblData.Content = text; 
        }

        // Deze method wordt opgeroepen om het label en image van luchtdruk aan te passen.
        public void UpdateLabelTemp(float temperatuur)
        {
            // Dit is nodig omdat er in C# meerdere threads bestaan. Je kan de xaml niet aapassen vanuit de main omdat dit een andere thread is.
            // Wanneer ik een label wil aanpassen of een image dan staan deze op een andere thread dan de waarden zoals temperatuur.
            // Om dit toch te laten werken moet ik toestemming geven om de label te laten aanpassen vanuit de main. 
            // Dit gebeurt met volgend stuk code dat ik elke keer gebruik om zo'n actie uit te voeren.
            Task.Run(() => this.Dispatcher.Invoke(() =>
            {
                lblTemp.Content = ($"{temperatuur:f2} °C"); // Plaats de waarde temperatuur op de label
            }));

            // Als de temperatuur de laag geef een waarschuwing en pas de afbeelding aan.
            if (temperatuur < 15)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblTempAlarm.Content = "Opgepast voor lage temperatuur!";
                    lblTempAlarm.Background = new SolidColorBrush(Colors.Blue);
                    imgTemp.Source = new BitmapImage(new Uri("/koude_temperatuur.jpg", UriKind.Relative)); // Verander de bron van de afbeelding.
                });
            }
            // Als de temperatuur de hoog geef een waarschuwing en pas de afbeelding aan.
            if (temperatuur > 22)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblTempAlarm.Content = "Opgepast voor hoge temperatuur!";
                    lblTempAlarm.Background = new SolidColorBrush(Colors.Red);
                    imgTemp.Source = new BitmapImage(new Uri("/warme_temperatuur.jpg", UriKind.Relative));
                });
            }
            // Als de temperatuur goed is pas dan de melding en de afbeelding aan.
            if (temperatuur > 15 && temperatuur < 22)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblTempAlarm.Content = "Goede temperatuur!";
                    lblTempAlarm.Background = new SolidColorBrush(Colors.Green);
                    imgTemp.Source = new BitmapImage(new Uri("/goede_temperatuur.jpg", UriKind.Relative));
                });
            }
        }

        // Deze method wordt opgeroepen om het label en image van luchtdruk aan te passen.
        public void UpdateLabelVocht(float vochtigheid)
        {
            Task.Run(() => this.Dispatcher.Invoke(() =>
            {
                lblVocht.Content = ($"{vochtigheid} %");
            }));

            if (vochtigheid < 30)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblVochtAlarm.Content = "opgepast voor lage luchtvochtigeid!";
                    lbVochtAlarm.Background = new SolidColorBrush(Colors.Red);
                    imgVocht.Source = new BitmapImage(new Uri("/lage_luchtvochtigheid.jpg", UriKind.Relative));
                });
            }
            if (vochtigheid > 70)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblVochtAlarm.Content = "opgepast voor hoge luchtvochtigheid!";
                    lblVochtAlarm.Background = new SolidColorBrush(Colors.Red);
                    imgVocht.Source = new BitmapImage(new Uri("/hoge_luchtvochtigheid.jpeg", UriKind.Relative));
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblVochtAlarm.Content = "Goede luchtvochtigheid!";
                    lblVochtAlarm.Background = new SolidColorBrush(Colors.Green);
                    imgVocht.Source = new BitmapImage(new Uri("/goede_luchtvochtigheid.jpg", UriKind.Relative));
                });
            }
        }

        // Deze method wordt opgeroepen om het label en image van luchtdruk aan te passen.
        public void UpdateLabelDruk(float luchtdruk)
        {
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

        // Wannneer de checkbox RGB-modus wordt aangevinkt dan veranderen de kleuren.
        private void cbRGB_Click(object sender, RoutedEventArgs e)
        {
            // Pas deze kleuren toe wanneer de RGB-modus geselecteerd wordt.
            if (cbRGB.IsChecked == true)
            {
                RGB(new SolidColorBrush(Colors.SkyBlue), new SolidColorBrush(Colors.Red), new SolidColorBrush(Colors.Green), new SolidColorBrush(Colors.Yellow));
            }
            // Dit zijn de standaard kleuren.
            else
            {
                RGB(new SolidColorBrush(Colors.LightGray), new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.LightGray));
            }
        }

        // Wijs de kleuren toe aan een object.
        private void RGB(SolidColorBrush bGrid, SolidColorBrush fLabel, SolidColorBrush fGroupbox, SolidColorBrush bGroupbox)
        {
            Weerstation.Background = bGrid;
            lblWeerstation.Foreground = fLabel;
            gbData.Foreground = fGroupbox;
            gbData.Background = bGroupbox;
            lblData.Foreground = fLabel;
            gbTemp.Foreground = fGroupbox;
            gbTemp.Background = bGroupbox;
            lblCTemp.Foreground = fLabel;
            lblTemp.Foreground = fLabel;
            gbVocht.Foreground = fGroupbox;
            gbVocht.Background = bGroupbox;
            lblCVocht.Foreground = fLabel;
            lblVocht.Foreground = fLabel;
            gbDruk.Foreground = fGroupbox;
            gbDruk.Background = bGroupbox;
            lblCDruk.Foreground = fLabel;
            lblDruk.Foreground = fLabel;
            cbRGB.Foreground = fLabel;
            cbRGB.Background = bGroupbox;
            Close_Weerstation.Foreground = fLabel;
            Close_Weerstation.Background = bGroupbox;
        }

        // Wanneer de window afgesloten wordt moet het lezen van de serialport gestopt worden.
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Dispose();
            }
        }

        // Sluit de window wanneer op de afsluitknop gedrukt wordt en stop de serialport.
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();

            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Dispose();
            }
        }
    }
}