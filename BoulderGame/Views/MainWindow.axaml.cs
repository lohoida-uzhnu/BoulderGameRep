using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia;
using Avalonia.Markup.Xaml.MarkupExtensions;

namespace BoulderGame
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RefreshSecretLevelState();
        }
        public void StartGameBut_Click(object? sender, RoutedEventArgs e)
        {
            RefreshSecretLevelState();
            DifficultyOverlay.IsVisible = true;
        }
        private void DifficultyButton_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not string difficultyName)
            {
                return;
            }

            if (difficultyName == "Secret" && !ScoreManager.IsSecretLevelUnlocked())
            {
                RefreshSecretLevelState();
                return;
            }

            var gameW = new GameScreen(difficultyName);
            gameW.Show();
            this.Hide();
        }
        private void CloseDifficultyMenu_Click(object? sender, RoutedEventArgs e)
        {
            DifficultyOverlay.IsVisible = false;
        }
        private string GetText(string key)
        {
            if (Application.Current != null && 
                Application.Current.TryGetResource(key, this.ActualThemeVariant, out var value) && 
                value != null)
            {
                return value.ToString();
            }₴₴
            return key;
        }
        private void RefreshSecretLevelState()
        {
            var bestScore = ScoreManager.GetCurrentUserBestScore();
            bool isUnlocked = bestScore >= 1000;

            SecretLevelButton.IsEnabled = isUnlocked;
            
            if (isUnlocked)
            {
                SecretLevelButton.Content = GetText("SecretBtnU");
                SecretLevelHint.Text = string.Format(GetText("SecretHintU"), bestScore);
            }
            else
            {
                SecretLevelButton.Content = GetText("SecretBtnL");
                SecretLevelHint.Text = string.Format(GetText("SecretHintL"), bestScore);
            }
        }
        
        public void StatButton_Click(object? sender, RoutedEventArgs e)
        {
            var statW = new StatWin();
            this.Hide();
            statW.Show();
        }
        public void SettingBut_Click(object? sender, RoutedEventArgs e)
        {
            var settingW = new SettingWin();
            this.Hide();
            settingW.Closed += (s, args) => this.Show();
            settingW.Show();
        }
        public void ExitButton_Click(object? sender, RoutedEventArgs e)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }
    }
}
