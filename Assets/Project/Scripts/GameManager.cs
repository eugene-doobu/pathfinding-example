using System;
using System.Collections;
using System.Collections.Generic;
using FTG.HexMap;
using FTG.PathFinding;
using UnityEngine;

namespace FTG
{
    public enum ePathFindingMode
    {
        BFS,
        DIJKSTRA,
        A_STAR,
    }
    
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private ePathFindingMode pathFindingMode;
        [SerializeField] private HexMapController hexMapController;
        [SerializeField] private Actor            actor;
        [SerializeField] private HexGrid          hexGrid;
        
        [field: SerializeField] public HexCell CurrentCell { get; set; }
        
        private BFSPathFinding _bfsPathFinding = new();
        
        private bool _isOnPathFindingRender;

        private void Start()
        {
            hexMapController.GameManager = this;
            CurrentCell = hexGrid.GetCell(Vector3.zero);
        }
        
        public void FindPath(HexCell end)
        {
            if (_isOnPathFindingRender)
                return;

            ClearGridState();
            
            _isOnPathFindingRender = true;
            
            IPathFinding pathFinding = null;
            switch (pathFindingMode)
            {
                case ePathFindingMode.BFS:
                    pathFinding = _bfsPathFinding;
                    break;
            }

            var path = pathFinding?.FindPath(CurrentCell, end);
            if (path == null) return;

            StartCoroutine(RenderPath(path));
        }
        
        private IEnumerator RenderPath(List<HexCell> path)
        {
            var startCell = path[0];
            actor.SetStartCell(startCell);
            startCell.Color = hexMapController.Colors[0];
            
            var lastCell = path[^1];
            lastCell.Color = hexMapController.Colors[1];
            hexGrid.Refresh();

            for (var i = 1; i < path.Count; i++)
            {
                actor.Target = path[i].transform;
                yield return null;
                yield return new WaitUntil(() => actor.AnimState == AnimationState.IDLE);
                CurrentCell = path[i];
                path[i].Color = hexMapController.Colors[0];
                hexGrid.Refresh();
            }
            
            _isOnPathFindingRender = false;
        }
        
        private void ClearGridState()
        {
            foreach (var cell in hexGrid.Cells)
            {
                cell.Color = Color.white;
            }
            hexGrid.Refresh();
        }
    }
}
