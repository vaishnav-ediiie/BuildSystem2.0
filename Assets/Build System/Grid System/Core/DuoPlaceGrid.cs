using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


namespace CustomGridSystem
{
    /// <summary> Grid where objects can occupy both Edges and Cells </summary>
    /// <typeparam name="TCellOccupant">Type of object that will occupy Cell</typeparam>
    /// <typeparam name="TEdgeOccupant">Type of object that will occupy Edge</typeparam>
    public class DuoPlaceGrid<TCellOccupant, TEdgeOccupant> : SimpleGrid
    {
        private Dictionary<string, TCellOccupant> allCellOccupants;
        private Dictionary<string, TEdgeOccupant> allEdgeOccupants;

        #region Constructors
        public DuoPlaceGrid()
        {
            allCellOccupants = new Dictionary<string, TCellOccupant>();
            allEdgeOccupants = new Dictionary<string, TEdgeOccupant>();
        }

        public DuoPlaceGrid(Vector2 cellSize, float gridYPos = 0) : base(cellSize, gridYPos)
        {
            allCellOccupants = new Dictionary<string, TCellOccupant>();
            allEdgeOccupants = new Dictionary<string, TEdgeOccupant>();
        }

        public DuoPlaceGrid(CellNumber lastCellNumber, float gridYPos = 0) : base(lastCellNumber, gridYPos)
        {
            allCellOccupants = new Dictionary<string, TCellOccupant>();
            allEdgeOccupants = new Dictionary<string, TEdgeOccupant>();
        }

        public DuoPlaceGrid(CellNumber lastCellNumber, Vector2 cellSize, float gridYPos = 0) : base(lastCellNumber, cellSize, gridYPos)
        {
            allCellOccupants = new Dictionary<string, TCellOccupant>();
            allEdgeOccupants = new Dictionary<string, TEdgeOccupant>();
        }

        public DuoPlaceGrid(Vector2 cellSize, Vector2 anchorPosition, float gridYPos = 0) : base(cellSize, anchorPosition, gridYPos)
        {
            allCellOccupants = new Dictionary<string, TCellOccupant>();
            allEdgeOccupants = new Dictionary<string, TEdgeOccupant>();
        }

        public DuoPlaceGrid(CellNumber lastCellNumber, Vector2 cellSize, Vector2 anchorPosition, float gridYPos = 0) : base(lastCellNumber, cellSize, anchorPosition, gridYPos)
        {
            allCellOccupants = new Dictionary<string, TCellOccupant>();
            allEdgeOccupants = new Dictionary<string, TEdgeOccupant>();
        }
        #endregion

        #region Cell Methods
        private bool IsCellOccupied(string place)
        {
            return allCellOccupants.ContainsKey(place);
        }

        public bool IsCellOccupied(CellNumber place)
        {
            return IsCellNumberValid(place) && allCellOccupants.ContainsKey(place.ToString());
        }

        public void OccupyCell(CellNumber place, TCellOccupant occupant)
        {
            if (!IsCellNumberValid(place))
            {
                Debug.LogError($"Trying to occupy cell beyond bonds: cellNumber: {place}, LastCell: {LastCellNumber}");
                return;
            }
            string placeID = place.ToString();
            if (IsCellOccupied(placeID))
            {
                allCellOccupants[placeID] = occupant;
            }
            else
            {
                allCellOccupants.Add(placeID, occupant);
            }
        }

        public void EmptyCell(CellNumber place)
        {
            if (!IsCellNumberValid(place))
            {
                Debug.LogError($"Trying to empty cell beyond bonds: cellNumber: {place}, LastCell: {LastCellNumber}");
                return;
            }
            
            string placeID = place.ToString();
            if (IsCellOccupied(placeID))
            {
                allCellOccupants.Remove(placeID);
            }
        }

        public void MoveCellOccupant(CellNumber fromPlace, CellNumber toPlace)
        {
            if (!IsCellNumberValid(fromPlace))
            {
                Debug.LogError($"Trying to move occupant from cell beyond bonds: cellNumber: {fromPlace}, LastCell: {LastCellNumber}");
                return;
            }
            if (!IsCellNumberValid(toPlace))
            {
                Debug.LogError($"Trying to move occupant to cell beyond bonds: cellNumber: {toPlace}, LastCell: {LastCellNumber}");
                return;
            }
            
            string fromPlaceID = fromPlace.ToString();
            string toPlaceID = toPlace.ToString();
            if (!IsCellOccupied(fromPlaceID)) return;

            if (IsCellOccupied(toPlaceID)) allCellOccupants[toPlaceID] = allCellOccupants[fromPlaceID];
            else allCellOccupants.Add(toPlaceID, allCellOccupants[fromPlaceID]);
            allCellOccupants.Remove(fromPlaceID);
        }

        /// <param name="place">Identifier for the place</param>
        /// <param name="defaultTo">What to return if the place is not occupied</param>
        /// <returns> Occupant at give place </returns>
        public TCellOccupant GetCellOccupant(CellNumber place, TCellOccupant defaultTo)
        {
            string placeID = place.ToString();
            if (IsCellOccupied(placeID)) return allCellOccupants[placeID];
            return defaultTo;
        }
        #endregion

        #region Edge Methods
        private bool IsEdgeOccupied(string place)
        {
            return allEdgeOccupants.ContainsKey(place);
        }

        public bool IsEdgeOccupied(EdgeNumber place)
        {
            return IsEdgeNumberValid(place) && allEdgeOccupants.ContainsKey(place.ToString());
        }

        public void OccupyEdge(EdgeNumber place, TEdgeOccupant occupant)
        {
            if (!IsEdgeNumberValid(place))
            {
                Debug.LogError($"Trying to occupy Edge beyond bonds: edgeNumber: {place}, LastCell: {LastCellNumber}");
                return;
            }
            
            string placeID = place.ToString();
            if (IsEdgeOccupied(placeID))
            {
                allEdgeOccupants[placeID] = occupant;
            }
            else
            {
                allEdgeOccupants.Add(placeID, occupant);
            }
        }

        public void EmptyEdge(EdgeNumber place)
        {
            if (!IsEdgeNumberValid(place))
            {
                Debug.LogError($"Trying to empty Edge beyond bonds: edgeNumber: {place}, LastCell: {LastCellNumber}");
                return;
            }
            
            string placeID = place.ToString();
            if (IsEdgeOccupied(placeID))
            {
                allEdgeOccupants.Remove(placeID);
            }
        }

        public void MoveEdgeOccupant(EdgeNumber fromPlace, EdgeNumber toPlace)
        {
            if (!IsEdgeNumberValid(fromPlace))
            {
                Debug.LogError($"Trying to move occupant from edge beyond bonds: edgeNumber: {fromPlace}, LastCell: {LastCellNumber}");
                return;
            }
            if (!IsEdgeNumberValid(toPlace))
            {
                Debug.LogError($"Trying to move occupant to edge beyond bonds: edgeNumber: {toPlace}, LastCell: {LastCellNumber}");
                return;
            }
            
            string fromPlaceID = fromPlace.ToString();
            string toPlaceID = toPlace.ToString();
            if (!IsEdgeOccupied(fromPlaceID)) return;

            if (IsEdgeOccupied(toPlaceID)) allEdgeOccupants[toPlaceID] = allEdgeOccupants[fromPlaceID];
            else allEdgeOccupants.Add(toPlaceID, allEdgeOccupants[fromPlaceID]);
            allEdgeOccupants.Remove(fromPlaceID);
        }

        /// <param name="place">Identifier for the place</param>
        /// <param name="defaultTo">What to return if the place is not occupied</param>
        /// <returns> Occupant at give place </returns>
        public TEdgeOccupant GetEdgeOccupant(EdgeNumber place, TEdgeOccupant defaultTo)
        {
            string placeID = place.ToString();
            if (IsEdgeOccupied(placeID)) return allEdgeOccupants[placeID];
            return defaultTo;
        }
        #endregion

        #region Serialization
        public string SerializeWithOccupants(Func<TCellOccupant, string> cellOccupantSerializer, Func<TEdgeOccupant, string> edgeOccupantSerializer)
        {
            Dictionary<string, string> cellOccupantData = new Dictionary<string, string>();
            Dictionary<string, string> edgeOccupantData = new Dictionary<string, string>();
            HashSet<TCellOccupant> cellTemp = new HashSet<TCellOccupant>();             // This is because same object can occupy multiple places
            HashSet<TEdgeOccupant> edgeTemp = new HashSet<TEdgeOccupant>();             // This is because same object can occupy multiple places

            foreach (KeyValuePair<string, TCellOccupant> occupant in allCellOccupants)
            {
                if (!cellTemp.Contains(occupant.Value))
                {
                    cellOccupantData.Add(occupant.Key, cellOccupantSerializer.Invoke(occupant.Value));
                    cellTemp.Add(occupant.Value);
                }
            }

            foreach (KeyValuePair<string, TEdgeOccupant> occupant in allEdgeOccupants)
            {
                if (!edgeTemp.Contains(occupant.Value))
                {
                    edgeOccupantData.Add(occupant.Key, edgeOccupantSerializer.Invoke(occupant.Value));
                    edgeTemp.Add(occupant.Value);
                }
            }

            return JsonConvert.SerializeObject(
                new DuoPlaceGridData()
                {
                    baseGridData = base.SerializeGrid(),
                    edgeOccupantData = edgeOccupantData,
                    cellOccupantData = cellOccupantData
                }
            );
        }

        public void DeserializeWithOccupants(string data, Func<string, TCellOccupant> cellOccupantDeserializer, Func<string, TEdgeOccupant> edgeOccupantDeserializer)
        {
            DuoPlaceGridData gridData = JsonConvert.DeserializeObject<DuoPlaceGridData>(data);
            base.DeserializeGrid(gridData.baseGridData);

            if (this.allCellOccupants != null) this.allCellOccupants.Clear();
            else this.allCellOccupants = new Dictionary<string, TCellOccupant>();

            if (this.allEdgeOccupants != null) this.allEdgeOccupants.Clear();
            else this.allEdgeOccupants = new Dictionary<string, TEdgeOccupant>();


            foreach (KeyValuePair<string, string> occupant in gridData.cellOccupantData)
            {
                TCellOccupant oc = cellOccupantDeserializer.Invoke(occupant.Value);
                if (!allCellOccupants.ContainsKey(occupant.Key))
                    allCellOccupants.Add(occupant.Key, oc);
            }

            foreach (KeyValuePair<string, string> occupant in gridData.edgeOccupantData)
            {
                TEdgeOccupant oc = edgeOccupantDeserializer.Invoke(occupant.Value);
                if (!allEdgeOccupants.ContainsKey(occupant.Key))
                    allEdgeOccupants.Add(occupant.Key, oc);
            }
        }
        #endregion
    }
}