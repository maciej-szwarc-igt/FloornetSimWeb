//<copyright file = "ConfViewModel.cs" company = "IGT">
//Copyright(c) 2024 IGT.All rights reserved.
//</ copyright>
using IGT.FloorNet.Tools.ServiceSimulator.Models.IConf;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using IGT.FloorNet.EX.OptionConfig;
using Newtonsoft.Json;


namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.IConf
{
    
    public class ConfViewModel : INotifyPropertyChanged
    {
        private readonly iConfig _iConfig;
        public event EventHandler<ConfSearch> IConfSearch;
        public event PropertyChangedEventHandler PropertyChanged;
        
        private string selectedCategory;
        private bool ToggleSendValue = false;

        public Dictionary<string, Dictionary<string, CategoryOptionInfo>> SmibConfigurations = new Dictionary<string, Dictionary<string, CategoryOptionInfo>>();
        public string _smib { get; set; }
        public string SMIBNumber
        {
            get { return _smib; }
            set
            {
                _smib = value;
                SmibConfigurations.Clear();
                selectedCategory = "";
                InitializeSupportedConfigurationsData();
            }
        }

        public ObservableCollection<string> _categories { get; set; }
        public ObservableCollection<string> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }
        public ObservableCollection<string> Name { get; }

        private CategoryOptionInfo _selectedItem;
        public CategoryOptionInfo SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        private string _responseMessage;
        public string ResponseMessage
        {
            get { return _responseMessage; }
            set
            {
                _responseMessage = value;
                OnPropertyChanged(nameof(ResponseMessage));
            }
        }

        private ObservableCollection<CategoryOptionInfo> _supportedConfigurationsData;
        public ObservableCollection<CategoryOptionInfo> SupportedConfigurationsData
        {
            get { return _supportedConfigurationsData; }
            set
            {
                _supportedConfigurationsData = value;
                OnPropertyChanged(nameof(SupportedConfigurationsData));
            }
        }


        public ConfViewModel(iConfig iConfigProxy)
        {
            _iConfig = iConfigProxy;
        }

      

        private void InitializeSupportedConfigurationsData()
        {
            _categories = new ObservableCollection<string>();
            var results = new ObservableCollection<CategoryOptionInfo>();

            _categories.Add("All");
            foreach (var category in SmibConfigurations)
            {
                _categories.Add(category.Key);
                foreach (var option in category.Value)
                {
                    results.Add(new CategoryOptionInfo(){
                       MessageCategory = category.Key,
                       setBy = option.Value.setBy,
                       ItemDetail = option.Value.ItemDetail
                    });
                }
            }
            SupportedConfigurationsData = results;
            Categories = _categories;
        }
        

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void PerformSearch(object sender, ConfSearch e)
        {
            selectedCategory = e.Category;

            if (string.IsNullOrWhiteSpace(selectedCategory))
            {
                // MessageBox removed (no WPF)
                return;
            }

            if(selectedCategory == "All")
            {
                InitializeSupportedConfigurationsData();
                return; 
            }

            IEnumerable<CategoryOptionInfo> results = GetFilteredOptions(selectedCategory);

            if (results == null || !results.Any())
            {
                // MessageBox removed (no WPF)
                return;
            }

            SupportedConfigurationsData.Clear();
            foreach (var result in results)
            {
                result.MessageCategory = selectedCategory;
                SupportedConfigurationsData.Add(result);
            }
        }

        public void DisplayMessage(string message)
        {
            ResponseMessage = message;
        }

        internal void SetupConfigurations(getOptionListResp optionsList)
        {
            if (optionsList == null || optionsList.optionGroups == null || optionsList.optionGroups.Count == 0 )
                return;

            foreach (msgCategoryOptions optionListResp in optionsList.optionGroups.OrderBy(p => p.messageCategory))
            {
                Dictionary<string, CategoryOptionInfo> dictionary;
                if (!SmibConfigurations.TryGetValue(optionListResp.messageCategory, out dictionary))
                    dictionary = new();

                foreach (t_optionItemDetail opt in optionListResp.optionItems)
                {
                    CategoryOptionInfo optionInfo;
                    if (dictionary.TryGetValue(opt.name, out optionInfo))
                    {
                        if (opt.value != null)
                        {
                            if (opt.value.ToString().StartsWith("{") && opt.value.ToString().EndsWith("}"))
                            {
                                try
                                {
                                    MessageDB myMessageDB = JsonConvert.DeserializeObject<MessageDB>(opt.value.ToString());

                                    opt.value = opt.value.ToString();
                                }
                                catch (Exception e)
                                {
                                }
                            }
                        }

                        optionInfo.ItemDetail.value = opt.value;
                        optionInfo.ItemDetail.Send = !opt.readOnly;
                    }
                    else
                    {
                        bool isMsgDB = false;
                        if(opt.value != null)
                        {
                            if(opt.value.ToString().StartsWith("{") && opt.value.ToString().EndsWith("}")) 
                            {
                                try 
                                {
                                    MessageDB myMessageDB = JsonConvert.DeserializeObject<MessageDB>(opt.value.ToString());
                                    isMsgDB = true;

                                    opt.value = opt.value.ToString();
                                } catch (Exception e) 
                                {
                                }
                            }
                        }
                        optionInfo = new CategoryOptionInfo()
                        {
                            MessageCategory = optionListResp.messageCategory,
                            ConfigSeq = optionListResp.configSeq,
                            setBy = optionListResp.setby,
                            ItemDetail = new OptionItemDetail()
                            {
                                name = opt.name,
                                value = opt.value,
                                Description = opt.description,
                                ReadOnly = opt.readOnly,
                                Send = !opt.readOnly
                            }
                        };
                        if (isMsgDB)
                            optionInfo.dataType = SupportedDatatypes.MessageDB;

                        dictionary.TryAdd(opt.name, optionInfo);
                    }
                }
                SmibConfigurations.TryAdd(optionListResp.messageCategory, dictionary);
            }
            InitializeSupportedConfigurationsData();
        }

        internal IEnumerable<CategoryOptionInfo> GetFilteredOptions(string category)
        {
            return SmibConfigurations.GetValueOrDefault(category)?.Values;
        }

        internal void ToggleSend()
        {
            ToggleSendValue = !ToggleSendValue;

            foreach(var option in SmibConfigurations.Values)
            {
                foreach(var item in option.Values)
                {
                    item.ItemDetail.Send = ToggleSendValue;
                }
            }

            if (string.IsNullOrEmpty(selectedCategory))
                InitializeSupportedConfigurationsData();
            else
                PerformSearch(this, new ConfSearch(selectedCategory));
        }
    }
}
