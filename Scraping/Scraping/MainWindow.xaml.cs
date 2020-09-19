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
using System.Net.Http;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

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
            var month = DateTime.Today.Month;
            monthText.Text = $"{month}月発売";
        }

        static void WindowCreate()
        {

        }
    }

    class WebData
    {
        static HttpClient client = new HttpClient();
        public string[] StringData { get; set; }
        public async Task GetString(int month)
        {
            var urlstring = @"http://yurinavi.com/yuri-calendar/";
            var document = default(IHtmlDocument);
            using(var stream = await client.GetStreamAsync(urlstring))
            {
                var parser = new HtmlParser();
                document = await parser.ParseDocumentAsync(stream);
            }
            
        }
    }
}
