using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Newtonsoft.Json;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Localizator
{
    public partial class MainWindow
    {
        private AddDialogWindow _addDialogWindow;

        private string _savedDir="";
        private Dictionary<string, Dictionary<string, string>> _locData;
        private Dictionary<string, Dictionary<string, string>> _locDataEN;

        private ContextMenuStrip _collectionRoundMenuStrip;

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
            RefreshButton.Click += RefreshButtonHandler;

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
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            MakeList();
        }

        private void GetJson()
        {
            if (_savedDir != null)
            {
                var jsonData = File.ReadAllText(_savedDir + "/Assets/StreamingAssets/Localization/ru.json");
                _locData = @JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);

                var jsonDataEN = File.ReadAllText(_savedDir + "/Assets/StreamingAssets/Localization/en.json");
                _locDataEN = @JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonDataEN);
            }
        }

        private void MakeList()
        {
            if (_locData == null)
            {
                return;
            }

            var texts = _locData["texts"];
            var textsEN = _locDataEN["texts"];

            var itemSource = new object[texts.Count];
            int i = texts.Count-1;

            foreach (string key in textsEN.Keys)
            {
                if (texts.ContainsKey(key)) continue;

                Console.WriteLine(" UNIQUE KEY ====> "+key);
            }

            foreach (var key in texts.Keys)
            {
                itemSource[i] = new Dictionary<string, string>() { {key, @""+texts[key]} };
                itemSource[i] = new {Key = key, ValueRU = @""+texts[key], ValueEN = @"" + textsEN[key]};
                i--;
            }

            ListBoxMain.ItemsSource = itemSource;

            AddContextMenu();
        }

        private void AddContextMenu()
        {
            var toolStripMenuItem1 = new ToolStripMenuItem {Text = "Удалить"};
            toolStripMenuItem1.Click += RemoveItemClickHandler;
            var toolStripMenuItem2 = new ToolStripMenuItem {Text = "Изменить"};
            toolStripMenuItem2.Click += ChangeItemClickHandler;

            _collectionRoundMenuStrip = new ContextMenuStrip();
            _collectionRoundMenuStrip.Items.AddRange(new ToolStripItem[] {toolStripMenuItem1, toolStripMenuItem2 });
            ListBoxMain.MouseUp += RightMouseHandler;
        }

        private void RemoveItemClickHandler(object sender, EventArgs  e)
        {
            Dictionary<string, string> selectedDict = ListBoxMain.SelectedItem as Dictionary<string, string>;

            foreach (KeyValuePair<string, string> kvp in selectedDict)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Уверен, что хочешь удалить ключ { " + kvp.Key + " }\nсо значением { " +
                    _locData["texts"][kvp.Key] + " }?!",
                    "Удаление ключа", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.OK:
                        _locData["texts"].Remove(kvp.Key);
                        SaveLoc();
                        InitData();
                        break;
                    default:
                        return;
                }
            }
        }
        
        private void ChangeItemClickHandler(object sender, EventArgs  e)
        {
            _addDialogWindow = new AddDialogWindow {Owner = this};
            _addDialogWindow.Closed += AddDialogHandler;

            Dictionary<string, string> selectedDict = ListBoxMain.SelectedItem as Dictionary<string, string>;
            foreach (KeyValuePair<string, string> kvp in selectedDict)
            {
                _addDialogWindow.KeyBox.Text = kvp.Key;
                _addDialogWindow.ValueRuBox.Text = kvp.Value;
                
                _locData["texts"].Remove(kvp.Key);
            }

            _addDialogWindow.ShowDialog();
        }

        private void RightMouseHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                _collectionRoundMenuStrip.Show(System.Windows.Forms.Cursor.Position);
                _collectionRoundMenuStrip.Visible = true;
            }
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
            
            if ((_addDialogWindow.ValueRU == "")||(_addDialogWindow.ValueRU == null))
            {
                MessageBox.Show("Поле Значение RU не может быть пустым", "", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            if ((_addDialogWindow.ValueEN == "") || (_addDialogWindow.ValueEN == null))
            {
                MessageBox.Show("Поле Значение EN не может быть пустым", "", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }


            if (!_locData["texts"].ContainsKey(_addDialogWindow.Key))
            {
                _locData["texts"].Add(SpaceToCamelCase(_addDialogWindow.Key), _addDialogWindow.ValueRU);
                _locDataEN["texts"].Add(SpaceToCamelCase(_addDialogWindow.Key), _addDialogWindow.ValueEN);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show(
                    "Ключ { " + _addDialogWindow.Key + " } уже существует в локализации\nсо значением { " +
                    _locData["texts"][_addDialogWindow.Key] + " }! \nТы хорошо подумал, что хочешь его перезаписать?!",
                    "Ключ уже существует", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.OK:
                        _locData["texts"].Remove(_addDialogWindow.Key);
                        _locData["texts"].Add(SpaceToCamelCase(_addDialogWindow.Key), _addDialogWindow.ValueRU);
                        _locDataEN["texts"].Remove(_addDialogWindow.Key);
                        _locDataEN["texts"].Add(SpaceToCamelCase(_addDialogWindow.Key), _addDialogWindow.ValueEN);
                        break;
                    default:
                        return;
                }
            }

            SaveLoc();
            InitData();
        }

        private void RefreshButtonHandler(object sender, RoutedEventArgs e)
        {
            InitData();
            SaveLoc();

            MessageBox.Show("Все ресурсы обновлены!!", "Камплит", MessageBoxButton.OK, MessageBoxImage.Asterisk);
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

        private string SpaceToCamelCase(string value)
        {
            string result = "";

            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                if (!char.IsSeparator(ch))
                {
                    result += value[i];
                }
                else
                {
                    i++;
                    if (i<value.Length)
                    {
                        result += char.ToUpper(value[i]);
                    }
                }
            }

            return result;
        }

        private void SaveLoc()
        {
            SaveJson();
            SaveKeysCS();
            SaveLocDataTextsCS();
        }

        private void SaveLocDataTextsCS()
        {
            string csText = "using System;\n\n";
            csText += "namespace Viktoriaplus.CyberCat.Localization {\n";
            csText += "    [Serializable]\n";
            csText += "    public class LocDataTexts {\n";
            
            var texts = _locData["texts"];
            foreach (var key in texts.Keys)
            {
                csText += "        public string "+key+";\n";
            }

            csText += "    }\n";
            csText += "}\n";
            
            File.WriteAllText(_savedDir + "/Assets/Scripts/Localization/LocDataTexts.cs", csText);
        }

        private void SaveKeysCS()
        {
            string csText = "namespace Viktoriaplus.CyberCat.Localization {\n";
            csText += "    public enum LocKeys {\n";

            var texts = _locData["texts"];
            var i = 0;
            foreach (var key in texts.Keys)
            {
                csText += "        "+key+" = "+i+",\n";
                i++;
            }

            csText += "    }\n";
            csText += "}";
            
            File.WriteAllText(_savedDir + "/Assets/Scripts/Localization/LocKeys.cs", csText);
        }

        private void SaveJson()
        {
            SaveJsonLang("ru", _locData);
            SaveJsonLang("en", _locDataEN);

        }

        private void SaveJsonLang(string lang, Dictionary<string, Dictionary<string, string>> locData)
        {
            string jsonText = "{\n";

            var i = 0;
            foreach (var key1 in locData.Keys)
            {
                jsonText += "  \"" + key1 + "\": {";

                var i2 = 0;
                var item = locData[key1];
                foreach (var key2 in item.Keys)
                {
                    jsonText += "\n    ";
                    var it = item[key2].Replace("\"", "\\\"").Replace("\n", "\\n");
                    jsonText += "\"" + key2 + "\":\"" + it + "\"";
                    if (i2 < item.Count - 1)
                    {
                        jsonText += ",";
                    }
                    i2++;
                }

                jsonText += "\n  }";

                if (i < locData.Count - 1)
                {
                    jsonText += ",\n";
                } else
                {
                    jsonText += "\n";
                }
                i++;
            }

            jsonText += "}";

            File.WriteAllText(_savedDir + "/Assets/StreamingAssets/Localization/"+ lang + ".json", jsonText);
        }
    }

    class TableItemSource
    {
        public string Key;
        public string Value;
        public TableItemSource(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}