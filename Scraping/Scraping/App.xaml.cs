using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Scraping
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 多重起動防止用のミューテックス
        /// </summary>
        private static Mutex mutex;
        /// <summary>
        /// アプリ終了時のイベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            // 多重起動の終了時は終了処理をしない
            if(App.mutex == null) { return; }
            
            // ミューテックスの解放
        }
        /// <summary>
        /// アプリ起動時のイベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // GUIDを利用して多重起動チェック
            // すでに起動されているなら即終了
            var guidAttrib = Assembly.GetEntryAssembly().GetCustomAttribute<GuidAttribute>();
            var guidString = guidAttrib.Value;
            App.mutex = new Mutex(false, guidString);
            if (!App.mutex.WaitOne(0, false))
            {
                App.mutex.Close();
                App.mutex = null;
                Shutdown();
                return;
            }
            // メインウィンドウ表示
            var window = new MainWindow();
            window.Show();
        }

    }
}
