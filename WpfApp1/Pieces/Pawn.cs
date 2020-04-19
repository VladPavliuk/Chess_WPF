using System.Collections.Generic;

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

            var direction = ControlledBy == Player.Black ? 1 : -1;

            var leftTop = board.Squares[pieceLocation.y + direction, pieceLocation.x - 1].CurrentPiece;
            var rightTop = board.Squares[pieceLocation.y + direction, pieceLocation.x + 1].CurrentPiece;

            if (leftTop != null && leftTop.ControlledBy != ControlledBy)
            {
                allowedMoves.Add((pieceLocation.y + direction, pieceLocation.x - 1));
            }

            if (rightTop != null && rightTop.ControlledBy != ControlledBy)
            {
                allowedMoves.Add((pieceLocation.y + direction, pieceLocation.x + 1));
            }

            for (var i = 1; i < (AlreadyMoved ? 2 : 3); i++)
            {
                if (board.Squares[pieceLocation.y + i * direction, pieceLocation.x].CurrentPiece != null)
                {
                    break;
                }

                allowedMoves.Add((pieceLocation.y + i * direction, pieceLocation.x));
            }

            ApplyTransformations(board, ref allowedMoves);

            return allowedMoves;
        }
    }
}
