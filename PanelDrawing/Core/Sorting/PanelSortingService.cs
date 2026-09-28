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
        private static int GetConnectorPriority(string? connector)
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
        private static WireInfo ParseWireCode(string? wireCode)
        {
            var parts = (wireCode ?? string.Empty).Split('/');

            if (parts.Length != 3)
            {
                return new WireInfo
                {
                    BaseWire = wireCode ?? string.Empty,
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


        private static string NormalizePin(string? pin)
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
