using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACSO
{
    public struct ThreeOpt
    {
        public void Optimize(Problem problem, Tour tour)
        {
            Graph graph = problem.graph;
            int x1 = 0, x2 = 0, x3 = 0, x4 = 0, x5 = 0, x6 = 0;
            int[] xb = new int[graph.numberCities + 2];
            int[] origb = new int[graph.numberCities + 2];

            int x1orig = 0, x2orig = 0, x3orig = 0, x4orig = 0,
            x5orig = 0, x6orig = 0;
            int firstb = 0, secondb = 0, thirdb = 0;

            for (int count = 1; count <= graph.numberCities; count++)
            {
                // Permute tour 
                for (int index = 1; index <= graph.numberCities; index++)
                {
                    tour.data[index] = tour.data[index + 1];
                }
                tour.data[graph.numberCities + 1] = tour.data[1];

                int bestGain = 0;
                int gain1 = 0;
                int gain2 = 0;
                int type2 = 3;

                // See page 445 in 
                // Design and Analysis of Algorithms by 
                // Levitin (Second Edition), Addison Wesley 
                x1 = 1;
                x2 = 2;
                for (int i = 5; i <= graph.numberCities - 2; i++)
                {
                    x5 = i;
                    x6 = i + 1;
                    for (int k = 3; k <= i - 2; k++)
                    {
                        x3 = k;
                        x4 = k + 1;
                        x1orig = x1;
                        x2orig = x2;
                        x3orig = x3;
                        x4orig = x4;
                        x5orig = x5;
                        x6orig = x6;
                        int first = graph.cost[tour.data[x1orig],
                        tour.data[x2orig]] +
                        graph.cost[tour.data[x3orig],
                        tour.data[x4orig]] +
                        graph.cost[tour.data[x5orig],
                        tour.data[x6orig]];

                        int second = graph.cost[tour.data[x2],
                       tour.data[x5]] +
                       graph.cost[tour.data[x1],
                       tour.data[x4]] +
                       graph.cost[tour.data[x3], tour.data[x6]];

                        int third = graph.cost[tour.data[x1],
                        tour.data[x4]] +
                        graph.cost[tour.data[x3], tour.data[x5]] +
                        graph.cost[tour.data[x2], tour.data[x6]];

                        gain1 = first - second;
                        gain2 = first - third;
                        if (gain1 > 0 && gain1 >= gain2 && gain1 > bestGain)
                        {
                            bestGain = gain1;
                            type2 = 1;
                            xb[1] = x1;
                            xb[2] = x2;
                            xb[3] = x3;
                            xb[4] = x4;
                            xb[5] = x5;
                            xb[6] = x6;
                            origb[1] = x1orig;
                            origb[2] = x2orig;
                            origb[3] = x3orig;
                            origb[4] = x4orig;
                            origb[5] = x5orig;
                            origb[6] = x6orig;
                            count = 0; // Must start another set of permutations 
                            firstb = first;
                            secondb = second;
                            thirdb = third;
                        }
                        else if (gain2 > 0 && gain2 > gain1 && gain2 > bestGain)
                        {
                            bestGain = gain2;
                            type2 = 2;
                            xb[1] = x1;
                            xb[2] = x2;
                            xb[3] = x3;
                            xb[4] = x4;
                            xb[5] = x5;
                            xb[6] = x6;
                            origb[1] = x1orig;
                            origb[2] = x2orig;
                            origb[3] = x3orig;
                            origb[4] = x4orig;
                            origb[5] = x5orig;
                            origb[6] = x6orig;
                            count = 0; // Must start another set of permutations 
                            firstb = first;
                            secondb = second;
                            thirdb = third;
                        }
                    }
                }
                if (type2 == 1)
                {
                    // Figure b on page 445 of Levitan’s book 
                    int[] next = new int[graph.numberCities + 2];
                    int[] temp = new int[graph.numberCities + 2];
                    int z = 0;
                    next[z++] = 0;
                    next[z++] = 1;
                    next[z++] = xb[4];
                    for (int j = 1; j <= origb[5] - origb[4]; j++)
                    {
                        next[z++] = xb[4] + j;
                    }
                    next[z++] = xb[2];
                    for (int j = 1; j <= origb[3] - origb[2]; j++)
                    {
                        next[z++] = xb[2] + j;
                    }
                    next[z++] = xb[6];
                    for (int j = 1; j <= graph.numberCities - origb[6]; j++)
                    {
                        next[z++] = xb[6] + j;
                    }
                    next[z++] = 1;

                    for (int index = 1;
                    index <= graph.numberCities + 1; index++)
                    {
                        temp[index] = tour.data[next[index]];
                    }

                    for (int index = 1;
                    index <= graph.numberCities + 1; index++)
                    {
                        tour.data[index] = temp[index];
                    }

                    // Test to see whether best tour is valid 

                    for (int index = 1;
                    index <= graph.numberCities; index++)
                    {
                        if (!tour.data.Contains(index))
                        {
                            problem.report.Alert("Invalid tour since city " + index + " is missing.\n");
                        }
                    }
                    tour.cost = 0;
                    for (int index = 1;
                    index <= graph.numberCities; index++)
                    {
                        tour.cost += graph.cost[tour.data[index],
                        tour.data[index + 1]];
                    }
                }
                else if (type2 == 2)
                {
                    // Figure c on page 445 of Levitan’s book 
                    int[] next = new int[graph.numberCities + 2];
                    int[] temp = new int[graph.numberCities + 2];
                    int z = 0;
                    //
                    //TODO:If this is for debugging use it somehow, else, comment out.
                    //string nextString = "";
                    //
                    next[z++] = 0;
                    next[z++] = 1;
                    //nextString += 1 + " ";
                    next[z++] = xb[4];
                    //nextString += xb[4] + " ";
                    for (int j = 1; j <= origb[5] - origb[4]; j++)
                    {
                        next[z++] = xb[4] + j;
                        //nextString += (xb[4] + j) + " ";
                    }
                    next[z++] = xb[3];
                    //nextString += xb[3] + " ";
                    for (int j = 1; j <= origb[3] - origb[2]; j++)
                    {
                        next[z++] = xb[3] - j;
                        //nextString += (xb[3] - j) + " ";
                    }
                    next[z++] = xb[6];
                    //nextString += xb[6] + " ";

                    for (int j = 1;
                    j <= graph.numberCities - origb[6]; j++)
                    {
                        next[z++] = xb[6] + j;
                        //nextString += (xb[6] + j) + " ";
                    }
                    next[z++] = 1;
                    //nextString += "1 ";

                    for (int index = 1;
                    index <= graph.numberCities + 1; index++)
                    {
                        temp[index] = tour.data[next[index]];
                    }

                    for (int index = 1;
                    index <= graph.numberCities + 1; index++)
                    {
                        tour.data[index] = temp[index];
                    }
                    // Test to see whether best tour is valid 
                    for (int index = 1;
                    index <= graph.numberCities; index++)
                    {
                        if (!tour.data.Contains(index))
                        {
                            problem.report.Alert("Invalid tour since city " + index + " is missing.\n");
                        }
                    }
                }
            }
        }
    }
}
