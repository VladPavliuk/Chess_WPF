using ChessBreaker.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ChessBreaker.Pieces
{
    public abstract class BasePiece
    {
        public abstract string Name { get; }

        public readonly Player ControlledBy;

        public BasePiece(Player controlledBy)
        {
            ControlledBy = controlledBy;
        }

        public abstract List<(int y, int x)> GetAllowedMoves(BoardState board);

        public virtual Dictionary<(int y, int x), List<((int y, int x), (int y, int x))>> GetAdditionalMoves(BoardState board) => new Dictionary<(int y, int x), List<((int y, int x), (int y, int x))>>();

        protected void FilterOutOfBoard(ref List<(int y, int x)> locationsToFilter)
        {
            locationsToFilter = locationsToFilter.Where(l => l.y < 8 && l.y >= 0 && l.x < 8 && l.x >= 0).ToList();
        }

        protected void FilterSamePlayerPices(BoardState board, ref List<(int y, int x)> locationsToFilter)
        {
            locationsToFilter = locationsToFilter.Where(l => board.Squares[l.y, l.x] == null
                    || (board.Squares[l.y, l.x].ControlledBy != ControlledBy)).ToList();
        }

        protected void FilterCheck(BoardState board, ref List<(int y, int x)> locationsToFilter)
        {
            var opositePlayer = ControlledBy == Player.White ? Player.Black : Player.White;
            var currentPieceLocation = board.GetPieceLocation(this);

            locationsToFilter = locationsToFilter.Where(l =>
            {
                var shadowBoard = board.Copy();

                shadowBoard.RecurtionLevel++;

                shadowBoard.Squares[l.y, l.x] = this;
                shadowBoard.Squares[currentPieceLocation.y, currentPieceLocation.x] = null;

                var opositePlayerPieces = shadowBoard.GetPlayerPieces(opositePlayer);

                var piecsWithCheck = opositePlayerPieces
                      .Select(p => new { piece = p, moves = p.GetAllowedMoves(shadowBoard) })
                      .Where(p => p.moves.Any(s =>
                      {
                          var piece = shadowBoard.Squares[s.y, s.x];

                          return piece != null && piece is King;
                      }
                      )).ToList();

                return !piecsWithCheck.Any();
            }).ToList();
        }

        protected void ApplyTransformations(BoardState board, ref List<(int y, int x)> locationsToFilter)
        {
            FilterOutOfBoard(ref locationsToFilter);
            FilterSamePlayerPices(board, ref locationsToFilter);

            // Looks weird
            if (board.RecurtionLevel < 1)
            {
                FilterCheck(board, ref locationsToFilter);
            }
        }

        protected List<(int y, int x)> GetLineMoves(
            BoardState board,
            List<(int y, int x)> allowedMoves,
            (int y, int x) coefficiants)
        {
            var pieceLocation = board.GetPieceLocation(this);

            for (int i = 1; i < 8; i++)
            {
                var y = pieceLocation.y + i * coefficiants.y;
                var x = pieceLocation.x + i * coefficiants.x;

                if (y < 0 || y > 7 || x < 0 || x > 7)
                {
                    break;
                }

                if (board.Squares[y, x] != null)
                {
                    if (board.Squares[y, x].ControlledBy != ControlledBy)
                    {
                        allowedMoves.Add((y, x));
                    }

                    break;
                }

                allowedMoves.Add((y, x));
            }

            return allowedMoves;
        }
    }
}
