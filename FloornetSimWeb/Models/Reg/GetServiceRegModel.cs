using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Reg
{
    public class GetServiceRegModel : ModelBase
    {
        private string _serviceName;
        private string _uid;


        public GetServiceRegModel()
        {
            Clear();
        }

        public string ServiceName
        {
            get => _serviceName;
            set
            {
                _serviceName = value;
                OnPropertyChanged("ServiceName");
            }
        }

        public string UId
        {
            get => _uid;
            set
            {
                _uid = value;
                OnPropertyChanged("UId");
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ServiceName) && !string.IsNullOrEmpty(UId);
        }

        public void Clear()
        {
            ServiceName = string.Empty;
            UId = Environment.MachineName;
        }
    }
}
