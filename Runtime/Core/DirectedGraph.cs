using System.Collections.Generic;

namespace TraitBasedOpinionSystem.Core
{
    public class DirectedGraph<NData, EData>
    {
        protected class Node<T>
        {
            public readonly uint id;
            public readonly T data;
            public readonly HashSet<uint> incoming;
            public readonly HashSet<uint> outgoing;

            public Node(uint id, T data)
            {
                this.id = id;
                this.data = data;
                incoming = new HashSet<uint>();
                outgoing = new HashSet<uint>();
            }
        }

        protected readonly Dictionary<uint, Dictionary<uint, EData>> _connections;
        protected readonly Dictionary<uint, Node<NData>> _nodes;

        public DirectedGraph()
        {
            _nodes = new Dictionary<uint, Node<NData>>();
            _connections = new Dictionary<uint, Dictionary<uint, EData>>();
        }

        public void AddNode(uint id, NData nodeData)
        {
            if (!_nodes.ContainsKey(id))
            {
                _nodes.Add(id, new Node<NData>(id, nodeData));
            }

            _connections.Add(id, new Dictionary<uint, EData>());
        }

        public void RemoveNode(uint id)
        {
            if (!_nodes.ContainsKey(id))
            {
                return;
            }

            _connections.Remove(id);

            var node = _nodes[id];

            foreach (var neighbor in node.incoming)
            {
                _connections[neighbor].Remove(id);
            }

            _nodes.Remove(id);
        }

        public bool HasNode(uint id)
        {
            return _nodes.ContainsKey(id);
        }

        public NData GetNode(uint id)
        {
            return _nodes[id].data;
        }

        public void AddEdge(uint source, uint target, EData edgeData)
        {

            _connections[source][target] = edgeData;
        }

        public void RemoveEdge(uint source, uint target)
        {
            _connections[source].Remove(target);
        }

        public bool HasEdge(uint source, uint target)
        {
            return _connections[source].ContainsKey(target);
        }

        public EData GetEdge(uint source, uint target)
        {
            return _connections[source][target];
        }
    }
}


