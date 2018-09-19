using System;

namespace ERecruitment.Core.Caching
{
    /// <summary>
    ///     Extensions
    /// </summary>
    public static class CacheExtensions
    {
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, 60, acquire);
        }


        /// <summary>
        ///     Returns the value stored in the cache manager.
        /// </summary>
        /// <typeparam name="T">The value to return of type T</typeparam>
        /// <param name="cacheManager">The cache manager</param>
        /// <param name="key">The key to use in getting the value</param>
        /// <param name="cacheTime">The amount of time to store the value in cache. Time is in minutes.</param>
        /// <param name="acquire">The function to call in other to get the value in case the value is not in memory.</param>
        /// <returns>The value in cache.</returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
        {
            if (string.IsNullOrWhiteSpace(key) || acquire == null)
                return default(T);

            //convert the key and cacheTime so as to create a new key with it.
            //var newKey = ConvertCacheTimeAndKey(key, cacheTime);

            //Check wether the object is still in cache.
            if (cacheManager.IsSet(key))
            {
                return cacheManager.Get<T>(key);
            }

            T result = acquire();
            //if (result != null)
            cacheManager.Set(key, result, cacheTime);
            return result;
        }

        private static bool IsStillInCache(ICacheManager cacheManager, string newKey)
        {
            if (string.IsNullOrWhiteSpace(newKey)) return false;
            int index = newKey.IndexOf("**", StringComparison.InvariantCulture);

            if (index > 0)
            {
                string cacheValue = newKey.Substring(index + 2).Trim();
                DateTime newTime = DateTime.Parse(cacheValue);
                DateTime presentDateTime = DateTime.Now;
                bool isInCache = cacheManager.IsSet(newKey);

                if (isInCache && (newTime > presentDateTime))
                    return true;

                if (isInCache)
                    cacheManager.Remove(newKey);
            }
            return false;
        }

        private static string ConvertCacheTimeAndKey(string key, int cacheTime)
        {
            string presentCacheTime = DateTime.Now.AddMinutes(cacheTime).ToString();
            return string.Format("{0}**{1}", key, presentCacheTime);
        }
    }
}