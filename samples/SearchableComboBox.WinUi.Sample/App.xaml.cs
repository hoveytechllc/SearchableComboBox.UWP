using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using SearchableComboBox.Samples;

namespace SearchableComboBox.WinUi.Sample
{
    public partial class App : Application, IMainThreadInvoker
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            BaseViewModel.Initialize(this);

            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;

        public Task Invoke(Action action)
        {
            m_window.DispatcherQueue.TryEnqueue(() =>
            {
                action();
            });

            return Task.CompletedTask;
        }
    }
}
