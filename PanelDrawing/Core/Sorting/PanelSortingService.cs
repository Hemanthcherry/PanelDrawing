using Panel_Drawing.Forms;
using PanelDrawing.Core.Constants;
using PanelDrawing.Models;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PanelDrawing.Core.Sorting
{
    public class PanelSortingService
    {
        private static int GetConnectorPriority(string connector)
        {
            if (string.IsNullOrWhiteSpace(connector))
                return 2;

            // Tier 0 → EQU J-connectors
            if (Regex.IsMatch(connector, @"_[Jj](?:[1-9]|1\d|2[0-4])$"))
                return 0;

            // Tier 1 → EQU lowercase connectors only (_a to _z excluding i & o)
            if (Regex.IsMatch(connector, @"_[a-hj-np-z]$"))
                return 1;

            // Tier 2 → everything else (DIS, REL, TBK, etc.)
            return 2;
        }

        public static List<PanelDetailsRow> SortPanelDetails(List<PanelDetailsRow> rows)
        {
            // 1️⃣ Pin-first ordering
            var pinSorted = rows
                .OrderBy(r => GetConnectorPriority(r.FromConnector)) // Connector comes first for Pin sequence
                .ThenBy(r => r.FromConnector, StringComparer.OrdinalIgnoreCase)
                .ThenBy(r => NormalizePin(r.FromPin))
                .ToList();

            var result = new List<PanelDetailsRow>();
            var processedBaseWires = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in pinSorted)
            {
                var wire = ParseWireCode(row.WireCode);

                // 🔹 Mono → keep at pin position
                if (wire.IsMono)
                {
                    result.Add(row);
                    continue;
                }

                // 🔹 Paired → output only once per BaseWire
                if (processedBaseWires.Contains(wire.BaseWire))
                    continue;

                // Collect all cores of this paired wire
                var pairedBlock = pinSorted
                    .Where(r =>
                    {
                        var w = ParseWireCode(r.WireCode);
                        return !w.IsMono && w.BaseWire.Equals(wire.BaseWire, StringComparison.OrdinalIgnoreCase);
                    })
                    .Select(r => new { Row = r, Core = ParseWireCode(r.WireCode).CoreNumber })
                    .OrderBy(x => x.Core)
                    .Select(x => x.Row);

                result.AddRange(pairedBlock);
                processedBaseWires.Add(wire.BaseWire);
            }

            return result;
        }

        public static void Export_PanelEquOri(StreamWriter writer, List<string> pinsList, string ori)
        {
           // var orientation = ori.Equals("RIGHT", StringComparison.OrdinalIgnoreCase) ? "R" : "L";
            if (pinsList.Count>0)
            {
                for (int i = 0; i < pinsList.Count; i++)
                {
                    writer.WriteLine($"{PanelConstants.txtEquName};{pinsList[i]};{ori}");
                }
            }
        }
        private static WireInfo ParseWireCode(string wireCode)
        {
            var parts = wireCode.Split('/');

            if (parts.Length == 2)
            {
                return new WireInfo
                {
                    BaseWire = wireCode,
                    IsMono = true,
                    CoreNumber = 0
                };
            }

            return new WireInfo
            {
                BaseWire = $"{parts[0]}/{parts[1]}",
                IsMono = false,
                CoreNumber = int.Parse(parts[2])
            };
        }      


        private static string NormalizePin(string pin)
        {
            if (string.IsNullOrWhiteSpace(pin))
                return "ZZZZ999999";

            var parts = SplitAlphaNumeric(pin);

            return string.Join("_", parts.Select(p =>
            {
                if (p is int n)
                    return n.ToString("D6"); // zero padded numeric
                return p.ToString();
            }));
        }

        //Split pin into segments: numbers as int, letters as string
        private static List<object> SplitAlphaNumeric(string input)
        {
            var parts = new List<object>();
            if (string.IsNullOrEmpty(input))
                return parts;

            var current = new StringBuilder();
            bool isDigit = char.IsDigit(input[0]);

            foreach (char c in input)
            {
                if (char.IsDigit(c) == isDigit)
                {
                    current.Append(c);
                }
                else
                {
                    parts.Add(isDigit ? int.Parse(current.ToString()) : current.ToString());
                    current.Clear();
                    current.Append(c);
                    isDigit = !isDigit;
                }
            }

            if (current.Length > 0)
                parts.Add(isDigit ? int.Parse(current.ToString()) : current.ToString());

            return parts;
        }    
    }
    public sealed class WireInfo
    {
        public string BaseWire { get; init; } = "";
        public bool IsMono { get; init; }
        public int CoreNumber { get; init; } // 0 for mono
    }

}

#region // Commented old code on Jan 27, 2026

//public static List<PanelDetailsRow> SortPanelDetails(List<PanelDetailsRow> rows)
//{
//    return rows
//        .OrderBy(r => r.FromConnector, StringComparer.OrdinalIgnoreCase)
//        .ThenBy(r => NormalizePin(r.FromPin))
//        .ThenBy(r => r.WireCode)
//        .ToList();
//}

//public static List<PanelDetailsRow> SortPanelDetails(List<PanelDetailsRow> rows)
//{
//    var result = rows
//        .Select(r => new
//        {
//            Row = r,
//            Wire = ParseWireCode(r.WireCode)
//        })
//        .OrderBy(x => x.Row.FromConnector, StringComparer.OrdinalIgnoreCase)

//        // 1️⃣ Group by base wire (STQ_78/12)
//        .ThenBy(x => x.Wire.BaseWire, StringComparer.OrdinalIgnoreCase)

//        // 2️⃣ Paired cables first, mono later
//        .ThenBy(x => x.Wire.IsMono) // false (paired) < true (mono)

//        // 3️⃣ Core number ordering (1,2,3,4)
//        .ThenBy(x => x.Wire.CoreNumber)

//        // 4️⃣ Pin ordering (stable & predictable)
//        .ThenBy(x => NormalizePin(x.Row.FromPin))

//        .Select(x => x.Row)
//        .ToList();

//    return result;
//}


//public static List<PanelDetailsRow> SortPanelDetails(List<PanelDetailsRow> rows)
//{
//    var result = new List<PanelDetailsRow>();
//    int i = 0;

//    rows = rows.OrderBy(r => r.FromConnector, StringComparer.OrdinalIgnoreCase)
//        .ThenBy(r => NormalizePin(r.FromPin))
//        .ToList();

//    while (i < rows.Count)
//    {
//        var current = rows[i];
//        if (result.Contains(current))
//        {
//            i++;
//            continue;
//        }
//        var wire = ParseWireCode(current.WireCode);

//        // Mono cable → copy as-is
//        if (wire.IsMono)
//        {
//            result.Add(current);
//            i++;
//            continue;
//        }

//        // Paired cable → detect contiguous block
//        var block = new List<(PanelDetailsRow Row, WireInfo Wire)>();              

//        int j = i;
//        while (j < rows.Count)
//        {
//            var w = ParseWireCode(rows[j].WireCode);

//            if (!w.IsMono && w.BaseWire == wire.BaseWire)
//            {
//                block.Add((rows[j], w));
//            }
//            j++;
//        }
//        // Sort ONLY inside the block by core number
//        foreach (var item in block.OrderBy(b => b.Wire.CoreNumber))
//            if (!result.Contains(item.Row))
//            {
//                result.Add(item.Row);
//            }

//        i ++;
//    }

//    return result;
//}


//private static WireInfo ParseWireCode(string wireCode)
//{
//    // Examples:
//    // STQ_78/12/1  -> paired
//    // SS_79/12     -> mono

//    var parts = wireCode.Split('/');

//    if (parts.Length == 2)
//    {
//        // Mono cable
//        return new WireInfo
//        {
//            BaseWire = wireCode,
//            IsMono = true,
//            CoreNumber = 0
//        };
//    }

//    if (parts.Length == 3 && int.TryParse(parts[2], out int core))
//    {
//        return new WireInfo
//        {
//            BaseWire = $"{parts[0]}/{parts[1]}",
//            IsMono = false,
//            CoreNumber = core
//        };
//    }

//    throw new FormatException($"Invalid WireCode: {wireCode}");
//}

//public static List<string> SortLinesByWireCodeGroup(List<string> lines)
//{
//    var parsed = lines.Select((line, index) =>
//    {
//        var parts = line.Split(',');
//        string connector = parts[0];
//        string pin = parts[1];
//        string wireCode = "";
//        int? coreNum = null;

//        if (parts.Length > 4 && !string.IsNullOrWhiteSpace(parts[4]))
//        {
//            var wireParts = parts[4].Split('/');
//            wireCode = wireParts[0]; // e.g. PB101_3
//            if (wireParts.Length >= 3 && int.TryParse(wireParts[2], out int n))
//                coreNum = n;
//        }

//        return new { line, connector, pin, wireCode, coreNum, index };
//    });

//    // Step 2: Group by wireCode
//    var groupedByWire = parsed
//        .GroupBy(x => x.wireCode)
//        .Select(group =>
//        {
//            // Order inside the group by core number, then by pin order
//            var orderedGroup = group
//                .OrderBy(x => x.coreNum ?? int.MaxValue)
//                .ThenBy(x => NormalizePin(x.pin))
//                .ThenBy(x => x.index)
//                .ToList();

//            // We'll use the *first pin* of this group to decide group order
//            var firstPin = orderedGroup.First().pin;

//            return new
//            {
//                WireCode = group.Key,
//                FirstPin = firstPin,
//                Items = orderedGroup
//            };
//        })
//        // Step 3: Order groups by the normalized pin of their first item
//        .OrderBy(g => NormalizePin(g.FirstPin))
//        .SelectMany(g => g.Items)
//        .Select(x => x.line)
//        .ToList();

//    return groupedByWire;
//}

/* private static string NormalizePin(string pin)
 {
     var parts = SplitAlphaNumeric(pin);
     return string.Join("_", parts.Select(p => p is int n ? n.ToString("D6") : p.ToString()));
 }*/

//public static string[] ConvertListInto1DArray(List<string> lstlist)
//{
//    string[] arrtemp = new string[lstlist[0].Split(',').Count() * lstlist.Count];
//    int intTempCount = 0;
//    for (int i = 0; i <= lstlist.Count - 1; i++)
//    {
//        for (int j = 0; j <= lstlist[i].Split(',').Count() - 1; j++)
//        {
//            string[] temparr = lstlist[i].Split(',');
//            arrtemp[intTempCount] = temparr[j];//lstlist[i].Split(',').ToString();
//            intTempCount++;
//        }
//    }
//    return arrtemp;
//}

//public static List<string> ConvertTextFileIntoList(string txtFileName)
//{
//    List<string> lstlineInfo = new List<string>();
//    foreach (string line in File.ReadLines(txtFileName))
//    {
//        lstlineInfo.Add(line);
//    }
//    return lstlineInfo;
//}


/*public static string[,] ConvertTextFileDataInto2DArray(string textFilePath)
{
    // Step 1: Read lines from file
    var lines = ConvertTextFileIntoList(textFilePath);

    // Step 2: Sort lines by connectName and wireCode
    var sortedLines = SortPanelDetails(lines).ToArray();

    // Step 3: Convert sorted lines to array for next processing
    string[,] data = CommonOperation.Conver1DArrayto2DArray(sortedLines, Path.GetFileName(textFilePath));

    return data;
}*/

/*  public static List<string> SortPanelDetails(List<string> lines)
  {
      var sortedLines = lines
          .GroupBy(line => line.Split(',')[0]) // group by connector
          .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase) // connector natural order
          .SelectMany(group => SortLinesByWireCodeGroup(group.ToList()))
          .ToList();

      return sortedLines;
  }*/


/*  private static List<object> SplitAlphaNumeric(string input)
  {
      var parts = new List<object>();
      if (string.IsNullOrEmpty(input))
          return parts;

      var current = "";
      bool isDigit = char.IsDigit(input[0]);

      foreach (char c in input)
      {
          if (char.IsDigit(c) == isDigit)
          {
              current += c;
          }
          else
          {
              // push previous part
              if (isDigit)
                  parts.Add(int.Parse(current));
              else
                  parts.Add(current);

              current = c.ToString();
              isDigit = !isDigit;
          }
      }

      // push last part
      if (!string.IsNullOrEmpty(current))
      {
          if (isDigit)
              parts.Add(int.Parse(current));
          else
              parts.Add(current);
      }

      return parts;
  }*/

//public static List<string> SortPins(List<string> pins)
//{
//    return pins
//        .OrderBy(p => NormalizePin(p), StringComparer.Ordinal)
//        .ToList();
//}
#endregion
