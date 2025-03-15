using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASD
{
    public class Lab04 : MarshalByRefObject
    {
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
            Array.Fill(infecDay, -1);
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
            Array.Fill(infecDay, -1);
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
            Array.Fill(infecDay, -1);
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
                int currDay = -1;
                if (dayOfInfec + 1 < serviceTurnoffDay[ver])
                {
                    currDay = dayOfInfec + 1;
                    for (int d = currDay + 1; d < serviceTurnonDay[ver]; d++)
                    {
                        queue.Enqueue((ver, d));
                    }
                }
                else
                {
                    currDay = serviceTurnonDay[ver] + 1;
                }
                if (currDay >= K + 1)
                    continue;
                foreach (int neig in G.OutNeighbors(ver))
                {
                    if (infecDay[neig] != -1 && infecDay[neig] <= currDay)
                        continue;
                    if (currDay >= serviceTurnoffDay[neig] && currDay <= serviceTurnonDay[neig])
                        continue;
                    infecDay[neig] = currDay;
                    queue.Enqueue((neig, currDay));
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
