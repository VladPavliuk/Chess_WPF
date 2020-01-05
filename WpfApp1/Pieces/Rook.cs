using System;

namespace WpfApp1.Pieces
{
    public class Rook : BasePiece
    {
        public Rook(string pathToImage) : base(pathToImage)
        {
        }

        public override string Name => nameof(Rook);

        public override int[] GetAllowedMoves(BoardState board, BasePiece piece)
        {
            throw new NotImplementedException();
        }
    }
}
