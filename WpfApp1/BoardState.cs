using System;
using ChessWpf.Pieces;

namespace ChessWpf
{
    public class BoardState
    {
        public readonly BoardSquare[,] Squares = new BoardSquare[8, 8];

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
