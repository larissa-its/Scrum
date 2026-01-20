using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

namespace MaxTemp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Diese Routine (EventHandler des Buttons Auswerten) liest die Werte
        /// zeilenweise aus der Datei temps.csv aus, merkt sich den höchsten Wert
        /// und gibt diesen auf der Oberfläche aus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAuswerten_Click(object sender, RoutedEventArgs e)
        {
            // Zugriff auf Datei erstellen.
            // Voraussetzung: temps.csv ist im Projekt als "Content" eingebunden und wird ins Ausgabeverzeichnis kopiert.
            var filename = "temps.csv";

            if (!File.Exists(filename))
            {
                MessageBox.Show(
                    "File 'temps.csv' was not found.\n" +
                    "Hint: In Visual Studio select temps.csv -> Properties -> Build Action = Content, Copy to Output Directory = Copy if newer.",
                    "File Warning");
                return;
            }

            StreamReader reader = null;

            try
            {
                reader = new StreamReader(filename);

                // Anfangswert setzen, um sinnvoll vergleichen zu können.
                double maxTemp = double.NegativeInfinity;
                bool foundAny = false;

                // In einer Schleife die Werte holen und auswerten. Den größten Wert "merken".
                // CSV-Format: Sensor,yyyy-MM-dd HH:mm:ss,Temperature
                var culture = CultureInfo.InvariantCulture;

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split(',');
                    if (parts.Length != 3)
                        continue;

                    // parts[2] = temperature
                    if (!double.TryParse(parts[2].Trim(), NumberStyles.Float, culture, out double temp))
                        continue;

                    foundAny = true;
                    if (temp > maxTemp)
                        maxTemp = temp;
                }

                // Höchstwert auf Oberfläche ausgeben.
                if (!foundAny)
                {
                    MessageBox.Show("No valid temperature values found in temps.csv.", "Result");
                    return;
                }

                MessageBox.Show($"Maximum temperature: {maxTemp.ToString("0.0", culture)} °C", "Result");
            }
            finally
            {
                // Datei wieder freigeben.
                if (reader != null)
                    reader.Dispose();
            }

            MessageBox.Show("Gleich kachelt das Programm...");
            //kommentieren Sie die Exception aus.
            //throw new Exception("peng");
        }
    }
}
