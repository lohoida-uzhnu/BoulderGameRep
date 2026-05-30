using System.Collections.Generic;
using System.Linq;

namespace BoulderGame.Model
{
    public class HeroInfo
    {
        public string Id { get; init; } = "";
        public string Name { get; init; } = "";
        public string RunAssetPath { get; init; } = "";
        public string IdleAssetPath { get; init; } = "";
        public int RequiredScore { get; init; }
    }

    public static class HeroCatalog
    {
        public const string DefaultHeroId = "pink";

        public static IReadOnlyList<HeroInfo> All { get; } = new List<HeroInfo>
        {
            new HeroInfo
            {
                Id = "pink",
                Name = "Pink Monster",
                RunAssetPath = "avares://BoulderGame/Assets/Pink_Monster_Run_6.png",
                IdleAssetPath = "avares://BoulderGame/Assets/Pink_Monster_Idle_4.png",
                RequiredScore = 0
            },
            new HeroInfo
            {
                Id = "owlet",
                Name = "Owlet Monster",
                RunAssetPath = "avares://BoulderGame/Assets/Owlet_Monster_Run_6.png",
                IdleAssetPath = "avares://BoulderGame/Assets/Owlet_Monster_Idle_4.png",
                RequiredScore = 100
            },
            new HeroInfo
            {
                Id = "dude",
                Name = "Dude Monster",
                RunAssetPath = "avares://BoulderGame/Assets/Dude_Monster_Run_6.png",
                IdleAssetPath = "avares://BoulderGame/Assets/Dude_Monster_Idle_4.png",
                RequiredScore = 300
            }
        };

        public static HeroInfo GetById(string? id)
        {
            return All.FirstOrDefault(hero => hero.Id == id) ?? All[0];
        }

        public static bool IsUnlocked(HeroInfo hero, int bestScore)
        {
            return bestScore >= hero.RequiredScore;
        }
    }
}
