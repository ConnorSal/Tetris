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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-I.png")),
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-J.png")),
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-L.png")),
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-O.png")),
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-S.png")),
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-T.png")),
    new BitmapImage(new Uri("pack://application:,,,/Assets/Block-Z.png"))
        };

        private readonly Image[,] imageControls;

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

                    Canvas.SetTop(imageControl, (r - 2) * cellSize);
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
                    imageControls[r, c].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawBlock(gameState.CurrentBlock);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Example input handling
            switch (e.Key)
            {
                case Key.Left:
                    // Move piece left
                    break;
                case Key.Right:
                    // Move piece right
                    break;
                case Key.Down:
                    // Move piece down faster
                    break;
                case Key.Up:
                    // Rotate piece
                    break;
            }
        }

        private void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Draw(gamestate);
        }


        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            // Example: hide the GameOver menu and reset your game
            GameOverMenu.Visibility = Visibility.Hidden;

            // Call your game restart logic here
            // For example, if you have a GameState object that manages everything:
            // gameState.Reset();
            // Or recreate the current block, clear lines, etc.

            ScoreText.Text = "Score: 0";
        }

    }
}