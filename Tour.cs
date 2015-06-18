using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACSO
{
    public struct Tour
    {
        public Tour(int c)
        {
            data = null;
            cost = c;
        }
        public Tour(List<int> d)
        {
            data = d;
            cost = 0;
        }
        public Tour(List<int> d, int c)
        {
            data = d;
            cost = c;
        }
        public List<int> data;
        public int cost;
    }
}
