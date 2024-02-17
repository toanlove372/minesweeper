using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridUtils
{
    public class TileCtrl : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;

        private GridManager gridManager;
        private Vector2Int cellIndex;
        private int value;

        public Vector2Int CellIndex { get => cellIndex; }

        public GridManager GridManager { get => gridManager; set => gridManager = value; }
        public int Value { get => value; }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetSize(Vector3 size)
        {
            this.transform.localScale = size;
        }

        public void InitPos(GridManager gridManager, Vector3Int cellIndex)
        {
            this.gridManager = gridManager;
            this.cellIndex = new Vector2Int(cellIndex.x, cellIndex.y);

            this.transform.SetParent(this.gridManager.grid.transform);
            this.name = string.Format("Tile ({0}, {1})", this.cellIndex.x, this.cellIndex.y);
            Vector3 tilePos = this.gridManager.grid.CellToLocal(cellIndex);
            this.transform.localPosition = tilePos;
        }

        public void SetValue(int value, Sprite sprite)
        {
            this.value = value;
            if (sprite != null)
            {
                this.spriteRenderer.sprite = sprite;
                this.spriteRenderer.color = Color.white;
            }
        }

        public void RemoveAll()
        {
            Destroy(this.gameObject);
        }

        //private void OnSelected()
        //{
        //    this.gridManager.OnTileSelected(this);
        //}

        //private void OnMouseDown()
        //{
        //    OnSelected();
        //}

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            string debugInfo;
            //debugInfo = string.Format("{0},{1}", this.cellIndex.x, this.cellIndex.y);
            debugInfo = this.value.ToString();
            UnityEditor.Handles.Label(transform.position, debugInfo, style);
        }
#endif
    }
}