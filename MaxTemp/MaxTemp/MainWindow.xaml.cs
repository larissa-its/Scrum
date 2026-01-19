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
            //Zugriff auf Datei erstellen.

            //Anfangswert setzen, um sinnvoll vergleichen zu können.


            //In einer Schleife die Werte holen und auswerten. Den größten Wert "merken".


            //Datei wieder freigeben.


            //Höchstwert auf Oberfläche ausgeben.

            MessageBox.Show("Gleich kachelt das Programm...");
            //kommentieren Sie die Exception aus.
            throw new Exception("peng");
        }
    }
}
