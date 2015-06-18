using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;

namespace ACSO
{
    public interface IProblemReport
    {
        void LogTextEvent(string EventText);
        void DrawTour(Point[] data, Tour tour);
        void Alert(string alertText);
        void ProblemFinished();
    }

    public struct Problem
    {
        public Graph graph;
        public int initialCost; //cost of initial tour.
        public int numberIterations;
        public double probabilityThreshold;
        public int knownOptimum;
        public IProblemReport report;
        //
        public void SetDefaults()
        {
            numberIterations = 20000;
            probabilityThreshold = .9;
            knownOptimum = 0;
            report = null;
        }
    }
}
