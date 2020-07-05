﻿using System.Collections.Generic;

namespace ChessWpf.Pieces
{
    public class King : BasePiece
    {
        public King(Player controlledBy) : base(controlledBy)
        {
        }

        public override string Name => nameof(King);

        public override string ImageName => "king";

        public override List<(int y, int x)> GetAllowedMoves(BoardState board)
        {
            var pieceLocation = board.GetPieceLocation(this);
            var allowedMoves = new List<(int y, int x)>();

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (j == 0 && i == 0)
                    {
                        continue;
                    }

                    allowedMoves.Add((pieceLocation.y + i, pieceLocation.x + j));
                }
            }

            ApplyTransformations(board, ref allowedMoves);

            return allowedMoves;
        }
    }
}
