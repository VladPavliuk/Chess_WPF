using ChessBreaker.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessBreaker
{
    public class BoardState
    {
        public readonly BasePiece[,] Squares = new BasePiece[8, 8];

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

        public Action<BasePiece, List<(int y, int x)>> OnPieceClick = delegate { };

        public Action<Player> CheckmateHandler { get; set; } = delegate { };

        public Action DrawHandler { get; set; } = delegate { };

        private Player CurrentPlayerInternal = Player.White;

        public Pawn EnPassantPawn { get; set; }

        private BasePiece ClickedPiece { get; set; }

        private bool IsCheckmate = false;

        private bool IsDraw = false;

        public BoardState()
        {
            //for (var i = 0; i < 8; i++)
            //{
            //    for (var j = 0; j < 8; j++)
            //    {
            //        Squares[i, j] = new BoardSquare();
            //    }
            //}

            InitDefaultChessPieces();
        }

        public void UpdatePieces(int y, int x)
        {
            if (ClickedPiece == null)
            {
                if (Squares[y, x] == null)
                {
                    return;
                }

                ClickedPiece = Squares[y, x];

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
                            Squares[move.Item2.y, move.Item2.x] = Squares[move.Item1.y, move.Item1.x];
                            Squares[move.Item1.y, move.Item1.x] = null;
                        }
                    }

                    //TODO: It should be in Board class, somthing like movePiece(from, to)
                    Squares[y, x] = ClickedPiece;
                    Squares[location.y, location.x] = null;

                    if ((y == 7 || y == 0) && Squares[y, x] is Pawn)
                    {
                        var promotionPieces = GetPromotionPieces();
                        // TODO: Add ability choose a promition piece.
                        Squares[y, x] = promotionPieces[0];
                        ClickedPiece = promotionPieces[0];
                    }

                    var newMoves = ClickedPiece.GetAllowedMoves(this).ToArray();

                    IsCheck = null;

                    var isCheck = GetPlayerPieces(CurrentPlayer).Any(piece =>
                        piece.GetAllowedMoves(this).Any(m => Squares[m.y, m.x] is King king && king.ControlledBy != CurrentPlayer));

                    if (isCheck)
                    {
                        IsCheck = OpositePlayer;
                    }

                    CurrentPlayer = OpositePlayer;
                }

                ClickedPiece = null;
            }
        }

        public BoardState(BasePiece[,] squares)
        {
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    Squares[i, j] = squares[i, j];
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
                    if (Squares[i, j] == piece)
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
                    var piece = Squares[i, j];

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
            Squares[0, 0] = new Rook(Player.Black);
            Squares[0, 1] = new Knight(Player.Black);
            Squares[0, 2] = new Bishop(Player.Black);
            Squares[0, 3] = new Queen(Player.Black);
            Squares[0, 4] = new King(Player.Black);
            Squares[0, 5] = new Bishop(Player.Black);
            Squares[0, 6] = new Knight(Player.Black);
            Squares[0, 7] = new Rook(Player.Black);

            for (var i = 0; i < 8; i++)
            {
                Squares[1, i] = new Pawn(Player.Black);
            }

            Squares[7, 0] = new Rook(Player.White);
            Squares[7, 1] = new Knight(Player.White);
            Squares[7, 2] = new Bishop(Player.White);
            Squares[7, 3] = new Queen(Player.White);
            Squares[7, 4] = new King(Player.White);
            Squares[7, 5] = new Bishop(Player.White);
            Squares[7, 6] = new Knight(Player.White);
            Squares[7, 7] = new Rook(Player.White);

            for (var i = 0; i < 8; i++)
            {
                Squares[6, i] = new Pawn(Player.White);
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
    }
}
