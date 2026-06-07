using System;
using System.Collections.Generic;


namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Reg
{
    public class DisableEGMModel : ModelBase
    {
        private bool _state;
        private string _disableKey;
        private t_Disablekey _keys;
        private string _uId;

        public DisableEGMModel()
        {
            Clear();
        }

        public bool State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }
        }

        public string DisableKey
        {
            get => _disableKey;
            set
            {
                _disableKey = value;
                OnPropertyChanged("DisableKey");
            }
        }

        public t_Disablekey Keys
        {
            get => _keys;
            set
            {
                _keys = value;
                OnPropertyChanged("Keys");
            }

        }

        public string uId
        {
            get => _uId;
            set
            {
                _uId = value;
                OnPropertyChanged("UID");
            }
        }

        public void Clear()
        {
            State = true;
            DisableKey = string.Empty;
            Keys = t_Disablekey.SELECT;
            uId = string.Empty;
        }
    }

    public enum t_Disablekey
    {
        SELECT,
        EGM_DENOM_ENABLE_DISABLE,
        EGM_JACKPOT_ENABLE_DISABLE,
        EGM_CASHLESS_ENABLE_DISABLE,
        EGM_FLOOR_ENABLE_DISABLE,
        EGM_QUEUE_ENABLE_DISABLE,
        EGM_NET_ENABLE_DISABLE,
        EGM_GAME_ENABLE_DISABLE,
        EGM_PAYTOCREDIT_ENABLE_DISABLE,
        EGM_VPC_ENABLE_DISABLE,
        EGM_CRDLD_VERIFY_ENABLE_DISABLE,
        EGM_CLG_ENABLE_DISABLE,
        EGM_BILLTAX_ENABLE_DISABLE,
        EGM_RG_ENABLE_DISABLE,
        EGM_SAN_ENABLE_DISABLE
    }

}
