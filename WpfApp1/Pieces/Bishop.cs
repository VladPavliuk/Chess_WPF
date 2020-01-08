using System.Collections.Generic;
using System.Linq;

namespace ChessWpf.Pieces
{
    public class Bishop : BasePiece
    {
        public Bishop(Player controlledBy) : base(controlledBy)
        {
        }

        public override string Name => nameof(Bishop);

        public override string ImageName => "bishop";

        public override List<(int y, int x)> GetAllowedMoves(BoardState board)
        {
            var allowedMoves = new List<(int y, int x)>();

            GetLineMoves(board, allowedMoves, (1, 1));
            GetLineMoves(board, allowedMoves, (-1, 1));
            GetLineMoves(board, allowedMoves, (1, -1));
            GetLineMoves(board, allowedMoves, (-1, -1));

            ApplyTransformations(board, allowedMoves);

            return allowedMoves;
        }
    }
}
