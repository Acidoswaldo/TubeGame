using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodePiece : MonoBehaviour
{
    public int value;
    public Point index;


    int nodeSize;
    int paddingX;
    int paddingY;


    [HideInInspector]
    public Vector2 pos;
    [HideInInspector]
    public RectTransform rect;
    Image img;

    public void Initialized(int v, Point p, Sprite piece, int size, int padX, int padY)
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        nodeSize = size;
        paddingX = padX;
        paddingY = padY;

        value = v;
        SetIndex(p);
        img.sprite = piece;

      
    }

    public void SetIndex(Point p)
    {
        index = p;
        ResetPosition();
        UpdateName();
    }

    public void ResetPosition()
    {
        pos = new Vector2((nodeSize * index.x) + paddingX, -(nodeSize * index.y) - paddingY);
    }

    void UpdateName()
    {
        transform.name = "Node [" + index.x + ", " + index.y + "]";
    }

}
