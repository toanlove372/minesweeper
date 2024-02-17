using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridUtils
{
    public class GridManager : MonoBehaviour
    {

        public Grid grid;
        public TileCtrl tilePrefab;
        public Sprite[] tileSprites;

        private int numbCellWidth;
        private int numbCellHeight;
        private MapData mapData;

        private TileCtrl[][] tileIndexes;

        public MapData MapData => this.mapData;

        public void Init(MapData mapData)
        {
            this.RemoveAll();

            this.mapData = mapData;
            this.numbCellWidth = mapData.width;
            this.numbCellHeight = mapData.height;
            CreateGrid();
        }

        private void CreateGrid()
        {
            this.tilePrefab.SetSize(this.grid.cellSize);
            Vector3Int cellPos;
            this.tileIndexes = new TileCtrl[this.numbCellHeight][];

            for (int i = 0; i < this.numbCellHeight; i++)
            {
                this.tileIndexes[i] = new TileCtrl[this.numbCellWidth];
                for (int j = 0; j < this.numbCellWidth; j++)
                {
                    cellPos = new Vector3Int(j, i, 0);

                    TileCtrl tile = Instantiate<TileCtrl>(this.tilePrefab);
                    tile.InitPos(this, cellPos);
                    int tileValue = this.mapData.GetValue(j, i);
                    tile.SetValue(tileValue, this.tileSprites[tileValue]);
                    this.tileIndexes[i][j] = tile;
                }
            }

            Vector3 bottomLeftGridPos = new Vector3(
                this.numbCellWidth * (this.grid.cellSize.x + this.grid.cellGap.x) * -0.5f + this.grid.cellSize.x * 0.5f,
                this.numbCellHeight * (this.grid.cellSize.y + this.grid.cellGap.y) * -0.5f + this.grid.cellSize.y * 0.5f);
            this.grid.transform.localPosition = bottomLeftGridPos;
        }

        public void RemoveAll()
        {
            for (int i = 0; i < this.numbCellHeight; i++)
            {
                for (int j = 0; j < this.numbCellWidth; j++)
                {
                    this.tileIndexes[i][j].RemoveAll();
                }
            }

            this.numbCellHeight = 0;
            this.numbCellWidth = 0;
            this.tileIndexes = null;
        }

        public Vector2Int WorldToGridPos(Vector3 position)
        {
            Vector3Int cellPos = this.grid.WorldToCell(position);
            return new Vector2Int(cellPos.x, cellPos.y);
        }

        public void SetValueAtPos(int x, int y, int value)
        {
            this.tileIndexes[y][x].SetValue(value, this.tileSprites[value]);
        }
    }
}