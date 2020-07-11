using System.Collections.Generic;

namespace ChessBreaker.Pieces
{
    public class Queen : BasePiece
    {
        public Queen(Player controlledBy) : base(controlledBy)
        {
        }

        public override string Name => nameof(Queen);

        public override List<(int y, int x)> GetAllowedMoves(BoardState board)
        {
            var allowedMoves = new List<(int y, int x)>();

            GetLineMoves(board, allowedMoves, (1, 1));
            GetLineMoves(board, allowedMoves, (-1, 1));
            GetLineMoves(board, allowedMoves, (1, -1));
            GetLineMoves(board, allowedMoves, (-1, -1));
            GetLineMoves(board, allowedMoves, (1, 0));
            GetLineMoves(board, allowedMoves, (0, 1));
            GetLineMoves(board, allowedMoves, (0, -1));
            GetLineMoves(board, allowedMoves, (-1, 0));

            ApplyTransformations(board, ref allowedMoves);

            return allowedMoves;
        }
    }
}
