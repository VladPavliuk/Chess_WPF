using ChessBreaker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessBreaker.Pieces
{
    public class King : BasePiece
    {
        public King(Player controlledBy) : base(controlledBy)
        {
        }

        public override string Name => nameof(King);

        public override List<(int y, int x)> GetAllowedMoves(BoardState board)
        {
            var pieceLocation = board.GetPieceLocation(this);
            var allowedMoves = new List<(int y, int x)>();

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (j == 0 && i == 0) continue;

                    allowedMoves.Add((pieceLocation.y + i, pieceLocation.x + j));
                }
            }

            allowedMoves.AddRange(GetAdditionalMoves(board).Select(m => m.Key));

            ApplyTransformations(board, ref allowedMoves);

            return allowedMoves;
        }

        public override Dictionary<(int y, int x), List<((int y, int x), (int y, int x))>> GetAdditionalMoves(BoardState board)
        {
            var castleMoves = base.GetAdditionalMoves(board);

            if (board.AlreadyMoved.Any(mp => mp == this)) return castleMoves;

            var pieceLocation = board.GetPieceLocation(this);

            var rooks = board.GetPlayerPieces(ControlledBy).Where(p => p is Rook && !board.AlreadyMoved.Any(mp => mp == p))
                .Select(p => new { piece = p, location = board.GetPieceLocation(p) })
                .Where(p => p.location.y == pieceLocation.y)
                .ToList();

            foreach (var rook in rooks)
            {
                var coeff = rook.location.x > pieceLocation.x ? 1 : -1;

                var castleTrack = new List<int>();

                for (int i = 1; i < Math.Abs(rook.location.x - pieceLocation.x); i++)
                {
                    castleTrack.Add(pieceLocation.x + coeff * i);
                }

                var castleTrackToCheck = castleTrack.Take(2).Select(sx => (pieceLocation.y, sx)).ToList();

                ApplyTransformations(board, ref castleTrackToCheck);

                if (castleTrackToCheck.Count == castleTrack.Take(2).Count() && castleTrack.All(sx => board.Squares[pieceLocation.y, sx] == null))
                {
                    var rookMoves = new List<((int y, int x), (int y, int x))>()
                    {
                        ((rook.location.y, rook.location.x), (pieceLocation.y, pieceLocation.x + coeff))
                    };

                    castleMoves.Add((pieceLocation.y, pieceLocation.x + 2 * coeff), rookMoves);
                }
            }

            return castleMoves;
        }
    }
}
