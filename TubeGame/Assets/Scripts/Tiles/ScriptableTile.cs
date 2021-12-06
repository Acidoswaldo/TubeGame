using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile", menuName = "Assets/Tile")]
public class ScriptableTile : ScriptableObject
{
    public string _tileName;
    public Sprite _tileSprite;

    [System.Serializable]
    public struct OpenDirections
    {
        public string name;
        public bool up;
        public bool down;
        public bool righ;
        public bool left;
    }
    public OpenDirections[] RotatedDirections = new OpenDirections[4];

}
