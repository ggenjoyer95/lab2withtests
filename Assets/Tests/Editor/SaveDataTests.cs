using NUnit.Framework;
using FluentAssertions;
using UnityEngine;
using UnityEngine.TestTools;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveDataTests
{
    [Test]
    public void SD_RoundTripSerialization_ReturnsSameData()
    {
        var orig = new SaveData
        {
            curScore = 100,
            bestScore = 200,
            cells = new List<CellData>
            {
                new CellData { x = 0, y = 0, val = 1 },
                new CellData { x = 1, y = 0, val = 2 },
                new CellData { x = 0, y = 1, val = 3 }
            }
        };

        SaveData deser;
        var bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, orig);
            ms.Seek(0, SeekOrigin.Begin);
            deser = (SaveData)bf.Deserialize(ms);
        }

        deser.Should().NotBeNull();
        deser.curScore.Should().Be(orig.curScore);
        deser.bestScore.Should().Be(orig.bestScore);
        deser.cells.Should().HaveCount(orig.cells.Count);

        for (int i = 0; i < orig.cells.Count; i++)
        {
            deser.cells[i].x.Should().Be(orig.cells[i].x);
            deser.cells[i].y.Should().Be(orig.cells[i].y);
            deser.cells[i].val.Should().Be(orig.cells[i].val);
        }
    }
}
