using System;
using System.Windows.Media.Imaging;

namespace WpfApp1.Pieces
{
    public abstract class BasePiece
    {
        public abstract string Name { get; }

        public bool AlreadyMoved { get; protected set; }

        public BitmapImage Image { get; private set; }

        public BasePiece(string pathToImage)
        {
            Image = new BitmapImage(new Uri(pathToImage));
        }

        public abstract int[] GetAllowedMoves(BoardState board, BasePiece piece);
    }
}
