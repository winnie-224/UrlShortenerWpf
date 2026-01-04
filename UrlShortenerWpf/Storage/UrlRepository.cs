using Microsoft.Data.Sqlite;
using UrlShortenerWpf.Models;

namespace UrlShortenerWpf.Storage
{
    public class UrlRepository
    {
        private readonly ShardRouter _router;
        public UrlRepository(ShardRouter router)
        {
            _router = router;

        }
        public void Save(UrlMapping mapping)
        {
            string dbPath = _router.GetShardDbPath(mapping.ShortKey);
            using var connection = new SqliteConnection($"Data Source={dbPath}");
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
                INSERT OR REPLACE INTO Urls 
                VALUES ($shortKey, $longUrl, $createdAt, $expiryAt);
                 ";
            cmd.Parameters.AddWithValue("$shortKey", mapping.ShortKey);
            cmd.Parameters.AddWithValue("$longUrl", mapping.LongUrl);
            cmd.Parameters.AddWithValue("$createdAt", mapping.CreatedAt.ToString("o"));
            if (mapping.ExpiryAt.HasValue)
            {
                cmd.Parameters.AddWithValue(
                    "$expiryAt",
                    mapping.ExpiryAt.Value.ToString("o"));
            }
            else
            {
                cmd.Parameters.AddWithValue(
                    "$expiryAt",
                    DBNull.Value);
            }
            cmd.ExecuteNonQuery();

        }
        public UrlMapping? Get(string shortKey)
        {
            string dbPath = _router.GetShardDbPath(shortKey);
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText =
                @"
                SELECT ShortKey, LongUrl, CreatedAt, ExpiryAt
                FROM Urls
                WHERE ShortKey = $shortKey;
                 ";
            cmd.Parameters.AddWithValue("$shortKey", shortKey);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;
            return new UrlMapping
            {
                ShortKey = reader.GetString(0),
                LongUrl = reader.GetString(1),
                CreatedAt = DateTime.Parse(reader.GetString(2)),
                ExpiryAt = reader.IsDBNull(3) ? null : DateTime.Parse(reader.GetString(3))
            };
        }

        public Dictionary<string, List<UrlMapping>> GetAllShardData()
        {
            var result = new Dictionary<string, List<UrlMapping>>();
            for(int i = 0; i<_router.ShardCount; i++)
            {
                string dbPath = _router.GetShardDbPathByIndex(i);
                var list = new List<UrlMapping>();

                using var connection = new SqliteConnection($"Data Source={dbPath}");
                connection.Open();

                using var cmd = connection.CreateCommand();
                cmd.CommandText =
                    @"
                    SELECT shortKey, LongUrl, CreatedAt, ExpiryAt
                    FROM Urls;
                    ";
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new UrlMapping
                    {
                        ShortKey = reader.GetString(0),
                        LongUrl = reader.GetString(1),
                        CreatedAt = DateTime.Parse(reader.GetString(2)),
                        ExpiryAt = reader.IsDBNull(3) 
                        ? null 
                        : DateTime.Parse(reader.GetString(3))
                    });
                }
                result[$"shard_{i}"] = list;
            }
            return result;
        }
    }
}
