using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Demeter
{
    public class LineSegment
    {
        Vector2 p1;
        Vector2 p2;

        public LineSegment(Vector2 p1, Vector2 p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public LineSegment(Vector2 p1, float angle, Rectangle rect)
        {
            this.p1 = p1;
            Ray helperRay = new Ray(p1, angle);
            List<Vector2> intersection = helperRay.Intersects(rect, true);
            this.p2 = intersection.First();
        }

        public List<Vector2> Intersects(Rectangle rect)
        {
            if (p1.X > p2.X)
            {
                Vector2 temp = p1;
                p1 = p2;
                p2 = temp;
            }

            Line helperLine = new Line(p1, p2);
            List<Vector2> intersection1 = helperLine.Intersects(rect);
            List<Vector2> intersection = new List<Vector2>();

            foreach (Vector2 point in intersection1)
            {
                Vector2 p = point;
                if (p.X >= p1.X && p.X <= p2.X)
                {
                    intersection.Add(point);
                }
            }

            return intersection;
        }

        public void Retrieve(int width, Retrieve retrieve)
        {
            if (p1.X == p2.X)
            {
                if (p1.Y < p2.Y)
                {
                    Vector2 increment = new Vector2(0, width);
                    float angle = (float)Math.PI / 2;

                    for (Vector2 point = p1; point.Y <= p2.Y; point += increment)
                    {
                        retrieve(point, angle);
                    }
                }
                else
                {
                    Vector2 increment = new Vector2(0, -width);
                    float angle = (float)-Math.PI / 2;

                    for (Vector2 point = p1; point.Y >= p2.Y; point += increment)
                    {
                        retrieve(point, angle);
                    }
                }
            }
            else
            {
                float k = (p1.Y - p2.Y) / (p1.X - p2.X);
                float b = p1.Y - k * p1.X;
                float angle = (float)Math.Atan((double)k);
                if (p1.X > p2.X)
                    angle += (float)Math.PI;
                Vector2 increment = new Vector2(width * (float)Math.Cos(angle), width * (float)Math.Sin(angle));

                if (p1.X < p2.X)
                {
                    for (Vector2 point = p1; point.X <= p2.X; point += increment)
                    {
                        retrieve(point, angle);
                    }
                }
                else
                {
                    for (Vector2 point = p1; point.X >= p2.X; point += increment)
                    {
                        retrieve(point, angle);
                    }
                }
            }
        }
    }

    public delegate void Retrieve(Vector2 point, float angle);
}
