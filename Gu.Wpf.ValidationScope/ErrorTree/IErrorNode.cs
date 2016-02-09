using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Gu.Wpf.ValidationScope
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    public interface IErrorNode : IReadOnlyList<ValidationError>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
        bool HasErrors { get; }

        ReadOnlyObservableCollection<ValidationError> Errors { get; }

        ReadOnlyObservableCollection<IErrorNode> Children { get; }

        DependencyObject Source { get; }
    }
}