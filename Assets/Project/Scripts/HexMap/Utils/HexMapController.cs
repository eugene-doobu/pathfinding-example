using UnityEngine;
using UnityEngine.EventSystems;

namespace FTG.HexMap
{
    public enum eInteractionMode
    {
        MAP_EDIT,
        MOVE,
    }

    public class HexMapController : MonoBehaviour
    {
        [SerializeField] private eInteractionMode interactionMode;
        
        private Color _activeColor;
        private int   _activeElevation;
        
        [field: SerializeField] public Color[] Colors  { get; set; }
        [field: SerializeField] public HexGrid HexGrid { get; set; }

        public GameManager GameManager { get; set; }

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
            GameManager.FindPath(cell);
            return;
            //TODO:
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