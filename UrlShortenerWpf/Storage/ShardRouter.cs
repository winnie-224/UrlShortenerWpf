using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortenerWpf.Storage
{
    public class ShardRouter
    {
        private readonly int _shardCount;
        public ShardRouter(int shardCount)
        {
            _shardCount = shardCount;

        }

        public int ShardCount => _shardCount;
        public int GetShardIndex(string shortKey)
        {
            int hash = shortKey.GetHashCode();
            hash = Math.Abs(hash);
            return hash % _shardCount;
        }
        public string GetShardDbPath(string shortKey)
        {
            int index = GetShardIndex(shortKey);
            return GetShardDbPathByIndex(index);
        }
        public string GetShardDbPathByIndex(int index)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(basePath, "Shards",$"shard_{index}.db");
        }
    }
}
