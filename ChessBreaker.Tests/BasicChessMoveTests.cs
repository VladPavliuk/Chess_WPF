using ChessBreaker.Pieces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
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
        public void TetsCustomPgn()
        {
            var pgn = "1. e4 {[%clk 0:09:53.9]} 1... e5 {[%clk 0:09:59.2]} 2. Nf3 {[%clk 0:09:47.5]} 2... Nf6 {[%clk 0:09:57.7]} 3. h4 {[%clk 0:09:42.9]} 3... h5 {[%clk 0:09:57.1]} 4. d3 {[%clk 0:09:35.1]} 4... d6 {[%clk 0:09:56.9]} 5. a3 {[%clk 0:09:31.9]} 5... a6 {[%clk 0:09:55.7]} 6. Ba2 {[%clk 0:09:30.5]} 6... Ba7 {[%clk 0:09:54.3]} 7. b4 {[%clk 0:09:25]} 7... b5 {[%clk 0:09:53.3]} 8. c3 {[%clk 0:09:23.9]} 8... c5 {[%clk 0:09:52.3]} 9. c4 {[%clk 0:09:22.7]} 9... d5 {[%clk 0:09:49.3]} 10. d4 {[%clk 0:09:21.6]} 10... dxe4 {[%clk 0:09:41.8]} 11. dxe5 {[%clk 0:09:20.7]} 11... exf3 {[%clk 0:09:38.3]} 12. exf6 {[%clk 0:09:19.6]} 12... g6 {[%clk 0:09:26.6]} 13. g3 {[%clk 0:09:17.9]} 13... cxb4 {[%clk 0:09:09.5]} 14. cxb5 {[%clk 0:09:16.8]} 14... Bb7 {[%clk 0:09:01.8]} 15. Bb2 {[%clk 0:09:15.6]} 15... Qxf6 {[%clk 0:08:58]} 16. Qxf3 {[%clk 0:09:14.9]} 16... Qxb2 {[%clk 0:08:51.5]} 17. Qxb7 {[%clk 0:09:13.5]} 17... Nxb7 {[%clk 0:08:50.1]} 18. Nxb2 {[%clk 0:09:10.9]} 18... Ke7 {[%clk 0:08:45.1]} 19. Ke2 {[%clk 0:09:10]} 19... Rfd8 {[%clk 0:08:42.4]} 20. Rfd1 {[%clk 0:09:08.9]} 20... Rxd1 {[%clk 0:08:38.4]} 21. Rxc8 {[%clk 0:09:08]} 21... Rc1 {[%clk 0:08:31.3]} 22. Ra8 {[%clk 0:09:01.6]} 22... Ra1 {[%clk 0:08:29.5]} 23. Rxa7 {[%clk 0:08:55.4]} 23... Rxa2 {[%clk 0:08:27.7]} 24. Kf3 {[%clk 0:08:48.1]} 24... Kf6 {[%clk 0:08:27]} 25. Rxb7 {[%clk 0:08:46.2]} 25... Rxb2 {[%clk 0:08:25.3]} 26. bxa6 {[%clk 0:08:37]} 26... bxa3 {[%clk 0:08:24.2]} 27. a7 {[%clk 0:08:36.1]} 27... a2 {[%clk 0:08:23.1]} 28. a8=Q {[%clk 0:08:34.6]} 28... a1=Q {[%clk 0:08:21.1]} 29. Kg2 {[%clk 0:08:24.6]} 29... Kg7 {[%clk 0:08:19.6]} 30. g4 {[%clk 0:08:22.8]} 30... g5 {[%clk 0:08:18.8]} 31. Kh3 {[%clk 0:08:16.3]} 31... Kh6 {[%clk 0:08:17.5]} 32. f4 {[%clk 0:08:15.4]} 32... f5 {[%clk 0:08:15.4]} 33. Kg3 {[%clk 0:07:59.4]} 33... Kg6 {[%clk 0:08:14.8]} 34. fxg5 {[%clk 0:07:55.4]} 34... fxg4 {[%clk 0:08:10.7]} 35. Kf4 {[%clk 0:07:48.9]} 1/2-1/2";

            TestPgn(pgn);
        }

        [Fact]
        public void TetsChessPgnServices()
        {
            var client = new HttpClient();

            var playersRes = client.GetAsync("https://api.chess.com/pub/country/FR/players").Result;
            var playersResBody = JObject.Parse(playersRes.Content.ReadAsStringAsync().Result);

            var games = playersResBody["players"]
                .Take(100)
                .Aggregate(new List<JToken>(), (acc, player) =>
                {
                    var playersGamesRes = client.GetAsync($"https://api.chess.com/pub/player/{player}/games/archives").Result;
                    var playersGamesResBody = JObject.Parse(playersGamesRes.Content.ReadAsStringAsync().Result);

                    var playerGames = playersGamesResBody["archives"].Take(30).Select(a =>
                    {
                        var gamesRes = client.GetAsync(a.ToString()).Result;
                        var gamesResBody = JObject.Parse(gamesRes.Content.ReadAsStringAsync().Result);

                        return gamesResBody["games"].Where(game =>
                            !game["pgn"].ToString().Contains("SetUp") && game["pgn"].ToString().Contains("won by checkmate") && game["rules"].ToString().Equals("chess"))
                        .Take(30).Select(game => game["pgn"]);
                    });

                    acc.AddRange(playerGames.SelectMany(_ => _));

                    return acc;
                })
                .ToArray();

            foreach (var game in games)
            {
                TestPgn(game.ToString());
            }
        }

        private void TestPgn(string pgn)
        {
            var pgnMoves = pgn.Split("\n").Last();

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
                        promotionPieceType = move.Split("=")[1];
                        location = move.Split("=")[0];
                    }

                    if (location.Contains("x"))
                    {
                        location = move.Split("x")[1];
                    }
                }
                else if (piecesNotaion.ContainsKey(move[0]))
                {
                    identityLocation = 1;
                    pieces = playerPieces.FindAll(p => p.GetType() == piecesNotaion[move[0]]);
                }

                var customLocation = ConvertToCustomLocationFormat(location);
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
