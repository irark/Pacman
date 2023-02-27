using System.Collections.Generic;

namespace Pacman.Models
{
    public class Ghost
    {
        public Ghost(int x, int y, List<List<Cell>> maze)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }
        public int Y { get; set; }
    }
}