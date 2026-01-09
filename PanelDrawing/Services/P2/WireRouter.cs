using PanelDrawing.CommonOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDrawing.Services.P2
{
    public static class WireRouter
    {
        public static void DrawWireLine()
        {
            double X1 = 0, Y1 = 0, X2 = 0, Y2 = 0;
            string c1 = string.Empty, c2 = string.Empty;
            string F_Type = string.Empty, T_Type = string.Empty;
            string F_Ori = string.Empty, T_Ori = string.Empty;
            string WireCode = string.Empty, GroupId = string.Empty, Wire_Length = string.Empty, Wire_Type = string.Empty, Wire_Type_Number = string.Empty, strPin = string.Empty;

            Constants.lst_WireCodes_Info_Processed = new List<string>();

            // keep the small GRID writer as in original
           // using (var writer = File.AppendText(Constants.el_ExecFilePath))
           // {
                //writer.WriteLine("GRID 2.0,2 ;");//, Constants.arrPanelDetails.GetLength(0)));
           // }

            // iterate upgraded panelDetailsList (preserves original for-loop semantics)
            foreach (var pd in Constants.panelDetailsList)
            {
                // skip rows which don't have coordinates
                if (string.IsNullOrWhiteSpace(pd.PinX)) continue;
                if (string.IsNullOrWhiteSpace(pd.PinY)) continue;
                if (string.IsNullOrWhiteSpace(pd.TPinX)) continue;
                if (string.IsNullOrWhiteSpace(pd.TPinY)) continue;

                // parse coordinates safely (original used Convert.ToDouble on strings)
                if (!double.TryParse(pd.PinX, out X1)) continue;
                if (!double.TryParse(pd.PinY, out Y1)) continue;
                if (!double.TryParse(pd.TPinX, out X2)) continue;
                if (!double.TryParse(pd.TPinY, out Y2)) continue;

                c1 = pd.FromConnector ?? string.Empty;
                c2 = pd.ToConnector ?? string.Empty;
                F_Type = pd.FromType ?? string.Empty;
                T_Type = pd.ToType ?? string.Empty;
                WireCode = pd.WireCode ?? string.Empty;
                F_Ori = pd.FromOrientation ?? string.Empty;
                T_Ori = pd.ToOrientation ?? string.Empty;
                strPin = pd.Usage ?? string.Empty;
                GroupId = pd.GroupId ?? string.Empty;
                Wire_Length = pd.WireLength ?? string.Empty;
                Wire_Type = pd.WireType ?? string.Empty;
                Wire_Type_Number = pd.WireTypeNumber ?? string.Empty;

                if (c1.EndsWith("_F"))
                {
                    X1 += 10;
                }

                if (!Constants.fromConnectorProcessed_P2.Contains(c1))
                {
                    Constants.fromConnectorProcessed_P2.Add(c1);
                    Constants.WiringOffset = 0;  // Assigning Offest to default when New Connector wiring starts
                }
                // preserve original behaviour: only route if X1 < X2 => only left to right
                if (X1 <= X2)
                {
                    ConnectionRequired(X1, Y1, X2, Y2, c1, c2, WireCode, F_Type, T_Type, F_Ori, T_Ori, GroupId, Wire_Length, Wire_Type, Wire_Type_Number);
                }
            }
        }

        private static void ConnectionRequired(double p1x, double p1y, double p2x, double p2y, string c1, string c2, string wCode,
            string iF_Type, string iT_Type, string iF_Ori, string iT_Ori, string groupId, string wire_Length, string wire_Type, string wire_Type_Core_Number)
        {
            string gauge = string.Empty;
            string wire_code = string.Empty;

            if (string.IsNullOrEmpty(wCode))
                wCode = string.Empty;

            var arrTemp = wCode.Split('/');
            wire_code = arrTemp.Length > 0 ? arrTemp[0] : string.Empty;
            gauge = arrTemp.Length > 1 ? arrTemp[1] : string.Empty;

            // Straight horizontal/overlap case
            // if(p1y == p2y)
         /*   if (false)
            {
                if (c1 == c2 && p1x == p2x)
                {
                    // overlapping wire scenario — do nothing
                }
                else
                {
                    //WireDrawing.DrawStraightLineConnection(
                    //        p1x, p1y, p2x, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, groupId, wire_Length, wire_Type, wire_Type_Core_Number);

                    //choose specialized straight - line routines based on wire_Type / wire_code
                    //if (wire_Type.Equals("86A9S") || wire_Type.Equals("86A9SS") || wire_Type.Equals("S") || wire_Type.Equals("S0") || wire_Type.Equals("S00")
                    //    || wire_code?.StartsWith("SS") == true | wire_Type.Equals("COAX") || wire_Type.Equals("TRIAX"))
                    //{
                    //    WireDrawing.Simple2PointConnection_Mono_StraightLine(
                    //        p1x, p1y, p2x, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, groupId, wire_Length, wire_Type, wire_Type_Core_Number);
                    //}
                    if (wire_Type.Equals("TP") || wire_Type.Equals("QUADRAX"))
                    {
                        WireDrawing.Simple2PointConnection_TP_StraightLine(
                            p1x, p1y, p2x, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, groupId, wire_Length, wire_Type, wire_Type_Core_Number);
                    }
                    else if (wire_Type.Equals("STP"))
                    {
                        WireDrawing.Simple2PointConnection_STP_StraightLine(
                            p1x, p1y, p2x, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, groupId, wire_Length, wire_Type, wire_Type_Core_Number);
                    }
                    else
                    {
                        WireDrawing.Simple2PointConnection_Mono_StraightLine(
                            p1x, p1y, p2x, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, groupId, wire_Length, wire_Type, wire_Type_Core_Number);
                    }
                }
            }
            else
            {*/
                // If either orientation is Top/Bottom, original code had commented Simple3PointConnection call.
                if (iF_Ori == "T" || iF_Ori == "B" || iT_Ori == "T" || iT_Ori == "B")
                {
                    //modOPCommand.Simple3PointConnection(p1x + 4, p1y, p2x + 4, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, iF_Ori, iT_Ori);
                }
                else
                {
                    // Z-line optimized routing (main path for non-horizontal)
                    WireDrawing.DrawWire(
                        p1x, p1y, p2x, p2y, wire_code, gauge, c1, c2, iF_Type, iT_Type, groupId, wire_Length, wire_Type, wire_Type_Core_Number, iF_Ori, iT_Ori);
                }
          //  }
        }   

    }
}
