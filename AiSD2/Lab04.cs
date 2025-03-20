using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASD
{
    public class Lab04 : MarshalByRefObject
    {
        //private int testCount = 0;
        /// <summary>
        /// Etap 1 - Wyznaczenie liczby oraz listy zainfekowanych serwisów po upływie K dni.
        /// Algorytm analizuje propagację infekcji w grafie i zwraca wszystkie dotknięte nią serwisy.
        /// </summary>
        /// <param name="G">Graf reprezentujący infrastrukturę serwisów.</param>
        /// <param name="K">Liczba dni propagacji infekcji.</param>
        /// <param name="s">Indeks początkowo zainfekowanego serwisu.</param>
        /// <returns>
        /// (int numberOfInfectedServices, int[] listOfInfectedServices) - 
        /// numberOfInfectedServices: liczba zainfekowanych serwisów,
        /// listOfInfectedServices: tablica zawierająca numery zainfekowanych serwisów w kolejności rosnącej.
        /// </returns>
        public (int numberOfInfectedServices, int[] listOfInfectedServices) Stage1(Graph G, int K, int s)
        {
            int n = G.VertexCount;
            int m = G.EdgeCount;
            Queue<(int, int)> queue = new Queue<(int, int)>(n); //vertex, day of inf
            queue.Enqueue((s, 1));
            int[] infecDay = new int[n];
            for (int i = 0; i < n; i++)
            {
                infecDay[i] = -1;
            }
            //Array.Fill(infecDay, -1);
            infecDay[s] = 1;
            while (queue.Count != 0)
            {
                var pair = queue.Dequeue();
                if (pair.Item2 == K)
                    continue;
                foreach (int neig in G.OutNeighbors(pair.Item1))
                {
                    if (infecDay[neig] != -1 && infecDay[neig] <= pair.Item2 + 1)
                        continue;
                    infecDay[neig] = pair.Item2 + 1;
                    queue.Enqueue((neig, pair.Item2 + 1));
                }
                
            }
            int count = 0;
            for (int i = 0; i < n; i++)
            {
                if (infecDay[i] != -1)
                    count++;
            }
            int[] infList = new int[count];
            int ind = 0;
            for (int v = 0; v < n; v++)
            {
                if (infecDay[v] != -1)
                { 
                    infList[ind++] = v;
                }
            }
            return (count, infList);
        }

        /// <summary>
        /// Etap 2 - Wyznaczenie liczby oraz listy zainfekowanych serwisów przy uwzględnieniu wyłączeń.
        /// Algorytm analizuje propagację infekcji z możliwością wcześniejszego wyłączania serwisów.
        /// </summary>
        /// <param name="G">Graf reprezentujący infrastrukturę serwisów.</param>
        /// <param name="K">Liczba dni propagacji infekcji.</param>
        /// <param name="s">Tablica początkowo zainfekowanych serwisów.</param>
        /// <param name="serviceTurnoffDay">Tablica zawierająca dzień, w którym dany serwis został wyłączony (K + 1 oznacza brak wyłączenia).</param>
        /// <returns>
        /// (int numberOfInfectedServices, int[] listOfInfectedServices) - 
        /// numberOfInfectedServices: liczba zainfekowanych serwisów,
        /// listOfInfectedServices: tablica zawierająca numery zainfekowanych serwisów w kolejności rosnącej.
        /// </returns>
        public (int numberOfInfectedServices, int[] listOfInfectedServices) Stage2(Graph G, int K, int[] s, int[] serviceTurnoffDay)
        {
            int n = G.VertexCount;
            int m = G.EdgeCount;
            int p = s.Length;
            Queue<(int, int)> queue = new Queue<(int, int)>(n); //(vertex, day of infec)
            int[] infecDay = new int[n];
            for (int i = 0; i < n; i++)
            {
                infecDay[i] = -1;
            }
            //Array.Fill(infecDay, -1);
            for (int i = 0; i < s.Length; i++)
            {
                queue.Enqueue((s[i], 1));
                infecDay[s[i]] = 1;
            }
            while (queue.Count != 0)
            {
                var pair = queue.Dequeue();
                int ver = pair.Item1;
                int dayOfInfec = pair.Item2;
                if (dayOfInfec == K || dayOfInfec + 1 >= serviceTurnoffDay[ver])
                    continue;
                foreach (int neig in G.OutNeighbors(ver))
                {
                    if (infecDay[neig] != -1 && infecDay[neig] <= pair.Item2 + 1)
                        continue;
                    if (dayOfInfec + 1 >= serviceTurnoffDay[neig])
                        continue;
                    infecDay[neig] = dayOfInfec + 1;
                    queue.Enqueue((neig, dayOfInfec + 1));
                }

            }
            int count = 0;
            for (int i = 0; i < n; i++)
            {
                if (infecDay[i] != -1)
                    count++;
            }
            int[] infList = new int[count];
            int ind = 0;
            for (int v = 0; v < n; v++)
            {
                if (infecDay[v] != -1)
                {
                    infList[ind++] = v;
                }
            }
            return (count, infList);
        }

        /// <summary>
        /// Used to determine the earliest day a server could be infected by a neighbor given, the range of days the neighbors was sick and the range of days the server was closed down.
        /// </summary>
        /// <returns>The smallest value in [a, b] but not in [c, d].</returns>
        static int SmallestOutside(int a, int b, int c, int d, int K)
        {
            if (c <= a && b <= d)
            {
                return K + 1;
            }
            if (b <= d && b >= c) // The sick range intersects butt is off to the left
            {
                return c - 1;
            }
            if (b > d)
            {
                if (a < c)
                {
                    return a;
                }
                if (a <= d)
                {
                    return d + 1;
                }
                return a;
            }
            // Now the entire sick range is to the left side of off range
            return a;
        }

        /// <summary>
        /// Etap 3 - Wyznaczenie liczby oraz listy zainfekowanych serwisów z możliwością ponownego włączenia wyłączonych serwisów.
        /// Algorytm analizuje propagację infekcji uwzględniając serwisy, które mogą być ponownie uruchamiane po określonym czasie.
        /// </summary>
        /// <param name="G">Graf reprezentujący infrastrukturę serwisów.</param>
        /// <param name="K">Liczba dni propagacji infekcji.</param>
        /// <param name="s">Tablica początkowo zainfekowanych serwisów.</param>
        /// <param name="serviceTurnoffDay">Tablica zawierająca dzień, w którym dany serwis został wyłączony (K + 1 oznacza brak wyłączenia).</param>
        /// <param name="serviceTurnonDay">Tablica zawierająca dzień, w którym dany serwis został ponownie włączony.</param>
        /// <returns>
        /// (int numberOfInfectedServices, int[] listOfInfectedServices) - 
        /// numberOfInfectedServices: liczba zainfekowanych serwisów,
        /// listOfInfectedServices: tablica zawierająca numery zainfekowanych serwisów w kolejności rosnącej.
        /// </returns>
        public (int numberOfInfectedServices, int[] listOfInfectedServices) Stage3(Graph G, int K, int[] s, int[] serviceTurnoffDay, int[] serviceTurnonDay)
        {
            int n = G.VertexCount;
            int m = G.EdgeCount;
            int p = s.Length;
            Queue<(int, int)> queue = new Queue<(int, int)>(n); //(vertex, day of infec)
            int[] infecDay = new int[n];
            for (int i = 0; i < n; i++)
            {
                infecDay[i] = -1;
            }
            //Array.Fill(infecDay, -1);
            for (int i = 0; i < s.Length; i++)
            {
                queue.Enqueue((s[i], 1));
                infecDay[s[i]] = 1;
            }
            int[] firstPeriod = new int[2]; // [pierwszy dzien w ktorym mozna zarazic, ostatni dzien w ktorym mozna zarazic]
            int[] secondPeriod = new int[2];
            int[] offPeriod = new int[2];

            while (queue.Count != 0)
            {
                var pair = queue.Dequeue();
                int ver = pair.Item1;
                int dayOfInfec = pair.Item2;
                // Zależnie od serwery, mogą być max 2 okresy w których będzie on mógł zarażać inne serwery
                // 1) Od następnego dnia do dnia przed wyłączeniem (może być to okres pusty)
                // 2) Od poranka po włączeniu do dnia K-tego włącznie
                secondPeriod[0] = Math.Max(serviceTurnonDay[ver] + 1, dayOfInfec + 1);
                secondPeriod[1] = Math.Max(secondPeriod[0], K);
                if (dayOfInfec + 1 < serviceTurnoffDay[ver])
                {
                    firstPeriod[0] = dayOfInfec + 1;
                    if (serviceTurnoffDay[ver] - 1 < firstPeriod[0])
                    {
                        firstPeriod[0] = K + 1;
                        firstPeriod[1] = K + 1;
                    }
                    else
                    {
                        firstPeriod[1] = serviceTurnoffDay[ver] - 1;
                    }
                }
                else
                {
                    firstPeriod[0] = K + 1;
                    firstPeriod[1] = K + 1;
                }
                if (firstPeriod[0] >= K + 1 && secondPeriod[0] >= K + 1)
                    continue;
                foreach (int neig in G.OutNeighbors(ver))
                {
                    offPeriod[0] = serviceTurnoffDay[neig];
                    offPeriod[1] = serviceTurnonDay[neig];
                    int earliestInfecDay = SmallestOutside(firstPeriod[0], firstPeriod[1], offPeriod[0], offPeriod[1], K);
                    earliestInfecDay = Math.Min(earliestInfecDay, SmallestOutside(secondPeriod[0], secondPeriod[1], offPeriod[0], offPeriod[1], K));
                    // Find the first day on which the neighbor can be infected by the 'ver' server
                    if (earliestInfecDay >= K + 1)
                        continue;
                    if (infecDay[neig] != -1 && infecDay[neig] <= earliestInfecDay)
                        continue;
                    infecDay[neig] = earliestInfecDay;
                    queue.Enqueue((neig, earliestInfecDay));
                }
            }
            int count = 0;
            for (int i = 0; i < n; i++)
            {
                if (infecDay[i] != -1)
                    count++;
            }
            int[] infList = new int[count];
            int ind = 0;
            for (int v = 0; v < n; v++)
            {
                if (infecDay[v] != -1)
                {
                    infList[ind++] = v;
                }
            }
            return (count, infList);
        }
    }
}
