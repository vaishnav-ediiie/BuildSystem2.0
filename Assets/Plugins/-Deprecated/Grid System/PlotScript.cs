// using GridSystemSpace;

using KSRecs.Deprecated.GridSystemSpace;
using UnityEngine;

public class PlotScript : MonoBehaviour
{
    private static readonly Vector2 plotSize = new Vector2(10, 10);
    private static readonly Vector2 cellSize = new Vector2(1.25f, 1.25f);
    private static readonly CellNumber cellsPerPlot = new CellNumber(8, 8);

    internal Vector2 GetPlotStartPoint() => new Vector2(transform.position.x - (plotSize.x - cellSize.x) / 2f, transform.position.z - (plotSize.y - cellSize.y) / 2f);
    internal static CellNumber GetPlotGridSize() => cellsPerPlot;
    internal static Vector2 GetPlotCellSize() => cellSize;
}