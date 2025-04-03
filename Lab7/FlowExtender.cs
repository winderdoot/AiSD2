using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASD
{
    public static class FlowExtender
    {

        /// <summary>
        /// Metoda wylicza minimalny s-t-przekrój grafu nieskierowanego.
        /// </summary>
        /// <param name="undirectedGraph">Nieskierowany graf</param>
        /// <param name="s">wierzchołek źródłowy</param>
        /// <param name="t">wierzchołek docelowy</param>
        /// <param name="minCut">minimalny przekrój</param>
        /// <returns>wartość przekroju</returns>
        public static double MinCut(this Graph<double> undirectedGraph, int s, int t, out Edge<double>[] minCut)
        {
            int N = undirectedGraph.VertexCount;
            DiGraph<double> network = new DiGraph<double>(N, undirectedGraph.Representation);
            foreach (Edge<double> e in undirectedGraph.DFS().SearchFrom(s))
            {
                network.AddEdge(e.From, e.To, e.Weight);
                network.AddEdge(e.To, e.From, e.Weight);
            }
            (double flowVal, DiGraph<double> maxFlow) = Flows.FordFulkerson<double>(network, s, t);
            DiGraph<double> residual = new DiGraph<double>(N, maxFlow.Representation);
            foreach (Edge<double> e in network.DFS().SearchFrom(s))
            {
                double cap = undirectedGraph.GetEdgeWeight(e.From, e.To);
                double flow = maxFlow.HasEdge(e.From, e.To) ? maxFlow.GetEdgeWeight(e.From, e.To) : 0;
                double extraFlow = cap - flow;
                if (extraFlow > 0)
                {
                    residual.AddEdge(e.From, e.To, extraFlow);
                }
                if (flow > 0)
                {
                    residual.AddEdge(e.To, e.From, flow);
                }
            }
            bool[] sReachable = new bool[N];//Can be reached from source in the residual network
            foreach (Edge<double> e in residual.DFS().SearchFrom(s))
            {
                //sReachable[e.From] = true;
                sReachable[e.To] = true;
            }
            // Teraz znaleźć krawędzie leżące na minimalnym przekroju
            List<Edge<double>> cut = new();
            foreach (Edge<double> e in network.DFS().SearchFrom(s))
            {
                if (sReachable[e.From] && !sReachable[e.To])
                {
                    cut.Add(e);
                }
            }
            minCut = cut.ToArray();
            return flowVal;
        }

        /// <summary>
        /// Metada liczy spójność krawędziową grafu oraz minimalny zbiór rozcinający.
        /// </summary>
        /// <param name="undirectedGraph">nieskierowany graf</param>
        /// <param name="cutingSet">zbiór krawędzi rozcinających</param>
        /// <returns>spójność krawędziowa</returns>
        public static int EdgeConnectivity(this Graph<double> undirectedGraph, out Edge<double>[] cutingSet)
        {
            cutingSet = null;
            return 0;
        }
        
    }
}
