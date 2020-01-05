namespace WpfApp1.Pieces
{
    public class Bishop : BasePiece
    {
        public Bishop(string pathToImage) : base(pathToImage)
        {
        }

        public override string Name => nameof(Bishop);

        public override int[] GetAllowedMoves(BoardState board, BasePiece piece)
        {
            throw new System.NotImplementedException();
        }
    }
}
