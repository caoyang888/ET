namespace ET
{
    public readonly struct SlotCellPos
    {
        public SlotCellPos(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }

        public int Row { get; }
        public int Column { get; }
    }
}
