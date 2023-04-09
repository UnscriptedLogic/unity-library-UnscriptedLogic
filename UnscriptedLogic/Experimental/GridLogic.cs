using UnityEngine;

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

    public struct Cell
    {
        private Vector2Int gridCoords;
        private Vector2 worldCoords;

        public Vector2Int GridCoords => gridCoords;
        public Vector2 WorldCoords => worldCoords;

        public Cell(Vector2 worldCoords, Vector2Int gridCoords)
        {
            this.worldCoords = worldCoords;
            this.gridCoords = gridCoords;
        }
    }

    public class GridLogic<CellItem>
    {
        private GridSettings settings;
        public Dictionary<Cell, CellItem> gridCells;

        public GridSettings Settings => settings;

        public GridLogic(GridSettings settings)
        {
            this.settings = settings;

            gridCells = new Dictionary<Cell, CellItem>();
        }

        public void CreateGrid(Action<Cell, Vector2> method)
        {
            Vector2 centerOffset = settings.Anchor - new Vector2((settings.Size.x - 1) * settings.CellSpacing / 2f, (settings.Size.y -  1) * settings.CellSpacing / 2f);

            ForEachNode((x, y) =>
            {
                Vector2 position = centerOffset + new Vector2(x * settings.CellSpacing, y * settings.CellSpacing);
                method(new Cell(position, new Vector2Int(x, y)), position);
            });
        }

        public Cell GetCellFromGrid(int coordx, int coordy)
        {
            Cell cell = new Cell();
            for (int i = 0; i < gridCells.Count; i++)
            {
                if (gridCells.ElementAt(i).Key.GridCoords == new Vector2Int(coordx, coordy))
                {
                    cell = gridCells.ElementAt(i).Key;
                    break;
                }
            }

            return cell;
        }

        public Cell GetCellFromWorldPosition(Vector2 worldPos)
        {
            Cell cell = new Cell();
            for (int i = 0; i < gridCells.Count; i++)
            {
                if (gridCells.ElementAt(i).Key.WorldCoords == worldPos)
                {
                    cell = gridCells.ElementAt(i).Key;
                    break;
                }
            }

            return cell;
        }

        public void ForEachNode(Action<int, int> method)
        {
            for (int x = 0; x < settings.Size.x; x++)
            {
                for (int y = 0; y < settings.Size.y; y++)
                {
                    method(x, y);
                }
            }
        }
    }
}
