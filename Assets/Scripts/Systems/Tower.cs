using UnityEngine;

public class Tower : Entity
{
    [Header("Default")]
    [SerializeField] protected TowerData data;
    [SerializeField] private Transform outLine;

    [Header("Rank")]
    [SerializeField] private Transform symbol;
    [SerializeField] private int rank = 1;
    private bool isMax = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (outLine == null) outLine = transform.Find("OutLine");
        if (symbol == null) symbol = transform.Find("Symbol");
    }
#endif

    public virtual void Attack(Monster _monster) { }

    public virtual void RankUp(int _amount = 1)
    {
        rank = Mathf.Clamp(rank + _amount, 1, 10);
        UpdateSymbols();
    }

    #region 심볼
    private void UpdateSymbols()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child != symbol && child != outLine)
                Destroy(child.gameObject);
        }

        if (rank == 10)
        {
            symbol.localPosition = Vector3.zero;
            if (!isMax)
            {
                symbol.localScale = Vector3.one * .5f;
                isMax = true;
            }
            return;
        }

        Vector2[] positions = GetArray(rank);

        for (int i = 0; i < positions.Length; i++)
        {
            if (i == 0)
                symbol.localPosition = positions[0];
            else
            {
                Transform clone = Instantiate(symbol, transform);
                clone.localPosition = positions[i];
            }
        }
    }

    private Vector2[] GetArray(int _rank)
    {
        float offset = symbol.localScale.x * 1.2f;
        float x = offset;
        float y = offset;

        Vector2[] grid =
        {
            new Vector2(-x,  y),
            new Vector2( 0f, y),
            new Vector2( x,  y),
            new Vector2(-x,  0f),
            new Vector2( 0f, 0f),
            new Vector2( x,  0f),
            new Vector2(-x, -y),
            new Vector2( 0f,-y),
            new Vector2( x, -y)
        };

        switch (_rank)
        {
            case 1: return new[] { grid[4] };
            case 2: return new[] { grid[1], grid[7] };
            case 3: return new[] { grid[1], grid[4], grid[7] };
            case 4: return new[] { grid[0], grid[2], grid[6], grid[8] };
            case 5: return new[] { grid[0], grid[2], grid[4], grid[6], grid[8] };
            case 6: return new[] { grid[0], grid[2], grid[3], grid[5], grid[6], grid[8] };
            case 7: return new[] { grid[0], grid[2], grid[3], grid[4], grid[5], grid[6], grid[8] };
            case 8: return new[] { grid[0], grid[1], grid[2], grid[6], grid[7], grid[8], grid[3], grid[5] };
            case 9:
            default: return grid;
        }
    }
    #endregion

    #region SET
    public void IsDragging(bool _on)
    {
        sr.sortingOrder = !_on ? 0 : 1000;
        outLine.GetComponent<SpriteRenderer>().sortingOrder = !_on ? 1 : 1001;
        symbol.GetComponent<SpriteRenderer>().sortingOrder = !_on ? 2 : 1002;
    }

    public virtual void SetRank(int _rank)
    {
        rank = Mathf.Clamp(_rank, 1, 10);
        UpdateSymbols();
    }

    public virtual void SetData(TowerData _data)
    {
        data = _data;

        gameObject.name = data.Name;
        if (data.Image != null) sr.sprite = data.Image;

        outLine.GetComponent<SpriteRenderer>().color = data.Color;
        symbol.GetComponent<SpriteRenderer>().color = data.Color;

        UpdateSymbols();
    }
    #endregion

    #region GET
    public int GetID() => data.ID;
    public int GetRank() => rank;
    public bool IsMax() => isMax;
    #endregion
}
