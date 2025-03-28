using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int curScore;
    public int bestScore;
    public List<CellData> cells = new List<CellData>();
}

[Serializable]
public class CellData
{
    public int x;
    public int y;
    public int val;
}
