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