﻿using System;
using System.Collections.Generic;
using RepoDb.Interfaces;
using System.Linq;
using System.Collections;

namespace RepoDb
{
    /// <summary>
    /// An object used for caching a result in the repository <i>Query</i> operation. This object is the default
    /// memory cache object used by the <i>RepoDb</i> repositories.
    /// </summary>
    public class MemoryCache : ICache
    {
        private static object _syncLock = new object();
        private readonly IList<CacheItem> _cacheList;

        /// <summary>
        /// Creates a new instance <i>RepoDb.MemoryCache</i> object.
        /// </summary>
        public MemoryCache() : this(180) { }

        /// <summary>
        /// Creates a new instance <i>RepoDb.MemoryCache</i> object.
        /// </summary>
        /// <param name="expirationInMinutes">An expiration in minutes of the cache item.</param>
        public MemoryCache(int expirationInMinutes)
        {
            if (expirationInMinutes < 0)
            {
                throw new ArgumentOutOfRangeException("Expiration in minutes.");
            }
            _cacheList = new List<CacheItem>();
            ExpirationInMinutes = expirationInMinutes;
        }

        /// <summary>
        /// Gets the cache item expiration in minutes.
        /// </summary>
        public int ExpirationInMinutes { get; set; }

        /// <summary>
        /// Adds a cache item value.
        /// </summary>
        /// <param name="key">The key to the cache.</param>
        /// <param name="value">The value of the cache.</param>
        public void Add(string key, object value)
        {
            Add(new CacheItem(key, value));
        }

        /// <summary>
        /// Adds a cache item value.
        /// </summary>
        /// <param name="item">
        /// The cache item to be added in the collection. This object must implement the <i>RepoDb.Interfaces.ICacheItem</i> interface.
        /// </param>
        public void Add(CacheItem item)
        {
            lock (_syncLock)
            {
                var cacheItem = GetItem(item.Key);
                if (cacheItem == null)
                {
                    _cacheList.Add(item);
                }
                else
                {
                    if (!IsExpired(cacheItem))
                    {
                        throw new InvalidOperationException($"An existing cache for key '{item.Key}' already exists.");
                    }
                    else
                    {
                        cacheItem.Value = item.Value;
                        cacheItem.Timestamp = DateTime.UtcNow;
                    }
                }
            }
        }

        /// <summary>
        /// Clears the collection of the cache.
        /// </summary>
        public void Clear()
        {
            _cacheList.Clear();
        }

        /// <summary>
        /// Checks whether the key is present in the collection.
        /// </summary>
        /// <param name="key">The name of the key to be checked.</param>
        /// <returns>A boolean value that signifies the presence of the key from the collection.</returns>
        public bool Contains(string key)
        {
            var cacheItem = GetItem(key);
            return cacheItem != null && !IsExpired(cacheItem);
        }

        /// <summary>
        /// Gets an object from the cache collection.
        /// </summary>
        /// <param name="key">The key of the cache object to be retrieved.</param>
        /// <returns>An object from the cache collection based on the given key.</returns>
        public object Get(string key)
        {
            var cacheItem = GetItem(key);
            if (cacheItem != null && !IsExpired(cacheItem))
            {
                return cacheItem.Value;
            }
            return null;
        }

        /// <summary>
        /// Gets the enumerator of the cache collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CacheItem> GetEnumerator()
        {
            return _cacheList
                .Where(cacheItem => !IsExpired(cacheItem))
                .GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator of the cache collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _cacheList.GetEnumerator();
        }

        /// <summary>
        /// Removes the item from the cache collection.
        /// </summary>
        /// <param name="key">
        /// The key of the item to be removed from the cache collection. If the given key is not present, this method will ignore it.
        /// </param>
        public void Remove(string key)
        {
            var item = GetItem(key);
            if (item != null)
            {
                _cacheList.Remove(item);
            }
            else
            {
                throw new InvalidOperationException($"The cache item with '{key}' is not found.");
            }
        }

        private CacheItem GetItem(string key)
        {
            return (CacheItem)_cacheList.FirstOrDefault(cacheItem =>
                string.Equals(cacheItem.Key, key, StringComparison.CurrentCulture));
        }

        private bool IsExpired(ICacheItem item)
        {
            return (DateTime.UtcNow - item.Timestamp).TotalMinutes >= ExpirationInMinutes;
        }
    }
}
