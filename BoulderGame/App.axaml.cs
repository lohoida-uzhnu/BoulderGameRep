using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using BoulderGame.Model;
using System;
using System.IO;
using System.Text.Json;

namespace BoulderGame;

public partial class App : Application
{
    public static string settingsPath = "settings.json";

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        string currentLang = "English-US";
        bool isDark = false;

        var loadedSettings = LoadSettings();
        currentLang = loadedSettings.Language;
        isDark = loadedSettings.IsDarkTheme;

        RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;
        SetLanguage(currentLang);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new AuthWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static void SetLanguage(string langCode)
    {
        var app = Application.Current;
        if (app != null && app.Resources.MergedDictionaries.Count > 0)
        {
            try
            {
                var uri = new Uri($"avares://BoulderGame/Resources/Languages/{langCode}.axaml");
                var newLang = new Avalonia.Markup.Xaml.Styling.ResourceInclude(uri) { Source = uri };
                app.Resources.MergedDictionaries[0] = newLang;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading language: {ex.Message}");

                if (langCode != "English-US") SetLanguage("English-US");
            }
        }
    }

    public static void SaveSettings(string lang, bool isDark)
    {
        var currentSettings = LoadSettings();
        SaveSettings(lang, isDark, currentSettings.SelectedHeroId, currentSettings.SelectedBackgroundId);
    }

    public static void SaveSettings(string lang, bool isDark, string selectedHeroId)
    {
        var currentSettings = LoadSettings();
        SaveSettings(lang, isDark, selectedHeroId, currentSettings.SelectedBackgroundId);
    }

    public static void SaveSettings(string lang, bool isDark, string selectedHeroId, string selectedBackgroundId)
    {
        var settingsObject = new AppSettings
        {
            Language = lang,
            IsDarkTheme = isDark,
            SelectedHeroId = selectedHeroId,
            SelectedBackgroundId = selectedBackgroundId
        };
        string json = JsonSerializer.Serialize(settingsObject);

        File.WriteAllText(settingsPath, json);
    }

    public static AppSettings LoadSettings()
    {
        if (!File.Exists(settingsPath))
        {
            return new AppSettings();
        }

        try
        {
            string json = File.ReadAllText(settingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            return new AppSettings();
        }
    }
}
