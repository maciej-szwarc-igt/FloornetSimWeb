using IGT.FloorNet.EX.RG;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.PCS
{
    public class PCSContainer
    {
        public e_PCSLimitType type { get; set; }
        public e_PCSPeriodType period { get; set; }
        public long threshold { get; set; }
        public long currentValue { get; set; }
        public bool limitReached { get; set; }

        public PCSContainer(e_PCSLimitType Type, e_PCSPeriodType Period, long Threshold, long CurrentValue)
        {
            type = Type;
            period = Period;
            threshold = Threshold;
            currentValue = CurrentValue;
            limitReached = CurrentValue >= Threshold; 
        }
    }
}

