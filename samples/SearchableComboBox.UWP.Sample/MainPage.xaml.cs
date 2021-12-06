using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SearchableComboBox.Samples;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SearchableComboBox.UWP.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();

            ThemeComboBox.SelectedIndex = 0;
        }

        private void OnThemeChanged(object sender, SelectionChangedEventArgs e)
        {
            ElementTheme theme;

            switch (ThemeComboBox.SelectedItem?.ToString())
            {
                case "Dark":
                    theme = ElementTheme.Dark;
                    break;
                case "Light":
                    theme = ElementTheme.Light;
                    break;
                default:
                    theme = ElementTheme.Default;
                    break;
            }

            RequestedTheme = theme;
        }
    }
}
