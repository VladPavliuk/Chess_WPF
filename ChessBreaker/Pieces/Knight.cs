using ChessBreaker.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ChessBreaker.Pieces
{
    public class Knight : BasePiece
    {
        public Knight(Player controlledBy) : base(controlledBy)
        {
        }

        public override string Name => nameof(Knight);

        public override List<(int y, int x)> GetAllowedMoves(BoardState board)
        {
            var pieceLocation = board.GetPieceLocation(this);
            var y = pieceLocation.y;
            var x = pieceLocation.x;

            var allowedMoves = new List<(int y, int x)>() { (1, 2), (2, 1) };

            allowedMoves = allowedMoves.Select(move => new List<(int y, int x)>() {
                ( move.y + y, move.x + x),
                (-move.y + y, move.x + x),
                (move.y + y, -move.x + x),
                (-move.y + y, -move.x + x),
            }).SelectMany(_ => _).ToList();

            ApplyTransformations(board, ref allowedMoves);

            return allowedMoves;
        }
    }
}
