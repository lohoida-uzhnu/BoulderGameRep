using Avalonia;
using Avalonia.Styling;

namespace BoulderGame
{
    public static class Loc
    {
        public static string Get(string key, ThemeVariant? theme = null)
        {
            if (Application.Current != null && 
                Application.Current.TryGetResource(key, theme ?? ThemeVariant.Default, out var value) && 
                value != null)
            {
                return value.ToString();
            }
            return key;
        }
    }
}