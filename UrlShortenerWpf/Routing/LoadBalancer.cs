using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace UrlShortenerWpf.Routing
{
    public class LoadBalancer
    {
        private readonly List<ServiceNode> _nodes;
        private int _currentIndex = 0;
        private readonly object _lock = new();

        public LoadBalancer(IEnumerable<ServiceNode> nodes)
        {
            _nodes = nodes.ToList();
            if(_nodes.Count == 0)
            {
                throw new ArgumentException("LoadBalancer requires atleast one ServiceNode");
            }

        }
        //Routes a request to the next healthy service node (round-robin).
        public T Route<T>(Func<ServiceNode, T> action)
        {
            var node = GetNextHealthyNode();
            return action(node);
        }
        // Returns the next healthy node using round-robin strategy.
        private ServiceNode GetNextHealthyNode()
        {
            lock (_lock)
            {
                int checkedCount = 0;
                while (checkedCount < _nodes.Count)
                {
                    var node = _nodes[_currentIndex];
                    _currentIndex = (_currentIndex + 1) % _nodes.Count;
                    if (node.IsHealthy)
                        return node;
                    checkedCount++;
                }
                throw new InvalidOperationException("No healthy service nodes available");
            }
        }
        //Expose nodes for UI montioring
        public IReadOnlyList<ServiceNode> Nodes => _nodes.AsReadOnly();
    }
}
