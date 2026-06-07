//
//<copyright file = "IConfSearch.cs" company = "IGT">
//Copyright(c) 2024 IGT.All rights reserved.
//</ copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.IConf
{
    

    public class ConfSearch : EventArgs
    {
        public string Category { get; }

        public ConfSearch(string category)
        {
            Category = category;
        }
    }
}
