using ChessBreaker.Pieces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChessBreaker.WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BoardState Board { get; set; }

        private readonly Dictionary<(Type, Player), BitmapImage> PiecesImages = new Dictionary<(Type, Player), BitmapImage>();

        public MainWindow()
        {
            var pieceTypes = new Type[] { typeof(Bishop), typeof(King), typeof(Knight), typeof(Pawn), typeof(Queen), typeof(Rook) };
            var players = new Player[] { Player.White, Player.Black };

            var imagesBasePath = System.IO.Path.GetFullPath(ConfigurationManager.AppSettings["ImagesBasePath"]);
            
            Func<string, string, BitmapImage> getImage = (string playerName, string pieceName) =>
            {
                return new BitmapImage(new Uri(System.IO.Path.Combine(imagesBasePath, "Pieces", playerName, $"{playerName.ToLower()}_{pieceName.ToLower()}.png")));
            };

            foreach (var player in players)
            {
                foreach (var pieceType in pieceTypes) {
                    PiecesImages.Add((pieceType, player), getImage(player.ToString(), pieceType.Name));
                }
            }

            InitializeComponent();
            InitBoardState();
            DrawBoard();
            DrawPieces();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DrawBoard();

            var clickedPoint = Mouse.GetPosition(CanvasElement);

            var x = (int)(clickedPoint.X * 8 / CanvasElement.Width);
            var y = (int)(clickedPoint.Y * 8 / CanvasElement.Height);

            Board.UpdatePieces(y, x);

            DrawPieces();

            Board.IsEndGame();
        }

        private void InitBoardState()
        {
            Board = new BoardState()
            {
                OnPieceClick = (BasePiece clickedPiece, List<(int y, int x)> moves) =>
                {
                    DrawShadowPieces(clickedPiece, moves);
                },
                CheckmateHandler = (Player player) =>
                {
                    MessageBox.Show(player.ToString() + " won!");
                },
                DrawHandler = () =>
                {
                    MessageBox.Show("DRAW!");
                },
            };
        }

        private void DrawShadowPieces(BasePiece shadowPiece, List<(int y, int x)> possibleMoves)
        {
            Func<BitmapImage, Image> getImage = (BitmapImage image) =>
               new Image()
               {
                   Width = CanvasElement.Width / 8,
                   Height = CanvasElement.Height / 8,
                   Source = image
               };

            var unit = (int)CanvasElement.Height / 8;

            foreach (var (y, x) in possibleMoves)
            {
                if (Board.Squares[y, x] != null)
                {
                    var pieceBorder = new Border();
                    pieceBorder.Background = new SolidColorBrush(Colors.Red);

                    var piece = Board.Squares[y, x];
                    pieceBorder.Child = getImage(PiecesImages[(piece.GetType(), piece.ControlledBy)]);

                    Canvas.SetTop(pieceBorder, y * unit);
                    Canvas.SetLeft(pieceBorder, x * unit);

                    CanvasElement.Children.Add(pieceBorder);
                }
                else
                {
                    var pieceImage = getImage(PiecesImages[(shadowPiece.GetType(), shadowPiece.ControlledBy)]);

                    pieceImage.Opacity = 0.3;

                    Canvas.SetTop(pieceImage, y * unit);
                    Canvas.SetLeft(pieceImage, x * unit);

                    CanvasElement.Children.Add(pieceImage);
                }
            }
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
                    if (Board.Squares[i, j] == null) continue;

                    var piece = Board.Squares[i, j];
                    var pieceImage = getImage(PiecesImages[(piece.GetType(), piece.ControlledBy)]);

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
