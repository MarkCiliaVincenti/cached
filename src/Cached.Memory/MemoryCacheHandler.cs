﻿namespace Cached.Memory
{
    using Caching;
    using Locking;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Handles memory-caching using Cached.
    /// </summary>
    public static class MemoryCacheHandler
    {
        /// <summary>
        /// Creates a new CacheHandler, based on the provided MemoryCache instance.
        /// </summary>
        /// <param name="memoryCache">The MemoryCache instance that the handler should use.</param>
        /// <param name="options">(Optional) override default MemoryCache entry options.</param>
        /// <returns></returns>
        public static ICache<IMemory> New(IMemoryCache memoryCache, MemoryCacheEntryOptions options = null)
            => new CacheHandler<IMemory>(new MemoryCacheProvider(memoryCache, options), new KeyBasedLock());
    }
}
