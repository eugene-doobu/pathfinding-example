using System.Collections;
using System.Collections.Generic;
using FTG.HexMap;
using UnityEngine;
using Utils;

namespace FTG.PathFinding
{
    public class DijkstraPathFinding : IPathFinding
    {
        private HexMapController _hexMapController;
        
        public void SetHexMapController(HexMapController hexMapController)
        {
            _hexMapController = hexMapController;
        }

        public List<HexCell> FindPath(HexCell start, HexCell end)
        {
            var pq        = new PriorityQueue<HexCell, int>();
            var cameFrom  = new Dictionary<HexCell, HexCell>();
            var costSoFar = new Dictionary<HexCell, int>();

            foreach (var cell in _hexMapController.HexGrid.Cells)
                costSoFar[cell] = int.MaxValue;
            
            pq.Enqueue(start, 0);
            cameFrom[start]  = start;
            costSoFar[start] = 0;
            
            while (pq.Count > 0)
            {
                var current = pq.Dequeue();
                if (current == end)
                    break;

                for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    var neighbor = current.GetNeighbor(d);
                    if (neighbor == null || cameFrom.ContainsKey(neighbor))
                        continue;

                    var cost = 1;
                    if (neighbor.Color == _hexMapController.Colors[2])
                        cost = 2;
                    if (neighbor.Color == _hexMapController.Colors[3])
                        cost = 3;

                    var newCost = costSoFar[current] + cost;
                    if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                    {
                        costSoFar[neighbor] = newCost;
                        var priority = newCost;
                        pq.Enqueue(neighbor, priority);
                        cameFrom[neighbor] = current;
                    }
                }
            }
            
            var path = new List<HexCell>();
            if (!cameFrom.ContainsKey(end))
                return path;

            for (var c = end; c != start; c = cameFrom[c])
                path.Add(c);
            
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
