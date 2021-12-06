using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid_Manager : MonoBehaviour
{
    //public ArrayLayout boardLayout;

    [Header("Game Settings")]
    int width = 3;
    int height = 5;
    public enum BoardSize
    {
        small,
        medium,
        big,
        huge
    }
    public BoardSize boardSize;

    [Header("Ui elements")]
    public Sprite[] pieces;
    public RectTransform gameBoard;
    public Canvas canvas;

    [Header("Prefabs")]
    [SerializeField] public GameObject nodePiece;
    [SerializeField] public GameObject startPiece;
    [SerializeField] public GameObject endPiece;



    Node[,] board;
    System.Random random;
    private void Start()
    {

        StartGame();
    }

    void StartGame()
    {
        switch (boardSize)
        {
            case BoardSize.small:
                width = 3;
                height = 6;
                break;
            case BoardSize.medium:
                width = 4;
                height = 7;
                break;
            case BoardSize.big:
                width = 5;
                height = 8;
                break;
            case BoardSize.huge:
                width = 6;
                height = 9;
                break;
            default:
                break;
        }
        string seed = GetRandomSeed();
        random = new System.Random(seed.GetHashCode());
        InitializeBoard();
        InstantiateBoard();
    }
    void InitializeBoard()
    {
        var screenNodeDivision = 1080 / (width + 2);
        Debug.Log("Screen: " + Screen.width + " / Division: " + screenNodeDivision);
        var r = gameBoard.sizeDelta;
        r.x = screenNodeDivision * width;
        gameBoard.sizeDelta = r;

        board = new Node[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                board[x, y] = new Node(fillPiece(), new Point(x, y));
            }
        }
    }

    void InstantiateBoard()
    {
        int nodeSizeX = Mathf.FloorToInt(gameBoard.rect.width / width);
        int nodeSizeY = Mathf.FloorToInt(gameBoard.rect.height / height);
        int nodeSize = nodeSizeX;
        if (nodeSizeX > nodeSizeY)
        {
            nodeSize = nodeSizeY;
        }

        var r = gameBoard.sizeDelta;
        r.y = nodeSize * height;
        gameBoard.sizeDelta = r;

        int paddingX = Mathf.FloorToInt((gameBoard.rect.width - (width * nodeSize)) / 2);
        int paddingY = Mathf.FloorToInt((gameBoard.rect.height - (height * nodeSize)) / 2); // el alto del panel menos la cantidad de cassillas por el nod size


        int spawnStartTry = Mathf.FloorToInt(height / 2) + Mathf.FloorToInt(width / 2);
        int spawnEndTry = (Mathf.FloorToInt(height / 2)-1) + (Mathf.FloorToInt(width / 2)-1);
        bool spawnedStart = false;
        bool spawnedEnd = false;


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int val = board[x, y].value;
                GameObject p = Instantiate(nodePiece, gameBoard);
                RectTransform rect = p.GetComponent<RectTransform>();
                var sizeDelta = rect.sizeDelta;
                sizeDelta.x = nodeSize;
                sizeDelta.y = nodeSize;
                rect.sizeDelta = sizeDelta;
                rect.anchoredPosition = new Vector2((nodeSize * x) + paddingX, -(nodeSize * y) - paddingY);
                NodePiece node = p.GetComponent<NodePiece>();
                node.Initialized(val, new Point(x, y), pieces[val], nodeSize, paddingX, paddingY);

                // Set Start and End pieces
                if (x == 0 && y <= Mathf.FloorToInt(height / 2) || y == 0 && x <= Mathf.FloorToInt(width / 2))
                {
                    if (!spawnedStart)
                    {
                        int chance = random.Next(0, spawnStartTry);
                        if (chance == 0)
                        {

                            spawnedStart = true;
                            GameObject sp = Instantiate(startPiece, gameBoard);
                            RectTransform startRect = sp.GetComponent<RectTransform>();
                            var startSizeDelta = startRect.sizeDelta;
                            startSizeDelta.x = nodeSize;
                            startSizeDelta.y = nodeSize;
                            startRect.sizeDelta = startSizeDelta;
                            if (y == 0)
                            {
                                //Debug.Log("start spawn on Height: " + x + "/" + y);
                                startRect.anchoredPosition = new Vector2((nodeSize * (x +1)) + paddingX, -(nodeSize * (y - 1)) - paddingY);
                                sp.transform.rotation = Quaternion.Euler(0, 0, -90);
                            }
                            else if (x == 0)
                            {
                                //Debug.Log("start spawn on width: " + x + "/" +y);
                                startRect.anchoredPosition = new Vector2((nodeSize * (x - 1)) + paddingX, -(nodeSize * y) - paddingY);
                            }
                        }
                        else
                        {
                           // Debug.Log("Failed Start: " + spawnStartTry + " Point: " + x + "/" + y);
                            spawnStartTry--;
                        }
                    }

                }
                if (x == width -1 && y > Mathf.FloorToInt(height / 2) || y == height -1 && x > Mathf.FloorToInt(width / 2))
                {
                    if (!spawnedEnd)
                    {
                        int chance = random.Next(0, spawnEndTry);
                        if (chance == 0)
                        {

                            spawnedEnd = true;
                            GameObject ep = Instantiate(endPiece, gameBoard);
                            RectTransform endRect = ep.GetComponent<RectTransform>();
                            var endSizeDelta = endRect.sizeDelta;
                            endSizeDelta.x = nodeSize;
                            endSizeDelta.y = nodeSize;
                            endRect.sizeDelta = endSizeDelta;
                            if (y == height-1)
                            {
                                Debug.Log("end spawn on Height: " + x + "/" + y);
                                endRect.anchoredPosition = new Vector2((nodeSize * (x)) + paddingX, -(nodeSize * (y + 2)) - paddingY);
                                ep.transform.rotation = Quaternion.Euler(0, 0, 90);
                            }
                            else if (x == width-1)
                            {
                                Debug.Log("end spawn on width: " + x + "/" + y);
                                endRect.anchoredPosition = new Vector2((nodeSize * (x + 2 )) + paddingX, -(nodeSize * (y +1)) - paddingY);
                                ep.transform.rotation = Quaternion.Euler(0, 0, 180);
                            }
                        }
                        else
                        {
                            Debug.Log("Failed end: " + spawnEndTry + " Point: " + x + "/" + y);
                            spawnEndTry--;
                        }
                    }

                }
                else
                {
                    Debug.Log("point:_ " + x + "/" + y + " is not fit for end spawning");
                }
            }
        }
    }



    int fillPiece()
    {
        int val = 0;
        val = random.Next(0, pieces.Length);
        //val = (random.Next(0, 100) / (100/pieces.Length));
        return val;
    }

    int GetValueAtPoint(Point p)
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return -1;
        return board[p.x, p.y].value;
    }

    void SetValueAtPoint(Point p, int v)
    {
        board[p.x, p.y].value = v;
    }

    private string GetRandomSeed()
    {
        string seed = "";
        string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!$%&@()";
        for (int i = 0; i < 20; i++)
        {
            seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
        }
        return seed;
    }
}

[System.Serializable]
public class Node
{
    public int value; //0= blank, 1 = stright tube, 2 = L Tube, 3 = DamagedTube
    public Point index;

    public Node(int v, Point i)
    {
        value = v;
        index = i;
    }
}
