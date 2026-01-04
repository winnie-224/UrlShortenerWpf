using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
namespace UrlShortenerWpf.Storage
{
    public static class ShardInitializer
    {
        public static void InitializeShards(int shardCount)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string shardDir = Path.Combine(basePath, "Shards");
            //Ensure shards folder exists
            if(!Directory.Exists(shardDir))
            {
                Directory.CreateDirectory(shardDir);
            }
            //Create each shard DB+table
            for(int i = 0; i<shardCount; i++)
            {
                string dbPath = Path.Combine(shardDir, $"shard_{i}.db");
                using var connection =
                    new SqliteConnection($"Data Source={dbPath}");
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText =
                @"
                CREATE TABLE IF NOT EXISTS Urls(
                    ShortKey TEXT PRIMARY KEY,
                    LongUrl TEXT,
                    CreatedAt TEXT,
                    ExpiryAt TEXT
                 );
                ";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
