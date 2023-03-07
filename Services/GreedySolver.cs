using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Pacman.Models;

namespace Pacman.Services
{
    public class GreedySolver
    {
        public static List<Point> FindPath(Maze maze, Point start, Point end)
        {
            var startCell = start;
            var endCell = end;

            var openSet = new Dictionary<Point, int>
            {
                [startCell] = 0
            };

            var cameFrom = new Dictionary<Point, Point>();

            var gScore = new Dictionary<Point, int>
            {
                [startCell] = 0
            };

            while (openSet.Count > 0)
            {
                var node = openSet.OrderBy(x => x.Value).First();
                var current = node.Key;
                if (current.Equals(endCell))
                    return ReconstructPath(cameFrom, current);

                openSet.Remove(current);

                var neighbors = maze.GetNeighbors(new Position(current.X, current.Y));
                foreach (var neighbor in neighbors)
                {
                    if (!gScore.TryGetValue(neighbor, out var neighborGScore))
                        neighborGScore = int.MaxValue;

                    var tentativeScore = neighborGScore + 1;
                    if (tentativeScore < neighborGScore)
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeScore;
                        openSet[neighbor] = tentativeScore;
                    }
                }
            }

            return null;
        }

        private static List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
        {
            var totalPath = new LinkedList<Point>();
            totalPath.AddLast(current);

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.AddFirst(current);
            }

            return totalPath.ToList();
        }
    }
}