using System;
using UnityEngine;

public class Cell
{
    private Vector2Int position;
    public Vector2Int Position
    {
        get => position;
        set
        {
            if (position != value)
            {
                position = value;
                OnPositionChanged?.Invoke(this, position);
            }
        }
    }

    private int cellValue;
    public int Value
    {
        get => cellValue;
        set
        {
            if (cellValue != value)
            {
                cellValue = value;
                OnValueChanged?.Invoke(this, cellValue);
            }
        }
    }

    public event Action<Cell, int> OnValueChanged;
    public event Action<Cell, Vector2Int> OnPositionChanged;

    public Cell(Vector2Int startPos, int startValue)
    {
        position = startPos;
        cellValue = startValue;
        Debug.Log($"[Cell] new cell {startPos} with {startValue}");
    }
}
