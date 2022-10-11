using CustomGridSystem;
using CustomGridSystem.Examples;
using UnityEngine;



public class AdjecentCellDemo : MonoBehaviour
{
    [SerializeField] private GridMaker gridMaker;
    [SerializeField] private Direction direction;
    [SerializeField] private Transform stickerCurrentCenter;
    [SerializeField] private Transform stickerAdjecentCenter;
    
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))  direction = Direction.Left;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) direction = Direction.Right;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))    direction = Direction.Up;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))  direction = Direction.Down;



        CellNumber number = gridMaker.SimpleGrid.CellPositionToNumber(transform.position);
        stickerCurrentCenter.position = gridMaker.SimpleGrid.CellNumberToPosition(number);
        stickerAdjecentCenter.position = gridMaker.SimpleGrid.CellNumberToPosition(gridMaker.SimpleGrid.AdjacentCellTo(number, direction));
        
        
    }
}
