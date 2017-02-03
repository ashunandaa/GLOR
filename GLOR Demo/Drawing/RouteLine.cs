using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GLOR_Demo.Drawing
{
    public class RouteLine
    {
        public Point firstPoint;
        public Point lastPoint;
        public Pen pen;

        public RouteLine(Point firstPoint, Point lastPoint, Pen pen)
        {
            this.firstPoint = firstPoint;
            this.lastPoint = lastPoint;
            this.pen = pen;
        }

    }
}
