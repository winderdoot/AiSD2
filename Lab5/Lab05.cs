using System.Linq;

/*
 * W tym zadaniu mamy dwa podejścia:
 * 1. znaleźć maksymalne drzewo rozpinające odwroconym kruskalem
 * ale kończymy w momencie gdy początek i koniec będą w jednej składowej
 * wtedy patrzymy na ostatnią dodaną krawędź i to jest nasze wąskie gardło
 * 
 * 2. Zrobić dijkstrę zmodyfikowanego - lepiej
 * 
 * 
 * 
 * II:
 * zewnętrzna pętla po maks szerokości krawędzi, od dużych dopuszczamy coarz weżśze
 * */




namespace ASD
{
    using ASD.Graphs;
    using System;
    using System.Collections.Generic;

    public class WidthComparer : Comparer<int>
    {
        public override int Compare(int x, int y)
        {
            if (x < y)
            {
                return -1;
            }
            if (x == y)
                return 0;
            return 1;
        }
    }


    public class Lab06 : System.MarshalByRefObject
    {
        public List<int> WidePath(DiGraph<int> G, int start, int end)
        {
            int N = G.VertexCount;

            PriorityQueue<int, int> queue = new PriorityQueue<int, int>(new WidthComparer(), N);
            int[] smallestWidthInPath = new int[N];
            int[] cameFrom = new int[N];

            for (int v = 0; v < N; v++)
            {
                if (v == start)
                    continue;
                queue.Insert(v, int.MaxValue);
                smallestWidthInPath[v] = -1;
                cameFrom[v] = -1;
            }
            cameFrom[start] = start;
            smallestWidthInPath[start] = 0;
            queue.Insert(start, 0);
            while (queue.Count > 0)
            {
                int v = queue.Extract();
                int vWidth = smallestWidthInPath[v];
                foreach (int nei in G.OutNeighbors(v))
                {
                    int newWidth = Math.Min(vWidth, G.GetEdgeWeight(v, nei));
                    if (newWidth == 0)
                    {
                        newWidth = G.GetEdgeWeight(v, nei);
                    }
                    if (newWidth > smallestWidthInPath[nei])
                    {
                        smallestWidthInPath[nei] = newWidth;
                        cameFrom[nei] = v;
                        queue.Insert(nei, newWidth);
                    }
                }
            }
            if (smallestWidthInPath[end] == -1)
            {
                return null;
            }
            List<int> path = new List<int>(N);
            int x = end;
            while (cameFrom[x] != x)
            {
                path.Add(x);
                x = cameFrom[x];
            }
            path.Reverse();
            return path;
        }


        public List<int> WeightedWidePath(DiGraph<int> G, int start, int end, int[] weights, int maxWeight)
        {
            return new List<int>();
        }
    }
}