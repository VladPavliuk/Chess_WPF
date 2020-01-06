using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp1.Pieces
{
    public class King : BasePiece
    {
        public King(Player controlledBy) : base(controlledBy)
        {
        }

        public override string Name => nameof(King);

        public override string ImageName => "king";

        public override int[][] GetAllowedMoves(BoardState board)
        {
            var pieceLocation = board.GetPieceLocation(this);
            var allowedMoves = new List<(int y, int x)>();

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (j == 0 && i == 0)
                        continue;

                    var y = pieceLocation.y + i;
                    var x = pieceLocation.x + j;

                    allowedMoves.Add((y, x));
                }
            }

            return ApplyTransformations(board, allowedMoves.Select(_ => new int[] { _.y, _.x }).ToArray());
        }
    }
}
