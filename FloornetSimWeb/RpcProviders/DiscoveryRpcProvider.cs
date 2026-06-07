using IGT.FloorNet.EX.Registration;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.Tools.ServiceSimulator.Models;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Discovery;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels;
using IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IGT.FloorNet.Tools.ServiceSimulator.RpcProviders
{
    public class DiscoveryRpcProvider : iDiscovery
    {
        private readonly DiscoveryViewModel _discoveryViewModel;

        private readonly ResponseViewModel _responseViewModel;

        private DiscoveryModel _discoveryModel { get { return _discoveryViewModel.DiscoveryModel; } }

        public DiscoveryRpcProvider(DiscoveryViewModel discoveryViewModel, ResponseViewModel responseViewModel)
        {
            _discoveryViewModel = discoveryViewModel;
            _responseViewModel = responseViewModel;
        }
        
        public async Task<getAllServiceInterfacesResp> getAllServiceInterfaces()
        {
            GetServiceInterfaceValues obj = new GetServiceInterfaceValues();
            List<serviceDescr> interfaceValues = new List<serviceDescr>();
            if (_discoveryModel.CardlessInterface)
            {
                interfaceValues.Add(obj.GetiCardlessValue());
            }
            if (_discoveryModel.EftInterface)
            {
                interfaceValues.Add(obj.GetiEftValue());
            }            
            if (_discoveryModel.PlayerInterface)
            {
                interfaceValues.Add(obj.GetiPlayerValue());
            }
            if (_discoveryModel.BonusInterface)
            {
                interfaceValues.Add(obj.GetiBonusValue());
            }
            if (_discoveryModel.DiagsInterface)
            {
                interfaceValues.Add(obj.GetiDiagsValue());
            }
            if (_discoveryModel.GatInterface)
            {
                interfaceValues.Add(obj.GetiGatValue());
            }
            if (_discoveryModel.ConfInterface)
            {
                interfaceValues.Add(obj.GetConfValue());
            }
            if (_discoveryModel.AMLInterface)
            {
                interfaceValues.Add(obj.GetAMLValue());
            }
            if (_discoveryModel.DownloadInterface)
            {
                interfaceValues.Add(obj.GetDownloadValue());
            }
            if (_discoveryModel.TitoInterface)
            {
                interfaceValues.Add(obj.GetiTitoValue());
            }

            if (_discoveryModel.PinInterface)
            {
                interfaceValues.Add(obj.GetiPinValue());
            }

            if (_discoveryModel.MarkerInterface)
            {
                interfaceValues.Add(obj.GetiMarkerValue());
            }

            if (_discoveryModel.RGInterface)
            {
                interfaceValues.Add(obj.GetiRGValue());
            }

            if (_discoveryModel.RegInterface)
            {
                interfaceValues.Add(obj.GetiRegValue());
            }
            if (_discoveryModel.HandpayInterface)
            {
                interfaceValues.Add(obj.GetiHandpayValue());
            }
            if (_discoveryModel.PCSInterface)
            {
                interfaceValues.Add(obj.GetPCSValue());
            }

            if (_discoveryModel.IsmInterface)
            {
                interfaceValues.Add(obj.GetiISMValue());
            }

            if (_discoveryModel.WatInterface)
            {
                interfaceValues.Add(obj.GetiWatValue());
            }

            var req = new Dictionary<string, object>
            {
                
            };
            var resp = new getAllServiceInterfacesResp()
            {
                interfaces = interfaceValues,
            };
            _responseViewModel.LogRpc(Constants.GetAllServiceInterface, req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }
        public async Task<getServiceInterfacesResp> getServiceInterfaces(List<string> interfaces)
        {
            string InterfaceName = "iCardless,iEft,iBonus,iDiags,iGat,iTito,iPin,iMarker,iRG,iReg, iAML,iConfig,iPlayer,iHandpay,iPCS,iISM";
            List<string> interfaceList = InterfaceName.Split(',').ToList();

            GetServiceInterfaceValues obj = new GetServiceInterfaceValues();
            List<serviceDescr> interfaceValues = new List<serviceDescr>();

            var result = interfaces.Except(interfaceList).ToList();

            if (interfaces.Contains("iCardless"))
            {
                if (_discoveryModel.CardlessInterface)
                {
                    interfaceValues.Add(obj.GetiCardlessValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iCardless"));
                }
            }
            if (interfaces.Contains("iEft"))
            {
                if (_discoveryModel.EftInterface)
                {
                    interfaceValues.Add(obj.GetiEftValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iEft"));
                }
            }
            if (interfaces.Contains("iPlayer"))
            {
                if (_discoveryModel.PlayerInterface)
                {
                    interfaceValues.Add(obj.GetiPlayerValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iPlayer"));
                }
            }

            if (interfaces.Contains("iBonus"))
            {
                if (_discoveryModel.BonusInterface)
                {
                    interfaceValues.Add(obj.GetiBonusValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iBonus"));
                }
            }

            if (interfaces.Contains("iDiags"))
            {
                if (_discoveryModel.DiagsInterface)
                {
                    interfaceValues.Add(obj.GetiDiagsValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iDiags"));
                }
            }

            if (interfaces.Contains("iGat"))
            {
                if (_discoveryModel.GatInterface)
                {
                    interfaceValues.Add(obj.GetiGatValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iGat"));
                }
            }

            if (interfaces.Contains("iConfig"))
            {
                if (_discoveryModel.ConfInterface)
                {
                    interfaceValues.Add(obj.GetConfValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iConfig"));
                }
            }

            if (interfaces.Contains("iDownload"))
            {
                if (_discoveryModel.DownloadInterface)
                {
                    interfaceValues.Add(obj.GetConfValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iDownload"));
                }
            }

            if (interfaces.Contains("iAML"))
            {
                if (_discoveryModel.AMLInterface)
                {
                    interfaceValues.Add(obj.GetAMLValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iAML"));
                }
            }
            if (interfaces.Contains("iTito"))
            {
                if (_discoveryModel.TitoInterface)
                {
                    interfaceValues.Add(obj.GetiTitoValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iTito"));
                }
            }

            if (interfaces.Contains("iPin"))
            {
                if (_discoveryModel.PinInterface)
                {
                    interfaceValues.Add(obj.GetiPinValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iPin"));
                }
            }

            if (interfaces.Contains("iMarker"))
            {
                if (_discoveryModel.MarkerInterface)
                {
                    interfaceValues.Add(obj.GetiMarkerValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iMarker"));
                }
            }

            if (interfaces.Contains("iRG"))
            {
                if (_discoveryModel.RGInterface)
                {
                    interfaceValues.Add(obj.GetiRGValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iRG"));
                }
            }

            if (interfaces.Contains("iReg"))
            {
                if (_discoveryModel.RegInterface)
                {
                    interfaceValues.Add(obj.GetiRegValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iReg"));
                }
            }
            if (interfaces.Contains("iHandpay"))
            {
                if (_discoveryModel.HandpayInterface)
                {
                    interfaceValues.Add(obj.GetiHandpayValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iHandpay"));
                }
            }

            if (interfaces.Contains("iISM"))
            {
                if (_discoveryModel.IsmInterface)
                {
                    interfaceValues.Add(obj.GetiISMValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iISM"));
                }
            }

            if (interfaces.Contains("iWat"))
            {
                if(_discoveryModel.IsmInterface)
                {
                    interfaceValues.Add(obj.GetiWatValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iWat"));
                }
            }

            if (interfaces.Count == 0 || result.Count != 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    if (result.Count != 0)
                    {
                        interfaceValues.Add(obj.GetNoServiceValue(result[i]));
                    }                    
                }
                if (interfaces.Count == 0)
                {
                    interfaceValues.Add(obj.GetNoServiceValue(""));
                }
            }
            if (interfaces.Contains("iPCS"))
            {
                if (_discoveryModel.PCSInterface)
                {
                    interfaceValues.Add(obj.GetPCSValue());
                }
                else
                {
                    interfaceValues.Add(obj.GetNoServiceValue("iPCS"));
                }
            }

            var req = new Dictionary<string, object>
            {
                {nameof(interfaces), interfaces}    
            };
            var resp = new getServiceInterfacesResp()
            {
                interfaces = interfaceValues,
            };           

            _responseViewModel.LogRpc("getServiceInterfaces", req, resp, RpcCallContext.Current);
            return await Task.FromResult(resp);
        }
        public Task<getSmibInterfacesResp> getSmibInterfaces(string uid)
        {
            throw new NotImplementedException();
        }        
    }  
}