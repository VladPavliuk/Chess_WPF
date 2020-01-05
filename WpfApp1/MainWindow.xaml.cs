using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
using WpfApp1.Pieces;
using Path = System.IO.Path;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BoardState Board { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitBoardState();
            //DrawBoard();
            DrawPieces();



        }

        void InitBoardState()
        {
            Board = new BoardState();

            Board.Squares[0, 0] = new Rook()
        }

        void DrawPieces()
        {
            var imagesBasePath = ConfigurationManager.AppSettings["ImagesBasePath"];

            var sadf = , "black_bishop.png"));

            Func<string, Image> getImagePath = (string imageName) =>
            new Image()
            {
                Width = CanvasElement.Width / 8,
                Height = CanvasElement.Height / 8,
                Source = new BitmapImage(new Uri(Path.Combine(imagesBasePath, imageName)))
            };



            Canvas.SetTop(i, 10);
            Canvas.SetLeft(i, 10);
            CanvasElement.Children.Add(i);
        }

        void DrawBoard()
        {
            if (CanvasElement.Height != CanvasElement.Width || CanvasElement.Height % 8 != 0)
                throw new Exception("Wrong canvas size!");

            var unit = (int)CanvasElement.Height / 8;

            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var square = new Rectangle()
                    {
                        Fill = new SolidColorBrush((i + j) % 2 == 0 ? Colors.Black : Colors.White),
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
