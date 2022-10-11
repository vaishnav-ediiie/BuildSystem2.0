using System;
using System.Collections.Generic;
using CustomGridSystem;
using UnityEngine;


[Serializable]
public class SimpleGridData
{
    public Vector2 cellSize;
    public Vector2 anchorPosition;
    public float gridYPos;
    public bool isFinite;
    public CellNumber lastCellNumber;
}


[Serializable]
public class UniPlaceGridData
{
    public string baseGridData;
    public Dictionary<string, string> occupantData;
}


[Serializable]
public class DuoPlaceGridData
{
    public string baseGridData;
    public Dictionary<string, string> cellOccupantData;
    public Dictionary<string, string> edgeOccupantData;
}