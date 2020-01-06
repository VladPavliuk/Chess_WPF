using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace WpfApp1.Pieces
{
    public abstract class BasePiece
    {
        public abstract string Name { get; }

        public abstract string ImageName { get; }

        public readonly Player ControlledBy;

        public readonly BitmapImage Image;

        public bool AlreadyMoved { get; set; }

        public BasePiece(Player controlledBy)
        {
            ControlledBy = controlledBy;

            var imagesBasePath = ConfigurationManager.AppSettings["ImagesBasePath"];

            switch (ControlledBy)
            {
                case Player.White:
                    {
                        Image = new BitmapImage(new Uri(Path.Combine(imagesBasePath, "Pieces", "White", $"white_{ImageName}.png")));
                        break;
                    }
                case Player.Black:
                    {
                        Image = new BitmapImage(new Uri(Path.Combine(imagesBasePath, "Pieces", "Black", $"black_{ImageName}.png")));
                        break;
                    }
                default:
                    {
                        throw new Exception("Undefined player");
                    }
            }
        }

        public abstract int[][] GetAllowedMoves(BoardState board);

        protected int[][] ApplyTransformations(BoardState board, int[][] locationsToFilter)
        {
            var filteredLocation = new List<(int y, int x)>();

            for (int i = 0; i < locationsToFilter.Length; i++)
            {
                var y = locationsToFilter[i][0];
                var x = locationsToFilter[i][1];

                if (x >= 8 || x < 0 || y >= 8 || y < 0)
                {
                    continue;
                }

                if (board.Squares[y, x].CurrentPiece == null
                    || (board.Squares[y, x].CurrentPiece.ControlledBy != ControlledBy && !(board.Squares[y, x].CurrentPiece is King)))
                {
                    filteredLocation.Add((y, x));
                }
            }

            return filteredLocation.Select(_ => new int[] { _.y, _.x }).ToArray();
        }

        protected List<(int y, int x)> GetLineMoves(
            BoardState board,
            List<(int y, int x)> allowedMoves,
            (int y, int x) coefficiants)
        {
            var pieceLocation = board.GetPieceLocation(this);

            for (int i = 1; i < 8; i++)
            {
                var y = pieceLocation.y + i * coefficiants.y;
                var x = pieceLocation.x + i * coefficiants.x;

                if (y < 0 || y > 7 || x < 0 || x > 7)
                {
                    break;
                }

                if (board.Squares[y, x].CurrentPiece != null)
                {
                    if (board.Squares[y, x].CurrentPiece.ControlledBy != ControlledBy)
                    {
                        allowedMoves.Add((y, x));
                    }

                    break;
                }

                allowedMoves.Add((y, x));
            }

            return allowedMoves;
        }
    }
}
