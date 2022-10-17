using System;
using System.Collections.Generic;
using CustomBuildSystem.Placed;
using UnityEngine;


namespace CustomBuildSystem
{
    public static class BuildEvents
    {
        // @formatter:off
        public static event Action<BuildSystem>                   OnBuildSystemCreated;
        public static event Action<BSS_PlacingEdge, PlacingState> OnEdgeStateChanged;
        public static event Action<BSS_PlacingCell, PlacingState> OnCellStateChanged;
        public static event Action<OccupantBaseMono>                OnItemDeleted;
        public static event Action                                OnGridUpdated;
        public static event Action<BuildSystem>                   OnBuildSystemDestroyed;

        
        internal static void Call_BuildSystemCreated(BuildSystem system)                               => OnBuildSystemCreated?.Invoke(system);
        internal static void Call_GridUpdate()                                                         => OnGridUpdated?.Invoke();
        internal static void Call_EdgeStateChanged(BSS_PlacingEdge placeableSo, PlacingState newState) => OnEdgeStateChanged?.Invoke(placeableSo, newState);
        internal static void Call_CellStateChanged(BSS_PlacingCell placeableSo, PlacingState newState) => OnCellStateChanged?.Invoke(placeableSo, newState);
        internal static void Call_OnItemDeleted(OccupantBaseMono itemDeleted)                            => OnItemDeleted?.Invoke(itemDeleted);
        internal static void Call_BuildSystemDestroyed(BuildSystem system)                             => OnBuildSystemDestroyed?.Invoke(system);
        // @formatter:on
    }

    public class BuildBrainBase
    {
        public BuildBrainBase()
        {
            GetMousePosition = new Vector2(Screen.width, Screen.height) / 2f;
        }


        /// <summary>
        /// Called every frame when we are placing something on Cell.
        /// </summary>
        /// <remarks>
        /// <para>current rotation = <c>placeable.Rotation</c></para>
        /// <para>current CellNumber = <c>placeable.CellNumber</c></para>
        /// <para>ScriptableObject being placed = <c>placeable.Scriptable</c></para>
        /// <para>if you want to check base conditions (set in inspector) call</para>
        /// <c> placeable.Scriptable.AreBaseConditionSatisfied(placeable)</c>
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
        /// <para>ScriptableObject being placed = <c>placeable.Scriptable</c></para>
        /// <para>if you want to check base conditions (set in inspector) call
        /// <c> placeable.Scriptable.AreBaseConditionSatisfied(placeable)</c>
        /// </para>
        /// </remarks>
        /// <param name="placeable">Object that we are placing</param>
        /// <returns>Return true to mark Okay, false to mark Error</returns>
        public virtual bool ValidateCellPlacement(BSS_PlacingCell placeable)
        {
            if (placeable.Placeable.isDecorator)
            {
                CellOccupantMono parent = placeable.BuildSystem.gridCurrent.GetCellOccupant(placeable.CellNumber, null);
                if (parent != null)
                    return placeable.Placeable.AreBaseConditionsSatisfied(placeable) && !parent.HasDecorator(placeable.Placeable);
            }

            return placeable.Placeable.AreBaseConditionsSatisfied(placeable);
        }

        /// <summary>
        /// Called every frame when we are placing something on Cell.
        /// </summary>
        /// <remarks>
        /// <para>current rotation = <c>placeable.Rotation</c></para>
        /// <para>current CellNumber = <c>placeable.CellNumber</c></para>
        /// <para>ScriptableObject being placed = <c>placeable.Scriptable</c></para>
        /// <para>if you want to check base conditions (set in inspector) call</para>
        /// <c> placeable.Scriptable.AreBaseConditionSatisfied(placeable)</c>
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
        /// <para>ScriptableObject being placed = <c>placeable.Scriptable</c></para>
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
        /// <para>ScriptableObject being placed = <c>placeable.Scriptable</c></para>
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
        /// <para>ScriptableObject being placed = <c>placeable.Scriptable</c></para>
        /// <para>if you want to check base conditions (set in inspector) call
        /// <c> placeable.Scriptable.AreBaseConditionSatisfied(placeable)</c>
        /// </para>
        /// </remarks>
        /// <param name="placeable">Object that we are placing</param>
        /// <returns>Return true if you want to show placing Okay, false if you want to show placing Error</returns>
        public virtual bool ValidateEdgePlacement(BSS_PlacingEdge placeable)
        {
            if (placeable.Placeable.isDecorator)
            {
                EdgeOccupantMono parent = placeable.BuildSystem.gridCurrent.GetEdgeOccupant(placeable.EdgeNumber, null);
                if (parent != null)
                    return placeable.Placeable.AreBaseConditionsSatisfied(placeable) && !parent.HasDecorator(placeable.Placeable);
            }

            return placeable.Placeable.AreBaseConditionsSatisfied(placeable);
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
        public virtual bool ShouldDeleteObject(OccupantBaseMono objectBeingDeleted)
        {
            return Input.GetMouseButtonDown(0);
        }

        public virtual Vector3 GetMousePosition { get; }
    }
}