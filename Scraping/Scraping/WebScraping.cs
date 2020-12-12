using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Collections.ObjectModel;

namespace Scraping
{
    class WebScraping
    {
        static HttpClient client = new HttpClient();

        public async Task<ObservableCollection<WebData>> GetWebDataAsync()
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
            var data = new ObservableCollection<WebData>();
            foreach(var date in dates)
            {
                if(date.TextContent != "")
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
                            data.Add(new WebData()
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
                            data.Add(new WebData()
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
                    if(bookData.Length == 2)
                    {
                        var title = bookData[0];
                        var author = bookData[1];
                        data.Add(new WebData()
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
                        data.Add(new WebData()
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

    class WebData
    {
        public string ReleaseDate { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
    }
}
