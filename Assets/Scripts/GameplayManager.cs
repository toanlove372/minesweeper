using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridUtils;

public class GameplayManager : MonoBehaviour
{
    private enum GameState
    {
        Init,
        Playing,
        Win,
        Lose,
    }

    public int width;
    public int height;
    public int mineCount;
    public GridManager gridManager;
    public MineFieldView mineFieldView;
    public GameUI gameUI;

    private MapData mapPlayData;
    private int flagLeft;
    private GameState gameState;
    private int score;

    public int FlagLeft => this.flagLeft;

    public event System.Action<int, int, int> onTileRevealed;
    public event System.Action<int, int, bool> onTileFlagged;

    public const int UnRevealedId = -1;
    public const int MineId = 8;
    public const int FlagId = 9;

    void Start()
    {
        this.Init();
    }

    private void Update()
    {
        if (this.gameState == GameState.Playing)
        {
            this.mineFieldView.OnUpdate();
        }
    }

    [ContextMenu("Init")]
    private void Init()
    {
        this.gameState = GameState.Init;

        this.score = 0;

        var map = GenerateMap(this.width, this.height, this.mineCount);
        SetMineMapNumber(map);
        this.gridManager.Init(map);

        this.mapPlayData = new MapData(this.width, this.height);
        for (var i = 0; i < this.mapPlayData.height; i++)
        {
            for (var j = 0; j < this.mapPlayData.width; j++)
            {
                this.mapPlayData.SetValue(j, i, UnRevealedId);
            }
        }

        this.flagLeft = this.mineCount;

        this.mineFieldView.Init(this, this.width, this.height);
        this.gameUI.Init(this);
        this.gameUI.onNewGameClicked -= StartNewGame;
        this.gameUI.onNewGameClicked += StartNewGame;

        this.gameState = GameState.Playing;
    }

    private void SwitchState(GameState state)
    {
        this.gameState = state;
        switch (state)
        {
            case GameState.Init:
                break;
            case GameState.Playing:
                break;
            case GameState.Win:
                this.gameUI.ShowWin(this.score);
                break;
            case GameState.Lose:
                this.gameUI.ShowLose(this.score);
                break;
        }
    }

    private void StartNewGame()
    {
        this.Init();
    }

    private static MapData GenerateMap(int width, int height, int numberOfMine)
    {
        var totalNumberOfTile = width * height;
        if (totalNumberOfTile < numberOfMine)
        {
            Debug.LogError("Not enough space for mine");
            return null;
        }

        var mapData = new MapData(width, height);

        var minePlaced = 0;
        for (var i = 0; i < totalNumberOfTile; i++)
        {
            var rand = Random.Range(i, totalNumberOfTile);
            if (rand >= i + numberOfMine - minePlaced)
            {
                continue;
            }

            minePlaced++;
            var randPos = i;

            var positionXByNumber = randPos % width;
            var positionYByNumber = randPos / width;

            mapData.SetValue(positionXByNumber, positionYByNumber, MineId);
        }

        return mapData;
    }

    private static void SetMineMapNumber(MapData mapData)
    {
        for (var i = 0; i < mapData.height; i++)
        {
            for (var j = 0; j < mapData.width; j++)
            {
                var value = mapData.GetValue(j, i);
                if (value != MineId)
                {
                    continue;
                }

                SetNumberByMine(mapData, j - 1, i + 1);
                SetNumberByMine(mapData, j, i + 1);
                SetNumberByMine(mapData, j + 1, i + 1);
                SetNumberByMine(mapData, j - 1, i);
                SetNumberByMine(mapData, j + 1, i);
                SetNumberByMine(mapData, j - 1, i - 1);
                SetNumberByMine(mapData, j, i - 1);
                SetNumberByMine(mapData, j + 1, i - 1);
            }
        }

        static void SetNumberByMine(MapData mapData, int x, int y)
        {
            var currentValue = mapData.GetValue(x, y);
            if (currentValue == -1 || currentValue == MineId)
            {
                return;
            }

            mapData.SetValue(x, y, currentValue + 1);
        }
    }

    public void StartRevealTile(int x, int y)
    {
        this.OnTileRevealed(x, y);

        if (this.gameState == GameState.Playing)
        {
            this.CheckWin();
        }
    }

    private void CheckWin()
    {
        //check win
        var isWin = true;
        for (var i = 0; i < this.mapPlayData.height; i++)
        {
            for (var j = 0; j < this.mapPlayData.width; j++)
            {
                if (this.mapPlayData.GetValue(j, i) == UnRevealedId)
                {
                    isWin = false;
                    break;
                }
            }
        }
        if (isWin)
        {
            this.gameUI.ShowWin(this.score);
        }
    }

    private void OnTileRevealed(int x, int y, bool checkFlag = false)
    {
        if (x < 0 || x == this.width || y < 0 || y == this.height)
        {
            return;
        }

        var currentValue = this.mapPlayData.GetValue(x, y);

        if (currentValue == FlagId)
        {
            this.OnTileFlagged(x, y);
            if (checkFlag == false)
            {
                return;
            }
            currentValue = this.mapPlayData.GetValue(x, y);
        }
        
        if (currentValue != UnRevealedId)
        {
            return;
        }

        var value = this.gridManager.MapData.GetValue(x, y);
        this.mapPlayData.SetValue(x, y, value);
        this.onTileRevealed?.Invoke(x, y, value);

        if (value != MineId)
        {
            this.score++;
        }

        if (value == 0)
        {
            //Debug.Log("Check zero at: " + x + ", " + y + ". width: " + this.width + ". height: " + this.height);
            this.OnTileRevealed(x - 1, y - 1,   true);
            this.OnTileRevealed(x, y - 1,       true);
            this.OnTileRevealed(x + 1, y - 1,   true);
            this.OnTileRevealed(x - 1, y,       true);
            this.OnTileRevealed(x + 1, y,       true);
            this.OnTileRevealed(x - 1, y + 1,   true);
            this.OnTileRevealed(x, y + 1,       true);
            this.OnTileRevealed(x + 1, y + 1,   true);
        }
        else if (value == MineId)
        {
            this.SwitchState(GameState.Lose);
        }
    }

    public void OnTileFlagged(int x, int y)
    {
        var currentValue = this.mapPlayData.GetValue(x, y);

        if (currentValue == FlagId)
        {
            this.flagLeft++;
            this.mapPlayData.SetValue(x, y, UnRevealedId);
            this.onTileFlagged?.Invoke(x, y, false);
            this.gameUI.UpdateFlagLeft();
            return;
        }

        if (currentValue != UnRevealedId)
        {
            return;
        }

        this.flagLeft--;
        this.mapPlayData.SetValue(x, y, FlagId);
        this.onTileFlagged?.Invoke(x, y, true);
        this.gameUI.UpdateFlagLeft();

        this.CheckWin();
    }
}
