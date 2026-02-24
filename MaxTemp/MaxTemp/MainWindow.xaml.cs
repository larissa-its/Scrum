using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace MaxTemp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnAuswerten_Click(object sender, RoutedEventArgs e)
        {
            string filename = "temps.csv";

            if (!File.Exists(filename))
            {
                MessageBox.Show("temps.csv not found in output folder!");
                return;
            }

            var culture = CultureInfo.InvariantCulture;

            // Date -> (Sensor -> MaxTemp)
            var dailyMax = new Dictionary<DateTime, Dictionary<string, double>>();

            // Speicherung aller Messwerte:
            // Date -> TimeOfDay -> Sensor -> List<double>
            var rawData = new Dictionary<DateTime, Dictionary<TimeSpan, Dictionary<string, double>>>();

            try
            {
                foreach (var line in File.ReadLines(filename))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split(',');
                    if (parts.Length != 3)
                        continue;

                    string sensor = parts[0].Trim();
                    string timestampText = parts[1].Trim();
                    string tempText = parts[2].Trim();

                    if (!DateTime.TryParseExact(timestampText,
                        "yyyy-MM-dd HH:mm:ss",
                        culture,
                        DateTimeStyles.None,
                        out DateTime timestamp))
                        continue;

                    if (!double.TryParse(tempText,
                        NumberStyles.Float,
                        culture,
                        out double temp))
                        continue;

                    DateTime day = timestamp.Date;
                    TimeSpan time = timestamp.TimeOfDay;

                    // ---------- DAILY MAX ----------
                    if (!dailyMax.ContainsKey(day))
                        dailyMax[day] = new Dictionary<string, double>();

                    if (!dailyMax[day].ContainsKey(sensor) || temp > dailyMax[day][sensor])
                        dailyMax[day][sensor] = temp;

                    // ---------- RAW DATA ----------
                    if (!rawData.ContainsKey(day))
                        rawData[day] = new Dictionary<TimeSpan, Dictionary<string, double>>();

                    if (!rawData[day].ContainsKey(time))
                        rawData[day][time] = new Dictionary<string, double>();

                    rawData[day][time][sensor] = temp;
                }

                if (dailyMax.Count == 0)
                {
                    MessageBox.Show("No valid data found.");
                    return;
                }

                var allSensors = dailyMax
                    .SelectMany(d => d.Value.Keys)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();

                // ----------------- dailymax.csv -----------------
                using (StreamWriter writer = new StreamWriter("dailymax.csv"))
                {
                    writer.Write("Date");
                    foreach (var sensor in allSensors)
                        writer.Write("," + sensor);
                    writer.WriteLine();

                    foreach (var day in dailyMax.Keys.OrderBy(d => d))
                    {
                        writer.Write(day.ToString("yyyy-MM-dd"));
                        foreach (var sensor in allSensors)
                        {
                            if (dailyMax[day].ContainsKey(sensor))
                                writer.Write("," + dailyMax[day][sensor].ToString("0.0", culture));
                            else
                                writer.Write(",");
                        }
                        writer.WriteLine();
                    }
                }

                // ----------------- WARME TAGE (>50°C) -----------------
                var hotDays = dailyMax
                    .Where(d => d.Value.Values.Any(temp => temp > 50.0))
                    .Select(d => d.Key)
                    .ToList();

                if (hotDays.Count == 0)
                {
                    MessageBox.Show("No hot days (>50°C) found.");
                    return;
                }

                // Time -> Sensor -> List<double>
                var averageData = new Dictionary<TimeSpan, Dictionary<string, List<double>>>();

                foreach (var day in hotDays)
                {
                    if (!rawData.ContainsKey(day))
                        continue;

                    foreach (var timeEntry in rawData[day])
                    {
                        TimeSpan time = timeEntry.Key;

                        if (!averageData.ContainsKey(time))
                            averageData[time] = new Dictionary<string, List<double>>();

                        foreach (var sensorEntry in timeEntry.Value)
                        {
                            string sensor = sensorEntry.Key;
                            double temp = sensorEntry.Value;

                            if (!averageData[time].ContainsKey(sensor))
                                averageData[time][sensor] = new List<double>();

                            averageData[time][sensor].Add(temp);
                        }
                    }
                }

                // ----------------- yearaverage.csv -----------------
                using (StreamWriter writer = new StreamWriter("yearaverage.csv"))
                {
                    writer.Write("Time");
                    foreach (var sensor in allSensors)
                        writer.Write("," + sensor);
                    writer.WriteLine();

                    foreach (var time in averageData.Keys.OrderBy(t => t))
                    {
                        writer.Write(time.TotalHours.ToString("0.00", culture));

                        foreach (var sensor in allSensors)
                        {
                            if (averageData[time].ContainsKey(sensor))
                            {
                                double avg = averageData[time][sensor].Average();
                                writer.Write("," + avg.ToString("0.0", culture));
                            }
                            else
                                writer.Write(",");
                        }

                        writer.WriteLine();
                    }
                }

                MessageBox.Show("dailymax.csv AND yearaverage.csv successfully created!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}

