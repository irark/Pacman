using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Pacman.Models
{
    public class Pacman
    {
        private readonly Timer pacmanTimer = new();
        public Pacman(int x, int y, List<List<Cell>> maze)
        {
            X = x;
            Y = y;
            Maze = maze;
            Maze[Y][X].Visited = true; 
        }

        public void Start()
        {
            pacmanTimer.Interval = 300;
            pacmanTimer.Enabled = true;
            pacmanTimer.Elapsed += Move;
        }

        public int X { get; set; } = 1;
        public int Y { get; set; } = 1;
        public List<List<Cell>> Maze { get; set; } = new();
        public DirectionType Direction { get; set; } = DirectionType.None;

        public void Move(object? sender, ElapsedEventArgs elapsedEventArgs)
        {
            switch (Direction)
            {
               case DirectionType.Down:
                   if (Y != Maze.Count - 1 && !Maze[Y + 1][X].IsWall)
                       Y += 1;
                   Maze[Y][X].Visited = true;
                   break;
               case DirectionType.Left:
                   if (X != 0 && !Maze[Y][X - 1].IsWall)
                       X -= 1;
                   Maze[Y][X].Visited = true;
                   break;
               case DirectionType.Right:
                   if (X != Maze[Y].Count - 1 && !Maze[Y][X + 1].IsWall)
                       X += 1;
                   Maze[Y][X].Visited = true;
                   break;
               case DirectionType.Up: 
                   if (Y != 0 && !Maze[Y - 1][X].IsWall)
                       Y -= 1;
                   Maze[Y][X].Visited = true;
                   break;
            }
        }
    }
}