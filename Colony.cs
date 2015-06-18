using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Windows;

namespace ACSO
{
    public class Colony
    {
        public readonly Problem problem;
        private Thread computation;
        public bool stop = false;
        private Tour bestTour = new Tour(Int32.MaxValue);
        public static Random random = new Random();
        private Graph graph = null;
        //
        private const int INTERVAL_REPORT_OUTPUT = 200;
        private const double GLOBAL_PHEROMONE_UPDATE = 0.1;
        private const int NUMBER_ANTS = 10;
        private Ant[] ants = new Ant[NUMBER_ANTS + 1];
        //Optimizer
        private ThreeOpt optimizer;
        //
        public Colony(Problem prob)
        {
            problem = prob;
            graph = problem.graph;
            int n = graph.numberCities;
            int[,] c = graph.cost;
            Tour initialTour = new Tour(new List<int>());
            if (c == null)
            {
                graph.cost = new int[n + 1, n + 1]; // natural indexing 
                for (int row = 1; row <= n; row++)
                {
                    for (int col = row; col <= n; col++)
                    {
                        if (row == col)
                        {
                            graph.cost[row, col] = 0;
                        }
                        else
                        {
                            double xDist = graph.rawData[row].X - graph.rawData[col].X;
                            double yDist = graph.rawData[row].Y - graph.rawData[col].Y;
                            graph.cost[row, col] = (int)(Math.Sqrt(xDist * xDist + yDist * yDist) + 0.5);
                            graph.cost[col, row] = graph.cost[row, col];
                        }
                    }
                }
            }
            else
            {
                graph.cost = c;
                for (int index = 1; index <= n; index++)
                {
                    graph.cost[index, index] = 0;
                }
            }
            // Obtain shortest city tour using greedy starting at 1 
            int numbersCitiesVisited = 1;
            int city = 1;
            initialTour.data.Add(0);
            initialTour.data.Add(1);
            do
            {
                // Find shortest distance from city 
                double[] distances = new double[n + 1];
                for (int index = 1; index <= n; index++)
                {
                    if (index == city || initialTour.data.Contains(index))
                    {
                        distances[index] = Int32.MaxValue;
                    }
                    else
                    {
                        distances[index] = graph.cost[city, index];
                    }
                }
                // Find shortest among distances 
                int shortestIndex = 0;
                double shortest = Double.MaxValue;
                for (int index = 1; index <= n; index++)
                {
                    if (distances[index] < shortest)
                    {
                        shortestIndex = index;
                        shortest = distances[index];
                    }
                }
                int previousCity = city;
                city = shortestIndex;
                initialTour.data.Add(city);
                numbersCitiesVisited++;
                initialTour.cost += graph.cost[previousCity, city];
            } while (initialTour.data.Count <= n);

            initialTour.cost += graph.cost[city, 1];
            problem.initialCost = initialTour.cost;
            initialTour.data.Add(1);
            //
            graph.edges = new GraphEdge[n + 1, n + 1]; // natural indexing 
            //
            for (int row = 1; row <= n; row++)
            {
                for (int col = 1; col <= n; col++)
                {
                    graph.edges[row, col].pheromone = 1.0 / (n * initialTour.cost);
                }
            }
            for (int row = 1; row <= n; row++)
            {
                for (int col = row; col <= n; col++)
                {
                    if (row == col)
                    {
                        graph.edges[row, col].heuristic = Double.MaxValue;
                    }
                    else
                    {
                        graph.edges[row, col].heuristic = 1.0 / graph.cost[row, col];
                        graph.edges[col, row].heuristic = graph.edges[row, col].heuristic;
                    }
                }
            }
        }
        public void Start()
        {
            computation = new Thread(new ThreadStart(Compute));
            computation.IsBackground = true;
            computation.Start();
        }
        public void Compute()
        {
            for (int iteration = 1; !stop && iteration <= problem.numberIterations; iteration++)
            {
                // Reuse ants array by inserting a fresh collection of 
                // new ants 
                for (int antNumber = 1; antNumber <= NUMBER_ANTS; antNumber++)
                {
                    int startingCity = random.Next(graph.numberCities) + 1;
                    Ant workerAnt = new Ant(problem, startingCity);
                    workerAnt.ConstructTour();
                    ants[antNumber] = workerAnt;
                }
                for (int antNumber = 1; antNumber <= NUMBER_ANTS; antNumber++)
                {
                    if (ants[antNumber].Tour.cost < bestTour.cost)
                    {
                        bestTour = ants[antNumber].Tour;
                    }
                }
                // Only the best ant so far deposits pheromone 
                for (int index = 1; index <= graph.numberCities; index++)
                {
                    graph.edges[bestTour.data[index], bestTour.data[index + 1]].pheromone =
                    (1.0 - GLOBAL_PHEROMONE_UPDATE) *
                    graph.edges[bestTour.data[index],
                    bestTour.data[index + 1]].pheromone +
                    GLOBAL_PHEROMONE_UPDATE * 1.0 /
                    bestTour.cost;
                }
                if (iteration == 1 || iteration % INTERVAL_REPORT_OUTPUT == 0)
                {
                    IntervalReport(iteration);
                }
            }
            FinalReport();
        }
        public void IntervalReport(int iteration)
        {
            problem.report.LogTextEvent("\nIteration: " + iteration + ":");
            problem.report.LogTextEvent(" Best tour cost: " + bestTour.cost + "\n");
            // Apply local optimization 
            optimizer.Optimize(problem, bestTour);
            problem.report.LogTextEvent("After applying 3-opt local search, Best tour cost:" + bestTour.cost + "\n");
            if (problem.knownOptimum != 0)
            {
                problem.report.LogTextEvent("\tError: " + String.Format("{0:f}", 100.0 *
                (bestTour.cost - problem.knownOptimum) / problem.knownOptimum) +
                " percent.\n");
            }
            // Test to see whether best tour is valid 
            for (int index = 1; index <= graph.numberCities; index++)
            {
                if (!bestTour.data.Contains(index))
                {
                    problem.report.LogTextEvent(" Invalid tour since city " + index + " is missing.\n");
                    return;
                }
            }
            problem.report.DrawTour(graph.rawData, bestTour);
        }
        public void FinalReport()
        {
            problem.report.LogTextEvent("Best tour: \n");
            foreach (int city in bestTour.data)
            {
                if (city != 0)
                {
                    problem.report.LogTextEvent(city + " ");
                }
            }
            // Final check on tour cost 
            int sum = 0;
            for (int index = 1; index <= graph.numberCities; index++)
            {
                sum += graph.cost[bestTour.data[index],
                bestTour.data[index + 1]];
            }
            problem.report.LogTextEvent("\nCost for this best tour: " + sum + "\n");
            problem.report.LogTextEvent("\n");
            // Test to see whether best tour is valid 
            for (int index = 1; index <= graph.numberCities; index++)
            {
                if (!bestTour.data.Contains(index))
                {
                    problem.report.LogTextEvent("Invalid tour since city " + index + " is missing.\n");
                    return;
                }
            }
            problem.report.LogTextEvent("All cities present and valid tour.\n");
            problem.report.ProblemFinished();
        }
    }
}