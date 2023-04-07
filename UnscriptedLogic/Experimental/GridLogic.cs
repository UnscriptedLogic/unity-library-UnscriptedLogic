using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnscriptedLogic.Experimental.Generation
{
    [System.Serializable]
    public struct GridSettings
    {
        [SerializeField] private Vector2Int gridSize = new Vector2Int(5, 5);
        [SerializeField] private float cellSpacing;
        [SerializeField] private float cellScale;
        [SerializeField] private Vector2 anchor = Vector2.zero;

        public Vector2 Anchor => anchor;
        public Vector2Int Size => gridSize;
        public float CellSpacing => cellSpacing;
        public float CellScale => cellScale;
        public int TotalSize => gridSize.x * gridSize.y;
        public int HighestAxis => Mathf.Max(gridSize.x, gridSize.y);

        public GridSettings(Vector2 anchor, Vector2Int gridSize, float cellSpacing, float cellScale)
        {
            this.anchor = anchor;
            this.gridSize = gridSize;
            this.cellSpacing = cellSpacing;
            this.cellScale = cellScale;
        }
    }

    public class GridLogic<Cell>
    {
        private GridSettings settings;
        public Cell[,] items;

        public GridSettings Settings => settings;

        public GridLogic(GridSettings settings)
        {
            this.settings = settings;

            items = new Cell[settings.Size.x, settings.Size.y];
        }

        public void CreateGrid(Action<int, int, Vector2> method)
        {
            Vector2 centerOffset = settings.Anchor - new Vector2((settings.Size.x - 1) * settings.CellSpacing / 2f, (settings.Size.y -  1) * settings.CellSpacing / 2f);
            for (int x = 0; x < settings.Size.x; x++)
            {
                for (int y = 0; y < settings.Size.y; y++)
                {
                    Vector2 position = centerOffset + new Vector2(x * settings.CellSpacing, y * settings.CellSpacing);
                    method(x, y, position);
                }
            }
        }
    }
}
