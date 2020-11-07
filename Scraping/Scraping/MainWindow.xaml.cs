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
    }
}
