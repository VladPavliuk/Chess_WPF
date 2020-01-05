using System;

namespace WpfApp1.Pieces
{
    public class Knight : BasePiece
    {
        public Knight(string pathToImage) : base(pathToImage)
        {
        }

        public override string Name => nameof(King);

        public override int[] GetAllowedMoves(BoardState board, BasePiece piece)
        {
            throw new NotImplementedException();
        }
    }
}
