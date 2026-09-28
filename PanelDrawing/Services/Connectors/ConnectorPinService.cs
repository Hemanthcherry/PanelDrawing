using System;
using System.Collections.Generic;
using System.Linq;
using PanelDrawing.Core.Constants;

namespace PanelDrawing.Services.Connectors
{
    public class ConnectorPinService
    {
        //public static bool ValidateFileSelection(string filePath)
        //{
        //    return File.Exists(filePath);
        //}

        public static bool SearchAndAppend(string istrName, List<string> list)
        {
            if (list.Contains(istrName))
            {
                return false;
            }

            list.Add(istrName);
            return true;
        }
      
        public static List<string> getPinsOfComponent(string connector)
        {
            if (string.IsNullOrWhiteSpace(connector))
                return new List<string>();

            connector = connector.Trim();

            var pins = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in PanelConstants.panelDetailsList)
            {
                // Match FromConnector
                if (row.FromConnector?.Trim().Equals(connector, StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (!string.IsNullOrWhiteSpace(row.FromPin))
                        pins.Add(row.FromPin.Trim());
                }

                // Match ToConnector
                if (row.ToConnector?.Trim().Equals(connector, StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (!string.IsNullOrWhiteSpace(row.ToPin))
                        pins.Add(row.ToPin.Trim());
                }
            }

            return pins.ToList();
        }

        public static List<string> GetPinsofOOTBRelay(string connector)
        {
            return PanelConstants.dataExtractionList
                .Where(x => x.ConnectorName == connector && !string.IsNullOrEmpty(x.PinNumber))
                .Select(x => x.PinNumber!)
                .ToList();
        }

    }
}
