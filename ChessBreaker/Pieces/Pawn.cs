using System;
using System.Collections.Generic;

namespace ChessBreaker.Pieces
{
    public class Pawn : BasePiece
    {
        public Pawn(Player controlledBy) : base(controlledBy)
        {
        }

        public override string Name => nameof(Pawn);

        public override List<(int y, int x)> GetAllowedMoves(BoardState board)
        {
            var pieceLocation = board.GetPieceLocation(this);

            if (pieceLocation.y == 0 || pieceLocation.y == 7) return new List<(int y, int x)>();

            var allowedMoves = new List<(int y, int x)>();

            var direction = ControlledBy == Player.Black ? 1 : -1;

            if (pieceLocation.x > 0)
            {
                var leftTop = board.Squares[pieceLocation.y + direction, pieceLocation.x - 1];

                if (leftTop != null && leftTop.ControlledBy != ControlledBy)
                {
                    allowedMoves.Add((pieceLocation.y + direction, pieceLocation.x - 1));
                }
            }

            if (pieceLocation.x < 7)
            {
                var rightTop = board.Squares[pieceLocation.y + direction, pieceLocation.x + 1];

                if (rightTop != null && rightTop.ControlledBy != ControlledBy)
                {
                    allowedMoves.Add((pieceLocation.y + direction, pieceLocation.x + 1));
                }
            }

            for (var i = 1; i < (AlreadyMoved ? 2 : 3); i++)
            {
                if (board.Squares[pieceLocation.y + i * direction, pieceLocation.x] != null)
                {
                    break;
                }

                allowedMoves.Add((pieceLocation.y + i * direction, pieceLocation.x));
            }

            ApplyTransformations(board, ref allowedMoves);

            return allowedMoves;
        }

        public override Dictionary<(int y, int x), List<((int y, int x), (int y, int x))>> GetAdditionalMoves(BoardState board)
        {
            var enPessantPawnsMoves = base.GetAdditionalMoves(board);

            var direction = ControlledBy == Player.Black ? 1 : -1;

            var pieceLocation = board.GetPieceLocation(this);

            if (board.EnPassantPawn != null)
            {
                var enPassantPawnLocation = board.GetPieceLocation(board.EnPassantPawn);

                if (enPassantPawnLocation.y == pieceLocation.y && Math.Abs(enPassantPawnLocation.x - pieceLocation.x) == 1)
                {
                    var enPessantPawnRollback = new List<((int y, int x), (int y, int x))>()
                    {
                        ((enPassantPawnLocation.y, enPassantPawnLocation.x), (enPassantPawnLocation.y + direction, enPassantPawnLocation.x))
                    };

                    enPessantPawnsMoves.Add((enPassantPawnLocation.y + direction, enPassantPawnLocation.x), enPessantPawnRollback);
                }
            }


            return enPessantPawnsMoves;
        }
    }
}
