using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class GameField : MonoBehaviour
{
    public static GameField Instance { get; private set; }
    [SerializeField] private int width = 4;
    [SerializeField] private int height = 4;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text bestScoreText;
    [SerializeField] private GameObject cellViewPrefab;
    [SerializeField] private RectTransform tilesContainer;
    [SerializeField] private Text currentScoreText;
    private List<Cell> cells = new List<Cell>();
    private Dictionary<Cell, CellView> cellToViewMap = new Dictionary<Cell, CellView>();
    private int curScore = 0;
    private int bestScore = 0;
    private string SavePath => Path.Combine(Application.persistentDataPath, "savegame.dat");

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadGame();
        if (cells.Count == 0)
        {
            CreateCellWithChance4();
            CreateCellWithChance4();
        }
        UpdateScore();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void CheckAndHandleGameOver()
    {
        if (CheckGameOver())
        {
            Debug.Log("[GameField] Game Over!");
            if (curScore > bestScore) {
                bestScore = curScore;
            }
            SaveGame();
            if (gameOverPanel != null) {
                gameOverPanel.SetActive(true);
            }
        }

    }

    public virtual void MoveLeft()
    {
        Debug.Log("[GameField] MoveLeft called");

        if (cells.Count == 0)
        {
            StartCoroutine(SpawnNewCellWithDelay(0.2f));
            return;
        }

        bool changed = MoveCells(Vector2Int.left);
        if (changed)
        {
            AfterMove();
        }
        else
        {
            CheckAndHandleGameOver();
        }
    }

    public virtual void MoveRight()
    {
        Debug.Log("[GameField] move right");
        if (cells.Count == 0)
        {
            StartCoroutine(SpawnNewCellWithDelay(0.2f));
            return;
        }
        bool changed = MoveCells(Vector2Int.right);
        if (changed)
        {
            AfterMove();
        }
        else
        {
            CheckAndHandleGameOver();
        }
    }

    public virtual void MoveUp()
    {
        Debug.Log("[GameField] move up");
        if (cells.Count == 0)
        {
            StartCoroutine(SpawnNewCellWithDelay(0.2f));
            return;
        }
        bool changed = MoveCells(new Vector2Int(0, -1));
        if (changed)
        {
            AfterMove();
        }
        else
        {
            CheckAndHandleGameOver();
        }
    }

    public virtual void MoveDown()
    {
        Debug.Log("[GameField] move down");
        if (cells.Count == 0)
        {
            StartCoroutine(SpawnNewCellWithDelay(0.2f));
            return;
        }
        bool changed = MoveCells(new Vector2Int(0, 1));
        if (changed)
        {
            AfterMove();
        }
        else
        {
            CheckAndHandleGameOver();
        }
    }

    private void AfterMove()
    {
        StartCoroutine(SpawnNewCellWithDelay(0.2f));
        CheckAndHandleGameOver();
    }

    private bool MoveCells(Vector2Int dir)
    {
        bool moved = false;
        bool[,] merged = new bool[width, height];
        Cell[,] board = new Cell[width, height];
        foreach (Cell c in cells) {
            board[c.Position.x, c.Position.y] = c;
        }
        if (dir == new Vector2Int(0, 1))
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = height - 2; y >= 0; y--)
                {
                    if (board[x, y] != null)
                    {
                        int currentY = y;
                        while (currentY < height - 1)
                        {
                            if (board[x, currentY + 1] == null)
                            {
                                board[x, currentY + 1] = board[x, currentY];
                                board[x, currentY] = null;
                                board[x, currentY + 1].Position = new Vector2Int(x, currentY + 1);
                                moved = true;
                                currentY++;
                            }
                            else if (board[x, currentY + 1].Value == board[x, currentY].Value && !merged[x, currentY + 1])
                            {
                                board[x, currentY + 1].Value++;
                                merged[x, currentY + 1] = true;
                                Cell oldCell = board[x, currentY];
                                if (oldCell != null) {
                                    RemoveAndDestroyCell(oldCell);
                                }
                                board[x, currentY] = null;
                                moved = true;
                                break;
                            }
                            else 
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
        else if (dir == new Vector2Int(0, -1))
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 1; y < height; y++)
                {
                    if (board[x, y] != null)
                    {
                        int currentY = y;
                        while (currentY > 0)
                        {
                            if (board[x, currentY - 1] == null)
                            {
                                board[x, currentY - 1] = board[x, currentY];
                                board[x, currentY] = null;
                                board[x, currentY - 1].Position = new Vector2Int(x, currentY - 1);
                                moved = true;
                                currentY--;
                            }
                            else if (board[x, currentY - 1].Value == board[x, currentY].Value && !merged[x, currentY - 1])
                            {
                                board[x, currentY - 1].Value++;
                                merged[x, currentY - 1] = true;
                                Cell oldCell = board[x, currentY];
                                if (oldCell != null) {
                                    RemoveAndDestroyCell(oldCell);
                                }
                                board[x, currentY] = null;
                                moved = true;
                                break;
                            }
                            else 
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
        else if (dir == Vector2Int.left)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 1; x < width; x++)
                {
                    if (board[x, y] != null)
                    {
                        int currentX = x;
                        while (currentX > 0)
                        {
                            if (board[currentX - 1, y] == null)
                            {
                                board[currentX - 1, y] = board[currentX, y];
                                board[currentX, y] = null;
                                board[currentX - 1, y].Position = new Vector2Int(currentX - 1, y);
                                moved = true;
                                currentX--;
                            }
                            else if (board[currentX - 1, y].Value == board[currentX, y].Value && !merged[currentX - 1, y])
                            {
                                board[currentX - 1, y].Value++;
                                merged[currentX - 1, y] = true;
                                Cell oldCell = board[currentX, y];
                                if (oldCell != null) {
                                    RemoveAndDestroyCell(oldCell);
                                }
                                board[currentX, y] = null;
                                moved = true;
                                break;
                            }
                            else {
                                break;
                            }
                        }
                    }
                }
            }
        }
        else if (dir == Vector2Int.right)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = width - 2; x >= 0; x--)
                {
                    if (board[x, y] != null)
                    {
                        int currentX = x;
                        while (currentX < width - 1)
                        {
                            if (board[currentX + 1, y] == null)
                            {
                                board[currentX + 1, y] = board[currentX, y];
                                board[currentX, y] = null;
                                board[currentX + 1, y].Position = new Vector2Int(currentX + 1, y);
                                moved = true;
                                currentX++;
                            }
                            else if (board[currentX + 1, y].Value == board[currentX, y].Value && !merged[currentX + 1, y])
                            {
                                board[currentX + 1, y].Value++;
                                merged[currentX + 1, y] = true;

                                Cell oldCell = board[currentX, y];
                                if (oldCell != null)
                                    RemoveAndDestroyCell(oldCell);

                                board[currentX, y] = null;
                                moved = true;
                                break;
                            }
                            else 
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
        List<Cell> newCells = new List<Cell>();
        for (int xx = 0; xx < width; xx++)
        {
            for (int yy = 0; yy < height; yy++)
            {
                if (board[xx, yy] != null) {
                    newCells.Add(board[xx, yy]);
                }
            }
        }
        cells = newCells;
        return moved;
    }

    [System.Serializable]
    public class SaveData
    {
        public int curScore;
        public int bestScore;
        public List<CellData> cells = new List<CellData>();
    }

    [System.Serializable]
    public class CellData
    {
        public int x;
        public int y;
        public int value;
    }

    private void SaveGame()
    {
        SaveData data = new SaveData();
        data.curScore = curScore;
        data.bestScore = bestScore;
        foreach (Cell c in cells)
        {
            CellData cd = new CellData();
            cd.x = c.Position.x;
            cd.y = c.Position.y;
            cd.value = c.Value;
            data.cells.Add(cd);
        }
        string path = SavePath;
        Debug.Log("[GameField] saving to " + path);
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            bf.Serialize(fs, data);
        }
    }

    private void LoadGame()
    {
        string path = SavePath;
        if (!File.Exists(path))
        {
            Debug.Log("[GameField] No save file. Start new game.");
            return;
        }
        Debug.Log("[GameField] Loading from " + path);
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            SaveData data = (SaveData)bf.Deserialize(fs);
            curScore = data.curScore;
            bestScore = data.bestScore;
            foreach (Cell c in cells)
            {
                if (cellToViewMap.TryGetValue(c, out CellView cv))
                {
                    Destroy(cv.gameObject);
                }
            }
            cells.Clear();
            cellToViewMap.Clear();
            foreach (CellData cd in data.cells)
            {
                Cell newCell = new Cell(new Vector2Int(cd.x, cd.y), cd.value);
                cells.Add(newCell);
                GameObject cellGO = Instantiate(cellViewPrefab, tilesContainer);
                CellView cellView = cellGO.GetComponent<CellView>();
                cellView.Init(newCell);
                cellToViewMap[newCell] = cellView;
            }
            UpdateScore();
        }
    }

    private IEnumerator SpawnNewCellWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CreateCellWithChance4();
        UpdateScore();
    }

    private void CreateCellWithChance4()
    {
        float rnd = Random.value;
        int newValue = (rnd < 0.2f) ? 2 : 1;
        CreateCell(newValue);
    }

    public void CreateCell(int value)
    {
        Vector2Int pos = GetEmptyPosition();
        if (pos.x == -1)
        {
            Debug.LogWarning("[GameField] No empty positions left.");
            return;
        }
        Cell newCell = new Cell(pos, value);
        cells.Add(newCell);
        GameObject cellGO = Instantiate(cellViewPrefab, tilesContainer);
        CellView cellView = cellGO.GetComponent<CellView>();
        cellView.Init(newCell);
        cellToViewMap[newCell] = cellView;
    }

    private void RemoveAndDestroyCell(Cell cell)
    {
        if (cell == null)
        {
            Debug.LogError("[GameField] null cell.");
            return;
        }
        cells.Remove(cell);
        if (cellToViewMap.TryGetValue(cell, out CellView view))
        {
            Destroy(view.gameObject);
            cellToViewMap.Remove(cell);
        }
        else
        {
            Debug.LogError("[GameField] not cell.");
        }
    }

    private Vector2Int GetEmptyPosition()
    {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool occupied = false;
                foreach (Cell c in cells)
                {
                    if (c.Position.x == x && c.Position.y == y)
                    {
                        occupied = true;
                        break;
                    }
                }
                if (!occupied)
                    emptyPositions.Add(new Vector2Int(x, y));
            }
        }

        if (emptyPositions.Count == 0)
            return new Vector2Int(-1, -1);

        int randIndex = Random.Range(0, emptyPositions.Count);
        return emptyPositions[randIndex];
    }

    private void UpdateScore()
    {
        int total = 0;
        foreach (Cell c in cells)
        {
            int realValue = (int)Mathf.Pow(2, c.Value);
            total += realValue;
        }
        curScore = total;
        if (currentScoreText != null)
            currentScoreText.text = curScore.ToString();
        
        if (curScore > bestScore)
        {
            bestScore = curScore;
        }
        if (bestScoreText != null)
            bestScoreText.text = bestScore.ToString();

    }

    private bool CheckGameOver()
    {
        if (cells.Count < width * height) {
            return false;
        }
        Cell[,] board = new Cell[width, height];
        foreach (Cell c in cells) {
            board[c.Position.x, c.Position.y] = c;
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y] == null) {
                    continue;
                }
                int val = board[x, y].Value;
                if (x < width - 1 && board[x + 1, y] != null && board[x + 1, y].Value == val) {
                    return false;
                }
                if (y < height - 1 && board[x, y + 1] != null && board[x, y + 1].Value == val) {
                    return false;
                }
            }
        }
        return true;
    }

    private void ResetGame()
    {
        foreach (var c in cells)
        {
            if (cellToViewMap.TryGetValue(c, out CellView cv)) {
                Destroy(cv.gameObject);
            }
        }
        cells.Clear();
        cellToViewMap.Clear();
        curScore = 0;
        UpdateScore();
        CreateCellWithChance4();
        CreateCellWithChance4();
    }

    public void StartNewGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "savegame.dat");
        if (File.Exists(path))  
        {
            File.Delete(path);
            Debug.Log("[GameField] Save file deleted. Start new game.");
        }
        if (gameOverPanel != null) {
            gameOverPanel.SetActive(false);
        }
        ResetGame();
    }
}
