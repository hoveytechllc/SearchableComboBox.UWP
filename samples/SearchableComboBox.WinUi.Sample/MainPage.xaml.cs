using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using SearchableComboBox.Samples;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SearchableComboBox.WinUi.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ThemeComboBox.SelectedIndex = 0;
        }

        public MainViewModel ViewModel { get; } = new MainViewModel();
        
        private void OnThemeChanged(object sender, SelectionChangedEventArgs e)
        {
            ElementTheme theme;
            var selected = ThemeComboBox.SelectedItem?.ToString();
            switch (selected)
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

            Frame.RequestedTheme = theme;

            //if (Content is FrameworkElement element)
            //{
            //    element.RequestedTheme = theme;

            //    var root = element.XamlRoot;
            //}
        }
    }
}
