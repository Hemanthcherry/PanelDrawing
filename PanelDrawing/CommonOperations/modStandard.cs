using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;

namespace PanelDrawing.CommonOperations
{
    public class modStandard
    {
        public static bool ValidateFileSelection(string filePath)
        {
            return File.Exists(filePath);
        }

        public static bool SearchAndAppend(string istrName, List<string> list)
        {
            if (list.Contains(istrName))
            {
                return false;
            }

            list.Add(istrName);
            return true;
        }
      
        public static string[] PinsOfConnector(string connector)
        {
            if (string.IsNullOrWhiteSpace(connector))
                return Array.Empty<string>();

            connector = connector.Trim();

            var pins = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in Constants.panelDetailsList)
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

            return pins.ToArray();
        }

        public static List<string> GetPinsofOOTBRelay(string connector)
        {
            var list = new List<string>();

            list = Constants.dataExtractionList.Where(x => x.ConnectorName == connector && !string.IsNullOrEmpty(x.PinNumber))
                                            .Select(x =>x.PinNumber).ToList();

            return list;
        }

        #region // Commented old code on Jan 27, 2026

        #region //Commented old PinsOfConnectorToArray code on 18 november, 2025
        //public static string[] PinsOfConnectorToArray(string istr, string[,] iSearcharr, int iSearchCol, int iOPCol)
        //{
        //    int T;
        //    int X = 0;
        //    string[] temp = new string[iSearcharr.GetLength(0)];

        //    for (T = 0; T <= iSearcharr.GetLength(0) - 1; T++)
        //    {
        //        string trimmedInput = istr.Trim();

        //        if (iSearcharr[T, iSearchCol].Trim() == trimmedInput)
        //        {
        //            string pin = iSearcharr[T, iOPCol];
        //            if (!IsAlreadyAdded(temp, pin, X))
        //            {
        //                temp[X] = pin;
        //                X = X + 1;
        //            }
        //        }
        //        else if (iSearcharr[T, 2].Trim() == trimmedInput)
        //        {
        //            string pin = iSearcharr[T, 3];
        //            if (!IsAlreadyAdded(temp, pin, X))
        //            {
        //                temp[X] = pin;
        //                X = X + 1;
        //            }
        //        }
        //    }
        //    // Resize the array to return only the used elements
        //    Array.Resize(ref temp, X);
        //    return temp;
        //}
        #endregion

        #region //Commented old IsAlreadyAdded code on 18 november, 2025
        // Helper method to check for duplicates
        //private static bool IsAlreadyAdded(string[] arr, string value, int length)
        //{
        //    for (int i = 0; i < length; i++)
        //    {
        //        if (arr[i] != null && arr[i].Trim() == value.Trim())
        //            return true;
        //    }
        //    return false;
        //}
        #endregion

        //public static int RowOfFoundStringIn1Darray(string searchStr, string[] array)
        //{
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        if (array[i] == searchStr)
        //        {
        //            return i; // Use i + 1 if you want to keep VB6's 1-based index behavior
        //        }
        //    }
        //    return 0;
        //}

        // Code Added by Anil
        /* public static string[] PinsOfConnectorToArray(string istr, string[,] iSearcharr, int iSearchCol, int iOPCol)
        {
            int T;
            int X;
            X = 0;
            string[] temp = new string[iSearcharr.GetLength(0)];
            for (T = 0; T <= iSearcharr.GetLength(0) - 1; T++)
            {
                if (iSearcharr[T, iSearchCol].Trim() == istr.Trim())
                {
                    temp[X] = iSearcharr[T, iOPCol];
                    X = X + 1;
                }
            }
            return temp;
        }*/

        // Code Added by Hemanth on 24-09-2025
        /* public static string[] PinsOfConnectorToArray(string istr, string[,] iSearcharr, int iSearchCol, int iOPCol)
        {
            int T;
            int X;
            X = 0;
            string[] temp = new string[iSearcharr.GetLength(0)];

            for (T = 0; T <= iSearcharr.GetLength(0) - 1; T++)
            {
                if (iSearcharr[T, iSearchCol].Trim() == istr.Trim())
                {
                    temp[X] = iSearcharr[T, iOPCol];
                    X = X + 1;
                }
                else if (iSearcharr[T, 2].Trim() == istr.Trim())
                {
                    temp[X] = iSearcharr[T, 3];
                    X = X + 1;
                }
            }
            return temp;
        }*/
        #endregion
    }
}
