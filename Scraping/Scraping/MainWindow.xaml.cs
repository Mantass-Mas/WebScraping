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
using System.Data;

namespace Scraping
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        static int month = DateTime.Today.Month;
        static readonly DataTable dataTable = new DataTable();
        public MainWindow()
        {
            InitializeComponent();
            dataTable.Columns.Clear();
            dataTable.Rows.Clear();
            dataTable.Columns.Add("発売日");
            dataTable.Columns.Add("タイトル");
            dataTable.Columns.Add("著者");
            //Start();
        }

        static async void Start()
        {
            var title = await WebData.GetString();
            var row = dataTable.NewRow();
            row[0] = title;
        }
        public DataView DataTableView => new DataView(dataTable);
    }

    class WebData
    {
        static HttpClient client = new HttpClient();
        public static async Task<string> GetString()
        {
            var urlstring = @"http://yurinavi.com/yuri-calendar/";
            var document = default(IHtmlDocument);
            using(var stream = await client.GetStreamAsync(urlstring))
            {
                var parser = new HtmlParser();
                document = await parser.ParseDocumentAsync(stream);
            }
            return document.Title;
        }
    }
}
