namespace CustomGridSystem
{
    public interface IGridType
    {
        bool IsCellNumberValid(CellNumber number);
        bool IsEdgeNumberValid(EdgeNumber number);
        CellNumber ValidateCellNumber(CellNumber number);
        CellNumber LastCellNumber { get; }
    }
}