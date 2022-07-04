using System;
using Reactive.Bindings;
using System.Reactive.Disposables;
using Reactive.Bindings.Extensions;

namespace Models
{
    public class Book : IDisposable
    {

        public ReactivePropertySlim<DateTime?> ReleaseDate { get; }

        public ReactivePropertySlim<string> Title { get; }

        public ReactivePropertySlim<string> Author { get; }

        public ReactivePropertySlim<string> Publisher { get; }

        protected CompositeDisposable disposables = new CompositeDisposable();

        private bool disposedValue;

        public Book() : this(null, string.Empty, string.Empty, string.Empty)
        {
        }

        public Book(DateTime? releaseDate, string title, string author, string publisher)
        {
            this.ReleaseDate = new ReactivePropertySlim<DateTime?>(releaseDate).AddTo(this.disposables);
            this.Title = new ReactivePropertySlim<string>(title).AddTo(this.disposables);
            this.Author = new ReactivePropertySlim<string>(author).AddTo(this.disposables);
            this.Publisher = new ReactivePropertySlim<string>(publisher).AddTo(this.disposables);

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Managed Object
                    this.disposables.Dispose();
                }

                // Unmanaged Objectがある場合はここに記述
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
