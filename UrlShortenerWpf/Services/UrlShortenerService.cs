using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerWpf.Cache;
using UrlShortenerWpf.Models;
using UrlShortenerWpf.Storage;

namespace UrlShortenerWpf.Services
{
    public class UrlShortenerService
    {
        private readonly InMemoryCache _cache;
        private readonly UrlRepository _repository;
        public UrlShortenerService(
            InMemoryCache cache,
            UrlRepository repository)
        {
            _cache = cache;
            _repository = repository;
        }

        //Create Short URL
        public string Shorten(string longUrl, TimeSpan?ttl = null)
        {
            string shortKey = ShortKeyGenerator.Generate();
            var mapping = new UrlMapping
            {
                ShortKey = shortKey,
                LongUrl = longUrl,
                CreatedAt = DateTime.UtcNow,
                ExpiryAt = ttl.HasValue?DateTime.UtcNow.Add(ttl.Value): null
            };

            _repository.Save(mapping);
            _cache.Set(shortKey, longUrl);

            return shortKey;
        }
        //Resolve short URL
        public string? Resolve(string shortKey)
        {
            //Cache first
            if (_cache.TryGet(shortKey, out var cachedUrl))
                return cachedUrl;
            //DB fallback
            var mapping = _repository.Get(shortKey);
            if (mapping == null)
                return null;
            //Expiry check
            if(mapping.ExpiryAt.HasValue &&
               mapping.ExpiryAt.Value < DateTime.UtcNow)
            {
                return null;
            }
            //Update cache
            _cache.Set(shortKey, mapping.LongUrl);
            return mapping.LongUrl;
        }
    }
}
