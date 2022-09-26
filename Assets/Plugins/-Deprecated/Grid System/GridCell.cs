using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace KSRecs.Deprecated.GridSystemSpace
{
    public class GridCell
    {
        public enum Place
        {
            // Note:  To understand the numbers assigned to below enumOptions, see GridCell.GetEdgePlace
            // Position WRT Center (x and z are center coordinates, s = cellSize/2)
            Left = 2, // x-s, z
            Right = 0, // x+s, z
            Up = 1, // x,   z+s
            Down = 3, // x,   z-s 
            Center = -1 // x,   z
        }

        private static Func<float, Place> GetAngleFunction; // Set by GridCell.SetAngleFunction
        private static float AngleWithCorner; // Set by GridCell.SetAngleFunction

        private Dictionary<Place, GridCellPlace> allOccupants;

        internal GridCell Init()
        {
            // completeCellOccupants = GridCellPlace.InitNew();

            allOccupants = new Dictionary<Place, GridCellPlace>()
            {
                { Place.Center, GridCellPlace.InitNew() },
                { Place.Left, GridCellPlace.InitNew() },
                { Place.Right, GridCellPlace.InitNew() },
                { Place.Up, GridCellPlace.InitNew() },
                { Place.Down, GridCellPlace.InitNew() }
            };
            return this;
        }

        internal void ClearSelf()
        {
            allOccupants[Place.Center].ClearSelf();
            allOccupants[Place.Left].ClearSelf();
            allOccupants[Place.Right].ClearSelf();
            allOccupants[Place.Up].ClearSelf();
            allOccupants[Place.Down].ClearSelf();
        }
        
        internal void AddOccupant(Place place, int floorNumber, GameObject occupant) => this.allOccupants[place].AddFloorOccupant(floorNumber, occupant);
        internal GameObject RemoveOccupant(Place place, int floorNumber) => this.allOccupants[place].RemoveFloorOccupant(floorNumber);
        internal GameObject GetOccupant(Place place, int floorNumber) => this.allOccupants[place].GetFloorOccupant(floorNumber);

        internal bool GetStats(out string output)
        {
            StringBuilder builder = new StringBuilder();

            string str;
            if (allOccupants[Place.Left].GetStats(out str)) builder.Append($"Left: {str}");
            if (allOccupants[Place.Right].GetStats(out str)) builder.Append($"Right: {str}");
            if (allOccupants[Place.Up].GetStats(out str)) builder.Append($"Up: {str}");
            if (allOccupants[Place.Down].GetStats(out str)) builder.Append($"Down: {str}");
            if (allOccupants[Place.Center].GetStats(out str)) builder.Append($"Center: {str}");
            output = builder.ToString();
            return builder.Length != 0;
        }

        /// <returns>
        /// returns true if the given place is occupied, otherwise false.
        /// </returns>
        internal bool IsPlaceOccupied(Place place, int floorNumber) => allOccupants[place].IsFloorOccupied(floorNumber);

        /// <returns>
        /// returns true if the given place is not-occupied, otherwise false.
        /// </returns>
        internal bool IsPlaceEmpty(Place place, int floorNumber) => allOccupants[place].IsFloorEmpty(floorNumber);

        internal static Place GetEdgePlace(Vector3 pointNearEdge, Vector3 cellCenterPos)
        {
            Vector3 direction = pointNearEdge - cellCenterPos;
            if (Mathf.Abs(direction.x) < 0.02f && Mathf.Abs(direction.z) < 0.02f)
                return Place.Center;

            float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.z, direction.x);
            return GetAngleFunction(angle);
        }

        private static Place GetAngleForSquare(float angle)
        {
            angle += 45f;
            if (angle < 0) angle += 360f;
            return (Place)(int)(angle / 90f);
        }

        private static Place GetAngleForRectangle(float angle)
        {
            angle += AngleWithCorner / 2f;
            if (angle < 0) angle += 360f;

            if (angle < AngleWithCorner) return Place.Left;
            if (angle < 180) return Place.Down;
            if (angle < AngleWithCorner + 180) return Place.Right;
            return Place.Up;
        }

        internal static Place OppositeTo(Place place)
        {
            if (place == Place.Down) return Place.Up;
            if (place == Place.Up) return Place.Down;
            if (place == Place.Left) return Place.Right;
            if (place == Place.Right) return Place.Left;
            return Place.Center;
        }

        internal static void SetAngleFunction()
        {
            if (Math.Abs(GridSystem.Instance.CellSize.x - GridSystem.Instance.CellSize.y) < 0.1f) GridCell.GetAngleFunction = GridCell.GetAngleForSquare;
            else GridCell.GetAngleFunction = GridCell.GetAngleForRectangle;
            AngleWithCorner = 2f * Mathf.Rad2Deg * Mathf.Atan2(GridSystem.Instance.CellSize.y / 2f, GridSystem.Instance.CellSize.x / 2f);
        }
    }


    internal struct GridCellPlace
    {
        private Dictionary<int, GameObject> allFloors;

        public GridCellPlace(Dictionary<int, GameObject> allFloors)
        {
            this.allFloors = allFloors;
        }

        internal static GridCellPlace InitNew() => new GridCellPlace(new Dictionary<int, GameObject>());

        internal bool IsFloorOccupied(int floorNumber) => allFloors.ContainsKey(floorNumber);
        internal bool IsFloorEmpty(int floorNumber) => !allFloors.ContainsKey(floorNumber);

        internal void AddFloorOccupant(int floorNumber, GameObject occupant)
        {
            if (IsFloorOccupied(floorNumber)) allFloors[floorNumber] = occupant;
            else allFloors.Add(floorNumber, occupant);
        }

        internal GameObject RemoveFloorOccupant(int floorNumber)
        {
            if (IsFloorOccupied(floorNumber))
            {
                GameObject occ = allFloors[floorNumber];
                allFloors.Remove(floorNumber);
                return occ;
            }

            return null;
        }

        internal GameObject GetFloorOccupant(int floorNumber)
        {
            if (IsFloorOccupied(floorNumber)) return this.allFloors[floorNumber];
            return null;
        }

        public bool GetStats(out string str)
        {
            if (allFloors.Count == 0)
            {
                str = "";
                return false;
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("");
            foreach (KeyValuePair<int, GameObject> kvp in allFloors)
            {
                builder.Append($"Floor {kvp.Key} {kvp.Value.name}");
                builder.Append(", ");
            }

            builder.Append("\n");
            str = builder.ToString();
            return true;
        }

        internal void ClearSelf() => allFloors.Clear();
    }
}