using UnityEngine;
using KSRecs.Deprecated.BuildSystemSpace;
using KSRecs.Deprecated.GridSystemSpace;
using KSRecs.Deprecated.GridSystemSpace.Drawer;


public class BuildSystemCustomComp : PlayerCompForBuildSystem
{
    [SerializeField] private LayerMask plotLayer;
    [SerializeField] private GridSquareVisual squareVisual;
    [SerializeField, ReadOnly]private GameObject builderParent;
    [SerializeField, ReadOnly]private Camera mainCam;
    [SerializeField, ReadOnly]private GridDrawer drawer;

    private bool isBuildiung = false;

    private void Start()
    {
        builderParent = GameObject.Find("Build System");
        drawer = FindObjectOfType<GridDrawer>();;
        builderParent.SetActive(false);
        mainCam = Camera.main;
    }

#if IncludeBuildSystem
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)
            && Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 10f, plotLayer)
            && hitInfo.collider.gameObject.TryGetComponent(out PlotScript plotScript))
        {
            EnterBuilderMode(plotScript);
        }
    }

    private void EnterBuilderMode(PlotScript plotScript)
    {
        if (isBuildiung) return;

        isBuildiung = true;
        builderParent.SetActive(true);

        Vector2 pos = plotScript.GetPlotStartPoint();
        Vector2 cellSize = PlotScript.GetPlotCellSize();
        CellNumber gridSize = PlotScript.GetPlotGridSize();
        
        GridSystem.Instance.SetupCall(gridSize, cellSize, pos);
        BuildSystem.Instance.SetupCall(mainCam, this);
        GridSystem.Instance.GridYPos = this.GetFloorHeight();
        
        drawer.SetupCall(squareVisual, CellNumber.zero, new CellNumber(50, 50));
        Debug.Log($"Position: {pos}, cellSize: {cellSize}, gridSize: {gridSize}, {GridSystem.Instance.GetCellPosition(new CellNumber())}");
        
    }

    void ExitBuilderMode()
    {
        builderParent.SetActive(false);
        isBuildiung = false;
    }
#endif
}