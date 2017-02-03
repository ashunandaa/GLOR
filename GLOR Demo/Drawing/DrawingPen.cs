using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GLOR_Demo.Drawing
{
    public sealed class DrawingPen
    {
        float[] dashValues = { 3, 3 };

        public Pen orangeDashedPen;
        public Pen orangeSolidPen;

        public Pen redDashedPen;
        public Pen redSolidPen;

        public Pen blueDashedPen;
        public Pen blueSolidPen;

        public Pen blackDashedPen;
        public Pen blackSolidPen;

        private DrawingPen()
        {
            orangeDashedPen = new Pen(Color.Orange, 1.7f);
            orangeDashedPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
            orangeDashedPen.DashPattern = dashValues;

            orangeSolidPen = new Pen(Color.Orange, 1.7f);
            orangeSolidPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;

            redDashedPen = new Pen(Color.Red, 1.7f);
            redDashedPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
            redDashedPen.DashPattern = dashValues;

            redSolidPen = new Pen(Color.Red, 1.7f);
            redSolidPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;

            blueDashedPen = new Pen(Color.Red, 1.7f);
            blueDashedPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
            blueDashedPen.DashPattern = dashValues;

            blueSolidPen = new Pen(Color.Red, 1.7f);
            blueSolidPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;

            blackDashedPen = new Pen(Color.Red, 1.7f);
            blackDashedPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
            blackDashedPen.DashPattern = dashValues;

            blackSolidPen = new Pen(Color.Red, 1.7f);
            blackSolidPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
        }

        private static DrawingPen sInstance = new DrawingPen();

        public static DrawingPen Pen
        {
            get { return sInstance; }
        }
    }
}
