using System;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Auth;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using System.Collections.Generic;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Auth
{
    public class IKeysViewModel
    {

        private readonly ResponseViewModel _responseViewModel;
        private static Dictionary<string, string> _smibPublicKeys = new Dictionary<string, string>();
        public RelayCommand GenerateKeysCommand { get; }

        public IKeysViewModel(ResponseViewModel responseViewModel)
        {
            _responseViewModel = responseViewModel;
            GenerateKeysCommand = new RelayCommand(Generate);
            GenerateKeys(keysModelBonus, true);
            GenerateKeys(keysModelEft, true);
            GenerateKeys(keysModelCard, true);
            GenerateKeys(keysModelISM, true);
            GenerateKeys(keysModelHandpay, true);
            GenerateKeys(keysModelWat, true);

        }
        public static IKeysModel keysModelEft{ get; set; } = new IKeysModel();
        public static IKeysModel keysModelBonus{ get; set; } = new IKeysModel();
        public static IKeysModel keysModelCard { get; set; } = new IKeysModel();
        public static IKeysModel keysModelISM { get; set; } = new IKeysModel();
        public static IKeysModel keysModelHandpay { get; set; } = new IKeysModel();
        public static IKeysModel keysModelWat { get; set; } = new IKeysModel();

        public void Generate(object obj)
        {
            var model = GetModel((string)obj);
            if (model != null)
            {
                GenerateKeys(model);
            }
            else
            {
                _responseViewModel.Log("There is no service selected to Generate Keys");
            } 
                
        }
        public void GenerateKeys(IKeysModel model, bool start = false)
        {
            if (start)
            {
                model.CurrentKeyNum = new Random().Next();
            }
            else
            {
                model.CurrentKeyNum++;
            }
            model.ECDsa = model.FloornetECDsaProvider.CreateECDsa(null, null);
            model.PublicKey = GetPublicKey(model.ECDsa);
            model.PrivateKey = GetPrivateKey(model.ECDsa);
        }

        public IKeysModel GetModel(string model)
        {
            switch(model)
            {
                case "eft":
                    return keysModelEft;
                case "bonus":
                    return keysModelBonus;
                case "card":
                    return keysModelCard;
                case "ISM":
                    return keysModelISM;
                case "handpay":
                    return keysModelHandpay;
                case "wat":
                    return keysModelWat;
                default: return null;
                    
            }
        }
        public static void SetSmibKey(string smib_uid, string publicKey)
        {
            _smibPublicKeys[smib_uid] = publicKey;
        }

        public static string GetSmibKey(string smib_uid)
        {
            _smibPublicKeys.TryGetValue(smib_uid, out var smibKey);
            return smibKey;
        }

        private string GetPublicKey(ECDsa ecdsa)
        {
            if (ecdsa == null)
                throw new ArgumentNullException(nameof(ecdsa));

            var keyParams = ecdsa.ExportParameters(false);
            byte[] keyBytes = new byte[64];
            Buffer.BlockCopy(keyParams.Q.X, 0, keyBytes, 0, 32);
            Buffer.BlockCopy(keyParams.Q.Y, 0, keyBytes, 32, 32);
            return Convert.ToBase64String(keyBytes);
        }

        private string? GetPrivateKey(ECDsa ecdsa)
        {
            if (ecdsa == null)
                throw new ArgumentNullException(nameof(ecdsa));

            try
            {
                var keyParams = ecdsa.ExportParameters(true);
                return Convert.ToBase64String(keyParams.D);
            }
            catch (Exception)
            {
                _responseViewModel.Log("Failed to get private key from ECDSA");
                return null;
            }
        }
    }
}
