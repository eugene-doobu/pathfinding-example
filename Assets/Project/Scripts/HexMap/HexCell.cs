using UnityEngine;

namespace FTG.HexMap
{
    public class HexCell : MonoBehaviour
    {
        public HexCoordinates coordinates;
        public Color          color;
        public RectTransform  uiRect;

        [SerializeField] private HexCell[] neighbors;

        private int elevation;

        public int Elevation
        {
            get => elevation;
            set
            {
                elevation = value;
                var position = transform.localPosition;
                position.y              = value * HexMetrics.elevationStep;
                transform.localPosition = position;

                var uiPosition = uiRect.localPosition;
                uiPosition.z         = elevation * -HexMetrics.elevationStep;
                uiRect.localPosition = uiPosition;
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
            return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
        }

        public HexEdgeType GetEdgeType(HexCell otherCell)
        {
            return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
        }
    }
}