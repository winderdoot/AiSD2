//using System.Linq;

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




//namespace ASD
//{
//    using ASD.Graphs;
//    using System;
//    using System.Collections.Generic;

//    public class Lab06 : System.MarshalByRefObject
//    {
//        public List<int> WidePath(DiGraph<int> G, int start, int end)
//        {
//            return new List<int>();
//        }


//        public List<int> WeightedWidePath(DiGraph<int> G, int start, int end, int[] weights, int maxWeight)
//        {
//            return new List<int>();
//        }
//    }
//}