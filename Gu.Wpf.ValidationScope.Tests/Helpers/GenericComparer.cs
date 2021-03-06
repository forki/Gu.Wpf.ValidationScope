﻿namespace Gu.Wpf.ValidationScope.Tests
{
    using System.Collections;

    internal abstract class GenericComparer<T> : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null || y == null)
            {
                return -1;
            }

            if (x is T && y is T)
            {
                return this.Compare((T)x, (T)y);
            }

            return -1;
        }

        protected abstract int Compare(T x, T y);
    }
}