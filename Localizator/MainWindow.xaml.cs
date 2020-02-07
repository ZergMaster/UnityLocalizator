using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using MessageBox = System.Windows.MessageBox;

namespace Localizator
{
    public partial class MainWindow
    {
        private AddDialogWindow _addDialogWindow;

        private string _savedDir="";
        private Dictionary<string, Dictionary<string, string>> _locData;

        private readonly Configuration _config = ConfigurationManager.OpenExeConfiguration(
            System.Reflection.Assembly.GetExecutingAssembly().Location);

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (_config.AppSettings.Settings["SavedDir"] != null)
            {
                _savedDir = _config.AppSettings.Settings["SavedDir"].Value;
                PathText.Text = _savedDir;
            }

            PathButton.Click += PathButtonHandler;
            AddButton.Click += AddButtonHandler;

            InitData();
        }

        private void InitData()
        {
            try
            {
                GetJson();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    "Локализация ru.json по адресу " + _savedDir +
                    "\\Assets\\StreamingAssets\\Localization\\ru.json не была обнаружена. Проверьте путь к root проекта." +
                    "\n \n Нажмите на кнопочку Path и введите корректный путь до проекта.",
                    "Некорретный путь к проекту",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            MakeList();
        }

        private void GetJson()
        {
            if (_savedDir != null)
            {
                var jsonData = File.ReadAllText(_savedDir + "/Assets/StreamingAssets/Localization/ru.json");
                _locData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);
            }
        }

        private void MakeList()
        {
            if (_locData == null)
            {
                return;
            }

            var texts = _locData["texts"];
            
            var itemSource = new object[texts.Count];
            int i = texts.Count-1;
            foreach (var key in texts.Keys)
            {
                itemSource[i] = new {Key = key, Value = texts[key]};
                i--;
            }

            ListBoxMain.ItemsSource = itemSource;
        }

        private void AddButtonHandler(object sender, RoutedEventArgs e)
        {
            _addDialogWindow = new AddDialogWindow {Owner = this};
            _addDialogWindow.Closed += AddDialogHandler;
            _addDialogWindow.ShowDialog();
        }

        private void AddDialogHandler(object sender, EventArgs e)
        {
            if ((_addDialogWindow.Key == "")||(_addDialogWindow.Key == null))
            {
                MessageBox.Show("Поле Ключ не должено быть пустым", "", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            
            if ((_addDialogWindow.Value == "")||(_addDialogWindow.Value == null))
            {
                MessageBox.Show("Поле Значение не может быть пустым", "", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            _locData["texts"].Add(_addDialogWindow.Key, _addDialogWindow.Value);

            SaveLoc();
            InitData();
        }
        private void PathButtonHandler(object sender, RoutedEventArgs e)
        {
            if (_config.AppSettings.Settings["SavedDir"] != null)
            {
                _savedDir = _config.AppSettings.Settings["SavedDir"].Value;
            }

            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = false;
            if (_savedDir!="")
            {
                folderBrowser.SelectedPath=_savedDir;
            }
            else
            {
                folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer ;
            }

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PathText.Text = folderBrowser.SelectedPath;
                
                _savedDir = folderBrowser.SelectedPath;

                if (_config.AppSettings.Settings["SavedDir"] == null)
                {
                    _config.AppSettings.Settings.Add("SavedDir", _savedDir);
                }
                else
                {
                    _config.AppSettings.Settings["SavedDir"].Value = _savedDir;
                };
                _config.Save(ConfigurationSaveMode.Modified);

                InitData();
            }
        }

        private void SaveLoc()
        {
            SaveJson();
            SaveKeysCS();
        }

        private void SaveKeysCS()
        {
            string csText = "namespace Viktoriaplus.CyberCat.Localization {\n";
            csText += "    public static class LocKeys {\n";
            csText += "        public enum keys {\n";

            var texts = _locData["texts"];
            var i = 0;
            foreach (var key in texts.Keys)
            {
                csText += "            "+key+" = "+i+",\n";
                i++;
            }

            csText += "        }\n";
            csText += "    }\n";
            csText += "}";
            
            File.WriteAllText(_savedDir + "/Assets/Scripts/Localization/LocKeys.cs", csText);
        }

        private void SaveJson()
        {
            string jsonText = "{\n";

            var i = 0;
            foreach (var key1 in _locData.Keys)
            {
                jsonText += "  \""+key1+"\": {";

                var i2 = 0;
                var item = _locData[key1];
                foreach (var key2 in item.Keys)
                {
                    jsonText += "\n    ";
                    jsonText += "\""+key2+"\":\""+item[key2]+"\"";
                    if (i2 < item.Count - 1)
                    {
                        jsonText += ",";
                    }
                    i2++;
                }

                jsonText += "\n  }";
                
                if (i < _locData.Count - 1)
                {
                    jsonText += ",\n";
                }
                else
                {
                    jsonText += "\n";
                }
                i++;
            }
            
            jsonText += "}";
            
            File.WriteAllText(_savedDir + "/Assets/StreamingAssets/Localization/ru.json", jsonText);
        }
    }
}