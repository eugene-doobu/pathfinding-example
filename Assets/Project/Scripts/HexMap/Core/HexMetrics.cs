using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FTG.HexMap
{
    public enum HexEdgeType 
    {
        Flat, Slope, Cliff
    }
    
    public static class HexMetrics
    {
        public const float outerRadius = 10f;
        public const float innerRadius = outerRadius * 0.866025404f;
        
        public const float solidFactor = 0.75f;
        public const float blendFactor = 1f - solidFactor;

        public const float elevationStep = 5f;
        
        public const int terracesPerSlope = 2;
        public const int terraceSteps = terracesPerSlope * 2 + 1;
        
        public const float horizontalTerraceStepSize = 1f / terraceSteps;
        public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);

        #region Corners
        /// <summary>
        /// 6각형 꼭지점 좌표<br/>
        /// 6각형이지만, 인덱스 계산을 쉽게 하기 위해 첫번째 꼭지점과 같게 7번째 꼭지점을 추가함 
        /// </summary>
        private static Vector3[] corners = 
        {
            new(0f, 0f, outerRadius),
            new(innerRadius, 0f, 0.5f * outerRadius),
            new(innerRadius, 0f, -0.5f * outerRadius),
            new(0f, 0f, -outerRadius),
            new(-innerRadius, 0f, -0.5f * outerRadius),
            new(-innerRadius, 0f, 0.5f * outerRadius),
            new Vector3(0f, 0f, outerRadius)
        };
        
        public static Vector3 GetFirstSolidCorner(HexDirection direction) 
        {
            return corners[(int)direction] * solidFactor;
        }

        public static Vector3 GetSecondSolidCorner(HexDirection direction) 
        {
            return corners[(int)direction + 1] * solidFactor;
        }

        public static Vector3 GetFirstCorner(HexDirection direction) 
        {
            return corners[(int)direction];
        }

        public static Vector3 GetSecondCorner(HexDirection direction) 
        {
            return corners[(int)direction + 1];
        }
        #endregion Corners
        
        public static Vector3 GetBridge(HexDirection direction) 
        {
            return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
        }
        
        public static Vector3 TerraceLerp (Vector3 a, Vector3 b, int step) 
        {
            float h = step * horizontalTerraceStepSize;
            a.x += (b.x - a.x) * h;
            a.z += (b.z - a.z) * h;
            float v = Mathf.Floor((step + 1) * 0.5f) * verticalTerraceStepSize;
            a.y += (b.y - a.y) * v;
            return a;
        }
        
        public static Color TerraceLerp(Color a, Color b, int step) 
        {
            float h = step * horizontalTerraceStepSize;
            return Color.Lerp(a, b, h);
        }
        
        public static HexEdgeType GetEdgeType (int elevation1, int elevation2) 
        {
            if (elevation1 == elevation2) 
            {
                return HexEdgeType.Flat;
            }
            
            int delta = elevation2 - elevation1;
            if (delta == 1 || delta == -1) 
            {
                return HexEdgeType.Slope;
            }
            return HexEdgeType.Cliff;
        }
    }
}
