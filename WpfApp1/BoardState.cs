using ChessWpf.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWpf
{
    public class BoardState
    {
        public readonly BoardSquare[,] Squares = new BoardSquare[8, 8];

        public Player CurrentPlayer
        {
            get
            {
                return CurrentPlayerInternal;
            }
            set
            {
                IsEndGameInternal();
                CurrentPlayerInternal = value;
            }
        }

        public Player OpositePlayer => CurrentPlayer == Player.White ? Player.Black : Player.White;

        public Player? IsCheck { get; set; } = null;

        public int RecurtionLevel { get; set; } = 0;

        public bool EndGame => IsCheckmate || IsDraw;

        public Action<Player> CheckmateHandler { get; set; }

        public Action DrawHandler { get; set; }

        private Player CurrentPlayerInternal = Player.White;

        public Pawn EnPassantPawn { get; set; }

        private bool IsCheckmate = false;

        private bool IsDraw = false;

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

        public void IsEndGame()
        {
            if (EndGame)
            {
                if (IsCheckmate)
                {
                    CheckmateHandler(OpositePlayer);
                }
                else if (IsDraw)
                {
                    DrawHandler();
                }
                else
                {
                    throw new Exception("The game ends what it's a checkmate or a draw!");
                }
            }
        }

        public List<BasePiece> GetPromotionPieces()
        {
            return new List<BasePiece>() { 
                new Queen(CurrentPlayer),
                new Rook(CurrentPlayer),
                new Bishop(CurrentPlayer),
                new Knight(CurrentPlayer)
            };
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

        private void IsEndGameInternal()
        {
            if (!GetPlayerPieces(OpositePlayer).Any(piece => piece.GetAllowedMoves(this).Any()))
            {
                if (IsCheck == OpositePlayer)
                {
                    IsCheckmate = true;
                }
                else
                {
                    IsDraw = true;
                }
            }
        }

        public class BoardSquare
        {
            public BasePiece CurrentPiece { get; set; }
        }
    }
}
