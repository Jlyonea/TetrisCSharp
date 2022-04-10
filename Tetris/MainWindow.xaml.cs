using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetris
{


    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[] //aqui é criado um array para armazenar  os blocos
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative)),
        };
        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative)),
        };

        private readonly Image[,] imageControls; //controlador do bloco descendo
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 25;


        private GameState gameState = new GameState();

        public MainWindow()
        { //
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid); 
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
                    { // Seta o formato do mapa
                        Width = cellSize,
                        Height = cellSize,
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);//para o bloco spawnar inteiro
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }
            return imageControls;
        }

        private void DrawGrid(GameGrid grid)
        { //  Desenha o grid com as imagens
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r, c].Source = tileImages[id];
                    imageControls[r, c].Opacity = 1;
                }
            }
        }

        private void DrawBlock(Block block)
        { // Desenha os blocos
            foreach(Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {  // Faz aparecer qual será o proximo bloco na interface
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.Id];
        }

        public void DrawHeldBlock(Block heldblock)
        { //Faz aparecer o bloco que está sendo segurado na interface.
            if(heldblock == null)
            {
                HoldImage.Source = blockImages[0];
            }
            else
            {
                HoldImage.Source = blockImages[heldblock.Id];
            }
        }

        private void DrawGhostBlock(Block block)
        { // Faz com que apareça onde o bloco cairá.
            int dropDistance = gameState.BlockDropDistance();

            foreach(Position p in block.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.Id];
            }
        }
        private void Draw(GameState gameState)
        { //Serve para puxar todas as funções Draw.
            DrawGrid(gameState.GameGrid);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            DrawGhostBlock(gameState.CurrentBlock);
            ScoreText.Text = $"Score: { gameState.Score}";
        }


        private async Task GameLoop()
        { //Faz com que o play again funcione, configura a velocidade 
            //em que as peças descem e faz aparecer o game over com o score
            Draw(gameState); //desenha o jogo

            while (!gameState.GameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (gameState.Score * delayDecrease * 2)); 
                await Task.Delay(500);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {gameState.Score}";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {  //Comandos para o jogo
            if (gameState.GameOver)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRIght();
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();
                    break;
                case Key.Z:
                    gameState.RotateBlockCCW();
                    break;
                case Key.C:
                    gameState.HoldBlock();
                    break;
                case Key.Space:
                    gameState.DropBlock();
                    break;
                default:
                    return;
            }

            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        { //faz com que o jogo permita o loop de continuar jogando, play again
            await GameLoop();
        }
       
        private async void PlayAgain_Click(Object sender, RoutedEventArgs e)
        { //Gera um jogo novo ao clickar no play again.
            gameState = new GameState();
            GameOverMenu.Visibility=Visibility.Hidden;
            await GameLoop();
        }


    }
}
