using System.Collections.Generic;
using FTG.HexMap;

namespace FTG.PathFinding
{
    public interface IPathFinding
    {
        void SetHexMapController(HexMapController hexMapController);
        
        List<HexCell> FindPath(HexCell start, HexCell end);
        
        void ClearCellState(HexCell cell);
    }
}
