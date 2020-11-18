using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Scraping
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ViewModel : ViewModelBase
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
        /// お気に入り表示かどうか
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
        /// データバインド用のリスト
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
        public ViewModel()
        {
            _bookList = WebScraping.GetWebData();
            ButtonClick = new ButtonClickCommand(this);
            FavoriteView = false;
            SetData();
        }
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
    }
    public class ButtonClickCommand : ICommand
    {
        private ViewModel _vm;
        public event EventHandler CanExecuteChanged;
        public ButtonClickCommand(ViewModel viewModel)
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
    public class Book
    {
        /// <summary>
        /// Bookデータ用クラス
        /// </summary>
        public string Title { get; set; }
        public string ReleaseDate { get; set; }
        public string Author { get; set; }
    }
}
