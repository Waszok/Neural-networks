using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberRecognitionApp
{
    public class Kohonen
    {
        //int _numIterations;
        float _alpha;
        Graph<Tuple<float, float, float>> _graph;

        public Kohonen()
        {
            //_numIterations = numIterations;
            _alpha = 1;
        }
        public float GetDistance(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            return (float) Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));
        }

        public float GetColor(System.Drawing.Bitmap bitmap, int color, int x, int y)
        {
            if (color == 1)
                return bitmap.GetPixel(x, y).R;
            else if (color == 2)
                return bitmap.GetPixel(x, y).G;
            else if (color == 3)
                return bitmap.GetPixel(x, y).B;
            return 0f;
        }

        public void LearnKohonen(Graph<Tuple<float, float, float>> graph, System.Drawing.Bitmap bitmap, double canvasWidth, double canvasHeight, int iterations)
        {
            Random r = new Random();
            for (int k = 0; k < iterations; k++)
            {
                int i = r.Next((int)canvasWidth);
                int j = r.Next((int)canvasHeight);


                //searching the vertex with the smallest distance between pixel color and vertex
                int minVertex = 0;
                float distance = GetDistance(graph.Vertices[0].data.Item1, graph.Vertices[0].data.Item2, graph.Vertices[0].data.Item3, GetColor(bitmap, 1, i, j), GetColor(bitmap, 2, i, j), GetColor(bitmap, 3, i, j));

                for (int l = 1; l < graph.Vertices.Count; l++)
                {
                    if(distance > GetDistance(graph.Vertices[l].data.Item1, graph.Vertices[l].data.Item2, graph.Vertices[l].data.Item3, GetColor(bitmap, 1, i, j), GetColor(bitmap, 2, i, j), GetColor(bitmap, 3, i, j)))
                    {
                        minVertex = l;
                        distance = GetDistance(graph.Vertices[l].data.Item1, graph.Vertices[l].data.Item2, graph.Vertices[l].data.Item3, GetColor(bitmap, 1, i, j), GetColor(bitmap, 2, i, j), GetColor(bitmap, 3, i, j));
                    }
                }

                //modyfing weights of founded vertex
                _alpha = 1 - (k - 1) /(float) iterations;
                float newR = graph.Vertices[minVertex].data.Item1 + _alpha * (GetColor(bitmap, 1, i, j) - graph.Vertices[minVertex].data.Item1);
                float newG = graph.Vertices[minVertex].data.Item2 + _alpha * (GetColor(bitmap, 2, i, j) - graph.Vertices[minVertex].data.Item2);
                float newB = graph.Vertices[minVertex].data.Item3 + _alpha * (GetColor(bitmap, 3, i, j) - graph.Vertices[minVertex].data.Item3);

                graph.Vertices[minVertex].data = new Tuple<float, float, float>(newR, newG, newB);

                //modyfing weights of his neighbours
                for (int w = 0; w < graph.Vertices.Count; w++)
                {
                    if (graph.Edges[minVertex, w] == true)
                    {
                        float newRNeigh = graph.Vertices[w].data.Item1 + _alpha * 0.005f * (GetColor(bitmap, 1, i, j) - graph.Vertices[w].data.Item1);
                        float newGNeigh = graph.Vertices[w].data.Item2 + _alpha * 0.005f * (GetColor(bitmap, 2, i, j) - graph.Vertices[w].data.Item2);
                        float newBNeigh = graph.Vertices[w].data.Item3 + _alpha * 0.005f * (GetColor(bitmap, 3, i, j) - graph.Vertices[w].data.Item3);

                        graph.Vertices[w].data = new Tuple<float, float, float>(newRNeigh, newGNeigh, newBNeigh);
                    }
                }
            }

            _graph = graph;
        }

        public Tuple<float, float, float> GetRGB(System.Drawing.Bitmap bitmap, int i, int j)
        {
            int minVertex = 0;
            float distance = GetDistance(_graph.Vertices[0].data.Item1, _graph.Vertices[0].data.Item2, _graph.Vertices[0].data.Item3, GetColor(bitmap, 1, i, j), GetColor(bitmap, 2, i, j), GetColor(bitmap, 3, i, j));

            for (int l = 1; l < _graph.Vertices.Count; l++)
            {
                if (distance > GetDistance(_graph.Vertices[l].data.Item1, _graph.Vertices[l].data.Item2, _graph.Vertices[l].data.Item3, GetColor(bitmap, 1, i, j), GetColor(bitmap, 2, i, j), GetColor(bitmap, 3, i, j)))
                {
                    minVertex = l;
                    distance = GetDistance(_graph.Vertices[l].data.Item1, _graph.Vertices[l].data.Item2, _graph.Vertices[l].data.Item3, GetColor(bitmap, 1, i, j), GetColor(bitmap, 2, i, j), GetColor(bitmap, 3, i, j));
                }
            }

            Tuple<float, float, float> t = new Tuple<float, float, float>(_graph.Vertices[minVertex].data.Item1, _graph.Vertices[minVertex].data.Item2, _graph.Vertices[minVertex].data.Item3);
            return t;
        }

        public Graph<Tuple<float, float, float>> GetGraph()
        {
            return _graph;
        }
    }
}
