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
        private List<Book> _favoriteList = new List<Book>();

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
                ViewList = new List<Book>(_favoriteList);
            }
            else
            {
                HeaderText = "全表示";
                ButtonText = "お気に入り表示に切り替え";
                ViewList = new List<Book>(_bookList);
            }
        }

        /// <summary>
        /// 登録確認ダイアログを出し、登録する場合は_favoriteListに追加する
        /// </summary>
        /// <param name="dataGrid">本のタイトル情報</param>
        public void AddFavorite(DataGrid dataGrid)
        {
            //選択されたセル(タイトル部分)からタイトル名のみを抽出する
            var cell = dataGrid.CurrentColumn.GetCellContent(dataGrid.CurrentItem) as TextBlock;
            var cell_text = cell.Text;
            cell_text = cell_text.Trim(' ', '(', ')');
            var end = cell_text[cell_text.Length - 1].ToString();
            //numには一応何巻かの情報が入るはず(使わないけど)
            var isNum = int.TryParse(end, out var num);
            while (isNum)
            {
                cell_text = cell_text.Remove(cell_text.Length - 1, 1);
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
        public string Title { get; set; }
        public string ReleaseDate { get; set; }
        public string Author { get; set; }
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
