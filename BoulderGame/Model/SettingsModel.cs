using System;
using System.Collections.Generic;
using System.Text;

namespace BoulderGame.Model
{
    public class AppSettings
    {
        public bool IsDarkTheme { get; set; } = false;
        public string Language { get; set; } = "English-US";
        public string SelectedHeroId { get; set; } = HeroCatalog.DefaultHeroId;
        public string SelectedBackgroundId { get; set; } = BackgroundCatalog.DefaultBackgroundId;
    }
}
