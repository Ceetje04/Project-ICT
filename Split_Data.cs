using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Project_ICT
{
    internal class Split_Data
    {
        public string text { get; set; }

        // De string die ontvangen werdt door de serialport de lezen wordt in deze klasse gesplist in aparte variabelen.
        public void split_string(string text, ref string temperatuur, ref string vochtigheid, ref string luchtdruk)
        {
            string arduino = text;
            string[] var = arduino.Split(";"); // Zoekt de drie puntkomma's en zet ze om in drie strings.;

            // Geef de drie variabelen die gevonden werden in de string een naam.
            // Temperatuur zal hier dezelfde waarde hebben als in de main dit komt doordat ik werk met passing by reference.
            temperatuur = var[0];
            vochtigheid = var[1];
            luchtdruk = var[2];
        }
    }
}