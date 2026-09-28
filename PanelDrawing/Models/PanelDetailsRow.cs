using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDrawing.Models
{
    public class PanelDetailsRow
    {
        public string? FromConnector { get; set; }
        public string? FromPin { get; set; }
        public string? ToConnector { get; set; }
        public string? ToPin { get; set; }
        public string? WireCode { get; set; }
        public string? PinX { get; set; }
        public string? PinY { get; set; }
        public string? TPinX { get; set; }
        public string? TPinY { get; set; }
        public string? Usage { get; set; }
        public string? FromType { get; set; }
        public string? ToType { get; set; }
        public string? FromOrientation { get; set; }
        public string? ToOrientation { get; set; }
        public string? GroupId { get; set; }
        public string? WireLength { get; set; }
        public string? WireType { get; set; }
        public string? WireTypeNumber { get; set; }
    }

}
