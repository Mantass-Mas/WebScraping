using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraping.Model
{
    /// <summary>
    /// Bookデータ用クラス
    /// </summary>
    public class Book
    {
        public int id;
        public string dateData;
        public string ReleaseDate { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public Book(int num, string date, string title, string author = "")
        {
            id = num;
            dateData = date;
            ReleaseDate = "";
            Title = title;
            Author = author;
            Console.WriteLine($"id:{id}/dateDate:{dateData}/ReleaseDate:{ReleaseDate}/Title:{Title}/Author:{Author}");
        }
    }
}
