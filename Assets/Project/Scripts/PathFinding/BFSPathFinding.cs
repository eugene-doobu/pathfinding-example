using System.Collections.Generic;
using FTG.HexMap;
using UnityEngine;

namespace FTG.PathFinding
{
    public class BFSPathFinding : MonoBehaviour, IPathFinding
    {
        private HexMapController _hexMapController;
        
        public void SetHexMapController(HexMapController hexMapController)
        {
            _hexMapController = hexMapController;
        }

        public List<HexCell> FindPath(HexCell start, HexCell end)
        {
            var queue = new Queue<HexCell>();
            var cameFrom = new Dictionary<HexCell, HexCell>();
            queue.Enqueue(start);
            cameFrom[start] = start;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == end)
                {
                    break;
                }

                for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    var neighbor = current.GetNeighbor(d);
                    if (neighbor == null || cameFrom.ContainsKey(neighbor))
                        continue;
                    
                    if (neighbor.Color == _hexMapController.Colors[2] || neighbor.Color == _hexMapController.Colors[3])
                        continue;

                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }

            var path = new List<HexCell>();
            if (!cameFrom.ContainsKey(end))
            {
                return path;
            }

            for (var c = end; c != start; c = cameFrom[c])
            {
                path.Add(c);
            }

            path.Add(start);
            path.Reverse();
            return path;
        }

        public void ClearCellState(HexCell cell)
        {
            if (cell.Color == _hexMapController.Colors[0] || cell.Color == _hexMapController.Colors[1])
                cell.Color = Color.white;
        }
    }
}
