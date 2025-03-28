using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    [SerializeField] private Text valueText;
    private Image backgroundImage;
    private Cell cell;
    private RectTransform rectTransform;
    private Color startColor;
    private Color endColor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        backgroundImage = GetComponent<Image>();
        ColorUtility.TryParseHtmlString("#FFFEF0", out startColor);
        ColorUtility.TryParseHtmlString("#FFFDD7", out endColor);
    }

    public void Init(Cell newCell)
    {
        cell = newCell;
        cell.OnValueChanged += UpdateValue;
        cell.OnPositionChanged += UpdatePosition;
        UpdateValue(cell, cell.Value);
        UpdatePosition(cell, cell.Position);
    }

    private void UpdateValue(Cell changedCell, int newValue)
    {
        int displayValue = (int)Mathf.Pow(2, newValue);
        if (valueText != null) {
            valueText.text = displayValue.ToString();
        }
        float minVal = 1f;
        float maxVal = 4f;
        float t = Mathf.InverseLerp(minVal, maxVal, newValue);
        t = Mathf.Clamp01(t);
        if (backgroundImage != null)
        {
            Color finalColor = Color.Lerp(startColor, endColor, t);
            backgroundImage.color = finalColor;
        }
    }

    private void UpdatePosition(Cell changedCell, Vector2Int newPos)
    {
        float cellSize = 130f;
        float spacing = 15f;
        float paddingLeft = 20f;    
        float paddingTop = 20f;
        float xPos = paddingLeft + (cellSize + spacing) * newPos.x;
        float yPos = -paddingTop - (cellSize + spacing) * newPos.y;
        rectTransform.anchoredPosition = new Vector2(xPos, yPos);
    }

    private void OnDestroy()
    {
        if (cell != null)
        {
            cell.OnValueChanged -= UpdateValue;
            cell.OnPositionChanged -= UpdatePosition;
        }
    }
}
