using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sir.tech
{
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("pack://application:,,,/Assets/TileEmpty.png")),
            new BitmapImage(new Uri("pack://application:,,,/Assets/TileCyan.png")),
            new BitmapImage(new Uri("pack://application:,,,/Assets/TileBlue.png")),
            new BitmapImage(new Uri("pack://application:,,,/Assets/TileOrange.png")),
            new BitmapImage(new Uri("pack://application:,,,/Assets/TileYellow.png")),
            new BitmapImage(new Uri("pack://application:,,,/Assets/TileGreen.png")),
            new BitmapImage(new Uri("pack://application:,,,/Assets/TilePurple.png")),
            new BitmapImage(new Uri("pack://application:,,,/Assets/TileRed.png"))
        };

        private readonly Image[,] imageControls;
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 25;
        private readonly Dictionary<(int r, int c), string> placedTiles = new();

        private SolidColorBrush activeColor = new SolidColorBrush(Color.FromRgb(100, 200, 255));
        private SolidColorBrush inactiveColor = new SolidColorBrush(Color.FromRgb(50, 50, 50));

        private bool leftPressed, rightPressed, downPressed, upPressed, spacePressed;
        private ShopManager shopManager;
        private GameState gamestate = new GameState();

        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += UpdateInputOverlay;
            imageControls = SetupGameCanvas(gamestate.GameGrid);
            shopManager = new ShopManager();
            gamestate.OnBlockPlaced += OnBlockPlaced;
            UpdateCoinDisplay();
        }

        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize
                    };
                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }
            return imageControls;
        }

        private void OnBlockPlaced(Block block, List<Position> positions)
        {
            PlayerData player = PlayerData.Load();
            string blockType = block.GetType().Name.Replace("Block", "");
            string equippedId = player.EquippedSkins.ContainsKey(blockType)
                ? player.EquippedSkins[blockType]
                : $"{blockType}_Default";

            var skin = shopManager.Items
                .FirstOrDefault(i => i.BlockType == blockType)?
                .Skins.FirstOrDefault(s => s.Id == equippedId);

            string tilePath = skin?.TilePath ?? $"Assets/Tile{blockType}.png";

            foreach (var p in positions)
                placedTiles[(p.Row, p.Column)] = tilePath;
        }

        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r, c].Opacity = 1;

                    if (placedTiles.TryGetValue((r, c), out string tilePath))
                    {
                        imageControls[r, c].Source = new BitmapImage(new Uri($"pack://application:,,,/Sir.tech;component/{tilePath}", UriKind.Absolute));
                    }
                    else
                    {
                        imageControls[r, c].Source = tileImages[id];
                    }
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                PlayerData player = PlayerData.Load();
                string blockType = block.GetType().Name.Replace("Block", "");
                string equippedId = player.EquippedSkins.ContainsKey(blockType)
                    ? player.EquippedSkins[blockType]
                    : $"{blockType}_Default";

                var skin = shopManager.Items
                    .FirstOrDefault(i => i.BlockType == blockType)?
                    .Skins.FirstOrDefault(s => s.Id == equippedId);

                string tilePath = skin?.TilePath ?? $"Assets/Tile{blockType}.png";
                imageControls[p.Row, p.Column].Source = new BitmapImage(new Uri($"pack://application:,,,/Sir.tech;component/{tilePath}", UriKind.Absolute));
            }
        }

        private void DrawGhostBlock(Block block)
        {
            int dropDistance = gamestate.BlockDropDistance();
            PlayerData player = PlayerData.Load();
            string blockType = block.GetType().Name.Replace("Block", "");
            string equippedId = player.EquippedSkins.ContainsKey(blockType)
                ? player.EquippedSkins[blockType]
                : $"{blockType}_Default";

            var skin = shopManager.Items
                .FirstOrDefault(i => i.BlockType == blockType)?
                .Skins.FirstOrDefault(s => s.Id == equippedId);

            string tilePath = skin?.TilePath ?? $"Assets/Tile{blockType}.png";

            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source =
                    new BitmapImage(new Uri($"pack://application:,,,/Sir.tech;component/{tilePath}", UriKind.Absolute));
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            PlayerData player = PlayerData.Load();
            string blockType = next.GetType().Name.Replace("Block", "");
            string equippedId = player.EquippedSkins.ContainsKey(blockType)
                ? player.EquippedSkins[blockType]
                : $"{blockType}_Default";

            var skin = shopManager.Items
                .FirstOrDefault(i => i.BlockType == blockType)?
                .Skins.FirstOrDefault(s => s.Id == equippedId);

            string imagePath = skin?.ImagePath ?? $"Assets/Block-{blockType}.png";
            NextImage.Source = new BitmapImage(new Uri($"pack://application:,,,/Sir.tech;component/{imagePath}", UriKind.Absolute));
            ScoreText.Text = $"{gamestate.Score}";
        }

        private void DrawHeldBlock(Block? heldBlock)
        {
            if (heldBlock == null)
            {
                HoldImage.Source = null;
                return;
            }

            PlayerData player = PlayerData.Load();
            string blockType = heldBlock.GetType().Name.Replace("Block", "");
            string equippedId = player.EquippedSkins.ContainsKey(blockType)
                ? player.EquippedSkins[blockType]
                : $"{blockType}_Default";

            var skin = shopManager.Items
                .FirstOrDefault(i => i.BlockType == blockType)?
                .Skins.FirstOrDefault(s => s.Id == equippedId);

            string imagePath = skin?.ImagePath ?? $"Assets/Block-{blockType}.png";
            HoldImage.Source = new BitmapImage(new Uri($"pack://application:,,,/Sir.tech;component/{imagePath}", UriKind.Absolute));
        }

        private void Draw(GameState gameState)
        {
            SyncPlacedTiles(gameState.GameGrid); //keeps placedTiles consistent
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gamestate.BlockQueue);
            DrawHeldBlock(gamestate.HeldBlock);
        }


        private async Task GameLoop()
        {
            Draw(gamestate);
            while (!gamestate.GameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (gamestate.Score * delayDecrease));
                await Task.Delay(delay);
                gamestate.MoveBlockDown();
                Draw(gamestate);
            }

            PlayerData player = PlayerData.Load();
            player.Coins += gamestate.Score;
            player.Save();
            UpdateCoinDisplay();

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {gamestate.Score}";
            CoinsEarnedText.Text = $"You earned {gamestate.Score} coins!";
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gamestate = new GameState();
            gamestate.OnBlockPlaced += OnBlockPlaced;
            GameOverMenu.Visibility = Visibility.Hidden;
            placedTiles.Clear();
            UpdateCoinDisplay();
            await GameLoop();
            ScoreText.Text = "Score: 0";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (ShopOverlay.Visibility == Visibility.Visible && e.Key == Key.Escape)
            {
                CloseShop_Click(null, null);
                return;
            }

            if (gamestate.GameOver)
                return;

            switch (e.Key)
            {
                case Key.Left:
                    gamestate.MoveBlockLeft();
                    break;
                case Key.Right:
                    gamestate.MoveBlockRight();
                    break;
                case Key.Down:
                    gamestate.MoveBlockDown();
                    break;
                case Key.Up:
                    gamestate.RotateBlockCW();
                    break;
                case Key.Z:
                    gamestate.RotateBlockCCW();
                    break;
                case Key.C:
                    gamestate.HoldBlock();
                    break;
                case Key.Space:
                    gamestate.DropBlock();
                    break;
                default:
                    return;
            }
            Draw(gamestate);
        }

        private void UpdateInputOverlay(object sender, EventArgs e)
        {
            //Track key states
            leftPressed = Keyboard.IsKeyDown(Key.Left);
            rightPressed = Keyboard.IsKeyDown(Key.Right);
            downPressed = Keyboard.IsKeyDown(Key.Down);
            upPressed = Keyboard.IsKeyDown(Key.Up);
            spacePressed = Keyboard.IsKeyDown(Key.Space);

            //Update arrow key images dynamically
            if (UpKey != null)
                UpKey.Source = new BitmapImage(new Uri(
                    $"pack://application:,,,/Sir.tech;component/Assets/{(upPressed ? "KeyUp_Pressed.png" : "UpArrow.png")}",
                    UriKind.Absolute));

            if (DownKey != null)
                DownKey.Source = new BitmapImage(new Uri(
                    $"pack://application:,,,/Sir.tech;component/Assets/{(downPressed ? "KeyDown_Pressed.png" : "DownArrow.png")}",
                    UriKind.Absolute));

            if (LeftKey != null)
                LeftKey.Source = new BitmapImage(new Uri(
                    $"pack://application:,,,/Sir.tech;component/Assets/{(leftPressed ? "KeyLeft_Pressed.png" : "LeftArrow.png")}",
                    UriKind.Absolute));

            if (RightKey != null)
                RightKey.Source = new BitmapImage(new Uri(
                    $"pack://application:,,,/Sir.tech;component/Assets/{(rightPressed ? "KeyRight_Pressed.png" : "RightArrow.png")}",
                    UriKind.Absolute));

            if (SpaceKey != null)
                SpaceKey.Source = new BitmapImage(new Uri(
                    $"pack://application:,,,/Sir.tech;component/Assets/{(spacePressed ? "KeySpace_Pressed.png" : "SpaceBar.png")}",
                    UriKind.Absolute));
        }



        //SHOP SYSTEM
        private void ShopButton_Click(object sender, RoutedEventArgs e)
        {
            BuildShopUI();
            ShopOverlay.Visibility = Visibility.Visible;
            ShopOverlay.IsHitTestVisible = true;
        }

        private void CloseShop_Click(object sender, RoutedEventArgs e)
        {
            ShopOverlay.Visibility = Visibility.Collapsed;
            ShopOverlay.IsHitTestVisible = false;
            UpdateCoinDisplay();

            if (gamestate.GameOver)
            {
                GameOverMenu.Visibility = Visibility.Visible;
                GameOverMenu.IsHitTestVisible = true;
            }
        }


        private void UpdateCoinDisplay()
        {
            PlayerData player = PlayerData.Load();
            CoinText.Text = $"{player.Coins}";
        }

        private void BuildShopUI()
        {
            ShopList.Children.Clear();
            PlayerData player = PlayerData.Load();
            ShopCoinText.Text = $"{player.Coins}";

            foreach (var item in shopManager.Items)
            {
                var exp = new Expander
                {
                    Header = $"{item.BlockType} Block",
                    Margin = new Thickness(8),
                    Foreground = Brushes.White,
                    Background = Brushes.Transparent,
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    IsExpanded = false
                };

                StackPanel sectionPanel = new StackPanel();

                //Equipped
                var equippedSection = new StackPanel { Margin = new Thickness(10, 5, 0, 5) };
                TextBlock equippedHeader = new TextBlock
                {
                    Text = "Currently Equipped:",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 5, 0, 5),
                    Foreground = Brushes.LightSkyBlue
                };
                equippedSection.Children.Add(equippedHeader);

                string? equippedId = player.EquippedSkins.ContainsKey(item.BlockType)
                    ? player.EquippedSkins[item.BlockType]
                    : item.Skins.First().Id;

                var equippedSkin = item.Skins.FirstOrDefault(s => s.Id == equippedId);
                if (equippedSkin != null)
                    equippedSection.Children.Add(CreateSkinRow(equippedSkin, "Equipped", true, Brushes.Gray));

                sectionPanel.Children.Add(equippedSection);

                //Owned
                var ownedSection = new StackPanel { Margin = new Thickness(10, 5, 0, 5) };
                TextBlock ownedHeader = new TextBlock
                {
                    Text = "Owned:",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 5, 0, 5),
                    Foreground = Brushes.LightGreen
                };
                ownedSection.Children.Add(ownedHeader);

                var ownedList = item.Skins.Where(s =>
                    player.OwnedSkins.ContainsKey(item.BlockType) &&
                    player.OwnedSkins[item.BlockType].Contains(s.Id) &&
                    s.Id != equippedId);

                foreach (var skin in ownedList)
                {
                    Button equipButton = new Button
                    {
                        Content = "Equip",
                        Width = 60,
                        Height = 25,
                        Margin = new Thickness(10, 0, 0, 0)
                    };
                    equipButton.Click += (s, e) =>
                    {
                        player.EquippedSkins[item.BlockType] = skin.Id;
                        player.Save();
                        BuildShopUI();
                    };
                    ownedSection.Children.Add(CreateSkinRow(skin, "Owned", true, Brushes.LightGreen, equipButton));
                }

                sectionPanel.Children.Add(ownedSection);

                //Shop
                var shopSection = new StackPanel { Margin = new Thickness(10, 5, 0, 5) };
                TextBlock shopHeader = new TextBlock
                {
                    Text = "Available for Purchase:",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 5, 0, 5),
                    Foreground = Brushes.Orange
                };
                shopSection.Children.Add(shopHeader);

                var unownedList = item.Skins.Where(s =>
                    !player.OwnedSkins.ContainsKey(item.BlockType) ||
                    !player.OwnedSkins[item.BlockType].Contains(s.Id));

                foreach (var skin in unownedList)
                {
                    Button buyButton = new Button
                    {
                        Content = $"Buy ({skin.Price})",
                        Width = 90,
                        Height = 25,
                        Margin = new Thickness(10, 0, 0, 0)
                    };
                    buyButton.Click += (s, e) =>
                    {
                        if (shopManager.PurchaseSkin(item.BlockType, skin.Id))
                        {
                            MessageBox.Show($"Purchased {skin.DisplayName}!");
                            BuildShopUI();
                            UpdateCoinDisplay();
                        }
                        else
                        {
                            MessageBox.Show("Not enough coins or already owned!");
                        }
                    };
                    shopSection.Children.Add(CreateSkinRow(skin, "Shop", true, Brushes.Orange, buyButton));
                }

                sectionPanel.Children.Add(shopSection);
                exp.Content = sectionPanel;
                ShopList.Children.Add(exp);
            }
        }

        private void SyncPlacedTiles(GameGrid grid)
        {
            // Remove entries that correspond to cleared cells
            var toRemove = placedTiles
                .Where(kv => grid[kv.Key.r, kv.Key.c] == 0)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var key in toRemove)
                placedTiles.Remove(key);
        }



        private StackPanel CreateSkinRow(Skin skin, string tag, bool showImage, Brush labelColor, Button extraButton = null)
        {
            StackPanel row = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(5)
            };

            if (showImage)
            {
                row.Children.Add(new Image
                {
                    Source = new BitmapImage(new Uri(skin.ImagePath, UriKind.Relative)),
                    Width = 32,
                    Height = 32,
                    Margin = new Thickness(0, 0, 8, 0)
                });
            }

            row.Children.Add(new TextBlock
            {
                Text = skin.DisplayName,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 150
            });

            row.Children.Add(new TextBlock
            {
                Text = $"[{tag}]",
                Foreground = labelColor,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            });

            if (extraButton != null)
                row.Children.Add(extraButton);

            return row;
        }

        private void ShopFromGameOver_Click(object sender, RoutedEventArgs e)
        {
            GameOverMenu.Visibility = Visibility.Collapsed;
            GameOverMenu.IsHitTestVisible = false;

            BuildShopUI();
            ShopOverlay.Visibility = Visibility.Visible;
            ShopOverlay.IsHitTestVisible = true;

            ShopOverlay.Focus();
        }



    }
}
