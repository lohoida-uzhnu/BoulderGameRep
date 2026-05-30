using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia;

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
        private void RefreshSecretLevelState()
        {
            var bestScore = ScoreManager.GetCurrentUserBestScore();
            
            if (bestScore >= 1000)
            {
                SecretLevelButton.IsEnabled = true;
                SecretLevelButton.Content = "Secret";
                SecretLevelHint.Text = "Secret level unlocked. Best score: " + bestScore;
            }
            else
            {
                SecretLevelButton.IsEnabled = false;
                SecretLevelButton.Content = "Secret - locked";
                SecretLevelHint.Text = $"Secret level unlocks after 1000 points. Best score: " + bestScore;
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
