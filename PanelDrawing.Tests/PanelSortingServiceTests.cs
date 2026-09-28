using PanelDrawing.Core.Sorting;
using PanelDrawing.Models;

namespace PanelDrawing.Tests;

[TestClass]
public sealed class PanelSortingServiceTests
{
    private static PanelDetailsRow Row(string fromConnector, string fromPin, string wireCode) => new()
    {
        FromConnector = fromConnector,
        FromPin = fromPin,
        WireCode = wireCode,
        ToConnector = "X1",
        ToPin = "1",
    };

    [TestMethod]
    public void SortPanelDetails_MonoWireCode_KeptAtPinPosition()
    {
        var rows = new List<PanelDetailsRow>
        {
            Row("C1", "2", "W2/18"),
            Row("C1", "1", "W1/18"),
        };

        var result = PanelSortingService.SortPanelDetails(rows);

        Assert.HasCount(2, result);
        Assert.AreEqual("1", result[0].FromPin);
        Assert.AreEqual("2", result[1].FromPin);
    }

    [TestMethod]
    public void SortPanelDetails_PairedWireCode_OutputsOncePerBaseWireOrderedByCore()
    {
        // "STQ_78/12/1" and "STQ_78/12/2" share the base wire "STQ_78/12" and should be
        // emitted together, ordered by core number, not duplicated per row.
        var rows = new List<PanelDetailsRow>
        {
            Row("C1", "2", "STQ_78/12/2"),
            Row("C1", "1", "STQ_78/12/1"),
        };

        var result = PanelSortingService.SortPanelDetails(rows);

        Assert.HasCount(2, result);
        Assert.AreEqual("STQ_78/12/1", result[0].WireCode);
        Assert.AreEqual("STQ_78/12/2", result[1].WireCode);
    }

    [TestMethod]
    public void SortPanelDetails_MalformedWireCode_DoesNotThrow()
    {
        // A wire code with no '/' at all used to crash ParseWireCode (parts.Length would be 1,
        // and the old code unconditionally indexed parts[2] for anything != 2 parts).
        var rows = new List<PanelDetailsRow>
        {
            Row("C1", "1", "NOSLASHES"),
            Row("C1", "2", ""),
        };

        var result = PanelSortingService.SortPanelDetails(rows);

        Assert.HasCount(2, result);
    }

    [TestMethod]
    public void SortPanelDetails_EquJConnector_SortsBeforePlainConnectors()
    {
        var rows = new List<PanelDetailsRow>
        {
            Row("TBK1", "1", "W1/18"),
            Row("EQU1_J1", "1", "W2/18"),
        };

        var result = PanelSortingService.SortPanelDetails(rows);

        Assert.AreEqual("EQU1_J1", result[0].FromConnector);
        Assert.AreEqual("TBK1", result[1].FromConnector);
    }
}
