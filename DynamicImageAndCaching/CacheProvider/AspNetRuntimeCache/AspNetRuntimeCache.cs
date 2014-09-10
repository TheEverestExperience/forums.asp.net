// -----------------------------------------------------------------------
// <copyright file="AspNetRuntimeCache.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CacheProvider.AspNetRuntimeCache
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;


    ///<remarks>
    /// http://stackoverflow.com/questions/39112/what-is-the-best-way-to-lock-cache-in-asp-net
    /// http://stackoverflow.com/questions/14047619/asp-net-caching-limit
    /// </remarks>

    /// <summary>
    /// AspNet Runtime Caching
    /// </summary>
    public class AspNetRuntimeCache : ICacheProvider
    {
        public int MinutesToAbsoluteExpiration = 100;
        private object _SyncLock = new object();

        #region messages

        private const string MsgExceptionWebApplicationExpected = "Web Application Expected";
        #endregion

        public T Get<T>(string cacheId, Func<T> getItemCallback) where T : class
        {
            if (HttpContext.Current == null)
            {
                throw new ArgumentNullException(MsgExceptionWebApplicationExpected);
            }

            var cache = HttpRuntime.Cache;
            var cachedItem = cache[cacheId] as T;
            if (cachedItem == null)
            {
                lock (_SyncLock)
                {
                    if (cache[cacheId] as T == null)
                    {
                        cachedItem = getItemCallback();
                        SetCache(cacheId, cachedItem);
                    }
                }
            }

            return cachedItem;
        }

        private void SetCache<T>(string cacheId, T itemToCache)
        {
            HttpRuntime.Cache.Insert(
                cacheId,
                itemToCache,
                null,
                DateTime.Now.AddMinutes(MinutesToAbsoluteExpiration),
                System.Web.Caching.Cache.NoSlidingExpiration);              
        }

        public void Clear()
        {
            IDictionaryEnumerator enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
                HttpRuntime.Cache.Remove(enumerator.Key.ToString());
        }
    }
}
