using NUnit.Framework;
using FluentAssertions;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class CellViewTests
{
    [Test]
    public void CV_ForceAwkDest_Ref()
    {
        var go = new GameObject("CVObj", typeof(RectTransform), typeof(Image));
        var cv = go.AddComponent<CellView>();

        var mAwk = typeof(CellView).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
        mAwk.Should().NotBeNull("Awk must exist in CV");
        mAwk.Invoke(cv, null);

        var rt = go.GetComponent<RectTransform>();
        rt.Should().NotBeNull("RT should be present");
        var bgField = typeof(CellView).GetField("backgroundImage", BindingFlags.Instance | BindingFlags.NonPublic);
        var bg = bgField.GetValue(cv) as Image;
        bg.Should().NotBeNull("BG should be assigned in Awake");

        var txtObj = new GameObject("Txt", typeof(RectTransform));
        txtObj.transform.SetParent(go.transform);
        var txt = txtObj.AddComponent<Text>();
        var fld = typeof(CellView).GetField("valueText", BindingFlags.NonPublic | BindingFlags.Instance);
        fld.SetValue(cv, txt);

        var cell = new Cell(new Vector2Int(0, 0), 1);

        var mInit = typeof(CellView).GetMethod("Init", BindingFlags.Instance | BindingFlags.Public);
        mInit.Should().NotBeNull("Init must exist in CV");
        mInit.Invoke(cv, new object[] { cell });

        cell.Value = 2;
        cell.Position = new Vector2Int(1, 1);
        txt.text.Should().Be("4");

        var mDest = typeof(CellView).GetMethod("OnDestroy", BindingFlags.Instance | BindingFlags.NonPublic);
        mDest.Should().NotBeNull("OnDest must exist in CV");
        mDest.Invoke(cv, null);

        Object.DestroyImmediate(go);
    }

}
