using System.Configuration;
using System.Data;
using System.Windows;
using WpfSingleInstanceByEventWaitHandle;

namespace FreshMealAlarm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		protected override void OnStartup(StartupEventArgs e)
		{
            WpfSingleInstance.Make("FreshMealAlarm-Application");

            base.OnStartup(e);

			// Create an instance of your main window but do not show it yet
			MainWindow mainWindow = new();
			Application.Current.MainWindow = mainWindow;
#if DEBUG
			mainWindow.Show();
#endif

			// If you want to show a different window first (e.g., a login window)
			// LoginWindow loginWindow = new LoginWindow();
			// loginWindow.Show(); 
		}
	}
}
