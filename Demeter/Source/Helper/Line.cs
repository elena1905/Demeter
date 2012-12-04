using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Demeter
{
    public class Line
    {
        Vector2 p1;
        Vector2 p2;

        public Line(Vector2 p1, Vector2 p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public Line(Vector2 p1, float angle)
        {
            this.p1 = p1;

            if (angle == Math.PI / 2 || angle == Math.PI / 2 * 3)
            {
                p2.X = p1.X;
                p2.Y = 0;
            }
            else
            {
                float k = (float)Math.Tan((double)angle);
                float b = p1.Y - k * p1.X;
                p2.X = 0;
                p2.Y = b;
            }
        }

        public List<Vector2> Intersects(Rectangle rect)
        {
            List<Vector2> intersection = new List<Vector2>();

            if (p1.X == p2.X)
            {
                if (p1.X >= rect.Left && p1.X <= rect.Right)
                {
                    intersection.Add(new Vector2(p1.X, rect.Top));
                    intersection.Add(new Vector2(p1.X, rect.Bottom));
                }
            }
            else
            {
                float k = (p1.Y - p2.Y) / (p1.X - p2.X);
                float b = p1.Y - k * p1.X;

                float intercept;

                intercept = (rect.Top - b) / k;
                if (intercept >= rect.Left && intercept < rect.Right)
                    intersection.Add(new Vector2(intercept, rect.Top));

                intercept = (rect.Bottom - b) / k;
                if (intercept >= rect.Left && intercept < rect.Right)
                    intersection.Add(new Vector2(intercept, rect.Bottom));

                intercept = k * rect.Left + b;
                if (intercept >= rect.Top && intercept < rect.Bottom)
                    intersection.Add(new Vector2(rect.Left, intercept));

                intercept = k * rect.Right + b;
                if (intercept >= rect.Top && intercept < rect.Bottom)
                    intersection.Add(new Vector2(rect.Right, intercept));
            }

            return intersection;
        }
    }
}
