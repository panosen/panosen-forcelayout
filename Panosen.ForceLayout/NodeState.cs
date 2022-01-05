using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panosen.ForceLayout
{
    public class Node
    {
        public Point Position { get; set; }

        public double Radius { get; set; }
    }

    public class Edge
    {
        public Node From { get; set; }

        public Node To { get; set; }
    }
}