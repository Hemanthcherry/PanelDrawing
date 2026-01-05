using Panel_Drawing.Forms;
using PanelDrawing.Objects;
using System.Text;
using System.Text.RegularExpressions;

namespace PanelDrawing.CommonOperations
{
    public class TextOperations
    {
        public static string[] ConvertListInto1DArray(List<string> lstlist)
        {
            string[] arrtemp = new string[lstlist[0].Split(',').Count()*lstlist.Count];
            int intTempCount = 0;
            for (int i = 0; i <= lstlist.Count - 1; i++)
            {
                for (int j = 0; j <= lstlist[i].Split(',').Count()-1; j++)
                {
                    string[] temparr = lstlist[i].Split(',');
                    arrtemp[intTempCount] = temparr[j];//lstlist[i].Split(',').ToString();
                    intTempCount++;
                }
            }
            return arrtemp;
        }

        public static List<string> ConvertTextFileIntoList(string txtFileName)
        {
            List<string> lstlineInfo = new List<string>();
            foreach (string line in File.ReadLines(txtFileName))
            {
                lstlineInfo.Add(line);
            }
            return lstlineInfo;
        }      


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

        public static List<PanelDetailsRow> SortPanelDetails(List<PanelDetailsRow> rows)
        {
            return rows
                .OrderBy(r => r.FromConnector, StringComparer.OrdinalIgnoreCase)
                .ThenBy(r => NormalizePin(r.FromPin))
                .ThenBy(r => r.WireCode)
                .ToList();
        }

        public static List<string> SortLinesByWireCodeGroup(List<string> lines)
        {
            var parsed = lines.Select((line, index) =>
            {
                var parts = line.Split(',');
                string connector = parts[0];
                string pin = parts[1];
                string wireCode = "";
                int? coreNum = null;

                if (parts.Length > 4 && !string.IsNullOrWhiteSpace(parts[4]))
                {
                    var wireParts = parts[4].Split('/');
                    wireCode = wireParts[0]; // e.g. PB101_3
                    if (wireParts.Length >= 3 && int.TryParse(wireParts[2], out int n))
                        coreNum = n;
                }

                return new { line, connector, pin, wireCode, coreNum, index };
            });

            // Step 2: Group by wireCode
            var groupedByWire = parsed
                .GroupBy(x => x.wireCode)
                .Select(group =>
                {
                    // Order inside the group by core number, then by pin order
                    var orderedGroup = group
                        .OrderBy(x => x.coreNum ?? int.MaxValue)
                        .ThenBy(x => NormalizePin(x.pin))
                        .ThenBy(x => x.index)
                        .ToList();

                    // We'll use the *first pin* of this group to decide group order
                    var firstPin = orderedGroup.First().pin;

                    return new
                    {
                        WireCode = group.Key,
                        FirstPin = firstPin,
                        Items = orderedGroup
                    };
                })
                // Step 3: Order groups by the normalized pin of their first item
                .OrderBy(g => NormalizePin(g.FirstPin))
                .SelectMany(g => g.Items)
                .Select(x => x.line)
                .ToList();

            return groupedByWire;
        }

        /* private static string NormalizePin(string pin)
         {
             var parts = SplitAlphaNumeric(pin);
             return string.Join("_", parts.Select(p => p is int n ? n.ToString("D6") : p.ToString()));
         }*/

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

        // Split pin into segments: numbers as int, letters as string
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
        public static List<string> SortPins(List<string> pins)
        {
            return pins
                .OrderBy(p => NormalizePin(p), StringComparer.Ordinal)
                .ToList();
        }
    }
}
