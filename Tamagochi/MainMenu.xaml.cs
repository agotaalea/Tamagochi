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
using System.Windows.Shapes;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Tamagochi
{
    public partial class MainMenu : Window
    {
        public string TamagochiName { get; private set; }
        public TamagotchiClass tamagochi { get; set; }

        public MainMenu()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Kérlek, adj meg egy nevet a Tamagochidnak!", "Hiányzó név", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TamagochiName = NameTextBox.Text;
            MainWindow mainWindow = new MainWindow();
            mainWindow.SetTamagochiName(this.TamagochiName);
            mainWindow.Show();
            this.Close();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = ".json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                var state = JsonSerializer.Deserialize<TamagochiState>(json);

                TamagotchiClass tamagochi = new TamagotchiClass();
                tamagochi.Name = state.Name;
                tamagochi.Hunger = state.Hunger;
                tamagochi.Energy = state.Energy;
                tamagochi.Happiness = state.Happiness;
                tamagochi.Age = state.Age;

                MessageBox.Show("Betöltés sikeres!", "Betöltés", MessageBoxButton.OK, MessageBoxImage.Information);

                this.NameTextBox.Text = tamagochi.Name;
                TamagochiName = NameTextBox.Text;
                MainWindow mainWindow = new MainWindow(tamagochi);
                mainWindow.SetTamagochiName(this.TamagochiName);
                mainWindow.Show();
                this.Close();
            }
        }
    }
}