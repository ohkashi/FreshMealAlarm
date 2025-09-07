using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace FreshMealAlarm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			var isLightTheme = IsLightTheme();
			var source = (HwndSource)PresentationSource.FromVisual(this);
			ToggleBaseColour(source.Handle, !isLightTheme);

			// Detect when the theme changed
			source.AddHook((IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) => {
				const int WM_SETTINGCHANGE = 0x001A;
				if (msg == WM_SETTINGCHANGE) {
					if (wParam == IntPtr.Zero && Marshal.PtrToStringUni(lParam) == "ImmersiveColorSet") {
						var isLightTheme = IsLightTheme();
						ToggleBaseColour(hwnd, !isLightTheme);
					}
				}

				return IntPtr.Zero;
			});
		}

		private readonly PaletteHelper _paletteHelper = new();

		private void ToggleBaseColour(nint hwnd, bool isDark)
		{
			var theme = _paletteHelper.GetTheme();
			var baseTheme = isDark ? BaseTheme.Dark : BaseTheme.Light;
			theme.SetBaseTheme(baseTheme);
			_paletteHelper.SetTheme(theme);
			UseImmersiveDarkMode(hwnd, isDark);
		}

		private static bool IsLightTheme()
		{
			using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
			var value = key?.GetValue("AppsUseLightTheme");
			return value is int i && i > 0;
		}

		[DllImport("dwmapi.dll")]
		private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

		private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
		private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

		private static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
		{
			if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763)) {
				var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
				if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 18985)) {
					attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
				}

				int useImmersiveDarkMode = enabled ? 1 : 0;
				return DwmSetWindowAttribute(handle, attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
			}

			return false;
		}
	}
}