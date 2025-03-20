using ASD.Graphs;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;


/*
 * 
 * UWAGA: Zrobienie mądrej reprezentacji grafowej żeby puścić na nim gotowy algorytm grafowy
 * to 90% roboty. Tak samo jest w tym zadaniu: algorytm stanów (wykorzystanie dynamitu) i psuzcenie dijkstry
 * 1 raz na tym grafie.
 * */

namespace ASD
{
    public class Maze : MarshalByRefObject
    {

        public static int GetMazeIndex(int x, int y, int Width, int Height)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return -1;
            return y * Width + x;
        }

        public static string ReconstructPath(PathsInfo<int> pinfo, int source, int exit, int Width, int Height)
        {
            StringBuilder sb = new StringBuilder();
            var path = pinfo.GetPath(source, exit);
            int last = path[0];
            for (int i = 1; i < path.Length; i++)
            {
                if (path[i] == last + 1)
                {
                    sb.Append('E');
                }
                else if (path[i] == last - 1)
                {
                    sb.Append('W');
                }
                else if (path[i] == last - Width)
                {
                    sb.Append('N');
                }
                else if (path[i] == last + Width)
                {
                    sb.Append('S');
                }
                last = path[i];
            }
            return sb.ToString();
        }

        /// <summary>
        /// Wersje zadania I oraz II
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt bez dynamitów lub z dowolną ich liczbą
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="withDynamite">informacja, czy dostępne są dynamity 
        /// Wersja I zadania -> withDynamites = false, Wersja II zadania -> withDynamites = true</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany (dotyczy tylko wersji II)</param> 
        public int FindShortestPath(char[,] maze, bool withDynamite, out string path, int t = 0)
        {
            int Height = maze.GetLength(0);
            int Width = maze.GetLength(1);
            int source = -1, exit = -1;
            //Console.WriteLine();

            //for (int x = 0; x < Height; x++)
            //{
            //    for (int y = 0; y < Width; y++)
            //    {
            //        Console.Write(maze[x, y]);
            //    }
            //    Console.WriteLine(); 
            //}

            DiGraph<int> g = new DiGraph<int>(Width * Height);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int from = GetMazeIndex(x, y, Width, Height);
                    if (maze[y, x] == 'S')
                        source = from;
                    else if (maze[y, x] == 'E')
                        exit = from;
                    (int, int)[] neighbors = new (int, int)[4] { (x - 1, y), (x, y - 1), (x + 1, y), (x, y + 1) };
                    foreach ((int toX, int toY) in  neighbors)
                    {
                        int to = GetMazeIndex(toX, toY, Width, Height);
                        if (to == -1)
                            continue;
                        if (maze[toY, toX] == 'X' && withDynamite)
                        {
                            g.AddEdge(from, to, t);
                        }
                        else if (maze[toY, toX] == 'O' || maze[toY, toX] == 'E' || maze[toY, toX] == 'S')
                        {
                            g.AddEdge(from, to, 1);
                        }
                    }
                }   
            }
            PathsInfo<int> pinfo = Paths.Dijkstra(g, source);
            if (!pinfo.Reachable(source, exit))
            {
                path = "";
                return -1;
            }

            path = ReconstructPath(pinfo, source, exit, Width, Height);
            return pinfo.GetDistance(source, exit);
        }

        public static int GetMazeIndexLeveled(int x, int y, int level, int Width, int Height)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return -1;
            return y * Width + x + Width * Height * level;
        }

        public static int GetVertexLevel(int ind, int Width, int Height)
        {
            return ind / Width * Height;
        }
        public static string ReconstructPathLeveled(PathsInfo<int> pinfo, int source, int exit, int Width, int Height, int k)
        {
            StringBuilder sb = new StringBuilder();
            var path = pinfo.GetPath(source, exit);
            int last = path[0];
            for (int i = 1; i < path.Length - 1; i++)
            {
                if (path[i] == last + 1 || (path[i] == last + 1 + Width * Height ))
                {
                    sb.Append('E');
                }
                else if (path[i] == last - 1 || path[i] == last - 1 + Width * Height)
                {
                    sb.Append('W');
                }
                else if (path[i] == last - Width || path[i] == last - Width + Width * Height)
                {
                    sb.Append('N');
                }
                else if (path[i] == last + Width || path[i] == last + Width + Width * Height)
                {
                    sb.Append('S');
                }
                last = path[i];
            }
            return sb.ToString();
        }

        /// <summary>
        /// Wersja III i IV zadania
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt z użyciem co najwyżej k lasek dynamitu
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="k">liczba dostępnych lasek dynamitu, dla wersji III k=1</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany</param>
        public int FindShortestPathWithKDynamites(char[,] maze, int k, out string path, int t)
        {
            int Height = maze.GetLength(0);
            int Width = maze.GetLength(1);
            int source = -1, exit = Width * Height * (k + 1);
            int[] allExits = new int[k + 1];
            Array.Fill(allExits, -1);

            //Console.WriteLine();

            //for (int x = 0; x < Height; x++)
            //{
            //    for (int y = 0; y < Width; y++)
            //    {
            //        Console.Write(maze[x, y]);
            //    }
            //    Console.WriteLine();
            //}

            DiGraph<int> g = new DiGraph<int>(Width * Height * (k + 1) + 1);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int z = 0; z <= k; z++) // Amount of dynamite used
                    {
                        int from = GetMazeIndexLeveled(x, y, z, Width, Height);
                        if (z == 0 && maze[y, x] == 'S')
                            source = from;
                        else if (maze[y, x] == 'E')
                            allExits[z] = from; 
                        (int, int)[] neighbors = new (int, int)[4] { (x - 1, y), (x, y - 1), (x + 1, y), (x, y + 1) };
                        foreach ((int toX, int toY) in neighbors)
                        {
                            int to = GetMazeIndexLeveled(toX, toY, z, Width, Height);
                            if (to == -1)
                                continue;
                            if (maze[toY, toX] == 'X' && z < k)
                            {
                                g.AddEdge(from, to + Width * Height, t);
                            }
                            else if (maze[toY, toX] == 'O' || maze[toY, toX] == 'E' || maze[toY, toX] == 'S')
                            {
                                g.AddEdge(from, to, 1);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < allExits.Length; i++)
            {
                g.AddEdge(allExits[i], exit, 0);
            }
            PathsInfo<int> pinfo = Paths.Dijkstra(g, source);
            if (!pinfo.Reachable(source, exit))
            {
                path = "";
                return -1;
            }
            path = ReconstructPathLeveled(pinfo, source, exit, Width, Height, k);
            //Console.WriteLine(path);

            return pinfo.GetDistance(source, exit);
        }
    }
}