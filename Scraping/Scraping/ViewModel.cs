﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Net.Http;

namespace Scraping
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string PropertyName)
        {
            var e = new PropertyChangedEventArgs(PropertyName);
            PropertyChanged?.Invoke(this, e);
        }
        /// <summary>
        /// すべての本のリスト
        /// </summary>
        public ObservableCollection<Book> BookList { get; set; } = new ObservableCollection<Book>();
        /// <summary>
        /// お気に入り登録した本のリスト
        /// </summary>
        public ObservableCollection<Book> FavoriteList { get; set; } = new ObservableCollection<Book>();
        /// <summary>
        /// お気に入り表示かどうか
        /// </summary>
        private bool _FavoriteView = false;
        public bool FavorateView
        {
            get
            {
                return _FavoriteView;
            }
            set
            {
                _FavoriteView = value;
                NotifyPropertyChanged(nameof(FavorateView));
            }
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

        public async Task<ObservableCollection<Book>> GetWebData()
        {
            var urlstring = @"http://yurinavi.com/yuri-calendar/";
            var document = default(IHtmlDocument);
            using (var stream = await client.GetStreamAsync(urlstring))
            {
                var parser = new HtmlParser();
                document = await parser.ParseDocumentAsync(stream);
            }
            var dates = document.QuerySelectorAll(@"td.column-1:not(#tablepress-152 > tbody > tr > td)");
            var books = document.QuerySelectorAll(@"td.column-3");
            var count = 0;
            var data = new ObservableCollection<Book>();
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