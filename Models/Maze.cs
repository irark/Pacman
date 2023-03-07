using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Pacman.Models
{
    public class Maze
    {
        private readonly List<List<Cell>> cells;

        public Maze(List<List<Cell>> cells) => this.cells = cells;

        

        public IEnumerable<Point> GetNeighbors(Position position)
        {
            var neighbors = new List<Point>(4);

            // TODO:
            var leftPosition = position.Left();
            var left = GetNeighbor(leftPosition);
            if (left != null)
                neighbors.Add(left.Value);

            var rightPosition = position.Right();
            var right = GetNeighbor(rightPosition);
            if (right != null)
                neighbors.Add(right.Value);

            var topPosition = position.Top();
            var top = GetNeighbor(topPosition);
            if (top != null)
                neighbors.Add(top.Value);

            var downPosition = position.Down();
            var down = GetNeighbor(downPosition);
            if (down != null)
                neighbors.Add(down.Value);

            return neighbors;
        }

        private Point? GetNeighbor(Position position)
        {
            if (IsPositionValid(position))
            {
                var cell = this[position];
                if (!cell.IsWall)
                    return new Point(cell.X, cell.Y);
            }

            return null;
        }

        private bool IsPositionValid(Position position)
            => position.X >= 0 &&
               position.X < 21 &&
               position.Y >= 0 &&
               position.Y < 27;

        public Cell this[Position position] => cells[position.X][position.Y];
        
    }
}