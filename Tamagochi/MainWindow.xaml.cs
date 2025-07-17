using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Text.Json;


namespace Tamagochi
{
    public partial class MainWindow : Window
    {
        //---------------------------ADATTAGOK------------------------
        private TamagotchiClass tamagochi;
        private DispatcherTimer timer;

        private bool fatigueWindowShown = false;
        private bool happinessWindowShown = false;
        private bool hungerWindowShown = false;

        private int happinessTickCounter = 0;
        private int energyTickCounter = 0;

        public string TamagochiName { get; private set; }
        private DateTime startTime;
        private DispatcherTimer ageTimer;

        //---------------------------KONSTRUKTOR------------------------
        public MainWindow(TamagotchiClass loadedTamagochi)
        {
            InitializeComponent();
            tamagochi = loadedTamagochi;
            SetTimers();
        }

        public MainWindow()
        {
            InitializeComponent();
            tamagochi = new TamagotchiClass();
            SetTimers();
        }

        //---------------------------IDŐZÍTŐK------------------------
        private void SetTimers()
        {
            tamagochi.HungerDepleted += Tamagochi_HungerDepleted;
            //tamagochi.EnergyDepleted += Tamagochi_EnergyDepleted;

            startTime = DateTime.Now;

            ageTimer = new DispatcherTimer();
            ageTimer.Interval = TimeSpan.FromSeconds(1);
            ageTimer.Tick += AgeTimer_Tick;
            ageTimer.Start();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            UpdateUI();
        }

        public void SetTamagochiName(string tamagochiName)
        {
            TamagochiName = tamagochiName;
            TamagochiInfo.Text = $"Név: {TamagochiName}, Kor: 0 perc";  
        }

        private void AgeTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan age = DateTime.Now - startTime;
            TamagochiInfo.Text = $"Név: {TamagochiName}, Kor: {age.TotalMinutes:F0} perc";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tamagochi.Hunger = Math.Min(100, tamagochi.Hunger + 1);

            if (tamagochi.Hunger == 100 && !hungerWindowShown)
            {
                hungerWindowShown = true;
                ShowHungerWindow();
            } 

            energyTickCounter++;
            if (energyTickCounter % 3 == 0)
            {
                tamagochi.Energy = Math.Max(0, tamagochi.Energy - 1);
            }

            if (tamagochi.Energy == 0)
            {
                if (!fatigueWindowShown)
                {
                    fatigueWindowShown = true;
                    ShowFatigueWindow();
                }
            }

            happinessTickCounter++;
            if (happinessTickCounter % 2 == 0) 
            {
                tamagochi.Happiness = Math.Max(0, tamagochi.Happiness - 1);
            }

            if (tamagochi.Happiness == 100 && !happinessWindowShown)
            {
                happinessWindowShown = true;
                ShowHappinessWindow();
            }

            UpdateUI();
        }

        //------------------------FELUGRÓ ABLAKOK------------------------
        private void ShowFatigueWindow()
        {
            Dispatcher.Invoke(() =>
            {
                
                FatigueWindow fatigueWindow = new FatigueWindow();
                fatigueWindow.ShowDialog();
            });
        }

        private void ShowHappinessWindow()
        {
            Dispatcher.Invoke(() =>
            {
                HappinessWindow happinessWindow = new HappinessWindow();
                happinessWindow.ShowDialog();
            });
        }

        private void ShowHungerWindow()
        {
            Dispatcher.Invoke(() =>
            {
                HungerWindow hungerWindow = new HungerWindow();
                hungerWindow.ShowDialog();
            });
        }

        private void Tamagochi_HungerDepleted()
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("A Tamagotchi nem kér több kaját!", "Értesítés", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        //---------------------------GOMBOK------------------------
        private void FeedButton_Click(object sender, RoutedEventArgs e)
        {
            tamagochi.Feed();
            UpdateUI();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            tamagochi.Play();
            UpdateUI();
        }

        private void RestButton_Click(object sender, RoutedEventArgs e)
        {
            tamagochi.Rest();
            UpdateUI();
        }

        //---------------------------UPDATEUI------------------------
        private void UpdateUI()
        {
            HungerBar.Value = tamagochi.Hunger;
            EnergyBar.Value = tamagochi.Energy;
            HappinessBar.Value = tamagochi.Happiness;

            FeedButton.IsEnabled = tamagochi.Hunger > 0;
            PlayButton.IsEnabled = tamagochi.Energy > 0;
        }

        //------------------------MENTÉS / BETÖLTÉS------------------------
        public void SaveState(string filePath)
        {
            var state = new TamagochiState
            {
                Name = tamagochi.Name,
                Hunger = tamagochi.Hunger,
                Energy = tamagochi.Energy,
                Happiness = tamagochi.Happiness,
                Age = tamagochi.Age
            };

            string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = ".json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var state = new TamagochiState
                {
                    Name = tamagochi.Name,
                    Hunger = tamagochi.Hunger,
                    Energy = tamagochi.Energy,
                    Happiness = tamagochi.Happiness,
                    Age = tamagochi.Age
                };

                string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(saveFileDialog.FileName, json);

                MessageBox.Show("Mentés sikeres!", "Mentés", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        public void LoadState(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("A mentési fájl nem található!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string json = File.ReadAllText(filePath);
            var state = JsonSerializer.Deserialize<TamagochiState>(json);

            tamagochi.Name = state.Name;
            tamagochi.Hunger = state.Hunger;
            tamagochi.Energy = state.Energy;
            tamagochi.Happiness = state.Happiness;
            tamagochi.Age = state.Age;

            UpdateUI();
        }
    }
}
