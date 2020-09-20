using ChessBreaker.Enums;
using ChessBreaker.Pieces;
using System;
using System.Diagnostics;
using System.Linq;

namespace ChessBreaker
{
    public static class ChessNetwork
    {
        static readonly Type[] PieceTypes = new Type[]
        {
                typeof(Pawn),
                typeof(Bishop),
                typeof(Knight),
                typeof(Rook),
                typeof(Queen),
                typeof(King),
        };

        public static void Test()
        {
            var boardState = new BoardState();
            var playedAs = Player.White;

            var neuralNetwork = new NeuralNetwork(
                PieceTypes.Length * 64,
                64 * 2,
                5,
                10,
                x => 1 / (1 + Math.Exp(-x)));

            LearnHowToStart(boardState, neuralNetwork, playedAs);
        }

        static double[] GetInputsFromBoard(BoardState boardState, Player playedAs)
        {
            var inputSize = PieceTypes.Length * 64 * 2;

            var inputs = new double[inputSize];

            for (int i = 0; i < PieceTypes.Length; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    int y = j / 8;
                    int x = j % 8;

                    if (boardState.Squares[y, x]?.GetType() == PieceTypes[i]
                        && boardState.Squares[y, x].ControlledBy == playedAs)
                    {
                        inputs[i * PieceTypes.Length + j] = 1d;
                    }
                }
            }

            for (int i = 0; i < PieceTypes.Length; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    int y = j / 8;
                    int x = j % 8;

                    if (boardState.Squares[y, x]?.GetType() == PieceTypes[i]
                        && boardState.Squares[y, x].ControlledBy != playedAs)
                    {
                        inputs[(inputSize / 2) + (i * PieceTypes.Length + j)] = 1d;
                    }
                }
            }

            return inputs;
        }

        static void LearnHowToStart(
            BoardState boardState,
            NeuralNetwork neuralNetwork,
            Player playedAs)
        {
            var learningRate = .02d;
            var stopTraining = false;

            for (int a = 0; a < 20000; a++)
            //while(!stopTraining)
            {
                var inputs = GetInputsFromBoard(boardState, playedAs);

                var outputs = neuralNetwork.ForwardPass(inputs);
                var training = new double[outputs.Length];
                var error = 0d;
                var maxValue = 0d;
                BasePiece maxValuePiece = null;

                //> Populate training data where AI should click first
                for (int i = 0; i < outputs.Length / 2; i++)
                {
                    int y = i / 8;
                    int x = i % 8;

                    if (boardState.Squares[y, x] != null
                        && boardState.Squares[y, x].ControlledBy == playedAs)
                    {
                        training[i] = 1;

                        if (outputs[i] > maxValue)
                        {
                            maxValue = outputs[i];
                            maxValuePiece = boardState.Squares[y, x];
                        }
                    }

                    error += Math.Pow(outputs[i] - training[i], 2);
                    //error += outputs[i] - training[i];
                }
                //<

                var allowedMoves = maxValuePiece.GetAllowedMoves(boardState);

                //boardState.GetPieceLocation(maxValuePiece)

                //> Populate training data where player should click second
                for (int i = outputs.Length / 2; i < outputs.Length; i++)
                {
                    int y = (i - outputs.Length / 2) / 8;
                    int x = i % 8;

                    if (allowedMoves.Any(l => l.y == y && l.x == x))
                    {
                        training[i] = 1;
                    }
                    error += training[i] - outputs[i];
                }
                //<
                error /= 2;

                //var error = desiredOutput[randomTrainningIndex] - neuralNetwork.Outputs[neuralNetwork.Outputs.Length - 1][0];
                //Console.WriteLine(randomTrainningIndex + "Error: " + error);
                Debug.WriteLine("error: " + error);
                neuralNetwork.ApplyBackpropagation(training, learningRate);
            }
        }
    }
}
