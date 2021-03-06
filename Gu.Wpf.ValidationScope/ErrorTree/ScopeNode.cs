namespace Gu.Wpf.ValidationScope
{
    using System;
    using System.Diagnostics;
    using System.Windows;

    [DebuggerDisplay("ScopeNode Errors: {Errors?.Count ?? 0}, Children: {Children.Count}, Source: {Source}")]
    public sealed class ScopeNode : ErrorNode
    {
        private readonly WeakReference<DependencyObject> sourceReference;

        public ScopeNode(DependencyObject source)
        {
            this.sourceReference = new WeakReference<DependencyObject>(source);
        }

        public override DependencyObject Source
        {
            get
            {
                DependencyObject source;
                this.sourceReference.TryGetTarget(out source);
                return source;
            }
        }
    }
}