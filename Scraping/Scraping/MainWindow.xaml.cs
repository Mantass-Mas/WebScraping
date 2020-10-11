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
            InitialWindow();
        }

        public async void InitialWindow()
        {
            /// <summary>
            /// Webスクレイピングを非同期処理で行い、
            /// DataGrid.ItemSourceにバインドする
            /// </summary>
            var webScraping = new WebScraping();
            dataGrid.ItemsSource = await webScraping.GetWebDataAsync();
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            /// <summary>
            /// DataGrid上でマウスホイールのスクロールイベントが発生したとき、
            /// 親要素のScrollViewerをスクロールさせる
            /// </summary>
            if(sender is DataGrid)
            {
                var element = sender as UIElement;

                while(element != null)
                {
                    if(element is ScrollViewer)
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
                if(dataGrid.CurrentCell != null)
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
                    cell_text = cell_text.Trim(' ', '(', ')');
                    var content = cell_text;
                    var window = new RegisterDialog();
                    window.RegisterContent.Text = content;
                    bool? res = window.ShowDialog();
                }
            }
        }
    }
}
