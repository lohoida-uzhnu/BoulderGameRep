using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using BoulderGame.Model;

namespace BoulderGame;

public partial class SettingWin : Window
{
    private AppSettings settings = new AppSettings();

    public SettingWin()
    {
        InitializeComponent();

        settings = App.LoadSettings();
        ThemeSwitch.IsChecked = settings.IsDarkTheme;
        SelectCurrentLanguage();
        RefreshHeroButtons();
        RefreshBackgroundButtons();
    }

    public void ThemeSwitch_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (ThemeSwitch != null)
        {
            bool isDark = ThemeSwitch.IsChecked == true;
            Application.Current!.RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;

            settings.IsDarkTheme = isDark;
            settings.Language = GetCurrentLanguage();
            SaveCurrentSettings();
        }
    }

    private void LangComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox &&
            comboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            string? langCode = selectedItem.Tag?.ToString();

            if (!string.IsNullOrEmpty(langCode))
            {
                App.SetLanguage(langCode);

                settings.Language = langCode;
                settings.IsDarkTheme = Application.Current!.RequestedThemeVariant == ThemeVariant.Dark;
                SaveCurrentSettings();
            }
        }
    }

    private string GetCurrentLanguage()
    {
        return (LangComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? settings.Language;
    }

    public void BackToMenu_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void SelectCurrentLanguage()
    {
        foreach (var item in LangComboBox.Items)
        {
            if (item is ComboBoxItem comboBoxItem &&
                comboBoxItem.Tag?.ToString() == settings.Language)
            {
                LangComboBox.SelectedItem = comboBoxItem;
                return;
            }
        }
    }

    private void RefreshHeroButtons()
    {
        HeroPanel.Children.Clear();

        int bestScore = ScoreManager.GetCurrentUserBestScore();
        BestScoreText.Text = $"Best score: {bestScore}";

        var selectedHero = HeroCatalog.GetById(settings.SelectedHeroId);
        if (!HeroCatalog.IsUnlocked(selectedHero, bestScore))
        {
            settings.SelectedHeroId = HeroCatalog.DefaultHeroId;
            SaveCurrentSettings();
        }

        foreach (var hero in HeroCatalog.All)
        {
            bool unlocked = HeroCatalog.IsUnlocked(hero, bestScore);
            bool selected = settings.SelectedHeroId == hero.Id;
            string buttonContent;

            if (selected)
            {
                buttonContent = $"{hero.Name} - selected";
            }
            else if (unlocked)
            {
                buttonContent = hero.Name;
            }
            else
            {
                buttonContent = $"{hero.Name} - need {hero.RequiredScore} points";
            }

            var button = new Button
            {
                Tag = hero.Id,
                IsEnabled = unlocked,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Content = buttonContent
            };

            button.Click += HeroButton_Click;
            HeroPanel.Children.Add(button);
        }
    }

    private void HeroButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string heroId)
        {
            return;
        }

        settings.SelectedHeroId = heroId;
        SaveCurrentSettings();
        RefreshHeroButtons();
    }

    private void RefreshBackgroundButtons()
    {
        BackgroundPanel.Children.Clear();

        foreach (var background in BackgroundCatalog.All)
        {
            bool selected = settings.SelectedBackgroundId == background.Id;
            string buttonContent;

            if (selected)
            {
                buttonContent = $"{background.Name} - selected";
            }
            else
            {
                buttonContent = background.Name;
            }

            var button = new Button
            {
                Tag = background.Id,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Content = buttonContent
            };

            button.Click += BackgroundButton_Click;
            BackgroundPanel.Children.Add(button);
        }
    }

    private void BackgroundButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string backgroundId)
        {
            return;
        }

        settings.SelectedBackgroundId = backgroundId;
        SaveCurrentSettings();
        RefreshBackgroundButtons();
    }

    private void SaveCurrentSettings()
    {
        App.SaveSettings(
            settings.Language,
            settings.IsDarkTheme,
            settings.SelectedHeroId,
            settings.SelectedBackgroundId);
    }
}
