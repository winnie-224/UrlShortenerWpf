using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerWpf.Services;

namespace UrlShortenerWpf.Routing
{
    public class ServiceNode
    {
        public string NodeId { get; }
        public UrlShortenerService Service { get; }
        public bool IsHealthy { get; private set; } = true;
        public int RequestCount { get; private set; }
        public ServiceNode(string nodeId, UrlShortenerService service)
        {
            NodeId = nodeId;
            Service = service;
        }
        public void MarkDown()
        {
            IsHealthy = false;
        }
        public void MarkUp()
        {
            IsHealthy = true;
        }
        public T Handle<T>(Func<UrlShortenerService, T> action)
        {
            if (!IsHealthy)
                throw new InvalidOperationException($"Service node {NodeId} is down");
            RequestCount++;
            return action(Service);

        }

    }
}
