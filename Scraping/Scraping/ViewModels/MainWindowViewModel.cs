using System;
using Prism.Mvvm;
using Prism.Regions;

namespace Scraping.ViewModels
{
    public class MainWindowViewModel : BindableBase, IDisposable
    {
        private string _title = "WebScraping App";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private IRegionManager regionManager = null;

        public MainWindowViewModel(IRegionManager rgnMgr)
        {
            
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach(var region in this.regionManager.Regions)
                    {
                        region.RemoveAll();
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
