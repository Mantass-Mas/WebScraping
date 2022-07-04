using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Models;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace UserControls
{
    public class ListItemViewModel : BindableBase, IDisposable
    {
        public ReadOnlyReactivePropertySlim<string> ReleaseDate { get; }

        public ReadOnlyReactivePropertySlim<string> Title { get; }

        public ReadOnlyReactivePropertySlim<string> Author { get; }

        public ReadOnlyReactivePropertySlim<string> Publisher { get; }

        private Book SourceBook { get; } = null;

        private CompositeDisposable disposable = new CompositeDisposable();

        private bool disposedValue;

        public ListItemViewModel(Book book)
        {
            this.SourceBook = book;
            this.SourceBook.AddTo(this.disposable);

            this.ReleaseDate = this.SourceBook.ReleaseDate.Where(v => v.HasValue).Select(v => v.Value.ToString("yyyy/MM/dd")).ToReadOnlyReactivePropertySlim().AddTo(this.disposable);

            this.Title = this.SourceBook.Title.ToReadOnlyReactivePropertySlim().AddTo(this.disposable);

            this.Author = this.SourceBook.Author.ToReadOnlyReactivePropertySlim().AddTo(this.disposable);

            this.Publisher = this.SourceBook.Publisher.ToReadOnlyReactivePropertySlim().AddTo(this.disposable);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~ListItemViewModel()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
