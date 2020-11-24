using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Controls;
using System.Data;
using System.Data.SQLite;
using System.Data.SQLite.Linq;

namespace Scraping
{
    /// <summary>
    /// ViewModelのベース
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// MainWindow用のViewModel
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// すべての本のリスト
        /// </summary>
        private List<Book> _bookList = new List<Book>();

        /// <summary>
        /// お気に入り登録した本のリスト
        /// </summary>
        private List<string> _favoriteTitles = new List<string>();

        /// <summary>
        /// 現在お気に入り表示かどうか
        /// </summary>
        private bool _favoriteView;
        public bool FavoriteView
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
        /// DataGridにバインドするリスト
        /// </summary>
        private List<Book> _viewList;
        public List<Book> ViewList
        {
            get
            {
                return _viewList;
            }
            set
            {
                _viewList = value;
                NotifyPropertyChanged();
            }
        }

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

        /// <summary>
        /// 現在の表示形式をViewに表示する用
        /// </summary>
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
        /// 表示方法変更ボタンクリック用のコマンド
        /// </summary>
        public ICommand ButtonClick { get; set; }

        /// <summary>
        /// コンストラクター
        /// </summary>
        public MainViewModel()
        {
            _bookList = WebScraping.GetWebData();
            ButtonClick = new ButtonClickCommand(this);
            FavoriteView = false;
            SetData();
        }

        /// <summary>
        /// Viewにデータをセットする
        /// </summary>
        public void SetData()
        {
            if (FavoriteView)
            {
                HeaderText = "お気に入り表示";
                ButtonText = "全表示に切り替え";
                var favoriteList = new List<Book>();
                foreach(var title in _favoriteTitles)
                {
                    foreach(var book in _bookList)
                    {
                        if (book.Title.Contains(title))
                        {
                            favoriteList.Add(book);
                        }
                    }
                }
                ReleaseDateSet(favoriteList);
                ViewList = new List<Book>(favoriteList);
            }
            else
            {
                HeaderText = "全表示";
                ButtonText = "お気に入り表示に切り替え";
                ReleaseDateSet(_bookList);
                ViewList = new List<Book>(_bookList);
            }
        }

        /// <summary>
        /// 登録確認ダイアログを出し、登録する場合は_favoriteListに追加する
        /// </summary>
        /// <param name="dataGrid">本のタイトル情報</param>
        public void AddFavorite(DataGrid dataGrid)
        {
            // お気に入り表示中は処理を実行しない
            if (FavoriteView)
            {
                return;
            }
            //選択されたセル(タイトル部分)からタイトル名のみを抽出する
            var cell = dataGrid.CurrentColumn.GetCellContent(dataGrid.CurrentItem) as TextBlock;
            var cell_text = cell.Text;
            cell_text = cell_text.Trim(' ', '(', ')');
            if(cell_text != "")
            {
                var end = cell_text[cell_text.Length - 1].ToString();
                //numには一応何巻かの情報が入るはず(使わないけど)
                var isNum = int.TryParse(end, out var num);
                while (isNum)
                {
                    cell_text = cell_text.Remove(cell_text.Length - 1, 1);
                    if (cell_text.Length <= 0)
                    {
                        cell_text = cell.Text;
                        break;
                    }
                    end = cell_text[cell_text.Length - 1].ToString();
                    isNum = int.TryParse(end, out num);
                }
                var title = cell_text.Trim(' ', '(', ')');
                var text = $"「{title}」\nをお気に入りに登録しますか？";
                var window = new RegisterDialog(text);
                bool? res = window.ShowDialog();
                //登録確認ダイアログで登録が押された場合はtrueが返ってくる
                if (res == true)
                {
                    if (!_favoriteTitles.Contains(title))
                    {
                        _favoriteTitles.Add(title);
                    }
                }
            }
        }

        /// <summary>
        /// ReleaseDateを表示したりしなかったり
        /// </summary>
        public void ReleaseDateSet(List<Book> books)
        {
            books = books.OrderByDescending(x => x.dateData).ToList();
            foreach (var book in books)
            {
                var releaseDates = books?.Select(x => x.ReleaseDate);
                if (!(releaseDates.Contains(book.dateData)))
                {
                    book.ReleaseDate = book.dateData;
                }
                else
                {
                    book.ReleaseDate = "";
                }
            }
        }

        public void DataBaseWrite()
        {
            var path = "favorite.db";
            using (var conn = new SQLiteConnection())
            {

            }
        }
    }

    /// <summary>
    /// 表示切替ボタン用のコマンド
    /// </summary>
    public class ButtonClickCommand : ICommand
    {
        private MainViewModel _vm;
        public event EventHandler CanExecuteChanged;
        public ButtonClickCommand(MainViewModel viewModel)
        {
            _vm = viewModel;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            if (_vm.FavoriteView)
            {
                _vm.FavoriteView = false;
            }
            else
            {
                _vm.FavoriteView = true;
            }
            _vm.SetData();
        }
    }

    /// <summary>
    /// Bookデータ用クラス
    /// </summary>
    public class Book
    {
        public string dateData;
        public string ReleaseDate { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public Book(string date, string title, string author = "")
        {
            dateData = date;
            ReleaseDate = "";
            Title = title;
            Author = author;
            //Console.WriteLine($"dateDate:{dateData}/ReleaseDate:{ReleaseDate}/Title:{Title}/Author:{Author}");
        }
    }

    /// <summary>
    /// データベース用クラス
    /// </summary>
    public class DataBase
    {
        /// <summary>
        /// プライマリーキー
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// タイトル名
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// 登録確認ダイアログ用のViewModel
    /// </summary>
    public class RegisterViewModel : ViewModelBase
    {
        /// <summary>
        /// ダイアログに表示するテキスト(本のタイトル等)
        /// </summary>
        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                NotifyPropertyChanged();
            }
        }
        public RegisterViewModel(string text)
        {
            Text = text;
        }
    }
}
