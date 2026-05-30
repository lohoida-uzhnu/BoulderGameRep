using Avalonia;
using System.Text.Json.Serialization;

namespace BoulderGame.Model 
{
    public class GameObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        [JsonIgnore]
        public Rect Bounds => new Rect(X, Y, Width, Height);
    }

    public class Player : GameObject
    {
        public double Speed { get; set; } = 7.0;
    }

    public class  PlayerRecord
    {
        public int Id { get; set; }
        public string Username { get; set; } = "Player";
        public int Score { get; set; }
        
    }

    public static class Session
    {
        public static string CurrentUsername { get; set; } = "Player";
    }

    public class Boulder : GameObject
    {
        public double Speed { get; set; }
    }

    public class Bonus : GameObject
    {
        public string Type { get; set; } = "";
        public double Speed { get; set; }
    }
}
