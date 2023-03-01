using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Pacman.Models;
using Pacman.Services;
using Timer = System.Timers.Timer;

namespace Pacman.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        public Models.Pacman Pacman { get; set; }
        public List<Ghost> Ghosts { get; set; } = new();
        public int CountOfGhosts { get; set; } = 5;
        public List<List<Cell>> Maze { get; set; } = new(30);
        public int[,] SimpleMaze { get; set; } = new int[30, 30];
        private bool _gameOver;
        private GameType _gameType = GameType.AStar;
        private Timer timer = new();
        private Timer ghostTimer = new();

        protected override void OnInitialized()
        {
            var lines = System.IO.File.ReadLines(@"Mazes\maze1.txt")?.ToList() ?? new List<string>();
            for (var i = 0; i < lines.Count(); i++)
            {
                lines[i] = lines[i].Replace("\r", "").Replace("\n", "");
                Maze.Add(new List<Cell>(lines[i].Length));
                for (var j = 0; j < lines[i].Length; j++)
                {
                    Maze[i].Add(new Cell() {X = j, Y = i, IsWall = lines[i][j] == '1'});
                    SimpleMaze[i, j] = lines[i][j] == '1' ? 1 : 0;
                }
            }
        }

        public void StartGame()
        {
            Maze.ForEach(i => i.ForEach(i => i.Visited = false));
            Ghosts = new List<Ghost>();
            StateHasChanged();
            switch (_gameType)
            {
                case GameType.Default:
                    StartDefaultGame();
                    break;
                case GameType.AStar:
                    StartAStarGame();
                    break;
                case GameType.Greedy:
                    break;
            }
        }

        private void StartDefaultGame()
        {
            Pacman = new(1, 1, Maze);
            for (var i = 0; i < CountOfGhosts; i++)
                Ghosts.Add(new Ghost(Maze));
            ghostTimer.Interval = 200;
            ghostTimer.Enabled = true;
            ghostTimer.Elapsed += TimerTick;
            timer.Interval = 100;
            timer.Enabled = true;
            timer.Elapsed += CheckPacman;
        }

        private Point GeneratePoint()
        {
            Random ran = new();
            Point start = new Point();
            while (true)
            {
                start.X = ran.Next(0, Maze.Count);
                start.Y = ran.Next(0, Maze[start.X].Count);
                if (!Maze[start.X][start.Y].IsWall)
                    return start;
            }
        }

        public Point Start { get; set; }
        public Point End { get; set; }

        private void StartAStarGame()
        {
            List<Point> res = null;
            while (res == null)
            {
                Start = GeneratePoint();
                End = GeneratePoint();
                Pacman = new(Start.Y, Start.X, Maze);
                res = AStarSolver.FindPath(SimpleMaze, Start, End);

            }

            foreach (var point in res)
            {
                if (point.Y > Pacman.X) Pacman.GoRight();
                if (point.Y < Pacman.X) Pacman.GoLeft();
                if (point.X > Pacman.Y) Pacman.GoDown();
                if (point.X < Pacman.Y) Pacman.GoUp();
            }
        }

        private void TimerTick(object? sender, ElapsedEventArgs elapsedEventArgs)
        {
            foreach (var ghost in Ghosts)
            {
                MoveGhost(ghost);
            }
        }

        private void MoveGhost(Ghost ghost)
        {
            ghost.MoveGhost();
            InvokeAsync(StateHasChanged);
        }

        private void CheckPacman(object? sender, ElapsedEventArgs e)
        {
            _gameOver = Ghosts.Any(i => i.X == Pacman.X && i.Y == Pacman.Y);
            if (_gameOver)
            {
                ghostTimer.Enabled = false;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotNetObjRef = DotNetObjectReference.Create(this);
                await JsRuntime.InvokeVoidAsync("initGame", dotNetObjRef);
            }
        }

        [JSInvokable]
        public void KeyPressed(string key)
        {
            if (_gameType != GameType.Default || _gameOver)
                return;
            switch (key)
            {
                case "ArrowDown":
                    Pacman.GoDown();
                    break;
                case "ArrowLeft":
                    Pacman.GoLeft();
                    break;
                case "ArrowRight":
                    Pacman.GoRight();
                    break;
                case "ArrowUp":
                    Pacman.GoUp();
                    break;
            }

            InvokeAsync(StateHasChanged);
        }
    }
}