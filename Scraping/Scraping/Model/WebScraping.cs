using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Net.Http;
using Scraping.View;

namespace Scraping.Model
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
            try
            {
                using (var stream = client.GetStreamAsync(urlstring).Result)
                {
                    var parser = new HtmlParser();
                    document = parser.ParseDocumentAsync(stream).Result;
                }
            }
            catch
            {
                Console.WriteLine("GetWebData()でError");
                var window = new ErrorDialog("インターネット接続でエラーが発生しました。\nインターネット接続を確認してください。");
                bool? res = window.ShowDialog();
                Environment.Exit(1);
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
                            data.Add(new Book(count, releaseDate, title, author));
                            count++;
                        }
                        else
                        {
                            var title = bookData[0];
                            data.Add(new Book(count, releaseDate, title));
                            count++;
                        }
                    }
                }
                else
                {
                    var bookData = books[count].TextContent.Split('\n');
                    if (bookData.Length == 2)
                    {
                        var releaseDate = "";
                        var index = data.Count - 1;
                        while(!(releaseDate != "" || index < 0))
                        {
                            releaseDate = data[index].dateData;
                            index--;
                        }
                        var title = bookData[0];
                        var author = bookData[1];
                        data.Add(new Book(count, releaseDate, title, author));
                        count++;
                    }
                    else
                    {
                        var releaseDate = "";
                        var index = data.Count - 1;
                        while (!(releaseDate != "" || index < 0))
                        {
                            releaseDate = data[index].dateData;
                            index--;
                        }
                        var title = bookData[0];
                        data.Add(new Book(count, releaseDate, title));
                        count++;
                    }
                }
            }
            return data;
        }
    }
}
