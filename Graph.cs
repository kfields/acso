using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;

namespace ACSO
{
    //Global
    public struct GraphNode
    {
    }
    public struct GraphEdge
    {
        public double heuristic; // Reciprocal of cost 
        public double pheromone;
    }
    public class Graph
    {
        public GraphEdge[,] edges;
        //public List<GraphNode> nodes;
        //
        public Point[] rawData;
        public int[,] cost;
        public int numberCities;
    }
}
