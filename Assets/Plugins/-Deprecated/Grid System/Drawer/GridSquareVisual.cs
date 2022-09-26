using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace KSRecs.Deprecated.GridSystemSpace.Drawer
{
    public class GridSquareVisual : MonoBehaviour
    {
        [SerializeField] private TMP_Text gridText;
        [SerializeField] private SpriteRenderer square;

        private static readonly Dictionary<GridCell.Place, int> sides = new Dictionary<GridCell.Place, int>()
        {
            { GridCell.Place.Left, 0 },
            { GridCell.Place.Right, 1 },
            { GridCell.Place.Up, 2 },
            { GridCell.Place.Down, 3 },
        };

        [SerializeField] private GameObject[] sidesObjects;

        public void Init(CellNumber number, Vector3 scale)
        {
            gridText.text = $"{number.row}, {number.column}";
            transform.localScale = scale;
        }

        internal void SetVisualColor(Color color)
        {
            square.color = color;
        }

        internal void SetActiveEdge(GridCell.Place edge, bool value)
        {
            sidesObjects[sides[edge]].gameObject.SetActive(value);
        }

        internal void SetVisualColorFor1Frame(Color color)
        {
            StartCoroutine(SetColEnum(color));
        }

        private IEnumerator SetColEnum(Color color)
        {
            square.color = color;
            yield return new WaitForSeconds(0.5f);
            square.color = Color.black;
        }
    }
}