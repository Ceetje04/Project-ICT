using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Project_ICT
{
    internal class Data_ontvangen
    {
        //private int "Opgepast voor lage temperatuur!";
        //private int "Opgepast voor hoge temperatuur!";
        //private int "Goede temperatuur!";
        //public int lblAlarm (int temeperatuur)
        //{

        //}

        //public string temperatuur { get; set; }
        //public string vochtigheid { get; set; }
        //public string luchtdruk { get; set; }

        //public void UpdateLabelTemp(double temperatuur) //Dit werkt niet omdat temperatuur een string is.
        //{

        //    Task.Run(() => this.Dispatcher.Invoke(() =>
        //    {
        //        lblTemp.Content = ($"{temperatuur} °C");
        //    }));

        //    if (temperatuur < 15)
        //    {
        //        this.Dispatcher.Invoke(() =>
        //        {
        //            lblTempAlarm.Content = "Opgepast voor lage temperatuur!";
        //            lblTempAlarm.Background = new SolidColorBrush(Colors.Blue);
        //        });
        //    }
        //    if (temperatuur > 25)
        //    {
        //        this.Dispatcher.Invoke(() =>
        //        {
        //            lblTempAlarm.Content = "Opgepast voor hoge temperatuur!";
        //            lblTempAlarm.Background = new SolidColorBrush(Colors.Red);
        //        });
        //    }
        //    if (temperatuur > 15 && temperatuur < 25)
        //    {
        //        this.Dispatcher.Invoke(() =>
        //        {
        //            lblTempAlarm.Content = "Goede temperatuur!";
        //            lblTempAlarm.Background = new SolidColorBrush(Colors.Green);
        //        });
        //    }
        //}

        public string text { get; set; }

        public void split_string(string text, ref string temperatuur, ref string vochtigheid, ref string luchtdruk)
        {
            string arduino = text;
            string[] var = arduino.Split(";"); // Zoekt de drie puntkomma's en zet ze om in drie strings.;

            temperatuur = var[0];
            vochtigheid = var[1];
            luchtdruk = var[2];
        }
    }
}
