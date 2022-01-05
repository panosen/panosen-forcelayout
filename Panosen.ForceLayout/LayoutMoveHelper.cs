using System;
using System.Collections.Generic;
using System.Text;

namespace Panosen.ForceLayout
{
    public static class LayoutMoveHelper
    {
        public static void MoveDiagramTo(List<Node> nodeStateList, Point point)
        {
            var centerNode = CenterOfGravity(nodeStateList);
            var delta = new Point(point.X - centerNode.X, point.Y - centerNode.Y);

            var deltaLength = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
            if (deltaLength == 0)
            {
                return;
            }

            foreach (var state in nodeStateList)
            {
                state.Position = new Point(state.Position.X + delta.X, state.Position.Y + delta.Y);
            }
        }

        private static Point CenterOfGravity(List<Node> nodeStateList)
        {
            var centroid = new Point(0, 0);

            if (nodeStateList.Count == 0)
            {
                return centroid;
            }

            foreach (var state in nodeStateList)
            {
                centroid.X += state.Position.X;
                centroid.Y += state.Position.Y;
            }
            centroid.X = centroid.X / nodeStateList.Count;
            centroid.Y = centroid.Y / nodeStateList.Count;
            return centroid;
        }
    }
}
