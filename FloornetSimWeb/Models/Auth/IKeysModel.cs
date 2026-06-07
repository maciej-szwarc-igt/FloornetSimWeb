using IGT.FloorNet.SecurityUtils;
using System.Security.Cryptography;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Auth
{
    public class IKeysModel : ModelBase
    {
        private string _privateKey = string.Empty;
        private string _publicKey = string.Empty;
        private long _currentKeyNum = 0;

        public string PrivateKey { get => _privateKey; set { _privateKey = value; OnPropertyChanged(nameof(PrivateKey)); } }
        public string PublicKey { get => _publicKey; set { _publicKey = value; OnPropertyChanged(nameof(PublicKey)); } }
        public long CurrentKeyNum { get => _currentKeyNum; set { _currentKeyNum = value; OnPropertyChanged(nameof(CurrentKeyNum)); } }
        public ECDsa ECDsa { get; set; }
        public FloornetECDsaProvider FloornetECDsaProvider { get; } = new FloornetECDsaProvider();
    }
}
