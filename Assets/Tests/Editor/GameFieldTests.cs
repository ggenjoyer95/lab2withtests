using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using FluentAssertions;

[TestFixture]
public class GameFieldTests
{
    private GameField gameField;
    private GameObject gameFieldObj;
    private GameObject tilesContainerObj;
    private GameObject cellPrefabObj;
    private GameObject gameOverPanelObj;
    private Text currentScoreText;
    private Text bestScoreText;

    [SetUp]
    public void SetUp()
    {
        gameFieldObj = new GameObject("GameFieldObj");
        gameField = gameFieldObj.AddComponent<GameField>();

        tilesContainerObj = new GameObject("TilesContainerObj");
        FieldInfo tilesContainerField = typeof(GameField).GetField("tilesContainer", BindingFlags.NonPublic | BindingFlags.Instance);
        tilesContainerField.Should().NotBeNull("tilesContainer field not found");
        tilesContainerField.SetValue(gameField, tilesContainerObj.AddComponent<RectTransform>());

        cellPrefabObj = new GameObject("CellViewPrefabObj");
        cellPrefabObj.AddComponent<RectTransform>();
        cellPrefabObj.AddComponent<Image>();
        cellPrefabObj.AddComponent<CellView>();
        FieldInfo prefabField = typeof(GameField).GetField("cellViewPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
        prefabField.Should().NotBeNull("cellViewPrefab field not found");
        prefabField.SetValue(gameField, cellPrefabObj);

        gameOverPanelObj = new GameObject("GameOverPanelObj");
        gameOverPanelObj.SetActive(false);
        FieldInfo panelField = typeof(GameField).GetField("gameOverPanel", BindingFlags.NonPublic | BindingFlags.Instance);
        panelField.Should().NotBeNull("gameOverPanel field not found");
        panelField.SetValue(gameField, gameOverPanelObj);

        GameObject currScoreObj = new GameObject("CurrentScoreTextObj");
        currentScoreText = currScoreObj.AddComponent<Text>();
        currentScoreText.text = "0";
        FieldInfo currScoreTextField = typeof(GameField).GetField("currentScoreText", BindingFlags.NonPublic | BindingFlags.Instance);
        currScoreTextField.Should().NotBeNull("currentScoreText field not found");
        currScoreTextField.SetValue(gameField, currentScoreText);

        GameObject bestScoreObj = new GameObject("BestScoreTextObj");
        bestScoreText = bestScoreObj.AddComponent<Text>();
        bestScoreText.text = "0";
        FieldInfo bestScoreTextField = typeof(GameField).GetField("bestScoreText", BindingFlags.NonPublic | BindingFlags.Instance);
        bestScoreTextField.Should().NotBeNull("bestScoreText field not found");
        bestScoreTextField.SetValue(gameField, bestScoreText);

        FieldInfo widthField = typeof(GameField).GetField("width", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo heightField = typeof(GameField).GetField("height", BindingFlags.NonPublic | BindingFlags.Instance);
        widthField.SetValue(gameField, 4);
        heightField.SetValue(gameField, 4);

        string savePath = Path.Combine(Application.persistentDataPath, "savegame.dat");
        if (File.Exists(savePath))
            File.Delete(savePath);

        MethodInfo awakeMethod = typeof(GameField).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        awakeMethod.Invoke(gameField, null);
        MethodInfo startMethod = typeof(GameField).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        startMethod.Invoke(gameField, null);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(gameFieldObj);
        UnityEngine.Object.DestroyImmediate(tilesContainerObj);
        UnityEngine.Object.DestroyImmediate(cellPrefabObj);
        UnityEngine.Object.DestroyImmediate(gameOverPanelObj);
        if (currentScoreText != null)
            UnityEngine.Object.DestroyImmediate(currentScoreText.gameObject);
        if (bestScoreText != null)
            UnityEngine.Object.DestroyImmediate(bestScoreText.gameObject);
        string savePath = Path.Combine(Application.persistentDataPath, "savegame.dat");
        if (File.Exists(savePath))
            File.Delete(savePath);
    }

    private void InvokePrivateMethod(string methodName, object[] parameters = null)
    {
        MethodInfo mi = typeof(GameField).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        mi.Should().NotBeNull($"Method {methodName} not found");
        mi.Invoke(gameField, parameters);
    }

    private object GetPrivateField(string fieldName)
    {
        FieldInfo fi = typeof(GameField).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        fi.Should().NotBeNull($"Field {fieldName} not found");
        return fi.GetValue(gameField);
    }


    [UnityTest]
    public IEnumerator MoveLeft_BasicMovement_CoversBranches()
    {
        yield return null;

        var cells = (List<Cell>)GetPrivateField("cells");
        for (int i = 0; i < cells.Count; i++)
            cells[i].Position = new Vector2Int(3, i);

        gameField.MoveLeft();
        yield return new WaitForSeconds(0.3f);

        cells = (List<Cell>)GetPrivateField("cells");
        cells.Count.Should().BeGreaterThan(2, "После движения должна появиться новая клетка");
    }

    [UnityTest]
    public IEnumerator MoveRight_BasicMovement_CoversBranches()
    {
        yield return null;

        var cells = (List<Cell>)GetPrivateField("cells");
        for (int i = 0; i < cells.Count; i++)
            cells[i].Position = new Vector2Int(0, i);

        gameField.MoveRight();
        yield return new WaitForSeconds(0.3f);

        cells = (List<Cell>)GetPrivateField("cells");
        cells.Count.Should().BeGreaterThan(2);
    }

    [UnityTest]
    public IEnumerator MoveUp_BasicMovement_CoversBranches()
    {
        yield return null;

        var cells = (List<Cell>)GetPrivateField("cells");
        for (int i = 0; i < cells.Count; i++)
            cells[i].Position = new Vector2Int(i, 3);

        gameField.MoveUp();
        yield return new WaitForSeconds(0.3f);

        cells = (List<Cell>)GetPrivateField("cells");
        cells.Count.Should().BeGreaterThan(2);
    }

    [UnityTest]
    public IEnumerator MoveDown_BasicMovement_CoversBranches()
    {
        yield return null;

        var cells = (List<Cell>)GetPrivateField("cells");
        for (int i = 0; i < cells.Count; i++)
            cells[i].Position = new Vector2Int(i, 0);

        gameField.MoveDown();
        yield return new WaitForSeconds(0.3f);

        cells = (List<Cell>)GetPrivateField("cells");
        cells.Count.Should().BeGreaterThan(2);
    }

    [UnityTest]
    public IEnumerator MoveLeft_NoMovement_CoversBranch()
    {
        yield return null;

        var cells = (List<Cell>)GetPrivateField("cells");
        foreach (var c in cells)
            c.Position = new Vector2Int(0, c.Position.y);

        gameField.MoveLeft();
        yield return null;

        cells = (List<Cell>)GetPrivateField("cells");
        cells.Count.Should().Be(2, "При невозможности движения новых клеток не должно появляться");
    }


    [UnityTest]
    public IEnumerator CreateCell_ManualValue_CoversMethod()
    {
        yield return null;
        gameField.CreateCell(2);
        yield return null;
        var cells = (List<Cell>)GetPrivateField("cells");
        cells.Count.Should().BeGreaterThan(2);
    }


    [UnityTest]
    public IEnumerator GetEmptyPosition_NoEmpty_CoversMethod()
    {
        yield return null;
        var cells = (List<Cell>)GetPrivateField("cells");
        foreach (var c in new List<Cell>(cells))
            InvokePrivateMethod("RemoveAndDestroyCell", new object[] { c });
        cells.Clear();

        for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
            {
                gameField.CreateCell(1);
                var newCells = (List<Cell>)GetPrivateField("cells");
                newCells[newCells.Count - 1].Position = new Vector2Int(x, y);
            }
        LogAssert.ignoreFailingMessages = true;
        gameField.CreateCell(1);
        LogAssert.ignoreFailingMessages = false;
        LogAssert.Expect(LogType.Warning, "[GameField] No empty positions left.");
        yield return null;
    }


    [UnityTest]
    public IEnumerator ResetGame_CoversMethod()
    {
        yield return null;
        InvokePrivateMethod("ResetGame");
        var cells = (List<Cell>)GetPrivateField("cells");
        cells.Count.Should().Be(2, "После сброса должно быть 2 клетки");
        yield return null;
    }


    [UnityTest]
    public IEnumerator StartNewGame_CoversMethod()
    {
        yield return null;
        gameField.StartNewGame();
        yield return null;
        var cells = (List<Cell>)GetPrivateField("cells");
        cells.Count.Should().Be(2, "После новой игры должно быть 2 клетки");
        GameObject panel = (GameObject)GetPrivateField("gameOverPanel");
        panel.activeSelf.Should().BeFalse("Панель GameOver должна быть скрыта");
    }


    [Test]
    public void OnApplicationQuit_CoversMethod()
    {
        InvokePrivateMethod("OnApplicationQuit");
        Assert.Pass();
    }


    [UnityTest]
    public IEnumerator MoveCells_Merge_CoversBranch()
    {
        yield return null;
        var cells = (List<Cell>)GetPrivateField("cells");
        foreach (var c in new List<Cell>(cells))
            InvokePrivateMethod("RemoveAndDestroyCell", new object[] { c });
        cells.Clear();
        gameField.CreateCell(1);
        gameField.CreateCell(1);
        cells = (List<Cell>)GetPrivateField("cells");
        cells[0].Position = new Vector2Int(0, 0);
        cells[1].Position = new Vector2Int(1, 0);
        gameField.MoveLeft();
        yield return new WaitForSeconds(0.3f);
        cells = (List<Cell>)GetPrivateField("cells");
        cells.Count.Should().Be(2, "После слияния MoveLeft добавляет новую клетку через SpawnNewCellWithDelay");
        bool hasVal2 = false;
        foreach (var c in cells)
            if (c.Value == 2) hasVal2 = true;
        hasVal2.Should().BeTrue("После слияния должна получиться клетка со значением 2 (2^2 = 4)");
    }


    [UnityTest]
    public IEnumerator SpawnNewCellWithDelay_AddsNewCellAfterDelay()
    {
        yield return null;
        int initialCount = tilesContainerObj.transform.childCount;
        MethodInfo spawnMethod = typeof(GameField).GetMethod("SpawnNewCellWithDelay", BindingFlags.NonPublic | BindingFlags.Instance);
        spawnMethod.Should().NotBeNull("SpawnNewCellWithDelay not found");
        IEnumerator routine = (IEnumerator)spawnMethod.Invoke(gameField, new object[] { 0.1f });
        yield return gameField.StartCoroutine(routine);
        int newCount = tilesContainerObj.transform.childCount;
        newCount.Should().Be(initialCount + 1, "Новая клетка не создана после SpawnNewCellWithDelay");

        GameObject newCell = tilesContainerObj.transform.GetChild(newCount - 1).gameObject;
        CellView cv = newCell.GetComponent<CellView>();
        int spawnedValue = 0;
        if (cv != null)
        {
            FieldInfo valField = typeof(CellView).GetField("value", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            spawnedValue = (int)valField.GetValue(cv);
        }
        else
        {
            Text txt = newCell.GetComponentInChildren<Text>();
            int.TryParse(txt.text, out spawnedValue);
        }
        Assert.IsTrue(spawnedValue == 2 || spawnedValue == 4, $"Spawned cell value {spawnedValue} is not 2 or 4");
    }

    [UnityTest]
    public IEnumerator SpawnNewCellWithDelay_DoesNothingIfBoardFull()
    {
        yield return null;
        int width = 4, height = 4;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                gameField.CreateCell(2);
        int countBefore = tilesContainerObj.transform.childCount;
        Assert.AreEqual(width * height, countBefore, "Поле не заполнено как ожидалось");
        MethodInfo spawnMethod = typeof(GameField).GetMethod("SpawnNewCellWithDelay", BindingFlags.NonPublic | BindingFlags.Instance);
        IEnumerator routine = (IEnumerator)spawnMethod.Invoke(gameField, new object[] { 0.05f });
        yield return gameField.StartCoroutine(routine);
        int countAfter = tilesContainerObj.transform.childCount;
        Assert.AreEqual(countBefore, countAfter, "Новая клетка создана, хотя поле заполнено");
    }

    [Test]
    public void SaveGame_CoversMethod()
    {
        FieldInfo curScoreField = typeof(GameField).GetField("curScore", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo bestScoreField = typeof(GameField).GetField("bestScore", BindingFlags.NonPublic | BindingFlags.Instance);
        curScoreField.SetValue(gameField, 100);
        bestScoreField.SetValue(gameField, 200);

        var cells = (List<Cell>)GetPrivateField("cells");
        if (cells.Count == 0)
        {
            gameField.CreateCell(1);
            cells = (List<Cell>)GetPrivateField("cells");
        }
        int expectedCellCount = cells.Count;

        InvokePrivateMethod("SaveGame");

        string path = Path.Combine(Application.persistentDataPath, "savegame.dat");
        File.Exists(path).Should().BeTrue("Файл сохранения не создан");

        BinaryFormatter bf = new BinaryFormatter();
        GameField.SaveData loadedData;
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            loadedData = (GameField.SaveData)bf.Deserialize(fs);
        }
        loadedData.Should().NotBeNull();
        loadedData.curScore.Should().Be(100);
        loadedData.bestScore.Should().Be(200);
        loadedData.cells.Count.Should().Be(expectedCellCount);
        for (int i = 0; i < expectedCellCount; i++)
        {
            var originalCell = cells[i];
            var cellData = loadedData.cells[i];
            cellData.x.Should().Be(originalCell.Position.x);
            cellData.y.Should().Be(originalCell.Position.y);
            cellData.value.Should().Be(originalCell.Value);
        }
    }

    [Test]
    public void LoadGame_DeserializesStateAndRestoresGameField()
    {
        Type saveDataType = typeof(GameField.SaveData);
        GameField.SaveData saveData = Activator.CreateInstance(saveDataType) as GameField.SaveData;
        saveData.curScore = 50;
        saveData.bestScore = 200;
        saveData.cells = new List<GameField.CellData>();
        GameField.CellData cd1 = new GameField.CellData();
        cd1.x = 0; cd1.y = 0; cd1.value = 2;
        GameField.CellData cd2 = new GameField.CellData();
        cd2.x = 1; cd2.y = 0; cd2.value = 4;
        saveData.cells.Add(cd1);
        saveData.cells.Add(cd2);

        string path = Path.Combine(Application.persistentDataPath, "savegame.dat");
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = File.Create(path))
        {
            bf.Serialize(fs, saveData);
        }
        File.Exists(path).Should().BeTrue("Сохраненный файл не создан для теста LoadGame");

        InvokePrivateMethod("LoadGame");

        currentScoreText.text.Should().Be("50");
        bestScoreText.text.Should().Be("200");
        int cellCount = tilesContainerObj.transform.childCount;
        cellCount.Should().Be(2);
        bool found2 = false, found4 = false;
        for (int i = 0; i < cellCount; i++)
        {
            GameObject cellObj = tilesContainerObj.transform.GetChild(i).gameObject;
            CellView cv = cellObj.GetComponent<CellView>();
            int val = 0;
            if (cv != null)
            {
                FieldInfo vField = typeof(CellView).GetField("value", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                val = (int)vField.GetValue(cv);
            }
            else
            {
                Text txt = cellObj.GetComponentInChildren<Text>();
                int.TryParse(txt.text, out val);
            }
            if (val == 2) found2 = true;
            if (val == 4) found4 = true;
        }
        found2.Should().BeTrue();
        found4.Should().BeTrue();

        FieldInfo currScoreField = typeof(GameField).GetField("currentScore", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo bestScoreField = typeof(GameField).GetField("bestScore", BindingFlags.NonPublic | BindingFlags.Instance);
        ((int)currScoreField.GetValue(gameField)).Should().Be(50);
        ((int)bestScoreField.GetValue(gameField)).Should().Be(200);
    }
}
