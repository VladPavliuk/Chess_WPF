﻿using ChessBreaker.Enums;
using System.Collections.Generic;

namespace ChessBreaker.Pieces
{
    public class Bishop : BasePiece
    {
        public Bishop(Player controlledBy) : base(controlledBy)
        {
        }

        public override string Name => nameof(Bishop);

        public override List<(int y, int x)> GetAllowedMoves(BoardState board)
        {
            var allowedMoves = new List<(int y, int x)>();

            GetLineMoves(board, allowedMoves, (1, 1));
            GetLineMoves(board, allowedMoves, (-1, 1));
            GetLineMoves(board, allowedMoves, (1, -1));
            GetLineMoves(board, allowedMoves, (-1, -1));

            ApplyTransformations(board, ref allowedMoves);

            return allowedMoves;
        }
    }
}
