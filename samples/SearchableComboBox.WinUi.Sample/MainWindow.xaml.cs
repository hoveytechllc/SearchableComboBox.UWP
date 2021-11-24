using Microsoft.UI.Xaml;
using SearchableComboBox.Samples;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SearchableComboBox.WinUi.Sample
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            if (Content is FrameworkElement element)
                element.DataContext = new MainViewModel();
        }
    }
}
