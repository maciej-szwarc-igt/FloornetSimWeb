using System.Collections.Generic;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Bonus
{
    public static class BonusHistoryInfo
    {
        public static long BonusId
        { get; set; }

        public static long HitSeqNum
        { get; set; }

    }
    public static class SetFunctionDataInfo
    {
        public static string Message
        { get; set; }
    }

    public class SetUIFunctionInfo
    {
        public string name
        { get; set; }

        public List<string> functions
        { get; set; }

    }

    public static class SetHostStateInfo
    {
        public static long BonusId
        { get; set; }

        public static bool HostState
        { get; set; }

    }
}
