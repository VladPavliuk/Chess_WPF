using System.Collections.Generic;
using System.Linq;

namespace ChessWpf.Pieces
{
    public class Rook : BasePiece
    {
        public Rook(Player controlledBy) : base(controlledBy)
        {
        }

        public override string Name => nameof(Rook);

        public override string ImageName => "rook";

        public override List<(int y, int x)> GetAllowedMoves(BoardState board)
        {
            var allowedMoves = new List<(int y, int x)>();

            GetLineMoves(board, allowedMoves, (1, 0));
            GetLineMoves(board, allowedMoves, (0, 1));
            GetLineMoves(board, allowedMoves, (0, -1));
            GetLineMoves(board, allowedMoves, (-1, 0));

            ApplyTransformations(board, allowedMoves);

            return allowedMoves;
        }
    }
}
