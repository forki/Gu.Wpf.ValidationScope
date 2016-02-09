namespace Gu.Wpf.ValidationScope
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;

    using JetBrains.Annotations;

    public abstract class Node : IErrorNode
    {
        protected static readonly IReadOnlyList<ValidationError> EmptyValidationErrors = new ValidationError[0];
        private readonly Lazy<ObservableCollection<IErrorNode>> lazyChildren = new Lazy<ObservableCollection<IErrorNode>>(() => new ObservableCollection<IErrorNode>());
        private ReadOnlyObservableCollection<IErrorNode> children;
        private ReadOnlyObservableCollection<ValidationError> errors;
        private bool hasErrors;
        private bool disposed;

        protected Node(bool hasErrors)
        {
            this.hasErrors = hasErrors;
        }

        protected Node(IErrorNode child)
        {
            this.lazyChildren.Value.Add(child);
            this.hasErrors = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { ((INotifyCollectionChanged)this.Errors).CollectionChanged += value; }
            remove { ((INotifyCollectionChanged)this.Errors).CollectionChanged -= value; }
        }

        public ReadOnlyObservableCollection<IErrorNode> Children => this.children ?? (this.children = new ReadOnlyObservableCollection<IErrorNode>(this.lazyChildren.Value));

        public virtual bool HasErrors
        {
            get
            {
                return this.hasErrors;
            }
            protected set
            {
                if (value == this.hasErrors)
                {
                    return;
                }

                this.hasErrors = value;
                this.OnPropertyChanged();
                this.OnHasErrorsChanged();
            }
        }

        public ReadOnlyObservableCollection<ValidationError> Errors
        {
            get
            {
                if (this.errors == null)
                {
                    this.errors = new ReadOnlyObservableCollection<ValidationError>(this.ErrorCollection);
                    ((INotifyPropertyChanged)this.errors).PropertyChanged += this.OnErrorsPropertyChanged;
                }

                return this.errors;
            }
        }

        int IReadOnlyCollection<ValidationError>.Count => this.Errors.Count;

        public abstract DependencyObject Source { get; }

        ValidationError IReadOnlyList<ValidationError>.this[int index] => this.Errors[index];

        protected internal ErrorCollection ErrorCollection { get; } = new ErrorCollection();

        protected IEnumerable<Node> AllChildren
        {
            get
            {
                if (!this.lazyChildren.IsValueCreated)
                {
                    yield break;
                }

                foreach (Node errorNode in this.lazyChildren.Value)
                {
                    yield return errorNode;
                    foreach (var child in errorNode.AllChildren)
                    {
                        yield return child;
                    }
                }
            }
        }

        IEnumerator<ValidationError> IEnumerable<ValidationError>.GetEnumerator() => this.Errors.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.Errors.GetEnumerator();

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.Dispose(true);
        }

        internal void RemoveChild(IErrorNode errorNode)
        {
            if (this.lazyChildren.IsValueCreated == false)
            {
                return;
            }

            if (this.lazyChildren.Value.Remove(errorNode))
            {
                this.OnChildrenChanged();
            }
        }

        internal void AddChild(IErrorNode errorNode)
        {
            var editableChildren = this.lazyChildren.Value;
            if (editableChildren.Contains(errorNode))
            {
                return;
            }

            editableChildren.Add(errorNode);
            this.HasErrors = true;
            this.OnChildrenChanged();
        }

        protected internal abstract IReadOnlyList<ValidationError> GetAllErrors();

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.errors != null)
                {
                    ((INotifyPropertyChanged)this.errors).PropertyChanged -= this.OnErrorsPropertyChanged;
                }
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }

        protected abstract void OnChildrenChanged();

        protected virtual void OnHasErrorsChanged()
        {
        }

        private void OnErrorsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e);
        }
    }
}