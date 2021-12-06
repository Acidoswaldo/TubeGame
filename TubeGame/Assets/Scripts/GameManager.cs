using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState state;
    public static Action<GameState> onGameStateChange;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        UpdateGameState(GameState.CreateBoard);
    }
    public void OnClickGoButton()
    {
        UpdateGameState(GameState.CellsMoving);
    }
    public void UpdateGameState(GameState newState)
    {
        state = newState;
        switch (newState)
        {
            case GameState.CreateBoard:
                handleCreateBoard();

                break;
            case GameState.PlayerTurn:

                break;
            case GameState.VictoryScreen:

                break;
            case GameState.loseScreen:

                break;
            default:
                Debug.LogError("State out of range exception");
                break;
        }

        onGameStateChange?.Invoke(newState);
    }

    private void handleCreateBoard()
    {
       
    }
}

public enum GameState
{
    CreateBoard,
    PlayerTurn,
    CellsMoving,
    VictoryScreen,
    loseScreen
}
