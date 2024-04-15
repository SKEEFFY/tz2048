using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    public TileState state { get; private set; }
    public TileCell cell { get; private set; }
    public int number { get; private set; }

    public bool locked { get; set; }

    private Image _background;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _background = GetComponent<Image>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetState(TileState state, int number)
    {
        this.state = state;
        this.number = number;

        _background.color = state.backgroundColor;
        _text.color = state.textColor;
        _text.text = number.ToString();
    }

    public void Spawn(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        transform.position = cell.transform.position;
    }

    public void MoveTo(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        Animate(cell.transform.position, false);
    }

    public void Merge(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = null;
        cell.tile.locked = true;

        Animate(cell.transform.position, true);
    }

    private void DestroyTile(bool merging)
    {
        if (merging)
        {
            Destroy(gameObject);
        }
    }

    private void Animate(Vector3 to, bool merging)
    {
        float duration = 0.1f;

        Vector3 from = transform.position;

        transform.DOMove(to, duration).OnComplete(() => DestroyTile(merging));
    }
}
