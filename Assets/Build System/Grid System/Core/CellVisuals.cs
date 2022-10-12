using TMPro;
using UnityEngine;

namespace CustomGridSystem
{
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

        public void Init(CellNumber cellNumber, bool displayNumber, Vector2 scale)
        {
            Init(cellNumber, displayNumber);
            transform.localScale = new Vector3(scale.x, transform.localScale.y, scale.y);
        }
    }
}