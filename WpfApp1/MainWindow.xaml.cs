using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.Pieces;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BoardState Board { get; set; }

        private BasePiece ClickedPiece { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitBoardState();
            DrawBoard();
            DrawPieces();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var clickedPoint = Mouse.GetPosition(CanvasElement);

            var x = (int)(clickedPoint.X * 8 / CanvasElement.Width);
            var y = (int)(clickedPoint.Y * 8 / CanvasElement.Height);

            if (ClickedPiece == null)
            {
                ClickedPiece = Board.Squares[y, x].CurrentPiece;
            }
            else
            {
                var moves = ClickedPiece.GetAllowedMoves(Board).ToArray();

                if (moves.Any(_ => _[0] == y && _[1] == x))
                {
                    ClickedPiece.AlreadyMoved = true;
                    var location = Board.GetPieceLocation(ClickedPiece);

                    Board.Squares[y, x].CurrentPiece = ClickedPiece;
                    Board.Squares[location.y, location.x].CurrentPiece = null;
                }

                ClickedPiece = null;
            }

            DrawBoard();
            DrawPieces();
        }

        private void InitBoardState()
        {
            Board = new BoardState();
        }

        private void DrawPieces()
        {
            Func<BitmapImage, Image> getImage = (BitmapImage image) =>
            new Image()
            {
                Width = CanvasElement.Width / 8,
                Height = CanvasElement.Height / 8,
                Source = image
            };

            var unit = (int)CanvasElement.Height / 8;

            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    if (Board.Squares[i, j].CurrentPiece == null)
                    {
                        continue;
                    }

                    var pieceImage = getImage(Board.Squares[i, j].CurrentPiece.Image);

                    Canvas.SetTop(pieceImage, i * unit);
                    Canvas.SetLeft(pieceImage, j * unit);

                    CanvasElement.Children.Add(pieceImage);
                }
            }
        }

        private void DrawBoard()
        {
            if (CanvasElement.Height != CanvasElement.Width || CanvasElement.Height % 8 != 0)
            {
                throw new Exception("Wrong canvas size!");
            }

            var unit = (int)CanvasElement.Height / 8;

            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var square = new Rectangle()
                    {
                        Fill = new SolidColorBrush((i + j) % 2 == 0 ? Colors.Brown : Colors.White),
                        Width = (int)CanvasElement.Height / 8,
                        Height = (int)CanvasElement.Height / 8
                    };

                    Canvas.SetTop(square, i * unit);
                    Canvas.SetLeft(square, j * unit);

                    CanvasElement.Children.Add(square);
                }
            }
        }
    }
}
