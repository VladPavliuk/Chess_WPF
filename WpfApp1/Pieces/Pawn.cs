using System.Collections.Generic;
using System.Linq;

namespace ChessWpf.Pieces
{
    public class Pawn : BasePiece
    {
        public Pawn(Player controlledBy) : base(controlledBy)
        {
        }

        public override string Name => nameof(Pawn);

        public override string ImageName => "pawn";

        public override List<(int y, int x)> GetAllowedMoves(BoardState board)
        {
            var pieceLocation = board.GetPieceLocation(this);
            var allowedMoves = new List<(int y, int x)>();

            for (var i = 1; i < (AlreadyMoved ? 2 : 3); i++)
            {
                var direction = ControlledBy == Player.Black ? 1 : -1;
                allowedMoves.Add((pieceLocation.y + i * direction, pieceLocation.x));
            }

            ApplyTransformations(board, allowedMoves);

            return allowedMoves;
        }
    }
}
