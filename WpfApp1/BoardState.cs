using WpfApp1.Pieces;

namespace WpfApp1
{
    public class BoardState
    {
        public readonly BoardSquare[,] Squares = new BoardSquare[8, 8];

        public class BoardSquare
        {
            public BasePiece CurrentPiece { get; set; }
        }
    }
}
