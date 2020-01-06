﻿using System.Collections.Generic;
using System.Linq;

namespace ChessWpf.Pieces
{
    public class Queen : BasePiece
    {
        public Queen(Player controlledBy) : base(controlledBy)
        {
        }

        public override string Name => nameof(Queen);

        public override string ImageName => "queen";

        public override int[][] GetAllowedMoves(BoardState board)
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

            return ApplyTransformations(board, allowedMoves.Select(_ => new int[] { _.y, _.x }).ToArray());
        }
    }
}
