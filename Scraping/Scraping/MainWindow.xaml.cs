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
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainViewModel();
            DataContext = vm;
        }

        /// <summary>
        /// DataGrid上のマウスホイール操作をWindow全体に伝える
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is DataGrid)
            {
                var element = sender as UIElement;

                while (element != null)
                {
                    if (element is ScrollViewer)
                    {
                        var viewer = (ScrollViewer)element;
                        viewer.ScrollToVerticalOffset(viewer.VerticalOffset - e.Delta / 3);
                        return;
                    }
                    element = VisualTreeHelper.GetParent(element) as UIElement;
                }
            }
        }

        /// <summary>
        /// 本のタイトル選択時にお気に入り登録する(登録確認あり)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            
            if (dataGrid.CurrentColumn?.Header.ToString() == "Title")
            {
                if (dataGrid.CurrentCell != null)
                {
                    var vm = DataContext as MainViewModel;
                    vm.AddFavorite(dataGrid);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
