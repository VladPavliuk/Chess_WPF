using System;

namespace WpfApp1.Pieces
{
    public class King : BasePiece
    {
        public King(string pathToImage) : base(pathToImage)
        {
        }

        public override string Name => nameof(King);

        public override int[] GetAllowedMoves(BoardState board, BasePiece piece)
        {
            throw new NotImplementedException();
        }
    }
}
