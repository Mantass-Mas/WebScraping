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
using Scraping.View;
using Scraping.Model;

namespace Scraping.ViewModel
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
        private List<Data> _favoriteData = new List<Data>();

        /// <summary>
        /// データベース管理
        /// </summary>
        private DataBaseManager _dataBaseManager;
        public DataBaseManager DBManager
        {
            get
            {
                return _dataBaseManager;
            }
        }

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
        /// 表示方法変更ボタンクリック時のコマンド
        /// </summary>
        public ICommand ViewChange { get; private set; }

        /// <summary>
        /// 全削除ボタンクリック時のコマンド
        /// </summary>
        public ICommand Delete { get; private set; }

        /// <summary>
        /// コンストラクター
        /// </summary>
        public MainViewModel()
        {
            _bookList = WebScraping.GetWebData();
            ViewChange = new ViewChangeCommand(this);
            Delete = new DeleteCommand(this);
            FavoriteView = false;
            _dataBaseManager = new DataBaseManager();
            _favoriteData = DBManager.DataBaseRead();
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
                foreach(var data in _favoriteData)
                {
                    foreach(var book in _bookList)
                    {
                        if (book.Title.Contains(data.Title))
                        {
                            favoriteList.Add(book);
                        }
                    }
                }
                favoriteList = ReleaseDateSet(favoriteList);
                ViewList = new List<Book>(favoriteList);
            }
            else
            {
                HeaderText = "全表示";
                ButtonText = "お気に入り表示に切り替え";
                _bookList = ReleaseDateSet(_bookList);
                ViewList = new List<Book>(_bookList);
            }
        }

        /// <summary>
        /// dataGridのタイトルを選択した際の処理
        /// 全表示時：お気に入り登録の処理
        /// お気に入り表示時：お気に入り削除の処理
        /// </summary>
        /// <param name="dataGrid">本のタイトル情報</param>
        public void FavoriteOperation(DataGrid dataGrid)
        {
            // 選択されたセル(タイトル部分)からタイトル名のみを抽出する
            var cell = dataGrid.CurrentColumn.GetCellContent(dataGrid.CurrentItem) as TextBlock;
            if(cell == null)
            {
                return;
            }
            var cell_text = cell.Text;
            cell_text = cell_text.Trim(' ', '(', ')');
            if (cell_text != "")
            {
                var end = cell_text[cell_text.Length - 1].ToString();
                var isNum = int.TryParse(end, out int num);
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
            }
            var title = cell_text.Trim(' ', '(', ')');

            /// お気に入り表示なら削除、全表示なら登録
            if (FavoriteView)
            {
                var window = new DeleteDialog(title);
                bool? res = window.ShowDialog();
                // 削除確認ダイアログで登録が押された場合のみtrueが返ってくる
                if(res == true)
                {
                    foreach(var data in _favoriteData)
                    {
                        if(data.Title == title)
                        {
                            DBManager.RemoveAt(data.Id);
                            _favoriteData.Remove(data);
                            SetData();
                            break;
                        }
                    }
                }
            }
            else
            {
                var window = new RegisterDialog(title);
                bool? res = window.ShowDialog();
                // 登録確認ダイアログで登録が押された場合のみtrueが返ってくる
                if (res == true)
                {
                    var favoriteTitles = _favoriteData.Select(x => x.Title);
                    if (!favoriteTitles.Contains(title))
                    {
                        var id = 0;
                        if (_favoriteData.Count == 0)
                        {
                            id = 1;
                        }
                        else
                        {
                            var lastId = _favoriteData.Last().Id;
                            var checkNum = 1;
                            foreach (var f in _favoriteData)
                            {
                                if (f.Id != checkNum)
                                {
                                    id = checkNum;
                                    break;
                                }
                                else
                                {
                                    checkNum++;
                                }
                            }
                            if (id == 0)
                            {
                                id = lastId + 1;
                            }
                        }
                        var data = new Data()
                        {
                            Id = id,
                            Title = title,
                        };
                        _favoriteData.Add(data);
                    }
                }
            }
        }

        /// <summary>
        /// 登録済みリストの全削除用
        /// </summary>
        public void AllDeleteFavorite()
        {
            _favoriteData.Clear();
            SetData();
        }

        /// <summary>
        /// ReleaseDateを表示したりしなかったり
        /// </summary>
        public List<Book> ReleaseDateSet(List<Book> books)
        {
            var sortedbooks = books.OrderBy(x => x.id).ToList();
            var count = 0;
            foreach (var book in sortedbooks)
            {
                var checkList = sortedbooks?.Take(count).Select(x => x.ReleaseDate);
                if (!(checkList.Contains(book.dateData)))
                {
                    book.ReleaseDate = book.dateData;
                }
                else
                {
                    book.ReleaseDate = "";
                }
                count++;
            }
            return sortedbooks;
        }

        /// <summary>
        /// 検索用
        /// </summary>
        /// <param name="searchType">タイトルか著者か</param>
        /// <param name="searchText">検索文字列</param>
        public void Search(string searchType, string searchText)
        {
            var resultList = new List<Book>();
            if(searchType == "タイトル名")
            {
                foreach(var book in ViewList)
                {
                    if (book.Title.Contains(searchText))
                    {
                        resultList.Add(book);
                    }
                }
            }
            else if(searchType == "著者名")
            {
                foreach (var book in ViewList)
                {
                    if (book.Author.Contains(searchText))
                    {
                        resultList.Add(book);
                    }
                }
            }
            resultList = ReleaseDateSet(resultList);
            ViewList = resultList;
        }

        /// <summary>
        /// 終了時データベース書き込み
        /// </summary>
        public void Save()
        {
            DBManager.DataBaseWrite(_favoriteData);
        }
    }

    /// <summary>
    /// 表示切替ボタン用のコマンド
    /// </summary>
    public class ViewChangeCommand : ICommand
    {
        private MainViewModel _vm;
        public event EventHandler CanExecuteChanged;
        public ViewChangeCommand(MainViewModel viewModel)
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
    /// データベース全削除ボタン用コマンド
    /// </summary>
    public class DeleteCommand : ICommand
    {
        private MainViewModel _vm;
        public event EventHandler CanExecuteChanged;
        public DeleteCommand(MainViewModel viewModel)
        {
            _vm = viewModel;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            var title = "すべてのタイトル";
            var window = new DeleteDialog(title);
            bool? res = window.ShowDialog();
            // 削除確認ダイアログで登録が押された場合のみtrueが返ってくる
            if (res == true)
            {
                _vm.DBManager.RemoveAll();
                _vm.AllDeleteFavorite();
            }
        }
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
        public RegisterViewModel(string title)
        {
            Text = $"「{title}」\nをお気に入りに登録しますか？";
        }
    }

    /// <summary>
    /// お気に入り削除確認ダイアログ用のViewModel
    /// </summary>
    public class DeleteViewModel : ViewModelBase
    {
        /// <summary>
        /// ダイアログに表示するテキスト
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
        public DeleteViewModel(string title)
        {
            Text = $"「{title}」\nをお気に入りから削除しますか？";
        }
    }
}
