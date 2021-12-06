using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] GridSize _gridSize;
    [Header("Gameobject Grid")]
    [SerializeField] private Tile _tilePrefab;
    private Dictionary<Vector2, Tile> _tiles;


    [Header("References")]
    [SerializeField] private Transform _Cam;
    [SerializeField] SpawnerManager _spawnerManager;

    [Header("Tile Managing")]
    [SerializeField] Tile selectedTile;
    [SerializeField] ScriptableTile[] _tilesSO;

    public enum GridSize
    {
        Small,
        Medium,
        Big,
        Huge,
    }

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
        Debug.Log("Changed State");
        if(state == GameState.CreateBoard)
        {
            GenerateGrid(_gridSize);
        }
        if(state == GameState.CellsMoving)
        {
            deselectPreviousTile();
        }
      
    }

    void GenerateGrid(GridSize gridSize)
    {
        int _width = 3;
        int _height = 5;
        float _cellSize = 1;
        switch (gridSize)
        {
            case GridSize.Small:
                _width = 3;
                _height = 5;
                _cellSize = 1;
                break;
            case GridSize.Medium:
                _width = 4;
                _height = 7;
                _cellSize = 0.8f;
                break;
            case GridSize.Big:
                _width = 5;
                _height = 8;
                _cellSize = 0.7f;
                break;
            case GridSize.Huge:
                _width = 6;
                _height = 10;
                _cellSize = 0.6f;
                break;
        }


        _tiles = new Dictionary<Vector2, Tile>();

        bool _startTileGenerated = false;
        int _startTilechances = _width - 1;

        bool _endTileGenerated = false;
        int _endTilechances = _width - 1;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var _tileToSpawn = _tilePrefab;
                _tileToSpawn.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
                int RandomRotation = Random.Range(0, 4);
                Quaternion Rotation = Quaternion.Euler(Vector3.zero);
                switch (RandomRotation)
                {
                    case 0:
                        Rotation = Quaternion.Euler(Vector3.zero);
                        break;
                    case 1:
                        Debug.Log("90 rotaion");
                        Rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                        break;
                    case 2:
                        Rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                        break;
                    case 3:
                        Debug.Log("270 rotaion");
                        Rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                        break;
                    default:
                        break;
                }
               
                var _spawnPos = new Vector3(x * _tileToSpawn.transform.localScale.x, y * _tileToSpawn.transform.localScale.y);
                var spawnedTile = Instantiate(_tileToSpawn, _spawnPos, Quaternion.identity);
                spawnedTile.transform.localRotation = Rotation;
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.transform.SetParent(this.transform);
                var RandomScriptableTile = _tilesSO[Random.Range(0, _tilesSO.Length)];
                spawnedTile.SetTile(this,RandomScriptableTile, new Vector2(x, y), new Vector3(_cellSize, _cellSize, _cellSize), RandomRotation);
                if (y == _height - 1 && !_startTileGenerated)
                {
                    int chance = Random.Range(0, _startTilechances);
                    if (chance == 0)
                    {
                        _spawnerManager.SetSpawnerPosition(new Vector3(x * spawnedTile.transform.localScale.x, (y + 1) * _tileToSpawn.transform.localScale.y), new Vector3(_cellSize, _cellSize,_cellSize), spawnedTile);
                        _startTileGenerated = true;
                    }
                    else
                    {
                        _startTilechances--;
                    }
                }

                if (y == 0 && !_endTileGenerated)
                {
                    int chance = Random.Range(0, _endTilechances);
                    if (chance == 0)
                    {
                        _spawnerManager.SetReciverPosition(new Vector3(x * spawnedTile.transform.localScale.x, (y - 1) * _tileToSpawn.transform.localScale.y), new Vector3(_cellSize, _cellSize, _cellSize), spawnedTile);
                        _endTileGenerated = true;
                    }
                    else
                    {
                        _endTilechances--;
                    }
                }
                spawnedTile.Init();
                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }
        float xPosOffset = 0;
        float yPosOffset = 0;
        if (_width % 2 == 0)
        {
            xPosOffset = 0.5f;
        }
        if (_height % 2 == 0)
        {
            yPosOffset = 0.5f;
        }

        _Cam.position = new Vector3((float)(_width / 2 - xPosOffset) * _tilePrefab.transform.localScale.x, (float)(_height / 2 - yPosOffset) * _tilePrefab.transform.localScale.y -0.7f, -10);
        GameManager.instance.UpdateGameState(GameState.PlayerTurn);
    }


    public void SetSelectedTile(Tile tile)
    {
        deselectPreviousTile();
        selectedTile = tile;
    }
    public void DeselectTile()
    {
        selectedTile = null;
    }

    void deselectPreviousTile()
    {
        if(selectedTile != null)
        {
            selectedTile.DeselectTile();
        }
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;
    }


    public void OnClickRotateCurrentSelectedTile()
    {
        selectedTile.RotateTile();
    }
}
