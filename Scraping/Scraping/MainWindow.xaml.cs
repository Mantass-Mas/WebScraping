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
            var vm = new ViewModel();
            DataContext = vm;
        }
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
        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;

            /// <summary>
            /// DataGridのタイトルセルが選択されたとき、
            /// MyBook登録確認ダイアログを表示
            /// </summary>
            if (dataGrid.CurrentColumn.Header.ToString() == "Title")
            {
                if (dataGrid.CurrentCell != null)
                {
                    //選択されたセルから純粋にタイトルのみを抽出する
                    var cell = dataGrid.CurrentColumn.GetCellContent(dataGrid.CurrentItem) as TextBlock;
                    var cell_text = cell.Text;
                    cell_text = cell_text.Trim(' ', '(', ')');
                    //var comic_num = 0;
                    var end = cell_text[cell_text.Length - 1].ToString();
                    var isNum = int.TryParse(end, out var num);
                    while (isNum)
                    {
                        cell_text = cell_text.Remove(cell_text.Length - 1, 1);
                        end = cell_text[cell_text.Length - 1].ToString();
                        isNum = int.TryParse(end, out num);
                    }
                    var title = cell_text.Trim(' ', '(', ')');
                    var window = new RegisterDialog();
                    window.RegisterContent.Text = title;
                    bool? res = window.ShowDialog();
                    //MyBook登録確認ダイアログで登録が押された場合はtrueが返ってくる
                    if (res == true)
                    {
                        //MyBooksにタイトルを追加
                        Registered(title);
                    }
                }
            }
        }
    }
}
