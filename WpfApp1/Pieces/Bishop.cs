﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp1.Pieces
{
    public class Bishop : BasePiece
    {
        public Bishop(Player controlledBy) : base(controlledBy)
        {
        }

        public override string Name => nameof(Bishop);

        public override string ImageName => "bishop";

        public override int[][] GetAllowedMoves(BoardState board)
        {
            var allowedMoves = new List<(int y, int x)>();

            GetLineMoves(board, allowedMoves, (1, 1));
            GetLineMoves(board, allowedMoves, (-1, 1));
            GetLineMoves(board, allowedMoves, (1, -1));
            GetLineMoves(board, allowedMoves, (-1, -1));

            return ApplyTransformations(board, allowedMoves.Select(_ => new int[] { _.y, _.x }).ToArray());
        }
    }
}
