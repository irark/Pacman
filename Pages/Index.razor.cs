using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Pacman.Models;

namespace Pacman.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] public IJSRuntime JsRuntime { get; set; }
        public Models.Pacman Pacman { get; set; }
        public List<Ghost> Ghosts { get; set; }
        public List<List<Cell>> Maze { get; set; } = new (30);
        protected override void OnInitialized()
        {
            var lines = System.IO.File.ReadLines(@"Mazes\maze1.txt")?.ToList() ?? new List<string>();
            //maze = maze.Replace("\r", "").Replace("\n", "");
            for (var i = 0; i < lines.Count(); i++)
            {
                lines[i] = lines[i].Replace("\r", "").Replace("\n", "");
                Maze.Add(new List<Cell>( lines[i].Length));
                for (var j = 0; j < lines[i].Length; j++)
                {
                    Maze[i].Add(new Cell() {X = j, Y = i, IsWall = lines[i][j] == '1'});
                }
            }

            Pacman = new (1, 1, Maze);
            Ghosts = new()
            {
                new Ghost(10, 12, Maze),
                new Ghost(15, 4, Maze),
                new Ghost(6, 21, Maze),
                new Ghost(7, 15, Maze),
                new Ghost(18, 25, Maze)
            };
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
                default:
                    break;
            }
            StateHasChanged();
        }
    }
}