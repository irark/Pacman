using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Pacman.Models
{
    public class Ghost
    {
        public Ghost(List<List<Cell>> maze)
        {
            Maze = maze;
            SetGhost();
        }

        private void SetGhost()
        {
            while (true)
            {
                Y = ran.Next(0, Maze.Count - 1);
                X = ran.Next(0, Maze[Y].Count - 1);
                if (!Maze[Y][X].IsWall && (X != 1 || Y != 1))
                    return;
            }
        }
        public int X { get; set; }
        public int Y { get; set; }
        public Action OnMoved { get; set; }
        private List<List<Cell>> Maze { get; set; }
        private DirectionType Direction { get; set; }
        private Random ran = new();
        private Timer ghostTimer = new();

        public void StartGhost()
        {
            ghostTimer.Interval = 300;
            ghostTimer.Enabled = true;
            ghostTimer.Elapsed += MoveGhost;   
        }

        public void StopGhost()
        {
            ghostTimer.Enabled = false;
        }
        public void MoveGhost(object? sender, ElapsedEventArgs elapsedEventArgs)
        {
            var canMove = false;
            OtherDirection();
            while (!canMove)
            {
                canMove = CheckDirection();
                if (!canMove)
                    ChangeDirection();
            }

            if (canMove)
            {
                switch (Direction)
                {
                    case DirectionType.Down:
                        Y++;
                        break;
                    case DirectionType.Left:
                        X--;
                        break;
                    case DirectionType.Right:
                        X++;
                        break;
                    case DirectionType.Up:
                        Y--;
                        break;
                }
            }
            OnMoved?.Invoke();
        }

        private bool CheckDirection()
        {
            return Direction switch
            {
                DirectionType.Down => DirectionOk(X, Y + 1),
                DirectionType.Left => DirectionOk(X - 1, Y),
                DirectionType.Right => DirectionOk(X + 1, Y),
                DirectionType.Up => DirectionOk(X, Y - 1),
                _ => false
            };
        }

        private bool DirectionOk(int x, int y)
        {
            if (x < 0)
                X = Maze[y].Count;

            if (x > Maze[y].Count)
                X = 0;
            return !Maze[y][x].IsWall;
        }

        private void ChangeDirection()
        {
            var which = ran.Next(0, 2);
            switch (Direction)
            {
                case DirectionType.Down:
                case DirectionType.Up:
                    Direction = which == 1 ? DirectionType.Left : DirectionType.Right;
                    break;
                case DirectionType.Left:
                case DirectionType.Right:
                    Direction = which == 1 ? DirectionType.Down : DirectionType.Up;
                    break;
            }
        }

        private void OtherDirection()
        {
            bool[] directions = new bool[5];
            switch (Direction)
            {
                case DirectionType.Down:
                case DirectionType.Up:
                    directions[2] = DirectionOk(X + 1, Y);
                    directions[4] = DirectionOk(X - 1, Y);
                    break;
                case DirectionType.Left:
                case DirectionType.Right:
                    directions[1] = DirectionOk(X, Y - 1);
                    directions[3] = DirectionOk(X, Y + 1);
                    break;
            }

            var which = ran.Next(0, 4);
            if (directions[which])
            {
                Direction = (DirectionType) which;
            }
        }
    }
}