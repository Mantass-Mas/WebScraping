using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Scraping.View;

namespace Scraping
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        // 重複起動チェック用
        private static Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            // 重複起動チェック
            Console.WriteLine("重複起動チェック開始");
            _mutex = new Mutex(false, "{E35BC3FC-BE3F-4AD3-BCD4-8950ACC4A747}");
            if(!_mutex.WaitOne(0, false))
            {
                _mutex.Close();
                _mutex = null;
                Console.WriteLine("重複起動してる");
                Shutdown();
                return;
            }

            var window = new MainWindow();
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if(_mutex == null)
            {
                return;
            }

            // ミューテックスの解放
            _mutex.ReleaseMutex();
            _mutex.Close();
            _mutex = null;
        }
    }
}
