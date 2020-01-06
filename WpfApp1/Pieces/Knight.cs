using System.Collections.Generic;
using System.Linq;

namespace WpfApp1.Pieces
{
    public class Knight : BasePiece
    {
        public Knight(Player controlledBy) : base(controlledBy)
        {
        }

        public override string Name => nameof(Knight);

        public override string ImageName => "knight";

        public override int[][] GetAllowedMoves(BoardState board)
        {
            var pieceLocation = board.GetPieceLocation(this);
            var y = pieceLocation.y;
            var x = pieceLocation.x;

            var allowedMoves = new List<(int y, int x)>() { (1, 2), (2, 1) };

            return ApplyTransformations(board, allowedMoves.Select(move => new int[][] {
                new int [] { move.y + y, move.x + x},
                new int [] { -move.y + y, move.x + x},
                new int [] { move.y + y, -move.x + x},
                new int [] { -move.y + y, -move.x + x },
            }).SelectMany(_ => _).ToArray());

        }
    }
}
