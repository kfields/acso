using System;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ACSO
{
    public class Ant
    {
        // Constants 
        private const int ALPHA = 1;
        private const int BETA = 5;
        private const double LOCAL_PHEROMONE_UPDATE = 0.1;
        // Fields 
        private Graph graph = null;
        private readonly Problem problem;
        private Tour tour = new Tour(new List<int>());
        private int startCity;
        // Properties 
        public Tour Tour
        {
            get { return tour; }
        }
        //
        public Ant(Problem prob, int startingCity)
        {
            graph = prob.graph;
            problem = prob;
            startCity = startingCity;
        }
        // Commands 
        public void ConstructTour()
        {
            tour.data.Add(0); // for natural indexing 
            tour.data.Add(startCity);
            int previousCity = startCity;
            do
            {
                int nextCity = AddEdgeFrom(previousCity);
                if (!tour.data.Contains(nextCity))
                {
                    tour.data.Add(nextCity);
                }
                tour.cost += graph.cost[previousCity, nextCity];

                // Ant colony system local trail update 
                graph.edges[previousCity, nextCity].pheromone =
                    (1.0 - LOCAL_PHEROMONE_UPDATE) *
                    graph.edges[previousCity, nextCity].pheromone +
                    LOCAL_PHEROMONE_UPDATE * (1.0 / (graph.numberCities * problem.initialCost));
                previousCity = nextCity;
            } while (tour.data.Count <= graph.numberCities);

            tour.cost += graph.cost[previousCity, startCity];
            tour.data.Add(startCity);
            if (tour.data.Count != graph.numberCities + 2)
            {
                problem.report.Alert("ERROR IN CONSTRUCTING TOUR");
            }
        }
        // Queries 
        public int AddEdgeFrom(int city)
        {
            // Based on modified Ant colony System heuristic 
            double r = Colony.random.NextDouble();
            if (r <= problem.probabilityThreshold)
            {
                double[] arcWeights = new double[graph.numberCities + 1];
                for (int index = 1; index <= graph.numberCities; index++)
                {
                    if (index == city ||
                    tour.data.Contains(index))
                    {
                        arcWeights[index] = 0.0;
                    }
                    else
                    {
                        arcWeights[index] = graph.edges[city, index].pheromone *
                            Math.Pow(graph.edges[city, index].heuristic, BETA);
                    }
                }
                // Get the largest in arcWeights 
                double largest = -1.0;
                int largestIndex = 0;
                for (int index = 1; index <= graph.numberCities; index++)
                {
                    if (arcWeights[index] > largest)
                    {
                        largest = arcWeights[index];
                        largestIndex = index;
                    }
                }
                if (arcWeights[largestIndex] == 0.0)
                {
                    // Return the first city not yet visited 
                    for (int index = 1; index <= graph.numberCities; index++)
                    {
                        if (!tour.data.Contains(index))
                        {
                            return index;
                        }
                    }
                }
                else
                {
                    return largestIndex;

                }
            }
            else
            { // Same as Ant System heuristic 
                double denominator = 0.0;
                for (int index = 1; index <= graph.numberCities; index++)
                {
                    if (index != city && !tour.data.Contains(index))
                    {
                        denominator += graph.edges[city, index].pheromone *
                        Math.Pow(graph.edges[city, index].heuristic,
                        BETA);
                    }
                }
                if (denominator == 0.0)
                {
                    // Return the first city not yet visited 
                    for (int index = 1;
                    index <= graph.numberCities; index++)
                    {
                        if (!tour.data.Contains(index))
                        {
                            return index;
                        }
                    }
                }
                // prob of going from city to index 
                double[] prob = new double[graph.numberCities + 1];
                for (int index = 1; index <= graph.numberCities; index++)
                {
                    if (index == city || tour.data.Contains(index))
                    {
                        prob[index] = 0.0;
                    }
                    else
                    {
                        prob[index] = graph.edges[city, index].pheromone *
                        Math.Pow(graph.edges[city, index].heuristic,
                        BETA) / denominator;
                    }
                }
                double rnd = Colony.random.NextDouble();
                double sum = 0.0;
                for (int index = 1; index <= graph.numberCities; index++)
                {
                    sum += prob[index];
                    if (rnd <= sum && index != city && !tour.data.Contains(index))
                    {
                        return index;
                    }
                }
            }
            // Unreachable 
            return 0;
        }
    }
}
