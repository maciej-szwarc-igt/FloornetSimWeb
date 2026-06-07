using System;
using System.Collections.Generic;
using IGT.FloorNet.EX.Registration;
using IGT.FloorNet.EX.Tito;
using IGT.FloorNet.EX.Tito.evt;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Discovery
{
    public class DiscoveryModel: ModelBase
    {
        private bool _cardless;
        private bool _eft;
        private bool _bonus;
        private bool _diags;
        private bool _gat;
        private bool _conf;
        private bool _tito;
        private bool _pin;
        private bool _marker;
        private bool _rg;
        private bool _reg;
        private bool _player;
        private bool _handpay;
        private bool _AML;
        private bool _PCS;
        private bool _ism;
        private bool _wat;

        public DiscoveryModel()
        {
            Init();
        }

        public bool CardlessInterface
        {
            get => _cardless;
            set
            {
                _cardless = value;
                OnPropertyChanged("CardlessInterface");
            }
        }

        public bool EftInterface
        {
            get => _eft;
            set
            {
                _eft = value;
                OnPropertyChanged("EftInterface");
            }
        }
        public bool AMLInterface
        {
            get => _AML;
            set
            {
                _AML = value;
                OnPropertyChanged("AMLInterface");
            }
        }
        public bool PlayerInterface
        {
            get => _player;
            set
            {
                _player = value;
                OnPropertyChanged("PlayerInterface");
            }
        }
        
        public bool BonusInterface
        {
            get => _bonus;
            set
            {
                _bonus = value;
                OnPropertyChanged("BonusInterface");
            }
        }

        public bool DiagsInterface
        {
            get => _diags;
            set
            {
                _diags = value;
                OnPropertyChanged("DiagsInterface");
            }
        }
        public bool GatInterface
        {
            get => _gat;
            set
            {
                _gat = value;
                OnPropertyChanged("GatInterface");
            }
        }

        public bool ConfInterface
        {
            get => _conf;
            set
            {
                _conf = value;
                OnPropertyChanged("ConfInterface");
            }
        }

        public bool DownloadInterface
        {
            get => _conf;
            set
            {
                _conf = value;
                OnPropertyChanged("DownloadInterface");
            }
        }

        public bool TitoInterface
        {
            get => _tito;
            set
            {
                _tito = value;
                OnPropertyChanged("TitoInterface");
            }
        }

        public bool PinInterface
        {
            get => _pin;
            set
            {
                _pin = value;
                OnPropertyChanged("PinInterface");
            }
        }
        public bool MarkerInterface
        {
            get => _marker;
            set
            {
                _marker = value;
                OnPropertyChanged("MarkerInterface");
            }
        }

        public bool RGInterface
        {
            get => _rg;
            set
            {
                _rg = value;
                OnPropertyChanged("RGInterface");
            }
        }

        public bool RegInterface
        {
            get => _reg;
            set
            {
                _reg = value;
                OnPropertyChanged("RegInterface");
            }
        }

        public bool HandpayInterface
        {
            get => _handpay;
            set { _handpay = value;OnPropertyChanged("HandpayInterface"); }
        }

        public bool PCSInterface
        {
            get => _PCS;
            set
            {
                _PCS = value;
                OnPropertyChanged("PCSInterface");
            }
        }

        public bool IsmInterface
        {
            get => _ism;
            set
            {
                _ism = value;
                OnPropertyChanged("IsmInterface");
            }
        }

        public bool WatInterface
        {
            get => _wat;
            set
            {
                _wat = value;
                OnPropertyChanged("WatInterface");
            }
        }

        public void Init()
        {
            CardlessInterface = true;
            EftInterface = true;
            BonusInterface = true;
            DiagsInterface = true;
            GatInterface = true;
            TitoInterface = true;
            PinInterface = true;
            MarkerInterface = true;          
            RGInterface = true;
            RegInterface = true;
            ConfInterface = true;
            AMLInterface = true;
            DownloadInterface = true;
            PlayerInterface = true;
            HandpayInterface = true;
            PCSInterface = true;
            IsmInterface = true;
            WatInterface = true;
        }

        public void Clear()
        {
            CardlessInterface = true;
            EftInterface = true;
            BonusInterface = true;
            DiagsInterface = true;
            GatInterface = true;
            TitoInterface = true;
            PinInterface = true;
            MarkerInterface = true;
            ConfInterface = true;
            RGInterface = true;
            RegInterface = true;
            DownloadInterface = true;
            PlayerInterface = true;
            HandpayInterface = true;
            AMLInterface = true;
            IsmInterface = true;
            WatInterface = true;
        }

        public string GetActiveInterfaces()
        {
            string response = CardlessInterface ? "iCardless," : "";
            response += EftInterface ? "iEft," : ""; 
            response += BonusInterface ? "iBonus," : "";
            response += DiagsInterface ? "iDiags," : "";
            response += GatInterface ? "iGat," : "";
            response += TitoInterface ? "iTito," : "";
            response += PinInterface ? "iPin," : "";
            response += MarkerInterface ? "iMarker," : "";
            response += RegInterface ? "iReg," : "";
            response += RGInterface ? "iRG," : "";
            response += PlayerInterface ? "iPlayer," : "";
            response += HandpayInterface ? "iHandpay," : "";
            response += AMLInterface ? "iAML," : "";
            response += PCSInterface ? "iPCS," : "";
            response += IsmInterface ? "iISM," : "";
            response += WatInterface ? "iWat," : "";

            return response.TrimEnd(',');
        }
    }
    public class GetServiceInterfaceValues
    {
        public serviceDescr GetiCardlessValue()
        {
            return new serviceDescr { @interface = "iCardless", name = "Cardless Service", version = "1.0" };
        }
        public serviceDescr GetiEftValue()
        {
            return new serviceDescr { @interface = "iEft", name = "EFT Service", version = "1.0" };
        }
        public serviceDescr GetiPlayerValue()
        {
            return new serviceDescr { @interface = "iPlayer", name = "Player Service", version = "1.0" };
        }
        public serviceDescr GetiBonusValue()
        {
            return new serviceDescr { @interface = "iBonus", name = "Bonus Service", version = "1.0" };
        }
        public serviceDescr GetiDiagsValue()
        {
            return new serviceDescr { @interface = "iDiags", name = "Diag Service", version = "1.0" };              
        }
        public serviceDescr GetiGatValue()
        {
            return new serviceDescr { @interface = "iGat", name = "Gat Service", version = "1.0" };
        }
        public serviceDescr GetConfValue()
        {
            return new serviceDescr { @interface = "iConfig", name = "Config Service", version = "1.0" };
        }
        public serviceDescr GetDownloadValue()
        {
            return new serviceDescr { @interface = "iDownload", name = "Download Service", version = "1.0" };
        }

        public serviceDescr GetAMLValue()
        {
            return new serviceDescr { @interface = "iAML", name = "AML Service", version = "1.0" };
        }
        public serviceDescr GetPCSValue()
        {
            return new serviceDescr { @interface = "iPCS", name = "PCS Service", version = "1.0" };
        }

        public serviceDescr GetiTitoValue()
        {
            return new serviceDescr { @interface = "iTito", name = "Tito Service", version = "1.0" };
        }
        public serviceDescr GetiPinValue()
        {
            return new serviceDescr { @interface = "iPin", name = "Pin Service", version = "1.0" };
        }
        public serviceDescr GetiMarkerValue()
        {
            return new serviceDescr { @interface = "iMarker", name = "Marker Service", version = "1.0" };
        }
        public serviceDescr GetiRGValue()
        {
            return new serviceDescr { @interface = "iRG", name = "RG Service", version = "1.0" };
        }
        public serviceDescr GetiISMValue()
        {
            return new serviceDescr { @interface = "iISM", name = "ISM Service", version = "1.0" };
        }
        public serviceDescr GetiRegValue()
        {
            return new serviceDescr { @interface = "iReg", name = "Registration Service", version = "1.0" };
        }
        public serviceDescr GetiHandpayValue()
        {
            return new serviceDescr { @interface = "iHandpay", name = "Handpay Service", version = "1.0" };
        }
        public serviceDescr GetiWatValue()
        {
            return new serviceDescr { @interface = "iWat", name = "Wat Service", version = "1.0" };
        }

        public serviceDescr GetNoServiceValue(string name)
        {
            return new serviceDescr { @interface = name};
        }
    }
}
