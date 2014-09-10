// -----------------------------------------------------------------------
// <copyright file="ICacheProvider.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CacheProvider
{
    using System;

    public interface ICacheProvider
    {
        T Get<T>(string cacheID, Func<T> getItemCallback) where T : class;
        void Clear();
    }
}
