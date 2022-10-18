using TMPro;
using UnityEngine;

namespace CustomGridSystem
{
    public class EdgeVisuals : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMesh;

        public void Init(string theName, bool isVert, bool displayText)
        {
            this.name = theName.ToString();
            if (displayText)
            {
                textMesh.text = theName;
            }
            else
            {
                textMesh.enabled = false;
            }

            if (isVert)
            {
                transform.Rotate(0f, 90f, 0f);
            }
        }
    }
}