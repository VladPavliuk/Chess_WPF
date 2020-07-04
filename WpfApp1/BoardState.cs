using ChessWpf.Pieces;
using System;
using System.Collections.Generic;

namespace ChessWpf
{
    public class BoardState
    {
        public readonly BoardSquare[,] Squares = new BoardSquare[8, 8];

        public Player? IsCheck { get; set; } = null;

        public int RecurtionLevel { get; set; } = 0;

        public BoardState()
        {
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    Squares[i, j] = new BoardSquare();
                }
            }

            InitDefaultChessPieces();
        }

        public BoardState(BoardSquare[,] squares)
        {
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    Squares[i, j] = new BoardSquare()
                    {
                        CurrentPiece = squares[i, j]?.CurrentPiece
                    };
                }
            }
        }

        public BoardState Copy()
        {
            return new BoardState(Squares)
            {
                IsCheck = IsCheck,
                RecurtionLevel = RecurtionLevel
            };
        }

        public (int y, int x) GetPieceLocation(BasePiece piece)
        {
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    if (Squares[i, j].CurrentPiece == piece)
                    {
                        return (i, j);
                    }
                }
            }

            throw new Exception("No such piece on the board!");
        }

        public List<BasePiece> GetPlayerPieces(Player player)
        {
            var pieces = new List<BasePiece>();

            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var piece = Squares[i, j]?.CurrentPiece;

                    if (piece != null && piece.ControlledBy == player)
                    {
                        pieces.Add(piece);
                    }
                }
            }

            return pieces;
        }

        private void InitDefaultChessPieces()
        {
            Squares[0, 0].CurrentPiece = new Rook(Player.Black);
            Squares[0, 1].CurrentPiece = new Knight(Player.Black);
            Squares[0, 2].CurrentPiece = new Bishop(Player.Black);
            Squares[0, 3].CurrentPiece = new Queen(Player.Black);
            Squares[0, 4].CurrentPiece = new King(Player.Black);
            Squares[0, 5].CurrentPiece = new Bishop(Player.Black);
            Squares[0, 6].CurrentPiece = new Knight(Player.Black);
            Squares[0, 7].CurrentPiece = new Rook(Player.Black);

            for (var i = 0; i < 8; i++)
            {
                Squares[1, i].CurrentPiece = new Pawn(Player.Black);
            }

            Squares[7, 0].CurrentPiece = new Rook(Player.White);
            Squares[7, 1].CurrentPiece = new Knight(Player.White);
            Squares[7, 2].CurrentPiece = new Bishop(Player.White);
            Squares[7, 3].CurrentPiece = new Queen(Player.White);
            Squares[7, 4].CurrentPiece = new King(Player.White);
            Squares[7, 5].CurrentPiece = new Bishop(Player.White);
            Squares[7, 6].CurrentPiece = new Knight(Player.White);
            Squares[7, 7].CurrentPiece = new Rook(Player.White);

            for (var i = 0; i < 8; i++)
            {
                Squares[6, i].CurrentPiece = new Pawn(Player.White);
            }
        }

        public class BoardSquare
        {
            public BasePiece CurrentPiece { get; set; }
        }
    }
}
