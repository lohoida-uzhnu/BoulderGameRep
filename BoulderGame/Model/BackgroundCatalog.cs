using System.Collections.Generic;
using System.Linq;

namespace BoulderGame.Model
{
    public class BackgroundInfo
    {
        public string Id { get; init; } = "";
        public string Name { get; init; } = "";
        public string AssetPath { get; init; } = "";
    }

    public static class BackgroundCatalog
    {
        public const string DefaultBackgroundId = "classic";

        public static IReadOnlyList<BackgroundInfo> All { get; } = new List<BackgroundInfo>
        {
            new BackgroundInfo
            {
                Id = "classic",
                Name = "Classic",
                AssetPath = "avares://BoulderGame/Assets/background.png"
            },
            new BackgroundInfo
            {
                Id = "tree",
                Name = "Big Tree",
                AssetPath = "avares://BoulderGame/Assets/Background_Tree.png"
            },
            new BackgroundInfo
            {
                Id = "field",
                Name = "Cloud Field",
                AssetPath = "avares://BoulderGame/Assets/Background_Field.png"
            }
        };

        public static BackgroundInfo GetById(string? id)
        {
            return All.FirstOrDefault(background => background.Id == id) ?? All[0];
        }
    }
}
