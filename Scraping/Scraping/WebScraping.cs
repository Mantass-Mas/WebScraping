﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace Scraping
{
    class WebScraping
    {
        static HttpClient client = new HttpClient();

        public async Task<string> GetWebDataAsync()
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
            var text = "";
            var count = 0;
            foreach(var date in dates)
            {
                if(date.TextContent != "")
                {
                    if (date.TextContent.Substring(0, 1) != "▼")
                    {
                        var dateArray = date.TextContent.Split('\n');
                        var bookData = books[count].TextContent.Split('\n');
                        if (bookData.Length == 2)
                        {
                            var title = bookData[0];
                            var author = bookData[1];
                            text += $"{dateArray[0]}({dateArray[1]})\nタイトル:{title}/著者:{author}\n";
                            count++;
                        }
                        else
                        {
                            var title = bookData[0];
                            text += $"{dateArray[0]}({dateArray[1]})\nタイトル:{title}\n";
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
                        text += $"タイトル:{title}/著者:{author}\n";
                        count++;
                    }
                    else
                    {
                        var title = bookData[0];
                        text += $"タイトル:{title}\n";
                        count++;
                    }
                }
            }
            return text;
        }
    }

    class WebData
    {
        public string _ReleaseDate { get; set; }
        public string _Title { get; set; }
        public string _Author { get; set; }
    }
}
