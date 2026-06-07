using System.Collections.Generic;
namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Download
{
    public enum GroupType
    {
        PKG,
        PKGS,
        SFL,
        Initial
    }
    public class FileGroupsContainer
    {
        public List<FileGroupContainer> Groups { get; set; } = new List<FileGroupContainer>();

        public string LastSelectedItem { get; set; } = string.Empty;
    }

    public class FileGroupContainer
    {
        public string fileGroup { get; set; }
        public string location { get; set; }
        public List<string> Files { get; set; }
        public List<string> SMIBs { get; set; }
        public GroupType GroupType { get; set; }
        public int DelaySecs { get; set; }
        public FileGroupContainer(string fileGroup, string location, GroupType type)
        {
            this.fileGroup = fileGroup;
            this.location = location;
            Files = new List<string>();
            SMIBs = new List<string>();
            GroupType = type;
            DelaySecs = 0;
        }
    }
}
