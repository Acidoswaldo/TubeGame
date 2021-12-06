using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] GameObject spawner;
    [SerializeField] GameObject reciver;

    Tile startTile;
    Tile endTile;

    private void Awake()
    {
        GameManager.onGameStateChange += GameManagerOnGameStateChanged;
    }
    private void OnDestroy()
    {
        GameManager.onGameStateChange -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState state)
    {
        if(state == GameState.CellsMoving)
        {
            SpawnCells();
        }
    }

    void SpawnCells()
    {

    }

    public void SetSpawnerPosition(Vector2 Pos, Vector3 scale, Tile StartTile)
    {
        var _spawner = Instantiate(spawner, Pos, Quaternion.identity);
        _spawner.transform.localScale = scale;
        startTile = StartTile;
    }
    public void SetReciverPosition(Vector2 Pos, Vector3 scale, Tile EndTile)
    {
        var _reciver = Instantiate(reciver, Pos, Quaternion.identity);
        _reciver.transform.localScale = scale;
        endTile = EndTile;
    }

   
}
