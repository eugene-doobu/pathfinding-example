using UnityEngine;
using UnityEngine.EventSystems;

namespace FTG.HexMap
{
    public class HexMapEditor : MonoBehaviour
    {
        [field: SerializeField] public Color[] Colors  { get; set; }
        [field: SerializeField] public HexGrid HexGrid { get; set; }

        private Color _activeColor;
        private int   _activeElevation;

        private void Awake()
        {
            SelectColor(0);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) HandleInput();
        }

        private void HandleInput()
        {
            var        inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit)) EditCell(HexGrid.GetCell(hit.point));
        }

        private void EditCell(HexCell cell)
        {
            cell.Color     = _activeColor;
            cell.Elevation = _activeElevation;
            HexGrid.Refresh();
        }

        public void SelectColor(int index)
        {
            _activeColor = Colors[index];
        }

        public void SetElevation(float elevation)
        {
            _activeElevation = (int)elevation;
        }
    }
}