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
        private Models.Pacman Pacman { get; set; }
        private List<Ghost> Ghosts { get; set; } = new();
        private int CountOfGhosts { get; set; } = 5;
        private List<List<Cell>> Maze { get; set; } = new(30);
        private List<List<Cell>> ReverseMaze { get; set; } = new(30);
        private int[,] SimpleMaze { get; set; } = new int[30, 30];
        public Point Start { get; set; }
        public Point End { get; set; }
        private bool _gameOver;
        private GameType _gameType = GameType.Default;
        private Timer timer = new();

        protected override void OnInitialized()
        {
            var lines = System.IO.File.ReadLines(@"Mazes\maze1.txt")?.ToList() ?? new List<string>();
            for (var i = 0; i < lines.Count(); i++)
            {
                lines[i] = lines[i].Replace("\r", "").Replace("\n", "");
                Maze.Add(new List<Cell>(lines[i].Length));
                ReverseMaze.Add(new List<Cell>(lines[i].Length));
                for (var j = 0; j < lines[i].Length; j++)
                {
                    Maze[i].Add(new Cell() {X = j, Y = i, IsWall = lines[i][j] == '1'});
                    ReverseMaze[i].Add(new Cell() {X = i, Y = j, IsWall = lines[i][j] == '1'});
                    SimpleMaze[i, j] = lines[i][j] == '1' ? 1 : 0;
                }
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

        private async Task StartGame()
        {
            Maze.ForEach(i => i.ForEach(i => i.Visited = false));
            Ghosts = new List<Ghost>();
            Start = new Point();
            End = new Point();
            _gameOver = false;
            await InvokeAsync(StateHasChanged);
            switch (_gameType)
            {
                case GameType.Default:
                    StartDefaultGame();
                    break;
                case GameType.AStar:
                    await StartAStarGame();
                    break;
                case GameType.Greedy:
                    await StartGreedyGame();
                    break;
            }
        }

        private void StartDefaultGame()
        {
            Pacman = new(1, 1, Maze);
            for (var i = 0; i < CountOfGhosts; i++)
            {
                var ghost = new Ghost(Maze);
                ghost.OnMoved = () =>
                {
                    CheckPacman();
                    InvokeAsync(StateHasChanged);
                };
                Ghosts.Add(ghost);
            }

            // timer.Interval = 1;
            // timer.Enabled = true;
            // timer.Elapsed += CheckPacman;
            Pacman.OnMoved = () =>
            {
                CheckPacman();
                InvokeAsync(StateHasChanged);
            };
            Pacman.Start();
            Ghosts.ForEach(i => i.StartGhost());
        }


        private async Task StartAStarGame()
        {
            List<Point> res = null;
            while (res == null)
            {
                Maze.ForEach(i => i.ForEach(i => i.Visited = false));
                Start = GeneratePoint();
                End = GeneratePoint();
                Pacman = new(Start.Y, Start.X, Maze);
                res = AStarSolver.FindPath(SimpleMaze, Start, End);
            }

            foreach (var point in res)
            {
                if (point.Y > Pacman.X)
                    Pacman.Direction = DirectionType.Right;
                if (point.Y < Pacman.X)
                    Pacman.Direction = DirectionType.Left;
                if (point.X > Pacman.Y)
                    Pacman.Direction = DirectionType.Down;
                if (point.X < Pacman.Y)
                    Pacman.Direction = DirectionType.Up;
                Pacman.Move(null, null);
                await InvokeAsync(StateHasChanged);
                await Task.Delay(300);
            }
        }

        private async Task StartGreedyGame()
        {
            List<Point> res = null;
            while (res == null)
            {
                Start = GeneratePoint();
                End = GeneratePoint();
                Pacman = new(Start.Y, Start.X, Maze);

                res = GreedySolver.FindPath(new Maze(ReverseMaze), Start, End);
            }

            foreach (var point in res)
            {
                if (point.Y > Pacman.X)
                    Pacman.Direction = DirectionType.Right;
                if (point.Y < Pacman.X)
                    Pacman.Direction = DirectionType.Left;
                if (point.X > Pacman.Y)
                    Pacman.Direction = DirectionType.Down;
                if (point.X < Pacman.Y)
                    Pacman.Direction = DirectionType.Up;
                Pacman.Move(null, null);
                await InvokeAsync(StateHasChanged);
                await Task.Delay(300);
            }
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

        private void CheckPacman()
        {
            if (_gameOver)
                return;
            _gameOver = Ghosts.Any(i => i.X == Pacman.X && i.Y == Pacman.Y);
            if (_gameOver)
            {
                Pacman.Stop();
                Ghosts.ForEach(i => i.StopGhost());
                InvokeAsync(StateHasChanged);
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
                    Pacman.Direction = DirectionType.Down;
                    break;
                case "ArrowLeft":
                    Pacman.Direction = DirectionType.Left;
                    break;
                case "ArrowRight":
                    Pacman.Direction = DirectionType.Right;
                    break;
                case "ArrowUp":
                    Pacman.Direction = DirectionType.Up;
                    break;
            }

            InvokeAsync(StateHasChanged);
        }
    }
}