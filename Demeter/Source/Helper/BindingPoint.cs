using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Demeter
{
    public class BindingPoint
    {
        List<Vector2> totalPoints;
        List<String> totalLevels;
        List<String> totalId;

        List<Vector2> points;
        List<String> levels;
        List<String> ids;
        Vector2 bindingPoint;

        public BindingPoint()
        {
            points = new List<Vector2>();
            levels = new List<string>();
            ids = new List<string>();
            totalPoints = new List<Vector2>();
            totalLevels = new List<string>();
            totalId = new List<string>();
        }

        public void Add(Vector2 bindingPoint,string level, string id)
        {
            totalPoints.Add(bindingPoint);
            totalLevels.Add(level);
            totalId.Add(id);
        }

        public void PassBindingPoint(string currentLevel, Vector2 playerPos)
        {
            for (int i = 0; i < totalLevels.Count; i++)
            {
                if (totalLevels[i] == currentLevel)
                {
                    if (playerPos.X >= totalPoints[i].X)
                    {
                        points.Add(totalPoints[i]);
                        levels.Add(totalLevels[i]);
                        ids.Add(totalId[i]);
                    }
                }
            }
        }

        public Vector2 JudgeBindingPoint(string currentLevel , Vector2 playerPos)
        {
            int minDistance = 10000;

            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i] == currentLevel)
                {
                    if (Math.Abs(points[i].X - playerPos.X) < minDistance)
                    {
                        minDistance = (int)Math.Abs(points[i].X - playerPos.X);
                        bindingPoint = points[i];
                    }
                }
            }
            return bindingPoint;
        }

        public Vector2 GetPosById(string currentLevel, string id)
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i] == currentLevel && ids[i] == id)
                {
                    bindingPoint = points[i];
                }
            }
            return bindingPoint;
        }
    }
}
