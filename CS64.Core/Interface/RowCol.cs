namespace CS64.Core.Interface
{
    public struct RowCol
    {
        public RowCol(uint col, uint row)
        {
            Row = row;
            Col = col;
        }

        public uint Row;
        public uint Col;
    }
}