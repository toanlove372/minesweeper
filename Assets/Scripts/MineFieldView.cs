using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineFieldView : MonoBehaviour
{
    public MineTile mineTilePrefab;
    public Grid grid;

    public FlagPlacer flagPlacer;
    public Transform flagIcon;

    public Camera mainCam;

    private MineTile[][] mineTiles;
    private GameplayManager gameplayManager;

    public bool IsPlacingFlag { get; set; }

    public void Init(GameplayManager gameplayManager, int width, int height)
    {
        this.gameplayManager = gameplayManager;
        gameplayManager.onTileRevealed -= this.OnTileRevealed;
        gameplayManager.onTileRevealed += this.OnTileRevealed;
        gameplayManager.onTileFlagged -= this.OnTileFlagged;
        gameplayManager.onTileFlagged += this.OnTileFlagged;

        if (this.mineTiles != null)
        {
            for (var i = 0; i < this.mineTiles.Length; i++)
            {
                for (var j = 0; j < this.mineTiles[i].Length; j++)
                {
                    var mineTile = this.mineTiles[i][j];
                    Destroy(mineTile.gameObject);
                }
            }
        }

        this.mineTiles = new MineTile[height][];
        for (var i = 0; i < height; i++)
        {
            this.mineTiles[i] = new MineTile[width];
            for (var j = 0; j < width; j++)
            {
                var mineTile = Instantiate(this.mineTilePrefab, this.grid.transform);
                mineTile.transform.localPosition = this.grid.CellToLocal(new Vector3Int(j, i));
                mineTile.Init(this, j, i);
                mineTile.SetValue(GameplayManager.UnRevealedId);
                this.mineTiles[i][j] = mineTile;
            }
        }

        //this.flagPlacer.Init(this);
    }

    public void OnUpdate()
    {
        if (this.IsPlacingFlag)
        {
            var screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            var worldPoint = Camera.main.ScreenToWorldPoint(screenPos);
            worldPoint.z = 0f;
            this.flagIcon.transform.position = worldPoint;
        }
        else
        {
            this.flagIcon.transform.position = this.flagPlacer.transform.position;
        }

        if (Input.GetMouseButtonDown(0))
        {
            var raycastedObject = this.GetRaycastedObject();
            if (raycastedObject != null)
            {
                var mineTile = raycastedObject.GetComponent<MineTile>();
                if (mineTile != null)
                {
                    mineTile.OnMineTileClicked();
                }
                else
                {
                    if (raycastedObject.name == "FlagPlacer")
                    {
                        this.IsPlacingFlag = true;
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            var raycastedObject = this.GetRaycastedObject();
            if (raycastedObject != null)
            {
                var mineTile = raycastedObject.GetComponent<MineTile>();
                if (mineTile != null && this.IsPlacingFlag)
                {
                    mineTile.OnSetFlag();
                }
            }

            this.IsPlacingFlag = false;
        }
    }

    private Transform GetRaycastedObject()
    {
        var screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        var worldPoint = this.mainCam.ScreenToWorldPoint(screenPos);
        var raycast = Physics2D.Raycast(new Vector2(worldPoint.x, worldPoint.y), Vector2.zero);
        if (raycast.collider != null)
        {
            return raycast.collider.transform;
        }

        return null;
    }

    private void OnTileRevealed(int x, int y, int value)
    {
        this.mineTiles[y][x].SetValue(value);
    }

    private void OnTileFlagged(int x, int y, bool isFlagged)
    {
        var id = isFlagged ? GameplayManager.FlagId : GameplayManager.UnRevealedId;
        this.mineTiles[y][x].SetValue(id);
    }

    public void OnMineTileClicked(int x, int y)
    {
        this.gameplayManager.StartRevealTile(x, y);
    }

    public void OnMineTileFlagged(int x, int y)
    {
        this.gameplayManager.OnTileFlagged(x, y);
    }
}
