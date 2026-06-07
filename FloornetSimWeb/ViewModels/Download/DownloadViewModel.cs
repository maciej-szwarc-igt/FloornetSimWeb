using IGT.FloorNet.EX.Download;
using IGT.FloorNet.MessageBus.Rpc;
using IGT.FloorNet.SecurityUtils.GatUtilities;
using IGT.FloorNet.Tools.ServiceSimulator.Models.Download;
using IGT.FloorNet.Tools.ServiceSimulator.Utils;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace IGT.FloorNet.Tools.ServiceSimulator.ViewModels.Download
{
    public class DownloadViewModel : INotifyPropertyChanged
    {
        private const int Mbe2DownloadPacketSize = 128;

        private iDownload _downloadProxy { get; set; }
        private FileGroupsContainer _fileGroupContainer { get; set; }
        private ResponseViewModel _responseViewModel;
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<string> _fileGroups { get; set; }
        private int groupIndex = 0;

        private string jsonfilePath;
        private string _downloadServiceIp;
        private string _user;
        private string _password;
        private IConfiguration _configuration;
        private List<string> _groups { get; set; } = new();
        public ObservableCollection<string> FileGroups { get { return _fileGroups; }
            set
            {
                _fileGroups = value;
                OnPropertyChanged(nameof(FileGroups));
            }
        }

        private string _smibText { get; set; }
        public string SMIBText
        {
            get { return _smibText; }
            set
            {
                _smibText = value;
                OnPropertyChanged(nameof(SMIBText));
            }
        }
        private string _delayBetweenFiles = "1000";
        public string DelayBetweenFiles
        {
            get { return _delayBetweenFiles; }
          
            set
            {
                
                _delayBetweenFiles = value;
                OnPropertyChanged(nameof(DelayBetweenFiles));
            }
        }
        private string _selectedFile { get; set; }
        public string SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                OnPropertyChanged(nameof(SelectedFile));
            }
        }

        private string _selectedSMIB { get; set; }
        public string SelectedSMIB
        {
            get { return _selectedSMIB; }
            set
            {
                _selectedSMIB = value;
                OnPropertyChanged(nameof(SelectedSMIB));
            }
        }

        private int _delaySecs;
        public int DelaySecs
        {
            get => _delaySecs;
            set
            {
                _delaySecs = value;
                OnPropertyChanged(nameof(DelaySecs));

            }
        }

        private ObservableCollection<string> _smibs { get; set; }
        public ObservableCollection<string> SMIBs
        {
            get { return _smibs; }
            set
            {
                _smibs = value;
                OnPropertyChanged(nameof(SMIBs));
            }
        }
        private ObservableCollection<string> _filesInGroup { get; set; }
        public ObservableCollection<string> FilesInGroup { get { return _filesInGroup; }
            set
            {
                _filesInGroup = value;
                OnPropertyChanged(nameof(FilesInGroup));
            }
        }
        private string _selectedItem { get; set; }
        public string SelectedItem { get { return _selectedItem; }
            set
            {
                _selectedItem = value;

                FilesInGroup = new();
                SMIBs = new();
                FileGroupContainer fGroup = _fileGroupContainer.Groups.Find(p => p.fileGroup == SelectedItem);

                if (fGroup != null)
                {
                    foreach (string SMIB in fGroup.SMIBs)
                    {
                       SMIBs.Add(SMIB);
                    }
                    foreach (string file in fGroup.Files)
                    {
                        FilesInGroup.Add(file);
                    }
                }
                OnPropertyChanged(nameof(SelectedItem));
            }
        }


        public DownloadViewModel(IConfiguration config, ResponseViewModel responseViewModel, iDownload downloadProxy)
        {
            _configuration = config;
            _responseViewModel = responseViewModel;
            _downloadProxy = downloadProxy;
            _downloadServiceIp = (string)_configuration["DownloadService:IpAddress"];
            _user = (string)_configuration["DownloadService:User"];
            _password = (string)_configuration["DownloadService:Password"];
            if (!_downloadServiceIp.EndsWith("/"))
                _downloadServiceIp += "/";

            DelayBetweenFiles = _delayBetweenFiles;
            FileGroups = new ObservableCollection<string>();
            SMIBs = new ObservableCollection<string>();
            FilesInGroup = new ObservableCollection<string>();
            _fileGroupContainer = new FileGroupsContainer();
            jsonfilePath = Directory.GetCurrentDirectory() + "\\fileGroups.json";

            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\fileserver"))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\fileserver");
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\fileserver\\config"))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\fileserver\\config");

            LoadFilesFromJson();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void DeleteFileFromGroup()
        {
            if (string.IsNullOrEmpty(SelectedFile) || string.IsNullOrEmpty(SelectedItem))
            {
                _responseViewModel.Log("Please select a file to delete.");
                return;
            }

            try
            {
                FileGroupContainer foundGroup = _fileGroupContainer.Groups.Find(group => group.fileGroup == SelectedItem);
                if (foundGroup != null && foundGroup.Files.Contains(SelectedFile))
                {
                    string filePath = $"{foundGroup.location}\\{SelectedFile}";
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    if(SelectedItem == "Initial")
                    {
                        string FileInConfigForSMIB =  $"{Directory.GetCurrentDirectory()}\\fileserver\\config\\{SelectedFile}";

                        if (File.Exists(FileInConfigForSMIB))
                            File.Delete(FileInConfigForSMIB);
                    }
                    foundGroup.Files.Remove(SelectedFile);
                    FilesInGroup.Remove(SelectedFile);
                    SaveFilesToJson();
                }
            }
            catch (Exception ex)
            {
                _responseViewModel.Log($"Error occurred while deleting file: {ex}");
            }
        }
        public void DeleteSMIB()
        {
            if (string.IsNullOrEmpty(SelectedItem) || string.IsNullOrEmpty(SelectedSMIB))
            {
                _responseViewModel.Log($"Please Select a SMIB to delete");
                return;
            }

            try
            {
                string smibToDelete = SelectedSMIB;
                FileGroupContainer foundGroup = _fileGroupContainer.Groups.Find(group => group.fileGroup == SelectedItem);
                if (foundGroup != null && foundGroup.SMIBs.Contains(smibToDelete))
                {
                    foundGroup.SMIBs.Remove(smibToDelete);
                   SMIBs.Remove(smibToDelete);
                   SaveFilesToJson();
                   _responseViewModel.Log($"SMIB '{smibToDelete}' deleted Successfully ");
                }
                else
                {
                    _responseViewModel.Log("Selected SMIB not found in the group ");
                }
            }

            catch (Exception ex)
            {
                _responseViewModel.Log($"Error occurred while deleting file: {ex}");
            }

        }
        public void AddSMIB()
        {
            if (string.IsNullOrEmpty(SMIBText))
            {
                return;
            }

            if (!SMIBs.Contains(SMIBText))
            {
                FileGroupContainer foundGroup = _fileGroupContainer.Groups.Find(group => group.fileGroup == SelectedItem);
                if(foundGroup != null)
                {
                    FileGroupContainer prevGroup = _fileGroupContainer.Groups.Find(gr => gr.SMIBs.Contains(SMIBText) && gr.GroupType == foundGroup.GroupType);
                    prevGroup?.SMIBs.Remove(SMIBText);
                }
                foundGroup?.SMIBs.Add(SMIBText);
                SMIBs.Add(SMIBText);
            }

            SMIBText = string.Empty;
           
            SaveFilesToJson();
        } 

        public void AddFileGroup(GroupType groupToAdd)
        {
            string fileGroupName = "";
            switch (groupToAdd)
            {
                case GroupType.PKG:
                    groupIndex++;
                    fileGroupName = $"PKG_{groupIndex}";
                    break;
                case GroupType.SFL:
                    groupIndex++;
                    fileGroupName = $"SFL_{groupIndex}";
                    break;
                case GroupType.PKGS:
                    groupIndex++;
                    fileGroupName = $"PKGS_{groupIndex}";
                    break;
               
            }
            string folderName = $"{Directory.GetCurrentDirectory()}\\Download\\{fileGroupName}";
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            _groups.Add(fileGroupName);
            FileGroups = new ObservableCollection<string>(_groups.OrderBy(s => s));
            _fileGroupContainer.Groups.Add(new FileGroupContainer(fileGroupName, folderName, groupToAdd));
            SelectedItem = fileGroupName;
            SaveFilesToJson();
        }

        public void AddFileToGroup(string[] fileNames)
        {
            if(string.IsNullOrEmpty(SelectedItem))
            {
                _responseViewModel.Log("Kindly select the file!");
                return;
            }

            foreach (string file in fileNames)
            {
                string fileName = System.IO.Path.GetFileName(file);
                string folderName = $"{Directory.GetCurrentDirectory()}\\Download\\{SelectedItem}";
                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);
                string destination = System.IO.Path.Combine(folderName, fileName);

                try
                {
                    FileGroupContainer foundGroup = _fileGroupContainer.Groups.Find(group => group.fileGroup == SelectedItem);
                    if(foundGroup != null)
                    {
                        switch (foundGroup.GroupType)
                        {
                            case GroupType.PKGS:
                                File.Copy(file, destination, true);
                                break;
                            case GroupType.PKG:                     
                                if (file.ToLower().EndsWith(".abs") && foundGroup.Files.Find(p => p.ToLower().EndsWith(".abs")) != null)
                                {
                                    File.Delete($"{foundGroup.location}\\{foundGroup.Files.Find(p => p.ToLower().EndsWith(".abs"))}");
                                    foundGroup.Files.RemoveAll(p => p.ToLower().EndsWith(".abs"));
                                    FilesInGroup.Add(foundGroup.Files.Find(p => p.ToLower().EndsWith(".sig")));
                                    FilesInGroup.Add(foundGroup.Files.Find(p => p.ToLower().EndsWith(".man")));
                                }

                                if (file.ToLower().EndsWith(".sig") && foundGroup.Files.Find(p => p.ToLower().EndsWith(".sig")) != null)
                                {
                                    File.Delete($"{foundGroup.location}\\{foundGroup.Files.Find(p => p.ToLower().EndsWith(".sig"))}");
                                    foundGroup.Files.RemoveAll(p => p.ToLower().EndsWith(".sig"));
                                    FilesInGroup.Add(foundGroup.Files.Find(p => p.ToLower().EndsWith(".abs")));
                                    FilesInGroup.Add(foundGroup.Files.Find(p => p.ToLower().EndsWith(".man")));
                                } 

                                if (file.ToLower().EndsWith(".man") && foundGroup.Files.Find(p => p.ToLower().EndsWith(".man")) != null)
                                {
                                    File.Delete($"{foundGroup.location}\\{foundGroup.Files.Find(p => p.ToLower().EndsWith(".man"))}");
                                    foundGroup.Files.RemoveAll(p => p.ToLower().EndsWith(".man"));
                                    FilesInGroup.Add(foundGroup.Files.Find(p => p.ToLower().EndsWith(".sig")));
                                    FilesInGroup.Add(foundGroup.Files.Find(p => p.ToLower().EndsWith(".abs")));
                                } 

                                if (file.ToLower().EndsWith(".abs") || file.ToLower().EndsWith(".sig"))
                                {
                                    File.Copy(file, destination, true);
                                }
                                break;
                            case GroupType.SFL:
                                if(file.EndsWith(".mot") || file.EndsWith(".abs"))
                                {
                                    _responseViewModel.Log($"Could not add this because it is not supported by SFL groups");
                                    return;
                                }

                                File.Copy(file, destination, true);
                                break;
                        }

                    }

                    if(SelectedItem == "Initial")
                    {
                        string FileInConfigForSMIB =  $"{Directory.GetCurrentDirectory()}\\fileserver\\config\\{fileName}";
                        File.Copy(file, FileInConfigForSMIB, true);
                    }

                    FilesInGroup.Add(fileName);
                    foundGroup?.Files.Add(fileName);

                }
                catch (Exception ex)
                {
                    _responseViewModel.Log($"Error occurred:{ex}");
                } 
            }
            SaveFilesToJson();
        }

        public async Task DownloadAll()
        {
            if (SelectedItem.StartsWith("PKGS"))
                AddPackages();
            else
                AddPackage();
        }
        public async Task AddPackage()
        {

            if (FilesInGroup == null || FilesInGroup.Count == 0)
            {
                _responseViewModel.Log("No files available to download.");
                return;
            }

            if (SMIBs == null || SMIBs.Count == 0)
            {
                _responseViewModel.Log("No SMIBs available.");
                return;
            }
            try
            {

                List<Task> smibTasks = new List<Task>();
                foreach (string smib in SMIBs)
                {

                    smibTasks.Add(Task.Run(() => SendFiles(smib)));
                }

                await Task.WhenAll(smibTasks);
            }

            catch (Exception ex)
            {
                _responseViewModel.Log($"Error occurred during download: {ex}");
            }
        }
    
        public async Task AddPackages()
        {
            if (FilesInGroup == null || FilesInGroup.Count == 0)
            {
                _responseViewModel.Log("No files available to download.");
                return;
            }

            if (SMIBs == null || SMIBs.Count == 0)
            {
                _responseViewModel.Log("No SMIBs available.");
                return;
            }
            
            try
            {
                //Build the list of PackageDesc objects from FilesInGroup
                List<PackageDesc> packages = new List<PackageDesc>();
                string basepath = $"{Directory.GetCurrentDirectory()}\\Download\\{SelectedItem}";

                foreach (var file in FilesInGroup)
                {
                    //Don't ask to download sig files
                    if (file.ToLower().EndsWith(".sig") || file.ToLower().EndsWith(".man") || file.ToLower().EndsWith(".ctf") || file.ToLower().EndsWith(".mot"))
                        continue;

                    string filePath = Path.Combine(basepath, file);
                    if (!File.Exists(filePath)) continue;

                    long filesize = new FileInfo(filePath).Length;
                    var fileBytes = File.ReadAllBytes(filePath).ToList();
                    if (file.ToLower().StartsWith("mbe2"))
                    {
                        fileBytes.AddRange(Enumerable.Repeat((byte)0x00, (int)(Mbe2DownloadPacketSize - filesize % Mbe2DownloadPacketSize)));
                    }
                    string crc = new Crc16().CalculateCrc(fileBytes.ToArray()).ToString("X4");

                    packages.Add(new PackageDesc
                    {
                        uri = $"{_downloadServiceIp}Download/{SelectedItem}/{file}",
                        name = file,
                        size = filesize,
                        crc = crc
                    });
                }

                string requestId = SelectedItem.Substring(SelectedItem.IndexOf("_")+1);
                

                // Send the packages to each SMIB
                List<Task> tasks = new List<Task>();

                foreach (string smib in SMIBs)
                {

                    RpcProxyContext.Current = RpcProxyContext.ToSMIB(smib);
                    tasks.Add(_downloadProxy.addPackages(_user, _password, requestId, packages, DelaySecs));
                }

                await Task.WhenAll(tasks);
                _responseViewModel.Log("Packages sent to all SMIBs.");
            }

            catch (Exception ex)
            {
                _responseViewModel.Log($"Error occurred during AddPackages: {ex}");
            }
        }
       
        private void SendFiles(string smib)
        {
            int delay;

            if (!int.TryParse(DelayBetweenFiles, out delay))
                delay = 1000;

            try
            {
                string basepath = $"{Directory.GetCurrentDirectory()}\\Download\\{SelectedItem}";

                foreach (string file in FilesInGroup)
                {
                    //Don't ask to download sig files
                    if (file.ToLower().EndsWith(".sig") || file.ToLower().EndsWith(".man") || file.ToLower().EndsWith(".ctf") || file.ToLower().EndsWith(".mot"))
                        continue;
                    string filePath = Path.Combine(basepath, file);
                    string fileName = Path.GetFileName(filePath);
                    long filesize = new FileInfo(filePath).Length;
                    RpcProxyContext.Current = RpcProxyContext.ToSMIB(smib);
                    var fileBytes = File.ReadAllBytes(filePath).ToList();
                    if (file.ToLower().StartsWith("mbe2"))
                    {
                        fileBytes.AddRange(Enumerable.Repeat((byte)0x00, (int)(Mbe2DownloadPacketSize - filesize % Mbe2DownloadPacketSize)));
                    }
                    string requestId = SelectedItem.Substring(SelectedItem.IndexOf("_")+1);
                    _downloadProxy.addPackage($"{_downloadServiceIp}Download/{SelectedItem}/{fileName}", _user, _password, requestId, fileName, filesize, new Crc16().CalculateCrc(fileBytes.ToArray()).ToString("X"));

                    Thread.Sleep(delay);
                }
            }
            catch (Exception ex)
            {
                _responseViewModel.Log($"Error Occurred during download: {ex}");
            }
        }
      

        public void SaveFilesToJson()
        {
            try
            {
                string directory = System.IO.Path.GetDirectoryName(jsonfilePath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                _fileGroupContainer.LastSelectedItem = SelectedItem;
                string json = JsonConvert.SerializeObject(_fileGroupContainer);

                File.WriteAllText(jsonfilePath, json);
            }
            catch(Exception ex)
            {
                _responseViewModel.Log($"Error occurred:{ex}");
            }
        }
        private void LoadFilesFromJson()
        {
            try
            {
                jsonfilePath = Directory.GetCurrentDirectory() + "\\fileGroups.json";
                if (File.Exists(jsonfilePath))
                {
                    string json = File.ReadAllText(jsonfilePath);

                    _fileGroupContainer = JsonConvert.DeserializeObject<FileGroupsContainer>(json);

                    if (_fileGroupContainer != null && _fileGroupContainer.Groups != null && _fileGroupContainer.Groups.Count >= 1)
                    {
                        foreach (FileGroupContainer group in _fileGroupContainer.Groups.OrderBy(g => g.fileGroup))
                        {
                            if (!_groups.Contains(group.fileGroup))
                                _groups.Add(group.fileGroup);
                            if (!FileGroups.Contains(group.fileGroup))
                                FileGroups.Add(group.fileGroup);

                            switch (group.GroupType)
                            {
                                case GroupType.PKG:
                                    groupIndex++;
                                    break;
                                case GroupType.SFL:
                                    groupIndex++;
                                    break;
                                case GroupType.PKGS:
                                    groupIndex++;
                                    break;
                                
                            }
                        }
                        if (!string.IsNullOrEmpty(_fileGroupContainer.LastSelectedItem) && _groups.Contains(_fileGroupContainer.LastSelectedItem))
                        {
                            SelectedItem = _fileGroupContainer.LastSelectedItem;
                        }
                        else if (_groups.Count > 0)
                        {
                            SelectedItem = _groups[0];
                        }
                    }

                }
                else
                {
                    string folderName = $"{Directory.GetCurrentDirectory()}\\Download\\Initial";
                    _groups.Add("Initial");
                    FileGroups.Add("Initial");
                    _fileGroupContainer.Groups.Add(new FileGroupContainer("Initial", folderName, GroupType.Initial));
                    SelectedItem = _groups[0];
                } 
            }
            catch (Exception ex)
            {
                _responseViewModel.Log($"Error occured:{ex}");
            }
        }

      

        private async void RemoveFiles(string smib)
        {
            int delay;

            if (!int.TryParse(DelayBetweenFiles, out delay))
                delay = 1000;

            try
            {
                string basepath = $"{Directory.GetCurrentDirectory()}\\Download\\{SelectedItem}";

                bool HasABS = FilesInGroup.Where(p => p.EndsWith(".abs")) != null;
                foreach (string file in FilesInGroup)
                {
                    //Don't ask to delete sig or man files if the ABS file is present
                    if ((file.ToLower().EndsWith(".sig") || file.ToLower().EndsWith(".man")) && HasABS)
                        continue;
                    string filePath = Path.Combine(basepath, file);
                    string fileName = Path.GetFileName(filePath);
                    RpcProxyContext.Current = RpcProxyContext.ToSMIB(smib);
                    var resp = await _downloadProxy.removePackage(fileName);
                    _responseViewModel.Log(JsonConvert.SerializeObject(resp));

                    Thread.Sleep(delay);
                }
            }
            catch (Exception ex)
            {
                _responseViewModel.Log($"Error Occurred during download: {ex}");
            }
        }
    }
}

