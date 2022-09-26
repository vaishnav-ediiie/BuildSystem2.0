using UnityEngine;
using UnityEditor;
using KSRecs.Deprecated.GridSystemSpace;
using KSRecs.Deprecated.GridSystemSpace.Drawer;


namespace KSRecs.Deprecated.BuildSystemSpace.Editor
{
    public class SetupWindow : EditorWindow
    {
        private GameObject player;
        private float playerHeight;
        private float floorHeight;
        private int numberOfColumns;
        private int numberOfRows;
        private Vector2 cellSize;
        private Vector2 startPoint;
        private LayerMask probsLayer;
        private Camera playerCamera;
        private GridSquareVisual squareVisual;

        private bool createVisuals;
        private bool groupEnabled;
        private int startColumn;
        private int endColumn;
        private int startRow;
        private int endRow;

        [MenuItem("Build System/Setup for this scene")]
        static void Init()
        {
            SetupWindow window = (SetupWindow)EditorWindow.GetWindow(typeof(SetupWindow));
            window.Show();
        }

        void OnGUI()
        {
            player = (GameObject)EditorGUILayout.ObjectField("Player", player, typeof(GameObject), true);
            probsLayer = EditorGUILayout.LayerField("Probs Layer", probsLayer);
            playerCamera = (Camera)EditorGUILayout.ObjectField("Player Camera", playerCamera, typeof(Camera), true);
            playerHeight = EditorGUILayout.FloatField("Player Height", playerHeight);
            floorHeight = EditorGUILayout.FloatField("Floor Height", floorHeight);
            numberOfColumns = EditorGUILayout.IntField("Number of Columns", numberOfColumns);
            numberOfRows = EditorGUILayout.IntField("Number of Rows", numberOfRows);
            cellSize = EditorGUILayout.Vector2Field("Cell Size", cellSize);
            startPoint = EditorGUILayout.Vector2Field("Start Point", startPoint);

            createVisuals = EditorGUILayout.BeginToggleGroup("Create Visuals", createVisuals);
            startColumn = EditorGUILayout.IntField("Start Column", startColumn);
            startRow = EditorGUILayout.IntField("Start Row", startRow);
            endColumn = EditorGUILayout.IntField("End Column", endColumn);
            endRow = EditorGUILayout.IntField("End Row", endRow);
            squareVisual = (GridSquareVisual)EditorGUILayout.ObjectField("Grid Square Visual", squareVisual, typeof(GridSquareVisual), true);
            EditorGUILayout.EndToggleGroup();


            EditorGUILayout.LabelField("Notes:");
            EditorGUILayout.LabelField("1. GridSquareVisual prefab should be present at \"Build System/Drawer/Grid Square Visuals\"");
            EditorGUILayout.LabelField("2. You can always change these settings.");
            EditorGUILayout.LabelField("3. You will need to create a Input System. Call \"BuildSystem.Instance.StartProbSetup(BuildProbSO)\" to start placement of a Prob.");
            EditorGUILayout.LabelField("4. See \"BuildSystemInputHandler\"");
            GUILayout.Space(20);
            if (GUILayout.Button("Finish Setup")) SetupNow();
        }

        private void SetupNow()
        {
            PlayerCompForBuildSystem playerComp = player.AddComponent<PlayerCompForBuildSystem>();
            GridSystem gridSystem = new GameObject("Grid System").AddComponent<GridSystem>();
            BuildSystem buildSystem = new GameObject("Build System").AddComponent<BuildSystem>();

            gridSystem.SetupCall(new CellNumber(numberOfRows, numberOfColumns), cellSize, startPoint);
            buildSystem.SetupCall(probsLayer, playerCamera, playerComp);

            if (createVisuals)
            {
                gridSystem.gameObject.AddComponent<GridDrawer>().SetupCall(squareVisual, new CellNumber(startRow, startColumn), new CellNumber(endRow, endColumn));
            }

            EditorWindow.GetWindow(typeof(SetupWindow)).Close();
        }
    }
}