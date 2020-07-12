using ChessBreaker.PgnReader;
using ChessBreaker.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var actualAllowedMoves = boardState.Squares[6, 0].GetAllowedMoves(boardState);
            var expectedAllowedMoves = new List<(int y, int x)>() { (5, 0), (4, 0) };

            Assert.Equal(expectedAllowedMoves, actualAllowedMoves);
        }

        [Fact]
        public void Tets()
        {
            //PgnReader.PgnReader.LoadPgnFile("");

            var pgnTest = "[Event \"Live Chess\"]\n[Site \"Chess.com\"]\n[Date \"2020.06.23\"]\n[Round \"-\"]\n[White \"Jeremias111\"]\n[Black \"123H321\"]\n[Result \"1-0\"]\n[ECO \"A20\"]\n[ECOUrl \"https://www.chess.com/openings/English-Opening-Kings-English-Variation-2.g3\"]\n[CurrentPosition \"8/8/2K5/2Q5/pk6/8/P2P3p/7B b - -\"]\n[Timezone \"UTC\"]\n[UTCDate \"2020.06.23\"]\n[UTCTime \"19:05:22\"]\n[WhiteElo \"820\"]\n[BlackElo \"763\"]\n[TimeControl \"600\"]\n[Termination \"Jeremias111 won by checkmate\"]\n[StartTime \"19:05:22\"]\n[EndDate \"2020.06.23\"]\n[EndTime \"19:20:36\"]\n[Link \"https://www.chess.com/live/game/5046647030\"]\n\n1. c4 {[%clk 0:09:52.6]} 1... e5 {[%clk 0:09:55.9]} 2. g3 {[%clk 0:09:28]} 2... Nc6 {[%clk 0:09:50.9]} 3. Bg2 {[%clk 0:09:26.6]} 3... Nf6 {[%clk 0:09:17.5]} 4. b3 {[%clk 0:09:21.8]} 4... d5 {[%clk 0:09:08.3]} 5. Bb2 {[%clk 0:09:07.6]} 5... dxc4 {[%clk 0:08:57]} 6. bxc4 {[%clk 0:08:52.7]} 6... Bc5 {[%clk 0:08:42.6]} 7. Nf3 {[%clk 0:08:45.4]} 7... O-O {[%clk 0:08:38.1]} 8. Nxe5 {[%clk 0:08:25.3]} 8... Nxe5 {[%clk 0:08:35.5]} 9. Bxe5 {[%clk 0:08:22.6]} 9... Bg4 {[%clk 0:08:12.1]} 10. Bxb7 {[%clk 0:07:59.2]} 10... Qd4 {[%clk 0:07:53.3]} 11. Bxd4 {[%clk 0:07:53.1]} 11... Bxd4 {[%clk 0:07:49.5]} 12. Nc3 {[%clk 0:07:35.6]} 12... Rab8 {[%clk 0:07:39.7]} 13. Bg2 {[%clk 0:07:17.4]} 13... Rbe8 {[%clk 0:07:34.9]} 14. O-O {[%clk 0:07:10.3]} 14... Bxe2 {[%clk 0:07:29.2]} 15. Nxe2 {[%clk 0:06:59.7]} 15... Bxa1 {[%clk 0:07:12.8]} 16. Qxa1 {[%clk 0:06:58.2]} 16... Nh5 {[%clk 0:07:01.7]} 17. Nf4 {[%clk 0:06:51.9]} 17... Nf6 {[%clk 0:06:51.5]} 18. Bf3 {[%clk 0:06:44.7]} 18... Re7 {[%clk 0:06:43.5]} 19. Nh5 {[%clk 0:06:37.1]} 19... g6 {[%clk 0:06:38.8]} 20. Nxf6+ {[%clk 0:06:32.7]} 20... Kg7 {[%clk 0:06:33.9]} 21. Nd5+ {[%clk 0:06:26.5]} 21... Kh6 {[%clk 0:06:32.9]} 22. Nxe7 {[%clk 0:06:20.1]} 22... Re8 {[%clk 0:06:30.5]} 23. Nd5 {[%clk 0:06:08]} 23... c6 {[%clk 0:06:26.3]} 24. Nc7 {[%clk 0:05:53.6]} 24... Rc8 {[%clk 0:06:23.8]} 25. Na6 {[%clk 0:05:44.7]} 25... c5 {[%clk 0:06:20.1]} 26. Re1 {[%clk 0:05:18.9]} 26... Rc6 {[%clk 0:06:18.1]} 27. Nb8 {[%clk 0:05:11.6]} 27... Rc8 {[%clk 0:06:15.9]} 28. Nd7 {[%clk 0:05:05.4]} 28... Rc7 {[%clk 0:06:14.4]} 29. Ne5 {[%clk 0:05:01.7]} 29... f6 {[%clk 0:06:10.6]} 30. Ng4+ {[%clk 0:04:52.2]} 30... Kg5 {[%clk 0:06:06.5]} 31. Qxf6+ {[%clk 0:04:48]} 31... Kh5 {[%clk 0:06:02.7]} 32. Qg7 {[%clk 0:04:37.9]} 32... Rxg7 {[%clk 0:05:59.2]} 33. Re5+ {[%clk 0:04:25.2]} 33... g5 {[%clk 0:05:51.1]} 34. Nf6+ {[%clk 0:04:11.7]} 34... Kg6 {[%clk 0:05:48.6]} 35. Re6 {[%clk 0:04:02.4]} 35... Kf5 {[%clk 0:05:42.6]} 36. Bg4+ {[%clk 0:03:24.1]} 36... Kg6 {[%clk 0:05:41.6]} 37. Ne8+ {[%clk 0:03:17.4]} 37... Kf7 {[%clk 0:05:39.8]} 38. Nxg7 {[%clk 0:03:11.1]} 38... Kxg7 {[%clk 0:05:38.9]} 39. Re7+ {[%clk 0:03:01.1]} 39... Kg6 {[%clk 0:05:36]} 40. Re6+ {[%clk 0:02:53.7]} 40... Kg7 {[%clk 0:05:32.6]} 41. Bf5 {[%clk 0:02:49.6]} 41... h5 {[%clk 0:05:28.5]} 42. Rg6+ {[%clk 0:02:43.9]} 42... Kf7 {[%clk 0:05:24.7]} 43. Be6+ {[%clk 0:02:39.2]} 43... Kxg6 {[%clk 0:05:21]} 44. Kf1 {[%clk 0:02:23.9]} 44... Kf6 {[%clk 0:05:18.3]} 45. Bg8 {[%clk 0:02:18.1]} 45... Kf5 {[%clk 0:05:12.6]} 46. Ke2 {[%clk 0:02:15.8]} 46... Kg4 {[%clk 0:05:10.9]} 47. Ke3 {[%clk 0:02:06.1]} 47... h4 {[%clk 0:05:09.7]} 48. gxh4 {[%clk 0:02:02.4]} 48... gxh4 {[%clk 0:05:08.4]} 49. Ke4 {[%clk 0:01:58]} 49... h3 {[%clk 0:05:05.7]} 50. Kd5 {[%clk 0:01:54.1]} 50... Kf3 {[%clk 0:05:04.6]} 51. Kxc5 {[%clk 0:01:51.4]} 51... Kxf2 {[%clk 0:05:03.6]} 52. Kc6 {[%clk 0:01:49.4]} 52... Kg1 {[%clk 0:05:00.9]} 53. c5 {[%clk 0:01:46.8]} 53... Kxh2 {[%clk 0:04:59.7]} 54. Bd5 {[%clk 0:01:42.9]} 54... Kg3 {[%clk 0:04:56.3]} 55. Kb7 {[%clk 0:01:31.8]} 55... Kf4 {[%clk 0:04:54.6]} 56. c6 {[%clk 0:01:30.2]} 56... a5 {[%clk 0:04:52.1]} 57. c7 {[%clk 0:01:28.1]} 57... Ke5 {[%clk 0:04:51.1]} 58. Bh1 {[%clk 0:01:25.7]} 58... Kd6 {[%clk 0:04:45.7]} 59. c8=Q {[%clk 0:01:22.7]} 59... h2 {[%clk 0:04:40.2]} 60. Qd8+ {[%clk 0:01:17.8]} 60... Kc5 {[%clk 0:04:38.3]} 61. Qe7+ {[%clk 0:01:10.3]} 61... Kd4 {[%clk 0:04:36.5]} 62. Kc6 {[%clk 0:01:06.8]} 62... a4 {[%clk 0:04:32.2]} 63. Qd6+ {[%clk 0:01:05.2]} 63... Kc4 {[%clk 0:04:22.6]} 64. Qd5+ {[%clk 0:01:02.5]} 64... Kb4 {[%clk 0:04:18.8]} 65. Qc5# {[%clk 0:00:59.7]} 1-0";

            var test = pgnTest.Split("\n").Last();

            var test1 = Regex.Replace(test, @"\{[^\}]+\}", string.Empty);

            var test2 = Regex.Split(test1, @"\s\s");

            var test3 = new List<string>();
            for (int i = 0; i < test2.Length - 1; i++)
            {
                var move = Regex.Replace(test2[i], @"\d+\.*\s", string.Empty);
                move = Regex.Replace(move, @"[\#\+]", string.Empty);

                test3.Add(move);
            }

            var boardState = new BoardState();

            foreach (var move in test3)
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

                var location = move.Substring(move.Length - 2);
                var customLocation = ConvertToCustomLocationFormat(location);
                (int y, int x) selectedPieceLocation = (0, 0);

                var piecesNotaion = new Dictionary<char, Type>() {
                    { 'N', typeof(Knight) },
                    { 'B', typeof(Bishop) },
                    { 'Q', typeof(Queen) },
                    { 'R', typeof(Rook) },
                    { 'K', typeof(King) }
                };

                if (char.IsLower(move[0]))
                {
                    var pawns = playerPieces.FindAll(p => p is Pawn);

                    var selectedPiece = pawns.First(pawn => pawn.GetAllowedMoves(boardState).Any(s => s == customLocation));

                    selectedPieceLocation = boardState.GetPieceLocation(selectedPiece);
                }
                else if (piecesNotaion.ContainsKey(move[0]))
                {
                    var pieces = playerPieces.FindAll(p => p.GetType() == piecesNotaion[move[0]]);

                    var selectedPiece = pieces.First(knight => knight.GetAllowedMoves(boardState).Any(s => s == customLocation));

                    selectedPieceLocation = boardState.GetPieceLocation(selectedPiece);
                }

                boardState.UpdatePieces(selectedPieceLocation.y, selectedPieceLocation.x);
                boardState.UpdatePieces(customLocation.y, customLocation.x);
            }
        }

        private (int y, int x) ConvertToCustomLocationFormat(string standardFormat)
        {
            var horizontal = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

            var x = horizontal.ToList().IndexOf(standardFormat[0]);
            var y = 8 - (int)char.GetNumericValue(standardFormat[1]);

            return (y, x);
        }
    }
}
