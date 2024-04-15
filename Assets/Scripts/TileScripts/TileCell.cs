using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates { get; set; }

    public Tile tile { get; set; }

    public bool isFree => tile == null;
    public bool isTaken => tile != null;
}
