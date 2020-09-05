using ChessBreaker.Enums;
using ChessBreaker.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessBreaker
{
    public class BoardState
    {
        public readonly BasePiece[,] Squares = new BasePiece[8, 8];

        public List<BasePiece> AlreadyMoved = new List<BasePiece>();

        public Player CurrentPlayer { get; set; }

        public Player OpositePlayer => CurrentPlayer == Player.White ? Player.Black : Player.White;

        public Player? IsCheck { get; private set; } = null;

        public int RecurtionLevel { get; set; } = 0;

        public event EventHandler<EndGameArgs> OnEndGame = delegate { };

        public Action OnMoveEnd = delegate { };

        public EndGameResult GameResult { get; private set; }

        public Action<BasePiece, List<(int y, int x)>> OnPieceClick = delegate { };

        public Pawn EnPassantPawn { get; set; }

        public readonly Dictionary<string, Type> PromotionPieces = new Dictionary<string, Type>() {
            { "Q", typeof(Queen) },
            { "R", typeof(Rook) },
            { "B", typeof(Bishop) },
            { "N", typeof(Knight) }
        };

        private BasePiece ClickedPiece { get; set; }

        public Pawn PromotionPiece { get; private set; } = null;

        public BoardState()
        {
            InitDefaultChessPieces();
        }

        public void DoPiecePromotion(string pieceType)
        {
            if (PromotionPiece == null) return;

            var promotionPieceLocation = GetPieceLocation(PromotionPiece);

            if (PromotionPieces.ContainsKey(pieceType))
            {
                Squares[promotionPieceLocation.y, promotionPieceLocation.x] = (BasePiece)Activator.CreateInstance(PromotionPieces[pieceType], ClickedPiece.ControlledBy);

                PromotionPiece = null;
                ClickedPiece = null;
                SwitchCurrentPlayer();
            }
            else
            {
                throw new Exception("Wrong piece type");
            }
        }

        public void UpdatePieces(int y, int x)
        {
            if (PromotionPiece != null || GameResult != EndGameResult.Undefined)
            {
                return;
            }

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

                // TODO: Rename moves -> baseMoves
                //var moves = ClickedPiece.GetAllowedMoves(this);

                //var additionalMoves = ClickedPiece.GetAdditionalMoves(this);
                //moves.AddRange(additionalMoves.Select(m => m.Key));

                //OnPieceClick(ClickedPiece, moves);
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

                    AlreadyMoved.Add(ClickedPiece);
                    //ClickedPiece.AlreadyMoved = true;

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

                    if ((y == 7 || y == 0) && Squares[y, x] is Pawn promotionPiece)
                    {
                        PromotionPiece = promotionPiece;
                        return;
                    }

                    SwitchCurrentPlayer();
                    OnMoveEnd();
                    var gameResult = GetGameResult();

                    if (gameResult != EndGameResult.Undefined)
                    {
                        GameResult = gameResult;

                        OnEndGame(this, new EndGameArgs()
                        {
                            Result = gameResult
                        });
                    }
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
                EnPassantPawn = EnPassantPawn,
                CurrentPlayer = CurrentPlayer,
                AlreadyMoved = AlreadyMoved.ToList(),
                ClickedPiece = ClickedPiece,
                RecurtionLevel = RecurtionLevel,
                PromotionPiece = PromotionPiece
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

        public EndGameResult GetGameResult()
        {
            if (!GetPlayerPieces(CurrentPlayer).Any(piece => piece.GetAllowedMoves(this).Any()))
            {
                if (IsCheck == CurrentPlayer)
                {
                    return EndGameResult.Checkmate;
                }
                else
                {
                    return EndGameResult.Draw;
                }
            }

            return EndGameResult.Undefined;
        }

        private void SwitchCurrentPlayer()
        {
            IsCheck = null;

            var isCheck = GetPlayerPieces(CurrentPlayer).Any(piece =>
                piece.GetAllowedMoves(this).Any(m => Squares[m.y, m.x] is King king && king.ControlledBy != CurrentPlayer));

            if (isCheck)
            {
                IsCheck = OpositePlayer;
            }

            CurrentPlayer = OpositePlayer;
        }

        private void InitDefaultChessPieces()
        {
            //Squares[0, 0] = new King(Player.White);

            //Squares[1, 2] = new King(Player.Black);

            //Squares[6, 3] = new Rook(Player.Black);

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

        public class EndGameArgs
        {
            public EndGameResult Result { get; set; }
        }
    }
}
