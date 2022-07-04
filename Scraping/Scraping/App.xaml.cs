using Prism.Ioc;
using Scraping.Views;
using System.Windows;
using System.Threading;
using Prism.Modularity;

namespace Scraping
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        private Mutex _mutex = new Mutex(false, "WebScrapingApp");

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        private void PrismApplication_Startup(object sender, StartupEventArgs e)
        {
            if (_mutex.WaitOne(0, false)) return;

            MessageBox.Show("すでに起動しています。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
            _mutex.Close();
            _mutex = null;
            Shutdown();
        }

        private void PrismApplication_Exit(object sender, ExitEventArgs e)
        {
            if(_mutex != null)
            {
                _mutex.ReleaseMutex();
                _mutex.Close();
            }
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            
        }
    }
}
