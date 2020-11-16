using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Scraping
{
    class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    class ViewModel : ViewModelBase
    {
        /// <summary>
        /// すべての本のリスト
        /// </summary>
        private List<Book> _bookList = new List<Book>();
        /// <summary>
        /// お気に入り登録した本のリスト
        /// </summary>
        private List<Book> _favoriteList = new List<Book>();
        /// <summary>
        /// お気に入り表示かどうか
        /// </summary>
        private bool _favoriteView;
        public bool FavorateView
        {
            get
            {
                return _favoriteView;
            }
            set
            {
                _favoriteView = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// データバインド用のリスト
        /// </summary>
        public ObservableCollection<Book> ViewList { get; set; }
        /// <summary>
        /// ボタンテキスト用
        /// </summary>
        private string _buttonText;
        public string ButtonText
        {
            get
            {
                return _buttonText;
            }
            set
            {
                _buttonText = value;
                NotifyPropertyChanged();
            }
        }
        private string _headerText;
        public string HeaderText
        {
            get
            {
                return _headerText;
            }
            set
            {
                _headerText = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// コンストラクター
        /// </summary>
        public ViewModel()
        {
            _bookList = WebScraping.GetWebData();
            _favoriteView = false;
            _headerText = "全表示";
            _buttonText = "お気に入り表示に切り替え";
            ViewList = new ObservableCollection<Book>(_bookList);
        }
    }
    public class Book
    {
        /// <summary>
        /// Bookデータ用クラス
        /// </summary>
        public string Title { get; set; }
        public string ReleaseDate { get; set; }
        public string Author { get; set; }
    }
    public class WebScraping
    {
        static HttpClient client = new HttpClient();

        public static List<Book> GetWebData()
        {
            var urlstring = @"http://yurinavi.com/yuri-calendar/";
            var document = default(IHtmlDocument);
            using (var stream = client.GetStreamAsync(urlstring).Result)
            {
                var parser = new HtmlParser();
                document = parser.ParseDocumentAsync(stream).Result;
            }
            var dates = document.QuerySelectorAll(@"td.column-1:not(#tablepress-152 > tbody > tr > td)");
            var books = document.QuerySelectorAll(@"td.column-3");
            var count = 0;
            var data = new List<Book>();
            foreach (var date in dates)
            {
                if (date.TextContent != "")
                {
                    if (date.TextContent.Substring(0, 1) != "▼")
                    {
                        var dateArray = date.TextContent.Split('\n');
                        var releaseDate = dateArray[0] + "(" + dateArray[1] + ")";
                        var bookData = books[count].TextContent.Split('\n');
                        if (bookData.Length == 2)
                        {
                            var title = bookData[0];
                            var author = bookData[1];
                            data.Add(new Book()
                            {
                                ReleaseDate = releaseDate,
                                Title = title,
                                Author = author
                            });
                            count++;
                        }
                        else
                        {
                            var title = bookData[0];
                            data.Add(new Book()
                            {
                                ReleaseDate = releaseDate,
                                Title = title,
                                Author = ""
                            });
                            count++;
                        }
                    }
                }
                else
                {
                    var bookData = books[count].TextContent.Split('\n');
                    if (bookData.Length == 2)
                    {
                        var title = bookData[0];
                        var author = bookData[1];
                        data.Add(new Book()
                        {
                            ReleaseDate = "",
                            Title = title,
                            Author = author
                        });
                        count++;
                    }
                    else
                    {
                        var title = bookData[0];
                        data.Add(new Book()
                        {
                            ReleaseDate = "",
                            Title = title,
                            Author = ""
                        });
                        count++;
                    }
                }
            }
            return data;
        }
    }
}
