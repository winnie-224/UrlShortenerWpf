using System.Configuration;
using System.Data;
using System.Windows;
using UrlShortenerWpf.Cache;
using UrlShortenerWpf.Routing;
using UrlShortenerWpf.Services;
using UrlShortenerWpf.Storage;

namespace UrlShortenerWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //System constants
            const int SHARD_COUNT = 3;
            const int CACHE_CAPACITY = 3;
            const int SERVICE_NODE_COUNT = 3;
            //Initialize shards
            ShardInitializer.InitializeShards(SHARD_COUNT);
            //Shared components
            var cache = new InMemoryCache(CACHE_CAPACITY);
            var shardRouter = new ShardRouter(SHARD_COUNT);
            var repository = new UrlRepository(shardRouter);
            //Create service nodes
            var nodes = new List<ServiceNode>();
            for(int i = 0; i<SERVICE_NODE_COUNT; i++)
            {
                var service = new UrlShortenerService(cache, repository);
                var node = new ServiceNode($"Node-{i + 1}", service);
                nodes.Add(node);
            }
            //Load balancer
            var loadBalancer = new LoadBalancer(nodes);
            //Launch UI
            var mainWindow = new MainWindow(loadBalancer, cache, repository);
            mainWindow.Show();
        }
    }

}
