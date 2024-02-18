using UnityEngine;
using UnityEngine.UI;

namespace FTG.HexMap
{
    public class HexGrid : MonoBehaviour
    {
        [SerializeField] private int width  = 6;
        [SerializeField] private int height = 6;

        [SerializeField] private HexCell cellPrefab;
        [SerializeField] private Text    cellLabelPrefab;

        [SerializeField] private Color defaultColor = Color.white;
        
        private HexCell[] _cells;

        private Canvas  _gridCanvas;
        private HexMesh _hexMesh;

        private void Awake()
        {
            _gridCanvas = GetComponentInChildren<Canvas>();
            _hexMesh    = GetComponentInChildren<HexMesh>();
            _cells      = new HexCell[height * width];

            for (int z = 0, i = 0; z < height; z++)
            {
                for (var x = 0; x < width; x++) CreateCell(x, z, i++);
            }
        }

        private void Start()
        {
            _hexMesh.Triangulate(_cells);
        }

        public void Refresh()
        {
            _hexMesh.Triangulate(_cells);
        }

        public HexCell GetCell(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            var coordinates = HexCoordinates.FromPosition(position);
            var index       = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            return _cells[index];
        }

        private void CreateCell(int x, int z, int i)
        {
            Vector3 position;
            position.x = (x + z * 0.5f - Mathf.Floor(z * 0.5f)) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);

            var cell = _cells[i] = Instantiate(cellPrefab);
            cell.transform.SetParent(transform, false);
            cell.transform.localPosition = position;
            cell.Coordinates             = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.Color                   = defaultColor;

            if (x > 0) cell.SetNeighbor(HexDirection.W, _cells[i - 1]);
            if (z > 0)
            {
                if ((z & 1) == 0)
                {
                    cell.SetNeighbor(HexDirection.SE, _cells[i - width]);
                    if (x > 0)
                        cell.SetNeighbor(HexDirection.SW, _cells[i - width - 1]);
                }
                else
                {
                    cell.SetNeighbor(HexDirection.SW, _cells[i - width]);
                    if (x < width - 1)
                        cell.SetNeighbor(HexDirection.SE, _cells[i - width + 1]);
                }
            }

            var label = Instantiate(cellLabelPrefab, _gridCanvas.transform, false);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text                           = cell.Coordinates.ToStringOnSeparateLines();

            cell.UIRect = label.rectTransform;
        }
    }
}