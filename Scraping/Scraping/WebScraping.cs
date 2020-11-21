using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Net.Http;

namespace Scraping
{
    /// <summary>
    /// Webスクレイピング用の静的クラス
    /// </summary>
    static class WebScraping
    {
        static HttpClient client = new HttpClient();

        /// <summary>
        /// サイトから新刊情報を取得し、Book型のリストとして返す(同期処理)
        /// </summary>
        /// <returns></returns>
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
