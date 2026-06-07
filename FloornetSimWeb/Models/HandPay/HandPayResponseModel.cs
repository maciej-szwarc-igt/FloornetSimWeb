using IGT.FloorNet.EX.Handpay;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.HandPay
{
    public class HandPayResponseModel : ModelBase
    {
        private Int64 _identityPK;
        private string _identity;
        private bool _pouchPayEnable;
        private bool _keyToCreditEnable;
        private t_selfServeOption _selfServeOption;

        public HandPayResponseModel()
        {
            _identityPK = 1;
            _identity = _identityPK.ToString();
            _pouchPayEnable = false;
            _keyToCreditEnable = false;
            _selfServeOption = t_selfServeOption.none;
        }
        public void IncrementIdentityPK()
        {
            ++_identityPK;
            Identity = _identityPK.ToString();
        }

        public Int64 IdentityPK 
        { 
            set { _identityPK = value; OnPropertyChanged(nameof(IdentityPK)); } 
        
        }

         public string Identity
        {
            get { return _identity; }
            set { _identity = value; if (value != string.Empty ) _identityPK = Int64.Parse(value) ;  OnPropertyChanged(nameof(Identity)); }
        }

        public bool PouchPayEnable
        {
            get { return _pouchPayEnable; }
            set { _pouchPayEnable = value; OnPropertyChanged(nameof(PouchPayEnable)); }
        }
        public bool KeyToCreditEnable
        {
            get { return _keyToCreditEnable; }
            set { _keyToCreditEnable = value; OnPropertyChanged(nameof(KeyToCreditEnable)); }
        }
        public t_selfServeOption SelfServeOption
        {
            get { return _selfServeOption; }
            set { _selfServeOption = value; OnPropertyChanged(nameof(SelfServeOption)); }
        }

        public IEnumerable<t_selfServeOption> GetSelfServeOptions
        {
            get
            {
                return Enum.GetValues(typeof(t_selfServeOption)).Cast<t_selfServeOption>();
            }
        }

        public void Clear()
        {
            Identity = _identityPK.ToString();
            SelfServeOption = t_selfServeOption.none;
            PouchPayEnable = false;
            KeyToCreditEnable = false;
        }
    }
}
