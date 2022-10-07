using CustomGridSystem;
using CustomGridSystem.Examples;
using UnityEngine;



public class CellEdgeDemo : MonoBehaviour
{
    [SerializeField] private GridMaker gridMaker;
    [SerializeField] private Direction direction;
    [SerializeField] private Transform sticker;

    private bool changed = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))  direction = Direction.Left;   changed = true;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) direction = Direction.Right;  changed = true;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))    direction = Direction.Up;     changed = true;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))  direction = Direction.Down;   changed = true;

        if (changed)
        {
            if (direction == Direction.Down || direction == Direction.Up) sticker.rotation = Quaternion.identity;
            else sticker.rotation = Quaternion.Euler(0f, 90f, 0f);
            changed = false;
        }
        

        CellNumber number = gridMaker.SimpleGrid.CellPositionToNumber(transform.position);
        EdgeNumber edge = number.GetEdgeNumber(direction);
        sticker.position = gridMaker.SimpleGrid.EdgeNumberToPosition(edge);
        Debug.Log(edge);
    }
}
