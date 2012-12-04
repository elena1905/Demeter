using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Demeter
{
    static class Helper
    {
        public static bool Contains(Rectangle rect, Vector2 v)
        {
            return v.X >= rect.Left && v.X <= rect.Right && v.Y >= rect.Top && v.Y <= rect.Bottom;
        }

        public static Vector2 ToVector2(Point p)
        {
            return new Vector2((float)p.X, (float)p.Y);
        }

        public static Point ToPoint(Vector2 v)
        {
            return new Point((int)v.X, (int)v.Y);
        }

        public static float Normalize(float angle)
        {
            angle %= (float)Math.PI * 2;
            if (angle < 0)
                angle += (float)Math.PI * 2;
            return angle;
        }
    }
}
