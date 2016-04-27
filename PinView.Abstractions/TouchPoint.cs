using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinView.Abstractions
{
    public class TouchPoint
    {
        public double X
        {
            get;
        }

        public double Y
        {
            get;
        }

        public TouchPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double DisplacementFrom(TouchPoint other)
        {
            if (other == null)
            {
                return double.MaxValue;
            }

            if (other.X < 0 || other.Y < 0 || X < 0 || Y < 0)
            {
                return double.MaxValue;
            }

            double deltaX = Math.Abs(other.X - X);
            double deltaY = Math.Abs(other.Y - Y);

            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}
