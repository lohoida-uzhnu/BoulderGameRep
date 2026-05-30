using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using BoulderGame.Model;
using BoulderGame.Rendering;

namespace BoulderGame
{
    public partial class GameScreen : Window
    {
        private Player player = new Player();
        private List<Boulder> boulders = new List<Boulder>();
        private List<Bonus> bonuses = new List<Bonus>();
        private DispatcherTimer gameTimer;
        private Random random = new Random();
        private readonly Bitmap boulderBitmap = LoadAssetBitmap("avares://BoulderGame/Assets/Boulder.png");
        private readonly Bitmap speedBonusBitmap = LoadAssetBitmap("avares://BoulderGame/Assets/Bonus_Speed.png");
        private readonly Bitmap lifeBonusBitmap = LoadAssetBitmap("avares://BoulderGame/Assets/Bonus_Life.png");
        private readonly Bitmap shieldBonusBitmap = LoadAssetBitmap("avares://BoulderGame/Assets/Bonus_Shield.png");
        private SpriteAnimator? playerRunAnimator;
        private SpriteAnimator? playerIdleAnimator;
        private readonly Image playerImage = new Image
        {
            Stretch = Stretch.Fill,
            RenderTransformOrigin = RelativePoint.Center
        };
        private readonly Ellipse shieldShape = new Ellipse
        {
            Fill = Brushes.Transparent,
            Stroke = Brushes.DeepSkyBlue,
            StrokeThickness = 4,
            IsVisible = false
        };

        private bool isGameOver = false;
        private int score = 0;
        private int frameCount = 0;
        private int lives = 3;
        private int hitDelay = 0;
        private int speedBonusTime = 0;
        private int shieldTime = 0;
        private bool shieldActive = false;

        private string difficulty = "Normal";
        private double playerSpeed = 7.0;
        private double startBoulderSpeed = 4.0;
        private double baseBoulderSpeed = 4.0;
        private int startSpawnInterval = 20;
        private int spawnInterval = 20;
        private int minSpawnInterval = 10;
        private int scoreMultiplier = 1;
        private double speedStep = 0.3;
        private int spawnStep = 1;
        private int progressLevel = 0;

        private bool moveLeft = false;
        private bool moveRight = false;
        private bool playerIsMoving = false;
        private bool wasPlayerMoving = false;
        private bool playerFacingRight = true;

        private const double GameWidth = 1000;
        private const double GameHeight = 800;

        private const double GrassHeight = 100;
        private Rect grassBounds;

        public GameScreen()
        {
            difficulty = "Normal";
            InitializeComponent();
            ApplySelectedBackground();
        }

        public GameScreen(string selectedDifficulty)
        {
            InitializeComponent();
            ApplySelectedBackground();

            difficulty = selectedDifficulty;
            SetDifficulty();

            grassBounds = new Rect(0, GameHeight - GrassHeight, GameWidth, GrassHeight);

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            gameTimer.Tick += GameLoop;

            StartGame();
        }

        private void SetDifficulty()
        {
            if (difficulty == "Easy")
            {
                playerSpeed = 8.0;
                startBoulderSpeed = 3.2;
                startSpawnInterval = 30;
                minSpawnInterval = 15;
                scoreMultiplier = 1;
                speedStep = 0.25;
                spawnStep = 1;
            }
            else if (difficulty == "Hard")
            {
                playerSpeed = 6.8;
                startBoulderSpeed = 5.2;
                startSpawnInterval = 18;
                minSpawnInterval = 8;
                scoreMultiplier = 2;
                speedStep = 0.45;
                spawnStep = 2;
            }
            else if (difficulty == "Secret")
            {
                playerSpeed = 8.5;
                startBoulderSpeed = 6.8;
                startSpawnInterval = 14;
                minSpawnInterval = 5;
                scoreMultiplier = 3;
                speedStep = 0.65;
                spawnStep = 2;
            }
            else
            {
                difficulty = "Normal";
                playerSpeed = 7.2;
                startBoulderSpeed = 4.2;
                startSpawnInterval = 24;
                minSpawnInterval = 10;
                scoreMultiplier = 1;
                speedStep = 0.35;
                spawnStep = 1;
            }
        }

        private void StartGame()
        {
            GameOverOverlay.IsVisible = false;

            player = new Player
            {
                Width = 80,
                Height = 80,
                X = GameWidth / 2 - 40,
                Y = GameHeight - GrassHeight - 80,
                Speed = playerSpeed
            };
            InitializePlayerSprite();

            boulders = new List<Boulder>();
            bonuses = new List<Bonus>();
            score = 0;
            frameCount = 0;
            lives = 3;
            hitDelay = 0;
            speedBonusTime = 0;
            shieldTime = 0;
            shieldActive = false;
            progressLevel = 0;
            isGameOver = false;
            baseBoulderSpeed = startBoulderSpeed;
            spawnInterval = startSpawnInterval;

            gameTimer.Start();
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            if (isGameOver) return;

            frameCount++;
            UpdateBonusesTime();
            UpdatePlayer();
            SpawnBoulders();
            SpawnBonuses();
            UpdateBoulders();
            UpdateBonuses();
            CheckCollisions();
            UpdateUI();
        }

        private void UpdateBonusesTime()
        {
            if (hitDelay > 0)
            {
                hitDelay--;
            }

            if (speedBonusTime > 0)
            {
                speedBonusTime--;
                player.Speed = playerSpeed + 3.0;
            }
            else
            {
                player.Speed = playerSpeed;
            }

            if (shieldTime > 0)
            {
                shieldTime--;
            }
            else
            {
                shieldActive = false;
            }
        }

        private void UpdatePlayer()
        {
            playerIsMoving = false;

            if (moveLeft && player.X > 0)
            {
                player.X -= player.Speed;
                playerIsMoving = true;
                playerFacingRight = false;
            }

            if (moveRight && player.X + player.Width < GameWidth)
            {
                player.X += player.Speed;
                playerIsMoving = true;
                playerFacingRight = true;
            }
        }

        private void SpawnBoulders()
        {
            UpdateDifficultyProgression();

            if (frameCount % spawnInterval == 0)
            {
                int boulderSize = 60;
                if (difficulty == "Secret")
                {
                    boulderSize = random.Next(40, 80);
                }

                Boulder newBoulder = new Boulder
                {
                    Width = boulderSize,
                    Height = boulderSize,
                    X = random.Next(0, (int)(GameWidth - boulderSize)),
                    Y = -boulderSize,
                    Speed = baseBoulderSpeed + (random.NextDouble() * 2)
                };
                boulders.Add(newBoulder);
            }
        }

        private void SpawnBonuses()
        {
            if (frameCount % 360 == 0)
            {
                int bonusNumber = random.Next(1, 4);
                string bonusType = "Speed";

                if (bonusNumber == 2)
                {
                    bonusType = "Life";
                }
                else if (bonusNumber == 3)
                {
                    bonusType = "Shield";
                }

                Bonus newBonus = new Bonus
                {
                    Type = bonusType,
                    Width = 35,
                    Height = 35,
                    X = random.Next(0, (int)(GameWidth - 35)),
                    Y = -35,
                    Speed = 3.0
                };

                bonuses.Add(newBonus);
            }
        }

        private void UpdateDifficultyProgression()
        {
            int newProgressLevel = score / 100;
            if (newProgressLevel == progressLevel)
            {
                return;
            }

            progressLevel = newProgressLevel;
            baseBoulderSpeed = startBoulderSpeed + (progressLevel * speedStep);
            spawnInterval = startSpawnInterval - (progressLevel * spawnStep);

            if (spawnInterval < minSpawnInterval)
            {
                spawnInterval = minSpawnInterval;
            }
        }

        private void UpdateBoulders()
        {
            for (int i = boulders.Count - 1; i >= 0; i--)
            {
                boulders[i].Y += boulders[i].Speed;

                if (boulders[i].Bounds.Intersects(grassBounds))
                {
                    boulders.RemoveAt(i);
                    score += scoreMultiplier;
                }
            }
        }

        private void UpdateBonuses()
        {
            for (int i = bonuses.Count - 1; i >= 0; i--)
            {
                bonuses[i].Y += bonuses[i].Speed;

                if (bonuses[i].Y > GameHeight)
                {
                    bonuses.RemoveAt(i);
                }
                else if (player.Bounds.Intersects(bonuses[i].Bounds))
                {
                    TakeBonus(bonuses[i].Type);
                    bonuses.RemoveAt(i);
                }
            }
        }

        private void TakeBonus(string bonusType)
        {
            if (bonusType == "Speed")
            {
                speedBonusTime = 300;
            }
            else if (bonusType == "Life")
            {
                if (lives < 5)
                {
                    lives++;
                }
            }
            else if (bonusType == "Shield")
            {
                shieldActive = true;
                shieldTime = 600;
            }
        }

        private void CheckCollisions()
        {
            if (hitDelay > 0)
            {
                return;
            }

            for (int i = boulders.Count - 1; i >= 0; i--)
            {
                if (player.Bounds.Intersects(boulders[i].Bounds))
                {
                    boulders.RemoveAt(i);

                    if (shieldActive)
                    {
                        shieldActive = false;
                        shieldTime = 0;
                    }
                    else
                    {
                        lives--;
                    }

                    hitDelay = 60;

                    if (lives <= 0)
                    {
                        GameOver();
                    }

                    break;
                }
            }
        }

        private void GameOver()
        {
            isGameOver = true;
            gameTimer.Stop();

            FinalScoreText.Text = $"Score: {score}";
            GameOverOverlay.IsVisible = true;
        }

        private void Restart_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            StartGame();
        }

        private void Stats_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var newRecord = new PlayerRecord
            {
                Username = Session.CurrentUsername,
                Score = this.score
            };

            ScoreManager.SaveScore(newRecord);

            var statsWin = new StatWin();
            statsWin.Show();
            this.Close();
        }

        private void MainMenu_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var mainWin = new MainWindow();
            mainWin.Show();
            this.Close();
        }

        private void UpdateUI()
        {
            for (int i = GameCanvas.Children.Count - 1; i >= 0; i--)
            {
                var child = GameCanvas.Children[i];
                if (child is Rectangle rect && rect.Name != "GrassLayer")
                {
                    GameCanvas.Children.RemoveAt(i);
                }
                else if (child is Image image && image.Tag?.ToString() == "GameObject")
                {
                    GameCanvas.Children.RemoveAt(i);
                }
            }

            ScoreText.Text = $"Score: {score}";
            DifficultyText.Text = $"Level: {difficulty} | Stage: {progressLevel + 1}";
            LivesText.Text = $"Lives: {lives}";
            BonusText.Text = "";

            if (shieldActive)
            {
                BonusText.Text = "Shield active";
            }
            else if (speedBonusTime > 0)
            {
                BonusText.Text = "Speed bonus";
            }

            UpdatePlayerSprite();

            foreach (var boulder in boulders)
            {
                var boulderImage = new Image
                {
                    Tag = "GameObject",
                    Width = boulder.Width,
                    Height = boulder.Height,
                    Source = boulderBitmap,
                    Stretch = Stretch.Fill
                };
                Canvas.SetLeft(boulderImage, boulder.X);
                Canvas.SetTop(boulderImage, boulder.Y);
                GameCanvas.Children.Add(boulderImage);
            }

            foreach (var bonus in bonuses)
            {
                var bonusImage = new Image
                {
                    Tag = "GameObject",
                    Width = bonus.Width,
                    Height = bonus.Height,
                    Source = GetBonusBitmap(bonus.Type),
                    Stretch = Stretch.Fill
                };

                Canvas.SetLeft(bonusImage, bonus.X);
                Canvas.SetTop(bonusImage, bonus.Y);
                GameCanvas.Children.Add(bonusImage);
            }
        }

        private Bitmap GetBonusBitmap(string bonusType)
        {
            if (bonusType == "Life")
            {
                return lifeBonusBitmap;
            }
            else if (bonusType == "Shield")
            {
                return shieldBonusBitmap;
            }
            else
            {
                return speedBonusBitmap;
            }
        }

        private void InitializePlayerSprite()
        {
            playerImage.Width = player.Width;
            playerImage.Height = player.Height;
            shieldShape.Width = player.Width + 10;
            shieldShape.Height = player.Height + 10;

            var selectedHero = GetSelectedHero();
            var runSpriteSheet = new SpriteSheet(
                selectedHero.RunAssetPath,
                frameWidth: 32,
                frameHeight: 32,
                frameCount: 6);
            var idleSpriteSheet = new SpriteSheet(
                selectedHero.IdleAssetPath,
                frameWidth: 32,
                frameHeight: 32,
                frameCount: 4);

            playerRunAnimator = new SpriteAnimator(runSpriteSheet, framesPerSecond: 12);
            playerIdleAnimator = new SpriteAnimator(idleSpriteSheet, framesPerSecond: 6);
            playerImage.Source = playerIdleAnimator.CurrentFrame;
            wasPlayerMoving = false;

            if (!GameCanvas.Children.Contains(shieldShape))
            {
                GameCanvas.Children.Add(shieldShape);
            }

            if (!GameCanvas.Children.Contains(playerImage))
            {
                GameCanvas.Children.Add(playerImage);
            }
        }

        private HeroInfo GetSelectedHero()
        {
            var settings = App.LoadSettings();
            var selectedHero = HeroCatalog.GetById(settings.SelectedHeroId);

            if (!HeroCatalog.IsUnlocked(selectedHero, ScoreManager.GetCurrentUserBestScore()))
            {
                selectedHero = HeroCatalog.GetById(HeroCatalog.DefaultHeroId);
            }

            return selectedHero;
        }

        private void ApplySelectedBackground()
        {
            var settings = App.LoadSettings();
            var selectedBackground = BackgroundCatalog.GetById(settings.SelectedBackgroundId);
            BackgroundImage.Source = new Bitmap(AssetLoader.Open(new Uri(selectedBackground.AssetPath)));
        }

        private static Bitmap LoadAssetBitmap(string assetPath)
        {
            return new Bitmap(AssetLoader.Open(new Uri(assetPath)));
        }

        private void UpdatePlayerSprite()
        {
            if (playerRunAnimator == null || playerIdleAnimator == null)
            {
                return;
            }

            SpriteAnimator currentAnimator;

            if (playerIsMoving)
            {
                currentAnimator = playerRunAnimator;
            }
            else
            {
                currentAnimator = playerIdleAnimator;
            }

            if (playerIsMoving != wasPlayerMoving)
            {
                currentAnimator.Reset();
            }

            currentAnimator.Update(true);
            playerImage.Source = currentAnimator.CurrentFrame;
            wasPlayerMoving = playerIsMoving;
            playerImage.RenderTransform = playerFacingRight ? null : new ScaleTransform(-1, 1);

            Canvas.SetLeft(playerImage, player.X);
            Canvas.SetTop(playerImage, player.Y);

            shieldShape.IsVisible = shieldActive;
            Canvas.SetLeft(shieldShape, player.X - 5);
            Canvas.SetTop(shieldShape, player.Y - 5);
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (isGameOver) return;
            if (e.Key == Key.A || e.Key == Key.Left) moveLeft = true;
            if (e.Key == Key.D || e.Key == Key.Right) moveRight = true;
        }

        private void Window_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.A || e.Key == Key.Left) moveLeft = false;
            if (e.Key == Key.D || e.Key == Key.Right) moveRight = false;
        }
    }
}
