using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CustomGridSystem.Examples
{
    public class SimpleGridMono : MonoBehaviour
    {
        [SerializeField] private bool isFinite;
        [SerializeField] private CellNumber lastCellNumber;
        [SerializeField] private Vector2 cellSize;
        [SerializeField] private bool showVisuals;
        [SerializeField] private CellNumber visualsFirstCell;
        [SerializeField] private CellNumber visualsLastCell;
        [SerializeField] private CellVisuals cellVisuals;

        public SimpleGrid TheGrid;

        void Awake()
        {
            Vector3 position = transform.position;
            if (isFinite) TheGrid = new SimpleGrid(lastCellNumber, cellSize, new Vector2(position.x, position.z), position.y);
            else TheGrid = new SimpleGrid(cellSize, new Vector2(position.x, position.z), position.y);

            if (showVisuals)
            {
                foreach (CellNumber cellNumber in CellNumber.LoopCells(visualsFirstCell, visualsLastCell))
                {
                    Instantiate(cellVisuals, TheGrid.CellNumberToPosition(cellNumber), Quaternion.identity, transform).Init(cellNumber, true);
                }
            }
        }

        // @formatter:off
        public CellNumber LastCellNumber => TheGrid.LastCellNumber;
        
        public string SerializeGrid() => TheGrid.SerializeGrid();
        
        public CellNumber CellPositionToNumberRaw(Vector3 position) => TheGrid.CellPositionToNumberRaw(position);
        public void SetCellSize(Vector2 newSize)                    => TheGrid.SetCellSize(newSize);
        public void SetLastCellNumber(CellNumber cellNumber)        => TheGrid.SetLastCellNumber(cellNumber);
        public void SetYPosition(float yPosition)                   => TheGrid.SetYPosition(yPosition);
        public void MoveBy(Vector2 delta)                           => TheGrid.MoveBy(delta);
        public void MoveTo(Vector2 newPosition)                     => TheGrid.MoveTo(newPosition);
        public bool IsCellNumberValid(CellNumber cellNumber)        => TheGrid.IsCellNumberValid(cellNumber);
        public bool IsEdgeNumberValid(EdgeNumber edgeNumber)        => TheGrid.IsEdgeNumberValid(edgeNumber);
        public CellNumber ValidateCellNumber(CellNumber cellNumber) => TheGrid.ValidateCellNumber(cellNumber);
        public CellNumber CellPositionToNumber(Vector3 position)    => TheGrid.CellPositionToNumber(position);
        public Vector3 CellNumberToPosition(CellNumber cellNumber)  => TheGrid.CellNumberToPosition(cellNumber);
        public Vector3 EdgeNumberToPosition(EdgeNumber edgeNumber)  => TheGrid.EdgeNumberToPosition(edgeNumber);
        public void DeserializeGrid(string data)                    => TheGrid.DeserializeGrid(data);

        public CellNumber AdjacentCellToRaw(CellNumber referenceCell, Direction direction) => TheGrid.AdjacentCellToRaw(referenceCell, direction);
        public CellNumber AdjacentCellTo(CellNumber referenceCell, Direction direction)    => TheGrid.AdjacentCellTo(referenceCell, direction);
        public EdgeNumber EdgePositionToNumber(Vector3 position, EdgeType edgeType)        => TheGrid.EdgePositionToNumber(position, edgeType);
        public EdgeNumber EdgePositionToNumber(Vector3 position, Direction direction)      => TheGrid.EdgePositionToNumber(position, direction);
        
        public void UpdateInfo(CellNumber lastCellNumber, Vector2 newCellSize, Vector2 newAnchor, float newYPosition) => TheGrid.UpdateInfo(lastCellNumber, newCellSize, newAnchor, newYPosition);
        // @formatter:on
    }
}
#if UNITY_EDITOR
namespace CustomGridSystem.Examples.Editor
{
    [CustomEditor(typeof(SimpleGridMono))]
    public class SimpleGridMonoDrawer : UnityEditor.Editor
    {
        SerializedProperty isFinite;
        SerializedProperty lastCellNumber;
        SerializedProperty cellSize;
        SerializedProperty showVisuals;
        SerializedProperty visualsFirstCell;
        SerializedProperty visualsLastCell;
        SerializedProperty cellVisuals;

        private void OnEnable()
        {
            isFinite = serializedObject.FindProperty("isFinite");
            lastCellNumber = serializedObject.FindProperty("lastCellNumber");
            cellSize = serializedObject.FindProperty("cellSize");
            showVisuals = serializedObject.FindProperty("showVisuals");
            visualsFirstCell = serializedObject.FindProperty("visualsFirstCell");
            visualsLastCell = serializedObject.FindProperty("visualsLastCell");
            cellVisuals = serializedObject.FindProperty("cellVisuals");
        }


        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(isFinite);
            
            GUI.enabled = isFinite.boolValue; 
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(lastCellNumber);
            EditorGUI.indentLevel--;
            GUI.enabled = true; 
            
            EditorGUILayout.PropertyField(cellSize);
            EditorGUILayout.PropertyField(showVisuals);
            
            GUI.enabled = showVisuals.boolValue; 
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(visualsFirstCell);
            EditorGUILayout.PropertyField(visualsLastCell);
            EditorGUILayout.PropertyField(cellVisuals);
            EditorGUI.indentLevel--;
            GUI.enabled = true; 

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }            
        }
    }
}
#endif