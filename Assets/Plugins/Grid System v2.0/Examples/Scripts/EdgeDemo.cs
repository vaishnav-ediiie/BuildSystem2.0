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
        EdgeNumber numberHorz = gridMaker.SimpleGrid.EdgePositionToNumber(transform.position, EdgeType.Vertical);
        EdgeNumber numberVert = gridMaker.SimpleGrid.EdgePositionToNumber(transform.position, EdgeType.Horizontal);
        stickerHorizontal.position = gridMaker.SimpleGrid.EdgeNumberToPosition(numberHorz);
        stickerVertical.position = gridMaker.SimpleGrid.EdgeNumberToPosition(numberVert);
    }
}
