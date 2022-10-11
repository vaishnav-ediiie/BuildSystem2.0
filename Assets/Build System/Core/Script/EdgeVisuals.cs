using TMPro;
using UnityEngine;


public class EdgeVisuals : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;

    public void Init(string text, bool isHorz, bool displayText)
    {
        this.name = text;
        if (displayText)
        {
            textMesh.text = text;
        }
        else
        {
            textMesh.enabled = false;
        }
        if (isHorz)
        {
            transform.Rotate(0f, 90f, 0f);
        }
    }
}
