using NUnit.Framework;
using FluentAssertions;
using UnityEngine;

public class CellTests
{
    [Test]
    public void Ctor_SetsPosAndVal()
    {
        var cell = new Cell(new Vector2Int(1, 2), 3);
        
        cell.Position.Should().Be(new Vector2Int(1, 2));
        cell.Value.Should().Be(3);
    }

    [Test]
    public void SetPos_FiresEvent()
    {
        var cell = new Cell(new Vector2Int(0, 0), 1);
        bool evtFired = false;
        cell.OnPositionChanged += (c, pos) => { evtFired = true; };

        cell.Position = new Vector2Int(5, 5);
        
        evtFired.Should().BeTrue("changing pos should fire event");
        cell.Position.Should().Be(new Vector2Int(5, 5));
    }

    [Test]
    public void SetPos_Same_NoEvent()
    {
        var cell = new Cell(new Vector2Int(0, 0), 1);
        bool evtFired = false;
        cell.OnPositionChanged += (c, pos) => { evtFired = true; };

        cell.Position = new Vector2Int(0, 0);
        
        evtFired.Should().BeFalse("same pos should not fire event");
    }

    [Test]
    public void SetVal_FiresEvent()
    {
        var cell = new Cell(new Vector2Int(0, 0), 1);
        bool evtFired = false;
        cell.OnValueChanged += (c, val) => { evtFired = true; };

        cell.Value = 2;
        
        evtFired.Should().BeTrue("changing val should fire event");
        cell.Value.Should().Be(2);
    }

    [Test]
    public void SetVal_Same_NoEvent()
    {
        var cell = new Cell(new Vector2Int(0, 0), 1);
        bool evtFired = false;
        cell.OnValueChanged += (c, val) => { evtFired = true; };

        cell.Value = 1;
        
        evtFired.Should().BeFalse("setting same val should not fire event");
    }
}
