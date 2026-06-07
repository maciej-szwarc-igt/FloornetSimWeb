
namespace IGT.FloorNet.Tools.ServiceSimulator.Utils
{
    public class OptionItemDetail
    {
        public string name { get; set; }
        public bool ReadOnly { get; set; }
        public dynamic value { get; set; }
        public string Description { get; set; }
        public bool Send { get; set; } = true;

        public OptionItemDetail()
        {
            value = "";
        }
    }

    public enum SupportedDatatypes
    {
        String,
        Bool,
        String40,
        String16,
        String32,
        String64,
        String128,
        Long,
        MessageDB
    } 
};
