using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ASD
{
    public class Lab06 : MarshalByRefObject
    {
        /// <summary>Etap I</summary>
        /// <param name="G">Graf opisujący połączenia szlakami turystycznymi z podanym czasem przejścia krawędzi w wadze.</param>
        /// <param name="waitTime">Czas oczekiwania Studenta-Podróżnika w danym wierzchołku.</param>
        /// <param name="s">Wierzchołek startowy (początek trasy).</param>
        /// <returns>Pierwszy element krotki to wierzchołek końcowy szukanej trasy. Drugi element to długość trasy w minutach. Trzeci element to droga będąca rozwiązaniem: sekwencja odwiedzanych wierzchołków (zawierająca zarówno wierzchołek początkowy, jak i końcowy).</returns>
        public (int t, int l, int[] path) Stage1(DiGraph<int> G, int[] waitTime, int s)
        {
            int N = G.VertexCount;
            DiGraph<int> newG = new DiGraph<int>(N, G.Representation);
            foreach (Edge<int> e in G.DFS().SearchAll())
            {
                int w = e.Weight + waitTime[e.From];
                if (e.From == s)
                    w = e.Weight;
                newG.AddEdge(e.From, e.To, w);
            }
            //for (int i = 0; i < N; i++)
            //{
            //    newG.AddEdge(i, N, 0); // Waga nie ma znaczenia bo to ujście i tak
            //}
            PathsInfo<int> pathInfo = Paths.Dijkstra(newG, s);
            int t = s;
            int l = pathInfo.GetDistance(s, t);
            for (int i = 0; i < N; i++)
            {
                if (i == s || !pathInfo.Reachable(s, i))
                    continue;
                if (pathInfo.GetDistance(s, i) > l)
                {
                    l = pathInfo.GetDistance(s, i);
                    t = i;
                }
            }
            
            return (t, l, pathInfo.GetPath(s, t).ToArray());
        }

        public class TupleComparer : Comparer<(int, int)>
        {
            public override int Compare((int, int) tup1, (int, int) tup2)
            {
                if (tup1.Item1 < tup2.Item1)
                {
                    return -1;
                }
                if (tup1.Item1 > tup2.Item1)
                {
                    return 1;
                }
                // Teraz koszty są równe i trzeba porównać po odległości
                if (tup1.Item2 < tup2.Item2)
                {
                    return -1;
                }
                if (tup1.Item2 > tup2.Item2)
                {
                    return 1;
                }
                return 0;
            }
        }

        public (int l, int c, int[] path)? Stage2(DiGraph<int> G, Graph<int> C, int[] waitTime, int s, int t)
        {
            int N = G.VertexCount;
            PriorityQueue<(int, int), int> queue = new PriorityQueue<(int, int), int>(new TupleComparer(), N); //((cena, odległość), wierzchołek)
            int[] price = new int[N];
            int[] dist = new int[N];
            int[] cameFrom = new int[N];
            for (int i = 0; i < N; i++)
            {
                price[i] = int.MaxValue;
                dist[i] = int.MaxValue;
                cameFrom[i] = -1;
            }
            DiGraph<int> newG = new DiGraph<int>(N, G.Representation);
            foreach (Edge<int> e in G.DFS().SearchAll())
            {
                int w = e.Weight + waitTime[e.From];
                if (e.From == s)
                    w = e.Weight;
                newG.AddEdge(e.From, e.To, w);
            }

            queue.Insert(s, (0, 0));
            price[s] = 0;
            dist[s] = 0;
            while (queue.Count > 0)
            {
                int u = queue.Extract();
                foreach (int nei in newG.OutNeighbors(u))
                {
                    int c = C.GetEdgeWeight(u, nei);
                    int w = newG.GetEdgeWeight(u, nei);
                    if (price[nei] > price[u] + c)
                    {
                        cameFrom[nei] = u;
                        price[nei] = price[u] + c;
                        dist[nei] = dist[u] + w;
                        queue.Insert(nei, (price[nei], dist[nei]));
                    }
                    else if (price[nei] == price[u] + c && dist[nei] > dist[u] + w)
                    {
                        //price[nei] = price[u] + c;
                        cameFrom[nei] = u;
                        dist[nei] = dist[u] + w;
                        queue.Insert(nei, (price[nei], dist[nei]));
                    }
                }
            }
            if (dist[t] == int.MaxValue)
            {
                return null;
            }

            LinkedList<int> path = new LinkedList<int>();
            path.AddFirst(t);
            int x = cameFrom[t];
            while (x != -1)
            {
                path.AddFirst(x);
                x = cameFrom[x];
            }
            return (dist[t], price[t], path.ToArray());
        }
    }
}