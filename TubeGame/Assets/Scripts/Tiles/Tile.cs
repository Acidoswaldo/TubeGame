using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SpriteRenderer _highlight;
    [SerializeField] private GameObject _selectedObject;
    [SerializeField] private ScriptableTile _tileSO;
    GridManager myManager;
    Camera mainCamera;

    Vector2 startPos;
    Vector2 MouseStartPos;
    Vector2 MouseEndPos;

    bool selected;
    bool deselectEnabled = true;
    bool isDragged;
    bool canGetDragged = false;
    float draggedTimer;
    public Vector2 tileCoords;

    [SerializeField] Vector3 startingScale;
    [SerializeField] DraggedDirection draggedDirection;
    [SerializeField] Tile changeTile;

    public int Rotations = 0;
    bool rotating;


    private void Start()
    {
        startPos = transform.position;
        mainCamera = Camera.main;
    }

    public void Init()
    {
        SetTileSprite();
    }

    void SetTileSprite()
    {
        _renderer.sprite = _tileSO._tileSprite;
    }

    private void OnMouseEnter()
    {
        if(GameManager.instance.state == GameState.PlayerTurn)
        {
            _highlight.gameObject.SetActive(true);
        }
    }
    private void OnMouseExit()
    {
        if (GameManager.instance.state == GameState.PlayerTurn)
        {
            _highlight.gameObject.SetActive(false);
            Color highlightColor = _highlight.color;
            highlightColor.a = 0.3f;
            _highlight.color = highlightColor;
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.instance.state == GameState.PlayerTurn)
        {
            draggedTimer = 0;
            MouseStartPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Color highlightColor = _highlight.color;
            highlightColor.a = 0.6f;
            _highlight.color = highlightColor;
            SelectTile();
        }
    }
    private void OnMouseDrag()
    {
        if (GameManager.instance.state == GameState.PlayerTurn)
        {
            draggedTimer += Time.deltaTime;
            if (draggedTimer > 0.1f)
            {
                isDragged = true;
            }
            if (isDragged)
            {
                MouseEndPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                DraggedDirection newDirection;
                if (Vector2.Distance(MouseEndPos, startPos) > 0.5f)
                {
                    newDirection = GetDragDirection((Vector3)MouseEndPos - (Vector3)startPos);
                }
                else
                {
                    newDirection = DraggedDirection.none;
                }
                if (canGetDragged)
                {
                    if (newDirection != draggedDirection)
                    {
                        draggedDirection = newDirection;
                        PrepareTileSwap(newDirection);
                    }
                }
            }
        }
    }
    private void OnMouseUp()
    {
        if (GameManager.instance.state == GameState.PlayerTurn)
        {
            Color highlightColor = _highlight.color;
            highlightColor.a = 0.3f;
            _highlight.color = highlightColor;

            if (deselectEnabled)
            {
                DeselectTile();
            }
            if (isDragged)
            {
                Debug.Log("was dragged");
                if (changeTile != null)
                {
                    ChangeTile();
                }
                PrepareTileSwap(DraggedDirection.none);
                DeselectTile();
                draggedDirection = DraggedDirection.none;
                isDragged = false;
            }
        }
       
    }



    public enum TileDirection
    {
        left,
        right,
        top,
        bot,
    }

    public void SelectTile()
    {
        LeanTween.cancel(this.gameObject);
        if (!selected)
        {
            deselectEnabled = false;
            selected = true;
            _selectedObject.SetActive(true);
            LeanTween.scale(this.gameObject, startingScale * 1.2f, 0.4f).setEase(LeanTweenType.easeOutExpo).setOnComplete(()=> { canGetDragged = true; });
            _renderer.sortingOrder = 1;
            myManager.SetSelectedTile(myManager.GetTileAtPosition(tileCoords));
        }
        else
        {
            deselectEnabled = true;
            //DeselectTile();
        }
    }
    public void DeselectTile()
    {
        canGetDragged = false;
        selected = false;
        _selectedObject.SetActive(false);
        LeanTween.scale(this.gameObject, startingScale, 1).setEase(LeanTweenType.easeOutExpo);
        _renderer.sortingOrder = 0;
        myManager.DeselectTile();
    }

    public void SetTile(GridManager manager, ScriptableTile tileSO, Vector2 pos, Vector3 scale, int rotationIndex)
    {
        myManager = manager;
        tileCoords = pos;
        startingScale = scale;
        _tileSO = tileSO;
        Rotations = rotationIndex;
    }

    public void resetPosition()
    {

    }

    private enum DraggedDirection
    {
        none,
        Up,
        Down,
        Right,
        Left
    }

    private DraggedDirection GetDragDirection(Vector3 dragVector)
    {
        float positiveX = Mathf.Abs(dragVector.x);
        float positiveY = Mathf.Abs(dragVector.y);
        DraggedDirection draggedDir;
        if (positiveX > positiveY)
        {
            draggedDir = (dragVector.x > 0) ? DraggedDirection.Right : DraggedDirection.Left;
        }
        else
        {
            draggedDir = (dragVector.y > 0) ? DraggedDirection.Up : DraggedDirection.Down;
        }
        return draggedDir;
    }

    void PrepareTileSwap(DraggedDirection direction)
    {
        Debug.Log("Prepare " + direction);
        LeanTween.cancel(this.gameObject);
        switch (direction)
        {
            case DraggedDirection.none:
                changeTile = null;
                Debug.Log("back to start pos");
                LeanTween.move(this.gameObject, startPos, 0.5f).setEase(LeanTweenType.easeOutExpo);
                break;
            case DraggedDirection.Up:
                if (checkDraggedTile(direction, out Tile neighbourUp))
                {
                    if (neighbourUp != null)
                    {
                        changeTile = neighbourUp;
                    }
                    else
                    {
                        changeTile = null;
                    }
                    LeanTween.move(this.gameObject, startPos + new Vector2(0, 0.5f), 0.5f).setEase(LeanTweenType.easeOutExpo);
                }
                else
                {
                    changeTile = null;
                    LeanTween.move(this.gameObject, startPos, 0.5f).setEase(LeanTweenType.easeOutExpo);
                }
                break;
            case DraggedDirection.Down:
                if (checkDraggedTile(direction, out Tile neighbourDown))
                {
                    if (neighbourDown != null)
                    {
                        changeTile = neighbourDown;
                    }
                    else
                    {
                        changeTile = null;
                    }
                    LeanTween.move(this.gameObject, startPos + new Vector2(0, -0.5f), 0.5f).setEase(LeanTweenType.easeOutExpo);
                }
                else
                {
                    changeTile = null;
                    LeanTween.move(this.gameObject, startPos, 0.5f).setEase(LeanTweenType.easeOutExpo);
                }
                break;
            case DraggedDirection.Right:
                if (checkDraggedTile(direction, out Tile neighbourRight))
                {
                    if (neighbourRight != null)
                    {
                        changeTile = neighbourRight;
                    }
                    else
                    {
                        changeTile = null;
                    }
                    LeanTween.move(this.gameObject, startPos + new Vector2(0.5f, 0), 0.5f).setEase(LeanTweenType.easeOutExpo);
                }
                else
                {
                    changeTile = null;
                    LeanTween.move(this.gameObject, startPos, 0.5f).setEase(LeanTweenType.easeOutExpo);
                }
                break;
            case DraggedDirection.Left:
                if (checkDraggedTile(direction, out Tile neighbourLeft))
                {
                    if (neighbourLeft != null)
                    {
                        changeTile = neighbourLeft;
                    }
                    else
                    {
                        changeTile = null;
                    }
                    LeanTween.move(this.gameObject, startPos + new Vector2(-0.5f, 0), 0.5f).setEase(LeanTweenType.easeOutExpo);
                }
                else
                {
                    changeTile = null;
                    LeanTween.move(this.gameObject, startPos, 0.5f).setEase(LeanTweenType.easeOutExpo);
                }
                break;
            default:
                changeTile = null;
                Debug.LogError("Direction out of range");
                break;
        }
    }

    bool checkDraggedTile(DraggedDirection direction, out Tile neighbourTile)
    {
        Vector2 neighborCoords = tileCoords;
        switch (draggedDirection)
        {
            case DraggedDirection.none:
                neighbourTile = null;
                return false;
                break;
            case DraggedDirection.Up:
                neighborCoords = tileCoords + new Vector2(0, 1);
                break;
            case DraggedDirection.Down:
                neighborCoords = tileCoords + new Vector2(0, -1);
                break;
            case DraggedDirection.Right:
                neighborCoords = tileCoords + new Vector2(1, 0);
                break;
            case DraggedDirection.Left:
                neighborCoords = tileCoords + new Vector2(-1, 0);
                break;
            default:
                neighbourTile = null;
                return false;
                break;
        }

        Tile neighbour = myManager.GetTileAtPosition(neighborCoords);
        if (neighbour != null)
        {
            neighbourTile = neighbour;
            return true;
        }
        else
        {
            neighbourTile = null;
            return false;
        }
    }


    //   Player Actions  //
    //___________________//

    void ChangeTile()
    {
        ScriptableTile NewTile = changeTile._tileSO;
        int OtherRotations = changeTile.Rotations;
        Quaternion OtherQuaternion = changeTile.transform.localRotation;
        changeTile.Rotations = Rotations;
        changeTile.transform.localRotation = transform.localRotation;

        Rotations = OtherRotations;
        transform.rotation = OtherQuaternion;
        changeTile._tileSO = this._tileSO;
        changeTile.SetTileSprite();
        this._tileSO = NewTile;
        SetTileSprite();
    }

    public void RotateTile()
    {
        if (!rotating)
        {
            rotating = true;
            Rotations++;
            if (Rotations > 3)
            {
                Rotations = 0;
            }
            Vector3 newRotation = transform.localRotation.eulerAngles;
            newRotation.z += 90;
            Debug.Log(newRotation);
            LeanTween.rotateLocal(this.gameObject, newRotation, 0.5f).setEase(LeanTweenType.easeOutExpo).setOnComplete(()=>
            {
                rotating = false;
                transform.localRotation = Quaternion.Euler(newRotation);
            });
        }
       
    }

}
