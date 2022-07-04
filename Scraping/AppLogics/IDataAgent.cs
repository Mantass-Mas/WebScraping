using System;
using System.Collections.Generic;
using System.Text;
using Models;
using Reactive.Bindings;
using System.Collections.ObjectModel;

namespace AppLogics
{
    class IDataAgent
    {
        public Book Book { get; }
        public ObservableCollection<Book> Books { get; }
    }
}
