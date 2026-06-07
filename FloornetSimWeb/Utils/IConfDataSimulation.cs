//
//<copyright file = IConfDataSimulation.cs" company = "IGT">
//Copyright(c) 2024 IGT.All rights reserved.
//</ copyright>
using IGT.FloorNet.Tools.ServiceSimulator.Models;

namespace IGT.FloorNet.Tools.ServiceSimulator.Utils
{
    
    public class CategoryOptionInfo : ModelBase
    {        
        public SupportedDatatypes dataType { get; set; }
        public long ConfigSeq { get; set; }
        public OptionItemDetail ItemDetail { get; set; }
        public int InternalSeq { get; set; }
        public bool ReadOnly { get; set; }
        public string MessageCategory { get; set; }
        public int Count { get; set; }
        public string setBy { get; set; }

    }
   
}
