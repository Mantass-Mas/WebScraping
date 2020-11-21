using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Scraping
{
    /// <summary>
    /// RegisterDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class RegisterDialog : Window
    {
        public RegisterDialog(string text)
        {
            InitializeComponent();
            var vm = new RegisterViewModel(text);
            DataContext = vm;
        }

        /// <summary>
        /// ダイアログ上で"登録"が押された場合に呼び出し元に伝える
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// ダイアログ上で"キャンセル"が押された場合に呼び出し元に伝える
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
