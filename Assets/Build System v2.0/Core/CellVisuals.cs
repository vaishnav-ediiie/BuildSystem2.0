using CustomGridSystem;
using TMPro;
using UnityEngine;


public class CellVisuals : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;

    public void Init(CellNumber cellNumber)
    {
        this.name = textMesh.text = cellNumber.ToString();
    }
}