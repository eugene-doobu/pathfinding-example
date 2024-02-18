using UnityEngine;

namespace FTG.HexMap
{
    public class HexCell : MonoBehaviour
    {
        [field: SerializeField] public HexCoordinates Coordinates { get; set; }
        [field: SerializeField] public Color          Color { get; set; }
        [field: SerializeField] public RectTransform  UIRect { get; set; }

        [SerializeField] private HexCell[] neighbors;

        private int _elevation;

        public int Elevation
        {
            get => _elevation;
            set
            {
                _elevation = value;
                var position = transform.localPosition;
                position.y              = value * HexMetrics.elevationStep;
                transform.localPosition = position;

                var uiPosition = UIRect.localPosition;
                uiPosition.z         = _elevation * -HexMetrics.elevationStep;
                UIRect.localPosition = uiPosition;
            }
        }

        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int)direction];
        }

        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            neighbors[(int)direction]                 = cell;
            cell.neighbors[(int)direction.Opposite()] = this;
        }

        public HexEdgeType GetEdgeType(HexDirection direction)
        {
            return HexMetrics.GetEdgeType(_elevation, neighbors[(int)direction]._elevation);
        }

        public HexEdgeType GetEdgeType(HexCell otherCell)
        {
            return HexMetrics.GetEdgeType(_elevation, otherCell._elevation);
        }
    }
}