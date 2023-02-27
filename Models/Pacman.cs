using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pacman.Models
{
    public class Pacman
    {
        public Pacman(int x, int y, List<List<Cell>> maze)
        {
            X = x;
            Y = y;
            Maze = maze;
            Maze[Y][X].Visited = true;
        }

        public int X { get; set; } = 1;
        public int Y { get; set; } = 1;
        public List<List<Cell>> Maze { get; set; } = new();
        public DirectionType Direction { get; set; } = DirectionType.Right;

        public void GoDown()
        {
            Direction = DirectionType.Down;
            if (Y != Maze.Count - 1 && !Maze[Y + 1][X].IsWall)
                Y += 1;
            Maze[Y][X].Visited = true;
        }

        public void GoLeft()
        {
            Direction = DirectionType.Left;
            if (X != 0 && !Maze[Y][X - 1].IsWall)
                X -= 1;
            Maze[Y][X].Visited = true;
        }

        public void GoRight()
        {
            Direction = DirectionType.Right;
            if (X != Maze[Y].Count - 1 && !Maze[Y][X + 1].IsWall)
                X += 1;
            Maze[Y][X].Visited = true;
        }

        public void GoUp()
        {
            Direction = DirectionType.Up;
            if (Y != 0 && !Maze[Y - 1][X].IsWall)
                Y -= 1;
            Maze[Y][X].Visited = true;
        }

        public async Task Start()
        {
        }
    }
}