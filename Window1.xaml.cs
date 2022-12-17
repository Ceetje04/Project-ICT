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

//namespace Project_ICT
//{
//    /// <summary>
//    /// Interaction logic for Window1.xaml
//    /// </summary>
//    //public partial class Window1 : Window
//    ////{

//        //SerialPort _serialPort;

//        //public Window1()
//        //{
//        //    InitializeComponent();

//        //    //_serialPort = new SerialPort();

//        //    //// Komt er data binnen op de seriële poort, vang ze op...
//        //    //_serialPort.DataReceived += _serialPort_DataReceived;

//        //    //cbxComPorts.Items.Add("None");
//        //    //foreach (string s in SerialPort.GetPortNames()) // Zoek de beschikbare poorten.
//        //    //{
//        //    //    cbxComPorts.Items.Add(s);
//        //    //}
//        //}

////        private void cbxComPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
////        {
////            if (_serialPort != null)
////            {
////                if (_serialPort.IsOpen)
////                    _serialPort.Close();

////                if (cbxComPorts.SelectedItem.ToString() != "None")
////                {
////                    _serialPort.PortName = cbxComPorts.SelectedItem.ToString();
////                    _serialPort.Open();
////                }
////            }
////        }
////        public void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
////        {
////            Lees alle tekst in, tot je een 'nieuwe lijn symbool' binnenkrijgt.
////            New line = '\n' = ASCII - waarde 10 = ALT 10.
////            string receivedText = _serialPort.ReadLine();

////            MainWindow mainwindow = new MainWindow();
////            mainwindow._serialPort_DataReceived(ref receivedText);

////            // Geef de ontvangen data door aan een method die op de UI thread loopt.
////            // Doe dat via een Action delegate... Delegates en Events zullen 
////            // in detail behandeld worden in het vak OOP.
////            Dispatcher.Invoke(new Action<string>(MainWindow.UpdateLabelData), receivedText);

////            Split_Data data_ontvangen = new Split_Data(); // Nieuwe klasse.

////            data_ontvangen.split_string(receivedText, ref _temperatuur, ref _vochtigheid, ref _luchtdruk); // Het splitsen van de string gebeurt in de klasse. Temperatuur kunnnen in de klasse en in de main aangepast worden door te werken met passing reference.

////            MainWindow.UpdateLabelTemp(float.Parse(_temperatuur, System.Globalization.CultureInfo.InvariantCulture)); // Ga naar de method voor het aanpassen van de temperatuur. Temperatuur wordt omgezet met een float.
////            MainWindow.UpdateLabelVocht(float.Parse(_vochtigheid, System.Globalization.CultureInfo.InvariantCulture)); // Ga naar de method voor het aanpassen van de vochtigheid. Vochtigheid wordt omgezet naar een float.
////            MainWindow.UpdateLabelDruk(float.Parse(_luchtdruk, System.Globalization.CultureInfo.InvariantCulture)); // Ga naar de method voor het aanpassen van de luchtdruk. Luchtdruk wordt omgezet naar een float.
////        }

////        private void btnStart_Click(object sender, RoutedEventArgs e)
////        {
////            MainWindow metingen = new MainWindow();
////            metingen.Show();
////        }

////        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
////        {
////            if (_serialPort != null && _serialPort.IsOpen)
////            {
////                _serialPort.Dispose();
////            }
////        }
////    }
////}
