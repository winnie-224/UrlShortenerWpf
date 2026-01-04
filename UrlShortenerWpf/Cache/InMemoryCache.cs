using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortenerWpf.Cache
{
    public class InMemoryCache
    {
        private readonly int _capacity;
        private readonly Dictionary<string, string> _cache;
        private readonly LinkedList<string> _lruList;
        private readonly object _lock = new();

        public int Hits { get; private set; }
        public int Misses { get; private set; }
        public string? LastEvictedKey { get; private set; }
        public InMemoryCache(int capacity)
        {
            _capacity = capacity;
            _cache = new Dictionary<string, string>();
            _lruList = new LinkedList<string>();
        }
        public bool TryGet(string key, out string value)
        {
            lock (_lock)
            {
                if(_cache.TryGetValue(key, out value))
                {
                    // Cache hit
                    Hits++;
                    _lruList.Remove(key);
                    _lruList.AddFirst(key);
                    return true;
                }
                //Cache miss
                Misses++;
                value = null;
                return false;

            }
        }

        public void Set(string key, string value)
        {
            lock (_lock)
            {
                LastEvictedKey = null;
                if (_cache.ContainsKey(key))
                {
                    _lruList.Remove(key);
                }
                else if (_cache.Count >= _capacity)
                {
                    //Evict least recently used item
                    string lruKey = _lruList.Last!.Value;
                    _lruList.RemoveLast();
                    _cache.Remove(lruKey);
                    LastEvictedKey = lruKey;
                }
                _cache[key] = value;
                _lruList.AddFirst(key);
            }
        }
        //Keys in cache from MRU to LRU
        public List<string> GetCacheKeysInOrder()
        {
            lock (_lock)
            {
                return _lruList.ToList();
            }
        }
    }
}
