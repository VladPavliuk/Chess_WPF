using ChessBreaker.Enums;
using ChessBreaker.Pieces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessBreaker
{
    public static class ChessAI
    {
        private const int MaxLevel = 3;

        public static ((int, int), (int, int)) GetOptimal(BoardState state)
        {
            var stateNode = new StateNode()
            {
                State = state
            };

            var bestState = PopulateChildStates(stateNode, player: state.CurrentPlayer);

            return bestState.NextMove;
        }

        static StateNode PopulateChildStates(
            StateNode stateNode,
            float alpha = float.MinValue,
            float beta = float.MaxValue,
            int recursionLevel = MaxLevel,
            Player player = Player.White)
        {
            if (recursionLevel <= 0 || stateNode.State.GameResult != EndGameResult.Undefined)
            {
                stateNode.BestMoveLevel = stateNode.Level;
                stateNode.Value = CalculateStateValue(stateNode);

                return stateNode;
            }

            var possibleMoves = GetPossibleMoves(stateNode.State, stateNode.State.CurrentPlayer);

            var states = new ConcurrentBag<StateNode>();
            var opositePlayer = player == Player.White ? Player.Black : Player.White;

            Parallel.ForEach(possibleMoves, (move, state) =>
            {
                var childStates = PopulateChildStates(new StateNode()
                {
                    Trace = move.Key,
                    State = move.Value,
                    Parent = stateNode,
                    Level = recursionLevel + 1
                }, alpha, beta, recursionLevel - 1, opositePlayer);

                if (player == Player.White)
                {
                    alpha = Math.Max((float)childStates.Value, (float)alpha);
                }
                else
                {
                    beta = Math.Min((float)childStates.Value, (float)beta);
                }

                if (beta <= alpha)
                {
                    state.Break();
                }


                states.Add(childStates);
            });

            var groupedStatesByValue = states.GroupBy(s => s.Value).OrderByDescending(g => g.Key);

            var bestMovesGroup = player == Player.White
                ? groupedStatesByValue.First()
                : groupedStatesByValue.Last();

            var bestMove = bestMovesGroup.OrderBy(s => s.BestMoveLevel).First();

            // For debug
            stateNode.Children = states;
            //

            stateNode.Value = bestMove.Value;
            stateNode.BestMoveLevel = stateNode.BestMoveLevel;
            stateNode.NextMove = bestMove.Trace;

            stateNode.BestState = bestMove;

            return stateNode;
        }

        static List<KeyValuePair<((int, int), (int, int)), BoardState>> GetPossibleMoves(BoardState state, Player player)
        {
            var pieces = state.GetPlayerPieces(player);

            var piecesMoves = pieces.ToDictionary(p => state.GetPieceLocation(p), p => p.GetAllowedMoves(state));

            return piecesMoves.Select(pieceMoves => pieceMoves.Value.ToDictionary(move => (pieceMoves.Key, move), move =>
            {
                var shadowState = state.Copy();

                shadowState.UpdatePieces(pieceMoves.Key.y, pieceMoves.Key.x);
                shadowState.UpdatePieces(move.y, move.x);

                return shadowState;
            })).SelectMany(_ => _).ToList();
        }

        static float CalculateStateValue(StateNode stateNode)
        {
            float getPlayerValue(Player player)
            {
                var opositePlayer = player == Player.White ? Player.Black : Player.White;

                var pieces = stateNode.State.GetPlayerPieces(player);

                var queens = pieces.Where(p => p is Queen).Count();
                var rooks = pieces.Where(p => p is Rook).Count();
                var bishops = pieces.Where(p => p is Bishop).Count();
                var knights = pieces.Where(p => p is Knight).Count();
                var pawns = pieces.Where(p => p is Pawn).Count();

                var check = stateNode.State.IsCheck == opositePlayer ? 1 : 0;
                var checkmate = stateNode.State.GameResult == EndGameResult.Checkmate ? 150 : 1;

                return checkmate * check + 10 * queens + 5 * rooks + 3 * bishops + 3 * knights + pawns;
            }

            return getPlayerValue(Player.White) - getPlayerValue(Player.Black);
        }
    }

    public class StateNode
    {
        public ((int, int), (int, int)) Trace { get; set; }

        public ((int, int), (int, int)) NextMove { get; set; }

        public int Level { get; set; }

        public int BestMoveLevel { get; set; }

        public ConcurrentBag<StateNode> Children { get; set; }

        public StateNode BestState { get; set; }

        public StateNode Parent { get; set; }

        public BoardState State { get; set; }

        public float? Value { get; set; }
    }
}
