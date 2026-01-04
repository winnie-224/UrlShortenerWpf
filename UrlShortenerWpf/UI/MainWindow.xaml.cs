using System.Windows;
using UrlShortenerWpf.Cache;
using UrlShortenerWpf.Routing;
using UrlShortenerWpf.Storage;

namespace UrlShortenerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly LoadBalancer _loadBalancer;
        private readonly InMemoryCache _cache;
        private readonly UrlRepository _repository;

        public MainWindow(
            LoadBalancer loadBalancer, 
            InMemoryCache cache,
            UrlRepository repository)
        {
            InitializeComponent();
            _loadBalancer = loadBalancer;
            _cache = cache;
            _repository = repository;

            RefreshMetrics();
        }
        private void OnShortenClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LongUrlBox.Text))
                return;
            string shortKey = _loadBalancer.Route(node =>
            node.Handle(service=>service.Shorten(LongUrlBox.Text))
            );
            ShortUrlResult.Text = $"Short key: {shortKey}";
            RefreshMetrics();
        }
        private void OnResolveClicked(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(ShortKeyBox.Text))
                return;
            string? url = _loadBalancer.Route(node =>
            node.Handle(service=>service.Resolve(ShortKeyBox.Text))
            );
            ShortUrlResult.Text = url != null ? $"Long URL: {url}" : "Short key not found";
            RefreshMetrics();
        }
        private void RefreshMetrics()
        {
            //Cache counters
            CacheHitsText.Text = $"Cache Hits: {_cache.Hits}";
            CacheMissesText.Text = $"Cache Misses: {_cache.Misses}";
            //Cache contents LRU order
            CacheItems.Items.Clear();
            foreach(var key in _cache.GetCacheKeysInOrder())
            {
                CacheItems.Items.Add(key);
            }
            //Eviction indicator
            EvictionText.Text = _cache.LastEvictedKey != null
                ? $"Evicted:{_cache.LastEvictedKey}"
                : "";
            //Node metrics
            NodeMetrics.Items.Clear();
            foreach (var node in _loadBalancer.Nodes)
            {
                NodeMetrics.Items.Add(
                            $"{node.NodeId} – Requests: {node.RequestCount}"
                );

            }
            //Shard data visualization
            ShardDataView.Items.Clear();

            var shardData = _repository.GetAllShardData();
            foreach (var shard in shardData)
            {
                ShardDataView.Items.Add(
                    $"{shard.Key}:"
                );
                foreach (var mapping in shard.Value)
                {
                    ShardDataView.Items.Add(
                        $"{mapping.ShortKey}->{mapping.LongUrl}"
                        );
                }
            }
        }
    }
}