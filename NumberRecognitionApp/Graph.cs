using System.Collections.Generic;

namespace NumberRecognitionApp
{
    public class Vertex<T>
    {
        public int index;
        public T data;
    }
    public class Graph<T>
    {
        public List<Vertex<T>> Vertices { get; set; }
        public bool[,] Edges { get; set; }

        public Graph(int graphSize)
        {
            Edges = new bool[graphSize, graphSize];
            Vertices = new List<Vertex<T>>();
        }
        
        public void AddVertex(T value)
        {
            Vertex<T> vertex = new Vertex<T>();
            vertex.index = Vertices.Count;
            vertex.data = value;

            Vertices.Add(vertex);
        }

        public bool HasEdge(int u, int v)
        { 
            return Edges[u, v];
        }

        public List<int> GetNeighbors(int v)
        {
            List<int> neighbors = new List<int>();
            for (int i = 0; i < Vertices.Count; i++)
            {
                if (Edges[v,i] == true) neighbors.Add(i);
            }

            return neighbors;
        }
    }
}
