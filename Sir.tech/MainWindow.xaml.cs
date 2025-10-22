using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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


        private readonly ImageSource[] blockImages = new ImageSource[]
        {
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-Empty.png")),
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-I.png")),
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-J.png")),
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-L.png")),
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-O.png")),
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-S.png")),
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-T.png")),
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-Z.png"))
        };

        private readonly Image[,] imageControls;
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 25;

        private GameState gamestate = new GameState();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gamestate.GameGrid);
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

        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

          private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.Id];
            ScoreText.Text = $"Score: {gamestate.Score}";
        } 

        private void DrawHeldBlock(Block? heldBlock)
        {
            if (heldBlock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.Id];
            }
        }

        private void DrawGhostBlock(Block block)
        {
            int dropDistance = gamestate.BlockDropDistance();
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.Id];
            }
        }

        private void Draw(GameState gameState)
        {
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

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {gamestate.Score}";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gamestate.GameOver)
            {
                return;
            }
                    //Inputs
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

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }


        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gamestate = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
            ScoreText.Text = "Score: 0";
        }

    }
}
