using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;
using System.Numerics;

namespace ASD
{

    public class Lab03GraphFunctions : System.MarshalByRefObject
    {

        // Część 1
        // Wyznaczanie odwrotności grafu
        //   0.5 pkt
        // Odwrotność grafu to graf skierowany o wszystkich krawędziach przeciwnie skierowanych niż w grafie pierwotnym
        // Parametry:
        //   g - graf wejściowy
        // Wynik:
        //   odwrotność grafu
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Graf wynikowy musi być w takiej samej reprezentacji jak wejściowy
        public DiGraph Lab03Reverse(DiGraph g)
        {
            int n = g.VertexCount;
            DiGraph res = new DiGraph(n, g.Representation);
            for (int v = 0; v < n; v++)
            {
                foreach (int u in g.OutNeighbors(v))
                {
                    res.AddEdge(u, v);
                }
            }
            return res;
        }

        public void DFS_BipartiteCheck(DiGraph g, ref int[] vert)
        {

        }

        // Część 2
        // Badanie czy graf jest dwudzielny
        //   0.5 pkt
        // Graf dwudzielny to graf nieskierowany, którego wierzchołki można podzielić na dwa rozłączne zbiory
        // takie, że dla każdej krawędzi jej końce należą do róźnych zbiorów
        // Parametry:
        //   g - badany graf
        //   vert - tablica opisująca podział zbioru wierzchołków na podzbiory w następujący sposób
        //          vert[i] == 1 oznacza, że wierzchołek i należy do pierwszego podzbioru
        //          vert[i] == 2 oznacza, że wierzchołek i należy do drugiego podzbioru
        // Wynik:
        //   true jeśli graf jest dwudzielny, false jeśli graf nie jest dwudzielny (w tym przypadku parametr vert ma mieć wartość null)
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Podział wierzchołków może nie być jednoznaczny - znaleźć dowolny
        //   3) Pamiętać, że każdy z wierzchołków musi być przyporządkowany do któregoś ze zbiorów
        //   4) Metoda ma mieć taki sam rząd złożoności jak zwykłe przeszukiwanie (za większą będą kary!)
        public bool Lab03IsBipartite(Graph g, out int[] vert)
        {
            int N = g.VertexCount;
            vert = new int[N];
            Stack<int> s = new Stack<int>();
            for (int v = 0; v < N; v++)
            {
                if (vert[v] == 0)
                {
                    s.Push(v);
                    vert[v] = 1;
                }
                while (s.Count > 0)
                {
                    int u = s.Pop();
                    int color = vert[u];
                    foreach (int neighbor in g.OutNeighbors(u))
                    {
                        if (vert[neighbor] == 0)
                        {
                            vert[neighbor] = color == 1 ? 2 : 1;
                            s.Push(neighbor);
                        }
                        else if (vert[neighbor] == color)
                        {
                            vert = null!;
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /*
         Union find działa w czasie odwrotności funkcji ackermanna
        DAG - Directed Acyclic Graph
        DiGraph - skierowany
        Graph - nieskierowany
         */

        // Część 3
        // Wyznaczanie minimalnego drzewa rozpinającego algorytmem Kruskala
        //   1 pkt
        // Schemat algorytmu Kruskala
        //   1) wrzucić wszystkie krawędzie do "wspólnego worka"
        //   2) wyciągać z "worka" krawędzie w kolejności wzrastających wag
        //      - jeśli krawędź można dodać do drzewa to dodawać, jeśli nie można to ignorować
        //      - punkt 2 powtarzać aż do skonstruowania drzewa (lub wyczerpania krawędzi)
        // Parametry:
        //   g - graf wejściowy
        //   mstw - waga skonstruowanego drzewa (lasu)
        // Wynik:
        //   skonstruowane minimalne drzewo rozpinające (albo las)
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Wykorzystać klasę UnionFind z biblioteki Graph
        //   3) Jeśli graf g jest niespójny to metoda wyznacza las rozpinający
        //   4) Graf wynikowy (drzewo) musi być w takiej samej reprezentacji jak wejściowy
        public Graph<int> Lab03Kruskal(Graph<int> g, out int mstw)
        {
            mstw = 0;
            int N = g.VertexCount;
            UnionFind components = new UnionFind(N);
            PriorityQueue<int, Edge<int>> queue = new PriorityQueue<int, Edge<int>>(N);
            Graph<int> tree = new Graph<int>(N, g.Representation);
            foreach (Edge<int> e in g.DFS().SearchAll())
            {
                queue.Insert(e, e.Weight);
            }
            while (queue.Count > 0)
            {
                Edge<int> f = queue.Extract();
                if (components.Find(f.From) != components.Find(f.To))
                {
                    tree.AddEdge(f.From, f.To, f.Weight);
                    components.Union(f.From, f.To);
                    mstw += f.Weight;
                }
            }

            return tree;
        }

        // Część 4
        // Badanie czy graf nieskierowany jest acykliczny
        //   0.5 pkt
        // Parametry:
        //   g - badany graf
        // Wynik:
        //   true jeśli graf jest acykliczny, false jeśli graf nie jest acykliczny
        // Uwagi:
        //   1) Graf wejściowy pozostaje niezmieniony
        //   2) Najpierw pomysleć jaki, prosty do sprawdzenia, warunek spełnia acykliczny graf nieskierowany
        //      Zakodowanie tego sprawdzenia nie powinno zająć więcej niż kilka linii!
        //      Zadanie jest bardzo łatwe (jeśli wydaje się trudne - poszukać prostszego sposobu, a nie walczyć z trudnym!)

        public bool Lab03IsUndirectedAcyclic(Graph g)
        {
            int N = g.VertexCount;
            Stack<int> stack = new Stack<int>();
            bool[] visited = new bool[N];
            int[] visitedFrom = new int[N];
            Array.Fill(visitedFrom, -1);
            for (int v = 0; v < N; v++)
            {
                if (visited[v])
                    continue;
                stack.Push(v);
                visited[v] = true;
                while (stack.Count != 0)
                {
                    int u = stack.Pop();
                    foreach (int neigh in g.OutNeighbors(u))
                    {
                        if (visitedFrom[neigh] == u || visitedFrom[u] == neigh)
                            continue;
                        if (visited[neigh])
                            return false;
                        visited[neigh] = true;
                        visitedFrom[neigh] = u;
                        stack.Push(neigh);
                    }
                }
            }
            return true;
        }

    }

}
