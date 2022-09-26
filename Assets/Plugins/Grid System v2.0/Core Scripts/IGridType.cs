namespace CustomGridSystem
{
    internal interface IGridType
    {
        bool IsCellNumberValid(int row, int col);
        CellNumber ValidateCellNumber(CellNumber number);
        CellNumber LastCellNumber { get; }
    }
}