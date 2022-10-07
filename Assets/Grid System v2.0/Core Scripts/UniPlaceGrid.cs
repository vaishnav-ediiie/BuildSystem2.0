using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomGridSystem
{
    /// <summary>
    /// Grid where objects can occupy either Edges and Cells based on TNumber
    /// </summary>
    /// <typeparam name="TNumber">Pass CellNumber if cells to be occupied, and EdgeNumber if edges are to be occupied</typeparam>
    /// <typeparam name="TOccupant">Type of object that will be occupying this grid</typeparam>
    public class UniPlaceGrid<TNumber, TOccupant> : SimpleGrid where TNumber : IGridNumber
    {
        private Dictionary<string, TOccupant> allOccupants;

        public UniPlaceGrid()
        {
            allOccupants = new Dictionary<string, TOccupant>();
        }
        
        public UniPlaceGrid(Vector2 cellSize, float gridYPos = 0) : base(cellSize, gridYPos)
        {
            allOccupants = new Dictionary<string, TOccupant>();
        }

        public UniPlaceGrid(CellNumber lastCellNumber, float gridYPos = 0) : base(lastCellNumber, gridYPos)
        {
            allOccupants = new Dictionary<string, TOccupant>();
        }

        public UniPlaceGrid(CellNumber lastCellNumber, Vector2 cellSize, float gridYPos = 0) : base(lastCellNumber, cellSize, gridYPos)
        {
            allOccupants = new Dictionary<string, TOccupant>();
        }

        public UniPlaceGrid(Vector2 cellSize, Vector2 anchorPosition, float gridYPos = 0) : base(cellSize, anchorPosition, gridYPos)
        {
            allOccupants = new Dictionary<string, TOccupant>();
        }

        public UniPlaceGrid(CellNumber lastCellNumber, Vector2 cellSize, Vector2 anchorPosition, float gridYPos = 0) : base(lastCellNumber, cellSize, anchorPosition, gridYPos)
        {
            allOccupants = new Dictionary<string, TOccupant>();
        }

        private bool IsOccupied(string place)
        {
            return allOccupants.ContainsKey(place);
        }

        public bool IsOccupied(TNumber place)
        {
            return allOccupants.ContainsKey(place.ToString());
        }

        public void Occupy(TNumber place, TOccupant occupant)
        {
            string placeID = place.ToString();
            if (IsOccupied(placeID))
            {
                allOccupants[placeID] = occupant;
            }
            else
            {
                allOccupants.Add(placeID, occupant);
            }
        }

        public void Empty(TNumber place)
        {
            string placeID = place.ToString();
            if (IsOccupied(placeID))
            {
                allOccupants.Remove(placeID);
            }
        }

        public void MoveOccupant(TNumber fromPlace, TNumber toPlace)
        {
            string fromPlaceID = fromPlace.ToString();
            string toPlaceID = toPlace.ToString();
            if (!IsOccupied(fromPlaceID)) return;

            if (IsOccupied(toPlaceID)) allOccupants[toPlaceID] = allOccupants[fromPlaceID];
            else allOccupants.Add(toPlaceID, allOccupants[fromPlaceID]);
            allOccupants.Remove(fromPlaceID);
        }
        
        /// <param name="place">Identifier for the place</param>
        /// <param name="defaultTo">What to return if the place is not occupied</param>
        /// <returns> Occupant at give place </returns>
        public TOccupant GetOccupant(TNumber place, TOccupant defaultTo)
        {
            string placeID = place.ToString();
            if (IsOccupied(placeID)) return allOccupants[placeID];
            return defaultTo;
        }
        
        public string SerializeWithOccupants(Func<TOccupant, string> occupantSerializer)
        {
            Dictionary<string, string> allOccupantData = new Dictionary<string, string>();

            foreach (KeyValuePair<string,TOccupant> occupant in allOccupants)
            {
                allOccupantData.Add(occupant.Key, occupantSerializer.Invoke(occupant.Value));
            }
            
            return JsonUtility.ToJson(
                new UniPlaceGridData()
                {
                    baseGridData = base.SerializeGrid(),
                    occupantData = allOccupantData
                }
            );
        }

        public void DeserializeWithOccupants(string data, Func<string, TOccupant> occupantDeserializer)
        {
            UniPlaceGridData gridData = JsonUtility.FromJson<UniPlaceGridData>(data);
            base.DeserializeGrid(gridData.baseGridData);
            
            if (this.allOccupants != null) this.allOccupants.Clear();
            else this.allOccupants = new Dictionary<string, TOccupant>();
            foreach (KeyValuePair<string,string> occupant in gridData.occupantData)
            {
                allOccupants.Add(occupant.Key, occupantDeserializer.Invoke(occupant.Value));
            }
        }
    }
}