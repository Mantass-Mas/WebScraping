using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

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
        public string Titile { get; set; }
        public string ReleaseDate { get; set; }
        public string Author { get; set; }
    }
}
