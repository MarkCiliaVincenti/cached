﻿namespace Cached.Caching
{
    using System;
    using System.Threading.Tasks;
    using Locking;

    /// <inheritdoc />
    public abstract class Cacher : ICacher
    {
        private readonly ILock _cacherLock;

        internal Cacher(ILock cacheLock)
        {
            _cacherLock = cacheLock ?? throw new ArgumentNullException(nameof(cacheLock));
        }

        /// <inheritdoc />
        public async Task<TResponse> GetOrFetchAsync<TResponse>(
            string key,
            Func<string, Task<TResponse>> fetchFactory)
        {
            if (fetchFactory == null)
            {
                throw new ArgumentNullException(nameof(fetchFactory));
            }

            var result = await TryGetFromCache<TResponse>(key).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return result.Value;
            }

            return await FetchAndAddToCache(key, fetchFactory);
        }

        private async Task<TResponse> FetchAndAddToCache<TResponse>(
            string key,
            Func<string, Task<TResponse>> fetchFactory)
        {
            using (await _cacherLock.LockAsync(key).ConfigureAwait(false))
            {
                var result = await TryGetFromCache<TResponse>(key).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return result.Value;
                }

                TResponse data = await fetchFactory(key).ConfigureAwait(false);
                await WriteToCache(key, data).ConfigureAwait(false);
                return data;
            }
        }

        /// <summary>
        ///     Writes the data to cache.
        /// </summary>
        /// <typeparam name="T">The type of item to be written.</typeparam>
        /// <param name="key">The cache key to be used for the data.</param>
        /// <param name="item">The item that is to be persisted in cache.</param>
        /// <returns></returns>
        protected abstract Task WriteToCache<T>(string key, T item);

        /// <summary>
        ///     Tries to retrieve data from cache based on the provided key.
        /// </summary>
        /// <typeparam name="T">The type of the item being searched for.</typeparam>
        /// <param name="key">The key to use when trying to locate data.</param>
        /// <returns>True if item is found, otherwise false.</returns>
        protected abstract Task<CacheResult<T>> TryGetFromCache<T>(string key);
    }
}