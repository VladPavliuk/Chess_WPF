using System;

namespace WpfApp1.Pieces
{
    public class Queen : BasePiece
    {
        public Queen(string pathToImage) : base(pathToImage)
        {
        }

        public override string Name => nameof(Queen);

        public override int[] GetAllowedMoves(BoardState board, BasePiece piece)
        {
            throw new NotImplementedException();
        }
    }
}
