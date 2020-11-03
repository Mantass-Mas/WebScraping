using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;

namespace Scraping
{
    class MyBooks
    {
        public List<string> Titles { get; set; }

        public void Registered(string title)
        {
            if (!Titles.Contains(title))
            {
                Titles.Add(title);
            }
        }
        
        public void ReadData()
        {
            try
            {
                using (var reader = new StreamReader("MyBook.txt", Encoding.UTF8))
                {
                    //1行ずつ読み込む
                    var line = default(string);
                    while((line = reader.ReadLine()) != null)
                    {
                        Titles.Add(line);
                    }
                }
            }
            catch
            {
                var errorWindow = new ErrorWindow();
                errorWindow.Msg.Text = "MyBookファイルが見つかりませんでした";
            }
        }
        public void WriteData()
        {
            try
            {
                using (var writer = new StreamWriter("MyBook.txt", false, Encoding.UTF8))
                {
                    foreach(var line in Titles)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
            catch
            {
                var errorWindow = new ErrorWindow();
                errorWindow.Msg.Text = "MyBookファイルが見つかりませんでした";
            }
        }
    }
    class WebData
    {
        public string ReleaseDate { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
    }
    class AppData
    {
        public ObservableCollection<WebData> webData;
        public MyBooks myBooks;
        public AppData()
        {
            Setup();
        }
        public async void Setup()
        {
            // Webスクレイピングを非同期処理で行う
            var webScraping = new WebScraping();
            webData = await webScraping.GetWebDataAsync();

            // MyBook情報をtxtから取得
            myBooks = new MyBooks();
            myBooks.ReadData();
        }
    }
}
