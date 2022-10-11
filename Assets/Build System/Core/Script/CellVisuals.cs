using CustomGridSystem;
using TMPro;
using UnityEngine;


public class CellVisuals : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;

    public void Init(CellNumber cellNumber, bool displayNumber)
    {
        this.name = cellNumber.ToString();
        if (displayNumber)
        {
            textMesh.text = cellNumber.ToString();
        }
        else
        {
            textMesh.enabled = false;
        }
    }
}