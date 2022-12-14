using CustomGridSystem;
using CustomGridSystem.Examples;
using UnityEngine;


public class EdgeDemo : MonoBehaviour
{
    [SerializeField] private GridMaker gridMaker;
    [SerializeField] private Transform stickerHorizontal;
    [SerializeField] private Transform stickerVertical;

    void Update()
    {
        EdgeNumber numberHorz = gridMaker.SimpleGrid.EdgePositionToNumber(transform.position, EdgeType.Horizontal);
        EdgeNumber numberVert = gridMaker.SimpleGrid.EdgePositionToNumber(transform.position, EdgeType.Vertical);
        stickerHorizontal.position = gridMaker.SimpleGrid.EdgeNumberToPosition(numberHorz);
        stickerVertical.position = gridMaker.SimpleGrid.EdgeNumberToPosition(numberVert);
        Debug.Log(numberHorz);
        Debug.Log(numberVert);
    }
}
