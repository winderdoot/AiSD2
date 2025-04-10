using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ASD
{
    public class Lab08 : MarshalByRefObject
    {
        public static int GetNodeInd(int r, int c, int layer, int w, int h) // layer = 0,1,2
        {
            return r * w + c + layer * w * h;
        }

        public static (int, int)[] GetGridNeighbors(int r, int c, int layer, int w, int h)
        {
            return new (int, int)[4]
            {
                (r - 1, c), (r, c + 1), (r + 1, c), (r, c - 1)
            };

        }

        /// <summary>Etap I</summary>
        /// <param name="P">Tablica która dla każdego pola zawiera informacje, ile maszyn moze lacznie wyjechac z tego pola</param>
        /// <param name="MachinePos">Tablica zawierajaca informacje o poczatkowym polozeniu maszyn</param>
        /// <returns>Pierwszy element kroki to liczba uratowanych maszyn, drugi to tablica indeksów tych maszyn</returns>
        public (int savedNum, int[] Saved) Stage1(int[,] P, (int row, int col)[] MachinePos)
        {
			int h = P.GetLength(0);
            int w = P.GetLength(1);
            /*
             Graf dzielimy na 3 warstwy: (indeksy warstw: 0,1,2)
            1: pozycje maszyn (początkowe) (w * h, w * h * 2 - 1)
            0: pola placu budowy (0, w * h - 1)
            2: wierchołki przejściowe (w * h * 2, w * h * 3 - 1)
            źródo = w * h * 3
            ujście = w  H * 3 + 1
             */
            int s = w * h * 3;
            int t = w * h * 3 + 1;
            DiGraph<int> network = new DiGraph<int>(w * h * 3 + 2);
            for (int r = 0; r < h; r++)
            {
                for (int c = 0; c < w; c++)
                {
                    //mamy pozycję pola na placu
                    int vfield = GetNodeInd(r, c, 0, w, h);
                    int vmachine = GetNodeInd(r, c, 1, w, h);
                    int vpass = GetNodeInd(r, c, 2, w, h);
                    network.AddEdge(vmachine, vfield, 1); // waga może być też większa, bez znaczenia
                    network.AddEdge(vfield, vpass, P[r, c]); // do wierzchołka przejściowego
                    (int, int)[] neighbors = GetGridNeighbors(r, c, 2, w, h);
                    foreach ((int nr, int nc) in neighbors)
                    {
                        if (nr < 0 || nr >= h || nc < 0 || nc >= w)
                            continue; //invalid pos
                        int nind = GetNodeInd(nr, nc, 0, w, h);
                        network.AddEdge(vpass, nind, int.MaxValue); // tutaj nieskonczonosc
                    }
                }
            }
            // źródło => maszyny
            foreach ((int mr, int mc) in MachinePos)
            {
                int mind = GetNodeInd(mr, mc, 1, w, h);
                network.AddEdge(s, mind, 1);
            }
            // przejściowe => ujście
            for (int c = 0; c < w; c++)
            {
                int r = 0;
                int vpass = GetNodeInd(r, c, 2, w, h);
                network.AddEdge(vpass, t, int.MaxValue);
            }
            (int flowVal, DiGraph<int> flowGraph) = Flows.FordFulkerson<int>(network, s, t);
            List<int> saved = new();
            for (int i = 0; i < MachinePos.Length; i++)
            {
                int machineInd = GetNodeInd(MachinePos[i].Item1, MachinePos[i].Item2, 1, w, h);
                if (flowGraph.HasEdge(s, machineInd))
                {
                    saved.Add(i);
                }
            }
            return (flowVal, saved.ToArray());
        }

        /// <summary>Etap II</summary>
        /// <param name="P">Tablica która dla każdego pola zawiera informacje, ile maszyn moze lacznie wyjechac z tego pola</param>
        /// <param name="MachinePos">Tablica zawierajaca informacje o poczatkowym polozeniu maszyn</param>
        /// <param name="MachineValue">Tablica zawierajaca informacje o wartosci maszyn</param>
        /// <param name="moveCost">Koszt jednego ruchu</param>
        /// <returns>Pierwszy element kroki to najwiekszy mozliwy zysk, drugi to tablica indeksow maszyn, ktorych wyprowadzenie maksymalizuje zysk</returns>
        public (int bestProfit, int[] Saved) Stage2(int[,] P, (int row, int col)[] MachinePos, int[] MachineValue, int moveCost)
        {
            int h = P.GetLength(0);
            int w = P.GetLength(1);
            /*
             Graf dzielimy na 3 warstwy: (indeksy warstw: 0,1,2)
            1: pozycje maszyn (początkowe) (w * h, w * h * 2 - 1)
            0: pola placu budowy (0, w * h - 1)
            2: wierchołki przejściowe (w * h * 2, w * h * 3 - 1)
            źródo = w * h * 3
            ujście = w  H * 3 + 1
             */
            int s = w * h * 3;
            int t = w * h * 3 + 1;
            NetworkWithCosts<int, int> network = new NetworkWithCosts<int, int>(w * h * 3 + 2);
            for (int r = 0; r < h; r++)
            {
                for (int c = 0; c < w; c++)
                {
                    //mamy pozycję pola na placu
                    int vfield = GetNodeInd(r, c, 0, w, h);
                    int vmachine = GetNodeInd(r, c, 1, w, h);
                    int vpass = GetNodeInd(r, c, 2, w, h);
                    //network.AddEdge(vmachine, vfield, 1, 0); // waga może być też większa, bez znaczenia
                    network.AddEdge(vfield, vpass, P[r, c], moveCost); // do wierzchołka przejściowego
                    (int, int)[] neighbors = GetGridNeighbors(r, c, 2, w, h);
                    foreach ((int nr, int nc) in neighbors)
                    {
                        if (nr < 0 || nr >= h || nc < 0 || nc >= w)
                            continue; //invalid pos
                        int nind = GetNodeInd(nr, nc, 0, w, h);
                        network.AddEdge(vpass, nind, int.MaxValue, 0); // tutaj nieskonczonosc
                    }
                }
            }
            // źródło => maszyny oraz poboczne maszyny => ujście (jeśli nie transportujemy jej)
            for (int i = 0; i < MachinePos.Length; i++)
            {
                (int mr, int mc) = MachinePos[i];
                int mind = GetNodeInd(mr, mc, 1, w, h);
                network.AddEdge(s, mind, 1, 0);
                network.AddEdge(mind, t, 1, 0);
                int vfield = GetNodeInd(mr, mc, 0, w, h);
                if (MachineValue[i] > 0)
                    network.AddEdge(mind, vfield, 1, -MachineValue[i]); // waga może być też większa, bez znaczenia
            }
            // przejściowe => ujście
            for (int c = 0; c < w; c++)
            {
                int r = 0;
                int vpass = GetNodeInd(r, c, 2, w, h);
                network.AddEdge(vpass, t, int.MaxValue, 0);
            }
            (int flowVal, int cost, DiGraph<int> flowGraph) = Flows.MinCostMaxFlow<int, int>(network, s, t);
            List<int> saved = new();
            for (int i = 0; i < MachinePos.Length; i++)
            {
                (int mr, int mc) = MachinePos[i];
                int machineInd = GetNodeInd(mr, mc, 1, w, h);
                int vfield = GetNodeInd(mr, mc, 0, w, h);
                if (flowGraph.HasEdge(machineInd, vfield) && flowGraph.GetEdgeWeight(machineInd, vfield) > 0)
                {
                    saved.Add(i);
                }
            }
            return (-cost, saved.ToArray()); // -cost czyli profit
        }
    }
}