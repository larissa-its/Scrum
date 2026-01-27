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
                // Dictionary speichert: (Tag, Sensor) -> maximale Temperatur
                var dailyMaxPerSensor = new Dictionary<(DateTime Day, string Sensor), double>();
                bool foundAny = false;

                // In einer Schleife die Werte holen und auswerten. Den größten Wert pro Tag und Sensor "merken".
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

                    var sensor = parts[0].Trim();
                    var timestampText = parts[1].Trim();
                    var tempText = parts[2].Trim();

                    // Timestamp parsen
                    if (!DateTime.TryParseExact(timestampText, "yyyy-MM-dd HH:mm:ss", culture, DateTimeStyles.None, out DateTime timestamp))
                        continue;

                    // Temperatur parsen
                    if (!double.TryParse(tempText, NumberStyles.Float, culture, out double temp))
                        continue;

                    foundAny = true;

                    // Nur das Datum (ohne Uhrzeit) verwenden für Tages-Gruppierung
                    var day = timestamp.Date;
                    var key = (Day: day, Sensor: sensor);

                    // Höchsten Wert für diesen Tag und Sensor merken
                    if (!dailyMaxPerSensor.TryGetValue(key, out double currentMax) || temp > currentMax)
                    {
                        dailyMaxPerSensor[key] = temp;
                    }
                }

                // Höchstwerte auf Oberfläche ausgeben.
                if (!foundAny)
                {
                    MessageBox.Show("No valid temperature values found in temps.csv.", "Result");
                    return;
                }

                // Ergebnis formatieren für Anzeige
                var resultBuilder = new StringBuilder();
                resultBuilder.AppendLine("Daily Maximum Temperatures per Sensor:");
                resultBuilder.AppendLine();

                // Sortiert nach Tag, dann Sensor
                foreach (var entry in dailyMaxPerSensor.OrderBy(kv => kv.Key.Day).ThenBy(kv => kv.Key.Sensor))
                {
                    resultBuilder.AppendLine(
                        $"{entry.Key.Day:yyyy-MM-dd} | {entry.Key.Sensor} -> {entry.Value.ToString("0.0", culture)} °C");
                }

                MessageBox.Show(resultBuilder.ToString(), "Daily Max per Sensor");
            }
            finally
            {
                // Datei wieder freigeben.
                if (reader != null)
                    reader.Dispose();
            }

            // MessageBox.Show("Gleich kachelt das Programm...");
            // kommentieren Sie die Exception aus.
            // throw new Exception("peng");
        }
    }
}
