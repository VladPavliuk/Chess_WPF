using ChessBreaker.PgnReader;
using ChessBreaker.Pieces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace ChessBreaker.Tests
{
    public class BasicChessMoveTests
    {
        [Fact]
        public void FirstWhitePawn_FirstMoves_Success()
        {
            var boardState = new BoardState();
            Assert.False(boardState.EndGame);

            var actualAllowedMoves = boardState.Squares[6, 0].GetAllowedMoves(boardState);
            var expectedAllowedMoves = new List<(int y, int x)>() { (5, 0), (4, 0) };

            Assert.Equal(expectedAllowedMoves, actualAllowedMoves);
        }

        [Fact]
        public void Tets()
        {
            var client = new HttpClient();
            var res = client.GetAsync("https://api.chess.com/pub/player/123h321/games/2020/06").Result;

            var resBody = JObject.Parse(res.Content.ReadAsStringAsync().Result);

            var games = resBody["games"].Select(game => game["pgn"]).ToArray();

            foreach (var game in games)
            {
                var pgnMoves = game.ToString().Split("\n").Last();

                var pgnMovesInternal = Regex.Split(Regex.Replace(pgnMoves, @"\{[^\}]+\}", string.Empty), @"\s\s");

                var moves = new List<string>();
                for (int i = 0; i < pgnMovesInternal.Length - 1; i++)
                {
                    var move = Regex.Replace(pgnMovesInternal[i], @"\d+\.*\s", string.Empty);
                    move = Regex.Replace(move, @"[\#\+]", string.Empty);

                    moves.Add(move);
                }

                string promotionPieceType = null;
                var boardState = new BoardState();

                foreach (var move in moves)
                {
                    var playerPieces = boardState.GetPlayerPieces(boardState.CurrentPlayer);

                    if (move.Equals("O-O") || move.Equals("O-O-O"))
                    {
                        var king = playerPieces.First(p => p is King);

                        var kingLocation = boardState.GetPieceLocation(king);

                        var castlingDirection = (move.Equals("O-O")) ? 1 : -1;

                        boardState.UpdatePieces(kingLocation.y, kingLocation.x);
                        boardState.UpdatePieces(kingLocation.y, kingLocation.x + 2 * castlingDirection);
                        continue;
                    }

                    BasePiece selectedPiece;
                    var location = move.Substring(move.Length - 2);
                    var customLocation = ConvertToCustomLocationFormat(location);

                    var piecesNotaion = new Dictionary<char, Type>() {
                        { 'N', typeof(Knight) },
                        { 'B', typeof(Bishop) },
                        { 'Q', typeof(Queen) },
                        { 'R', typeof(Rook) },
                        { 'K', typeof(King) }
                    };

                    List<BasePiece> pieces = null;
                    int identityLocation = 0;

                    if (char.IsLower(move[0]))
                    {
                        pieces = playerPieces.FindAll(p => p is Pawn);

                        if (location.Contains("="))
                        {
                            location = move.Split("=")[0];
                            promotionPieceType = move.Split("=")[1];

                            customLocation = ConvertToCustomLocationFormat(location);
                        }
                    }
                    else if (piecesNotaion.ContainsKey(move[0]))
                    {
                        identityLocation = 1;
                        pieces = playerPieces.FindAll(p => p.GetType() == piecesNotaion[move[0]]);
                    }

                    var matchedPieces = pieces.Where(piece => piece.GetAllowedMoves(boardState).Any(s => s == customLocation)).ToArray();

                    if (matchedPieces.Length == 1)
                    {
                        selectedPiece = matchedPieces[0];
                    }
                    else if (matchedPieces.Length > 1)
                    {
                        if (char.IsLetter(move[identityLocation]))
                        {
                            selectedPiece = matchedPieces.First(p => boardState.GetPieceLocation(p).x == GetHorizontalCoordinate(move[identityLocation]));
                        }
                        else if (char.IsDigit(move[identityLocation]))
                        {
                            selectedPiece = matchedPieces.First(p => boardState.GetPieceLocation(p).y == GetVerticalCoordinate(move[identityLocation]));
                        }
                        else
                        {
                            throw new Exception($"Error while reading pgn step: {move}");
                        }
                    }
                    else
                    {
                        throw new Exception($"No matched pieces: {move}");
                    }

                    var selectedPieceLocation = boardState.GetPieceLocation(selectedPiece);

                    boardState.UpdatePieces(selectedPieceLocation.y, selectedPieceLocation.x);
                    boardState.UpdatePieces(customLocation.y, customLocation.x);

                    if (promotionPieceType != null)
                    {
                        boardState.DoPiecePromotion(promotionPieceType);
                    }
                }
            }

            //Assert.True(boardState.EndGame);
        }

        private (int y, int x) ConvertToCustomLocationFormat(string pgnFormat)
        {
            return (GetVerticalCoordinate(pgnFormat[1]), GetHorizontalCoordinate(pgnFormat[0]));
        }

        private int GetHorizontalCoordinate(char pgnFormat)
        {
            var horizontal = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

            return horizontal.ToList().IndexOf(pgnFormat);
        }

        private int GetVerticalCoordinate(char pgnFormat)
        {
            return 8 - (int)char.GetNumericValue(pgnFormat);
        }
    }
}
