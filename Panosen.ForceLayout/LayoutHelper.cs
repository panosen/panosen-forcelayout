using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panosen.ForceLayout
{
    public class Options
    {
        /// <summary>
        /// 重复次数
        /// </summary>
        public int RepeatTimes { get; set; } = 240;

        /// <summary>
        /// 中心点
        /// </summary>
        public Point CenterPoint { get; set; } = new Point(680, 300);

        /// <summary>
        /// 两个圆之间的最佳距离 = 圆心之间的距离 - R1 - R2
        /// </summary>
        public double BestDistance { get; set; } = 100;
    }

    public class LayoutHelper
    {
        public static void Layout(List<Node> nodeStateList, List<Edge> edges, Options options = null)
        {
            options = options ?? new Options();

            for (int i = 0; i < options.RepeatTimes; i++)
            {
                LayoutOnce(nodeStateList, edges, options);
            }

            if (options.CenterPoint != null)
            {
                LayoutMoveHelper.MoveDiagramTo(nodeStateList, options.CenterPoint);
            }
        }

        public static void LayoutOnce(List<Node> nodeList, List<Edge> edgeList, Options options = null)
        {
            options = options ?? new Options();

            Dictionary<Node, Point> nodeForceMap = nodeList.ToDictionary(v => v, v => new Point(0, 0));

            foreach (var nodeState in nodeList)
            {
                foreach (var item in nodeList.Where(v => v != nodeState))
                {
                    var repulsion = RepulsiveForce(nodeState, item, options.BestDistance);

                    nodeForceMap[nodeState].X = AddForce(nodeForceMap[nodeState].X, repulsion.X);
                    nodeForceMap[nodeState].Y = AddForce(nodeForceMap[nodeState].Y, repulsion.Y);

                    nodeForceMap[item].X = AddForce(nodeForceMap[item].X, -repulsion.X);
                    nodeForceMap[item].Y = AddForce(nodeForceMap[item].Y, -repulsion.Y);
                }

                var toNodes = edgeList.Where(v => v.From == nodeState).Select(v => v.To).ToList();
                var fromNodes = edgeList.Where(v => v.To == nodeState).Select(v => v.From).ToList();
                foreach (var item in toNodes.Union(fromNodes))
                {
                    var traction = TractionForce(nodeState, item, options.BestDistance);

                    nodeForceMap[nodeState].X = AddForce(nodeForceMap[nodeState].X, traction.X);
                    nodeForceMap[nodeState].Y = AddForce(nodeForceMap[nodeState].Y, traction.Y);

                    nodeForceMap[item].X = AddForce(nodeForceMap[item].X, -traction.X);
                    nodeForceMap[item].Y = AddForce(nodeForceMap[item].Y, -traction.Y);
                }
            }

            foreach (var item in nodeForceMap)
            {
                var deltaX = item.Value.X > 0 ? Math.Min(3, item.Value.X) : Math.Max(-3, item.Value.X);
                var deltaY = item.Value.Y > 0 ? Math.Min(3, item.Value.Y) : Math.Max(-3, item.Value.Y);

                item.Key.Position.X = Math.Round(item.Key.Position.X + deltaX, 4);
                item.Key.Position.Y = Math.Round(item.Key.Position.Y + deltaY, 4);
            }
        }

        private static double AddForce(double force, double delta)
        {
            if (force * delta <= 0)
            {
                return force + delta;
            }
            else if (force > 0)
            {
                return Math.Max(force, delta);
            }
            else
            {
                return Math.Min(force, delta);
            }
        }

        /// <summary>
        /// 排斥力
        /// </summary>
        public static Point RepulsiveForce(Node first, Node second, double bestDistance)
        {
            double deltaX = first.Position.X - second.Position.X;
            double deltaY = first.Position.Y - second.Position.Y;
            double delta = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            var distance = delta - first.Radius - second.Radius;

            if (distance >= bestDistance)
            {
                return new Point(0, 0);
            }

            var repulsion = (bestDistance - distance) / 2;

            return new Point(Math.Round(deltaX / delta * repulsion, 4), Math.Round(deltaY / delta * repulsion, 4));
        }

        /// <summary>
        /// 吸引力
        /// </summary>
        public static Point TractionForce(Node first, Node second, double bestDistance)
        {
            double deltaX = first.Position.X - second.Position.X;
            double deltaY = first.Position.Y - second.Position.Y;
            double delta = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            var distance = delta - first.Radius - second.Radius;

            if (distance <= bestDistance)
            {
                return new Point(0, 0);
            }

            var traction = (distance - bestDistance) / 2;

            return new Point(Math.Round(-deltaX / delta * traction, 4), Math.Round(-deltaY / delta * traction, 4));
        }
    }
}
