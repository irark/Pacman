namespace Pacman.Models
{
    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool Visited { get; set; }
        public bool IsWall { get; set; }
    }
}