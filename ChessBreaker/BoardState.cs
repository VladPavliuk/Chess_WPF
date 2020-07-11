using ChessBreaker.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessBreaker
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

        public Action<BasePiece, List<(int y, int x)>> OnPieceClick;

        public Action<Player> CheckmateHandler { get; set; }

        public Action DrawHandler { get; set; }

        private Player CurrentPlayerInternal = Player.White;

        public Pawn EnPassantPawn { get; set; }

        private BasePiece ClickedPiece { get; set; }

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

        public void UpdatePieces(int y, int x)
        {
            if (ClickedPiece == null)
            {
                if (Squares[y, x].CurrentPiece == null)
                {
                    return;
                }

                ClickedPiece = Squares[y, x].CurrentPiece;

                if (ClickedPiece.ControlledBy != CurrentPlayer)
                {
                    ClickedPiece = null;
                    return;
                }

                var moves = ClickedPiece.GetAllowedMoves(this);
                var additionalMoves = ClickedPiece.GetAdditionalMoves(this);

                moves.AddRange(additionalMoves.Select(m => m.Key));

                OnPieceClick(ClickedPiece, moves);
            }
            else
            {
                if (ClickedPiece.ControlledBy != CurrentPlayer)
                {
                    return;
                }

                //TODO: Do not recalculate possible moves again, save it from the prev step.
                var moves = ClickedPiece.GetAllowedMoves(this);
                var additionalMoves = ClickedPiece.GetAdditionalMoves(this);

                moves.AddRange(additionalMoves.Select(m => m.Key));

                if (moves.Any(_ => _.y == y && _.x == x))
                {
                    EnPassantPawn = null;
                    ClickedPiece.AlreadyMoved = true;

                    var location = GetPieceLocation(ClickedPiece);

                    if (ClickedPiece is Pawn pawnPiece && Math.Abs(y - location.y) == 2)
                    {
                        EnPassantPawn = pawnPiece;
                    }

                    // For castling and enPassant pawn
                    if (additionalMoves.ContainsKey((y, x)))
                    {
                        foreach (var move in additionalMoves[(y, x)])
                        {
                            Squares[move.Item2.y, move.Item2.x].CurrentPiece = Squares[move.Item1.y, move.Item1.x].CurrentPiece;
                            Squares[move.Item1.y, move.Item1.x].CurrentPiece = null;
                        }
                    }

                    //TODO: It should be in Board class, somthing like movePiece(from, to)
                    Squares[y, x].CurrentPiece = ClickedPiece;
                    Squares[location.y, location.x].CurrentPiece = null;

                    if ((y == 7 || y == 0) && Squares[y, x].CurrentPiece is Pawn)
                    {
                        var promotionPieces = GetPromotionPieces();
                        // TODO: Add ability choose a promition piece.
                        Squares[y, x].CurrentPiece = promotionPieces[0];
                        ClickedPiece = promotionPieces[0];
                    }

                    var newMoves = ClickedPiece.GetAllowedMoves(this).ToArray();

                    IsCheck = null;

                    var isCheck = GetPlayerPieces(CurrentPlayer).Any(piece =>
                        piece.GetAllowedMoves(this).Any(m => Squares[m.y, m.x].CurrentPiece is King king && king.ControlledBy != CurrentPlayer));

                    if (isCheck)
                    {
                        IsCheck = OpositePlayer;
                    }

                    CurrentPlayer = OpositePlayer;
                }

                ClickedPiece = null;
            }
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
