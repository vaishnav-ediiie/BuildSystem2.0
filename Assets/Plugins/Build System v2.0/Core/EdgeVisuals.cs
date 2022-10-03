using CustomGridSystem;
using TMPro;
using UnityEngine;


public class EdgeVisuals : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;

    public void Init(EdgeNumber en)
    {
        this.name = textMesh.text = en.ToString();
        if (en.edgeType == EdgeType.Horizontal)
        {
            transform.Rotate(0f, 90f, 0f);
        }
    }
    
    public void Init(string text, bool isHorz)
    {
        this.name = textMesh.text = text;
        if (isHorz)
        {
            transform.Rotate(0f, 90f, 0f);
        }
    }
}
