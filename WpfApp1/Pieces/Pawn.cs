using System;

namespace WpfApp1.Pieces
{
    public class Pawn : BasePiece
    {
        public Pawn(string pathToImage) : base(pathToImage)
        {
        }

        public override string Name => nameof(Pawn);

        public override int[] GetAllowedMoves(BoardState board, BasePiece piece)
        {
            throw new NotImplementedException();
        }
    }
}
