using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineTile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    private MineFieldView mineField;
    private int x;
    private int y;

    public void Init(MineFieldView mineField, int x, int y)
    {
        this.mineField = mineField;
        this.x = x;
        this.y = y;
    }

    public void SetValue(int value)
    {
        this.spriteRenderer.sprite = this.sprites[value + 1];
    }

    public void OnMineTileClicked()
    {
        this.mineField.OnMineTileClicked(this.x, this.y);
    }

    public void OnSetFlag()
    {
        this.mineField.OnMineTileFlagged(this.x, this.y);
    }

    //public void OnMouseDown()
    //{
    //    if (this.mineField.IsPlacingFlag)
    //    {
    //        this.mineField.IsPlacingFlag = false;
    //        this.SetValue(GameplayManager.FlagId);
    //        return;
    //    }

    //    this.mineField.OnMineTileClicked(this.x, this.y);
    //}

    //public void OnMouseUp()
    //{
    //    if (this.mineField.IsPlacingFlag)
    //    {
    //        this.mineField.IsPlacingFlag = false;
    //        this.SetValue(GameplayManager.FlagId);
    //    }
    //}
}
