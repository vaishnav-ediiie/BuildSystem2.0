using System;
using System.Collections.Generic;
using UnityEngine;


namespace CustomBuildSystem
{
    public class BuildSystemBrain
    {
        public event Action<BSS_PlacingEdge, PlacingState> OnEdgeStateChanged;
        public event Action<BSS_PlacingCell, PlacingState> OnCellStateChanged;
        public event Action<IMonoPlaceable>                OnItemDeleted;
        public event Action                                OnGridUpdated;
        public Dictionary<int, PlaceableSOBase> AllPlaceableData;

        internal void Call_GridUpdate() => OnGridUpdated?.Invoke();
        internal void Call_EdgeStateChanged(BSS_PlacingEdge placeableSo, PlacingState newState) => OnEdgeStateChanged?.Invoke(placeableSo, newState);
        internal void Call_CellStateChanged(BSS_PlacingCell placeableSo, PlacingState newState) => OnCellStateChanged?.Invoke(placeableSo, newState);
        internal void Call_OnItemDeleted(IMonoPlaceable itemDeleted) => OnItemDeleted?.Invoke(itemDeleted);

        internal void CopyFrom(BuildSystemBrain source, bool copyPlaceableDataFromLastBrain)
        {
            this.OnGridUpdated = source.OnGridUpdated;
            this.OnItemDeleted = source.OnItemDeleted;
            this.OnEdgeStateChanged = source.OnEdgeStateChanged;
            this.OnCellStateChanged = source.OnCellStateChanged;
            if (copyPlaceableDataFromLastBrain)
                this.AllPlaceableData = source.AllPlaceableData;
        }


        /// <summary>
        /// Called every frame when we are placing something on Cell.
        /// </summary>
        /// <remarks>
        /// <para>current rotation = <c>placeable.Rotation</c></para>
        /// <para>current CellNumber = <c>placeable.CellNumber</c></para>
        /// <para>ScriptableObject being placed = <c>placeable.PlaceableSo</c></para>
        /// <para>if you want to check base conditions (set in inspector) call</para>
        /// <c> placeable.PlaceableSo.AreBaseConditionSatisfied(placeable)</c>
        /// </remarks>
        /// <param name="placeable">Object that we are placing</param>
        /// <returns>+1 to rotate object by 90 degrees, -1 to rotate object by -90 degrees, 0 to not rotate</returns>
        public virtual int ShouldRotateCell(BSS_PlacingCell placeable)
        {
            if (Input.GetKeyDown(KeyCode.R))
                return 0;
            return 1;
        }

        /// <summary> Called every time when the object is rotated or CellNumber is changed. </summary>
        /// <remarks>
        /// <para>current rotation = <c>placeable.Rotation</c></para>
        /// <para>current CellNumber = <c>placeable.CellNumber</c></para>
        /// <para>ScriptableObject being placed = <c>placeable.PlaceableSo</c></para>
        /// <para>if you want to check base conditions (set in inspector) call
        /// <c> placeable.PlaceableSo.AreBaseConditionSatisfied(placeable)</c>
        /// </para>
        /// </remarks>
        /// <param name="placeable">Object that we are placing</param>
        /// <returns>Return +1 to rotate object by 90 degrees, -1 to rotate object by -90 degrees , 0 to not rotate</returns>
        public virtual bool ValidateCellPlacement(BSS_PlacingCell placeable)
        {
            return placeable.PlaceableSo.AreBaseConditionsSatisfied(placeable);
        }
        
        /// <summary>
        /// Called every frame when we are placing something on Cell.
        /// </summary>
        /// <remarks>
        /// <para>current rotation = <c>placeable.Rotation</c></para>
        /// <para>current CellNumber = <c>placeable.CellNumber</c></para>
        /// <para>ScriptableObject being placed = <c>placeable.PlaceableSo</c></para>
        /// <para>if you want to check base conditions (set in inspector) call</para>
        /// <c> placeable.PlaceableSo.AreBaseConditionSatisfied(placeable)</c>
        /// </remarks>
        /// <param name="placeable">Scriptable that we are placing</param>
        /// <returns>true when you want to confirm placement, false otherwise</returns>
        public virtual bool ShouldPlaceCell(BSS_PlacingCell placeable)
        {
            return placeable.CanPlace && Input.GetMouseButtonDown(0);
        }
        
        /// <summary>
        /// Called every frame when we are placing something on Edge.
        /// </summary>
        /// <remarks>
        /// <para>current rotation = <c>placeable.Rotation</c></para>
        /// <para>current EdgeNumber = <c>placeable.EdgeNumber</c></para>
        /// <para>ScriptableObject being placed = <c>placeable.PlaceableSo</c></para>
        /// </remarks>
        /// <param name="placeable">Object that we are placing</param>
        /// <returns>true when you want to rotate object by 180 degrees, false otherwise</returns>
        public virtual bool ShouldRotateEdge(BSS_PlacingEdge placeable)
        {
            return Input.GetKeyDown(KeyCode.R);
        }
        
        /// <summary>
        /// Called every frame when we are placing something on Edge.
        /// Access current rotation via placeable.Rotation, current EdgeNumber via placeable.EdgeNumber
        /// </summary>
        /// <remarks>
        /// <para>current rotation = <c>placeable.Rotation</c></para>
        /// <para>current EdgeNumber = <c>placeable.EdgeNumber</c></para>
        /// <para>ScriptableObject being placed = <c>placeable.PlaceableSo</c></para>
        /// </remarks>
        /// <param name="placeable">Object that we are placing</param>
        /// <returns>true when you want to confirm placement, false otherwise</returns>
        public virtual bool ShouldPlaceEdge(BSS_PlacingEdge placeable)
        {
            return placeable.CanPlace && Input.GetMouseButtonDown(0);
        }

        /// <summary>
        /// Called every time when the EdgeNumber is changed.
        /// </summary>
        /// <remarks>
        /// <para>current rotation = <c>placeable.Rotation</c></para>
        /// <para>current EdgeNumber = <c>placeable.EdgeNumber</c></para>
        /// <para>ScriptableObject being placed = <c>placeable.PlaceableSo</c></para>
        /// <para>if you want to check base conditions (set in inspector) call
        /// <c> placeable.PlaceableSo.AreBaseConditionSatisfied(placeable)</c>
        /// </para>
        /// </remarks>
        /// <param name="placeable">Object that we are placing</param>
        /// <returns>Return true if you want to show placing Okay, false if you want to show placing Error</returns>
        public virtual bool ValidateEdgePlacement(BSS_PlacingEdge placeable)
        {
            return placeable.PlaceableSo.AreBaseConditionsSatisfied(placeable);
            /*if (placeable.PlaceableSo.cellsCount <= 1)
            {
                if (placeable.PlaceableSo.isDecorator)
                {
                    EdgePlaceable parent = placeable.BuildSystem.gridCurrent.GetEdgeOccupant(placeable.EdgeNumber, null);
                    return parent != null && !parent.HasDecorator(placeable.PlaceableSo) ;
                } 
                return ! placeable.BuildSystem.gridCurrent.IsEdgeOccupied(placeable.EdgeNumber);
            }

            
            bool isHorz = (placeable.EdgeNumber.edgeType == EdgeType.Horizontal);
            EdgeNumber first;
            if (isHorz)
            {
                int startRow = placeable.EdgeNumber.CellAfter.row - placeable.PlaceableSo.centerCellIndex;
                first = new EdgeNumber(startRow, placeable.EdgeNumber.CellAfter.column, placeable.EdgeNumber.edgeType);
            }
            else
            {
                int startCol = placeable.EdgeNumber.CellAfter.column - placeable.PlaceableSo.centerCellIndex;
                first = new EdgeNumber(placeable.EdgeNumber.CellAfter.row, startCol, placeable.EdgeNumber.edgeType);
            }

            CellNumber current = new CellNumber(0, 0);
            bool check = !placeable.PlaceableSo.isDecorator;

            for (int _ = 0; _ < placeable.PlaceableSo.cellsCount; _++)
            {
                EdgeNumber en = first + current;
                bool isNumberValid = placeable.BuildSystem.gridCurrent.IsEdgeNumberValid(en);
                bool isOccupied = placeable.BuildSystem.gridCurrent.IsEdgeOccupied(en);
                if (!isNumberValid || isOccupied == check)
                {
                    return false;
                }

                if (isHorz) current.row++;
                else current.column++;
            }

            return true;*/
        }
        
        /// <summary> Called every frame. </summary>
        /// <param name="currentState">Current state of Build System</param>
        /// <returns>true if you want to enter Delete Mode</returns>
        public virtual bool ShouldEnterDeleteMove(BuiltSystemState currentState)
        {
            return currentState.GetType() == typeof(BSS_Idle) && Input.GetKeyDown(KeyCode.X);
        }
        
        /// <summary> Called every frame. </summary>
        /// <param name="objectBeingDeleted"></param>
        /// <returns>true if you want to delete this object</returns>
        public virtual bool ShouldDeleteObject(IMonoPlaceable objectBeingDeleted)
        {
            return Input.GetMouseButtonDown(0);
        }
    }
}