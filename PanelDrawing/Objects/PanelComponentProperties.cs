using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDrawing.Objects
{
    public class PanelComponentProperties
    {
        public string ComponentName { get; set; }
        public string ComponentType { get; set; }
        public string MacroName { get; set; }
        public string MaxPin { get; set; }
        public string PartNumber { get; set; }
        public string Accessory { get; set; }
        public string SamplePin { get; set; }
        public string GroupId { get; set; }
        public string WireLength { get; set; }
        public string WireType { get; set; }
        public string CBTypeName { get; set; }
        public string CBVoltage { get; set; }
        public string AssociatedPartNumbers { get; set; }
        public string EquipmentBox { get; set; }
        public string Looms { get; set; }
        public string ShuntList { get; set; }

        public double CompWidth { get; set; }

        public double CompHeight { get; set; }
    }
}
