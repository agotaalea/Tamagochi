using System.Windows;

namespace Tamagochi
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            bool? result = mainMenu.ShowDialog();
            
            if (string.IsNullOrWhiteSpace(mainMenu.TamagochiName))
            {
                MessageBox.Show("A név nincs megadva vagy az ablakot bezárták.", "Kilépés", MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
            }
        }
    }
}
