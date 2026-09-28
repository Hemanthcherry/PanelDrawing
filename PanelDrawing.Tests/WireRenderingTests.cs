using System.IO;
using PanelDrawing.Core.Constants;
using PanelDrawing.Services.P2;

namespace PanelDrawing.Tests;

[TestClass]
public sealed class WireRenderingTests
{
    [TestInitialize]
    public void ResetP2State()
    {
        // WireRouter.DrawWireLine normally resets this per run; tests call WireRenderer.DrawWire
        // directly, so the same reset has to happen here to keep each test deterministic.
        PanelConstants.WiringOffset = 0;
        PanelConstants.UsedX3Values = new HashSet<double>();
        PanelConstants.lst_WireCodes_Info_Processed = new HashSet<string>(StringComparer.Ordinal);
    }

    private static string RenderWire(double x1, double y1, double x2, double y2, string wireType = "SS",
        string fromOrientation = "", string toOrientation = "")
    {
        using var writer = new StringWriter();

        WireRenderer.DrawWire(writer, x1, y1, x2, y2, "W1", "18", "C1", "C2", "TBK", "TBK",
            "G1", "1000", wireType, "1", fromOrientation, toOrientation);

        return writer.ToString();
    }

    [TestMethod]
    public void DrawWire_SameY_RoutesStraight()
    {
        var output = RenderWire(x1: 0, y1: 100, x2: 200, y2: 100);

        StringAssert.Contains(output, "0,100 200,100");
    }

    [TestMethod]
    public void DrawWire_SameXWithinCTypeMargin_RoutesCType()
    {
        // X1 == X2 forces the C-type path (isCType = X1 == X2 || |X1-X2| < 50).
        var output = RenderWire(x1: 100, y1: 0, x2: 100, y2: 200);

        // Header is "GRI 0.5, 2;" / "ADD L154 :W0", then four coordinate lines
        // (X1,Y1 / X3,Y3 / X4,Y4 / X2,Y2) before ";;NOP;;"
        var lines = output.Split(Environment.NewLine);
        Assert.AreEqual("100,0", lines[2]);
        Assert.AreEqual("100,200", lines[5]);
        Assert.AreEqual(";;NOP;;", lines[6]);
    }

    [TestMethod]
    public void DrawWire_DifferentXBeyondCTypeMargin_RoutesZType()
    {
        // |X1-X2| >= 50 and Y1 != Y2 forces the Z-type path.
        var output = RenderWire(x1: 0, y1: 0, x2: 200, y2: 200);

        var lines = output.Split(Environment.NewLine);
        Assert.AreEqual("0,0", lines[2]);
        Assert.AreEqual("200,200", lines[5]);
        StringAssert.Contains(output, ";;NOP;;");
    }

    [TestMethod]
    public void GetX3Value_RightOrientation_BendsLeftOfX1()
    {
        double x3 = WireRenderer.GetX3Value(FromOrientation: "R", ToOrientation: "", X1: 300);

        Assert.AreEqual(300 - 80, x3);
    }

    [TestMethod]
    public void GetX3Value_LeftOrientation_BendsRightOfX1()
    {
        double x3 = WireRenderer.GetX3Value(FromOrientation: "L", ToOrientation: "", X1: 300);

        Assert.AreEqual(300 + 80, x3);
    }

    [TestMethod]
    public void GetX3Value_NoOrientationBelowMargin_BendsRight()
    {
        // MarginX defaults to 60; X1 <= 80 + MarginX takes the "bend right" else branch.
        double x3 = WireRenderer.GetX3Value(FromOrientation: "", ToOrientation: "", X1: 50);

        Assert.AreEqual(50 + 80, x3);
    }

    [TestMethod]
    public void GetUniqueX3_CollidingValue_StepsAwayFromExisting()
    {
        PanelConstants.UsedX3Values.Add(100);

        double result = WireRenderer.GetUniqueX3(100);

        Assert.AreNotEqual(100, result);
        Assert.IsGreaterThan(2, Math.Abs(result - 100));
    }

    [TestMethod]
    public void DrawWire_FirstOccurrenceOfWireCode_EmitsEndSymbols()
    {
        var output = RenderWire(x1: 0, y1: 100, x2: 200, y2: 100);

        StringAssert.Contains(output, "ADD I2");
    }

    [TestMethod]
    public void DrawWire_RepeatedWireCode_SuppressesDuplicateEndSymbols()
    {
        using var writer = new StringWriter();

        WireRenderer.DrawWire(writer, 0, 100, 200, 100, "DUPWIRE", "18", "C1", "C2", "TBK", "TBK",
            "G1", "1000", "SS", "1", "", "");
        WireRenderer.DrawWire(writer, 0, 300, 200, 300, "DUPWIRE", "18", "C1", "C2", "TBK", "TBK",
            "G1", "1000", "SS", "1", "", "");

        var output = writer.ToString();
        int endSymbolCount = output.Split("ADD I2").Length - 1;

        // First call draws source+destination end symbols (2). Second call, same wire code, draws none more.
        Assert.AreEqual(2, endSymbolCount);
    }

    [TestMethod]
    public void DrawWire_IncrementsWiringOffsetAfterEachWire()
    {
        using var writer = new StringWriter();

        Assert.AreEqual(0, PanelConstants.WiringOffset);

        WireRenderer.DrawWire(writer, 0, 100, 200, 100, "W1", "18", "C1", "C2", "TBK", "TBK",
            "G1", "1000", "SS", "1", "", "");

        Assert.AreEqual(4, PanelConstants.WiringOffset);
    }

    [TestMethod]
    [DataRow("MONO")]
    [DataRow("SS")]
    [DataRow("TP")]
    [DataRow("STP")]
    [DataRow("COAX")]
    [DataRow("TRIAX")]
    [DataRow("QUADRAX")]
    public void DrawWire_AllSupportedWireTypes_RenderWithoutError(string wireType)
    {
        var output = RenderWire(x1: 0, y1: 100, x2: 200, y2: 100, wireType: wireType);

        Assert.IsFalse(string.IsNullOrWhiteSpace(output));
    }
}
