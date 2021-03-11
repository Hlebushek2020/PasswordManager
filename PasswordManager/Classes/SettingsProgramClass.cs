using PasswordManager.Classes.Settings;
using PasswordManager.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Text;
using System.Windows.Media.Imaging;
using System.Linq;

namespace PasswordManager.Classes
{
    public class SettingsProgram
    {
        [JsonIgnore]
        private const int CountLocalizationStrings = 66;

        [JsonIgnore]
        public static string ProgramResourceFolder { get; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\SergeyGovorunov\\{App.ServiceProgramName}";

        [JsonIgnore]
        public DefaultSettings Default { get; } = new DefaultSettings();

        public bool ClearProgramResourceFolder { get; set; } = false;

        #region Language
        [JsonIgnore]
        private string language;
        public string Language
        {
            get { return language; }
            set
            {
                string source = $"{Path.GetDirectoryName(App.ResourceAssembly.Location)}\\Language\\{value}.xaml";
                if (File.Exists(source))
                {
                    ResourceDictionary resource = new ResourceDictionary
                    {
                        Source = new Uri(source, UriKind.Absolute)
                    };
                    if (resource.Count == CountLocalizationStrings)
                    {
                        App.Current.Resources.MergedDictionaries.Clear();
                        App.Current.Resources.MergedDictionaries.Add(resource);
                        language = value;
                    }
                    else
                        throw new Exception("Unable to change language, localization file is damaged.");
                }
                else
                    throw new Exception("Unable to change language, localization file not found.");
            }
        }
        #endregion

        #region Background
        public string BackgroundFile { get; set; } = string.Empty;
        public Color BackgroundColor { get; set; }
        [JsonIgnore]
        public Brush Background { get; private set; } = new SolidColorBrush(Color.FromRgb(250, 235, 215));
        public Dictionary<Windows, bool> BackgroundFlags { get; } = new Dictionary<Windows, bool>()
        {
            { Windows.Main, false },
            { Windows.Editor, false },
            { Windows.SectionEditor, false },
            { Windows.CipherProgress, false }
        };

        public void SetBackground()
        {
            if (string.IsNullOrEmpty(BackgroundFile) == false)
            {
                string img = $"{ProgramResourceFolder}\\{BackgroundFile}";
                if (File.Exists(img) && (BackgroundFlags[Windows.Main] || BackgroundFlags[Windows.Editor] ||
                    BackgroundFlags[Windows.SectionEditor] || BackgroundFlags[Windows.CipherProgress]))
                {
                    Uri uri = new Uri(img, UriKind.Absolute);
                    Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(uri),
                        Stretch = Stretch.UniformToFill
                    };
                }
                else
                    Background = new SolidColorBrush(BackgroundColor);
            }
            else
                Background = new SolidColorBrush(BackgroundColor);
        }
        #endregion

        #region Text Brush
        public SolidColorBrush TextBrush { get; set; } = Brushes.Black;
        public Dictionary<Windows, bool> TextBrushFlags { get; } = new Dictionary<Windows, bool>() 
        {
            { Windows.Main, false },
            { Windows.Editor, false },
            { Windows.SectionEditor, false },
            { Windows.Settings, false },
            { Windows.CipherProgress, false }
        };
        #endregion

        #region Menu Background
        public SolidColorBrush MenuBackground { get; set; } = Brushes.Transparent;
        public SolidColorBrush MenuBackgroundMouseOver { get; private set; } = new SolidColorBrush(Color.FromArgb(127, 255, 255, 255));
        public SolidColorBrush MenuBackgroundPressed { get; private set; } = new SolidColorBrush(Color.FromArgb(127, 255, 255, 255));
        public Dictionary<Windows, bool> MenuBackgroundFlags { get; } = new Dictionary<Windows, bool>()
        {
            { Windows.Main, false },
            { Windows.Editor, false },
            { Windows.SectionEditor, false },
            { Windows.CipherProgress, false }
        };

        public void SetMenuBackgroundForEvents()
        {
            Color color = MenuBackground.Color;
            if (color.A >= 127)
                color = Color.FromArgb((byte)(color.A - 127), color.R, color.G, color.B);
            else
                color = Color.FromArgb((byte)(color.A + 127), color.R, color.G, color.B);
            MenuBackgroundMouseOver = new SolidColorBrush(color);
            if (color.A >= 10)
                MenuBackgroundPressed = new SolidColorBrush(Color.FromArgb(0, color.R, color.G, color.B));
            else
                MenuBackgroundPressed = new SolidColorBrush(Color.FromArgb(255, color.R, color.G, color.B));
        }
        #endregion

        #region Element Background
        public SolidColorBrush ElementBackground { get; set; } = new SolidColorBrush(Color.FromRgb(255, 248, 220));
        public Dictionary<Windows, bool> ElementBackgroundFlags { get; } = new Dictionary<Windows, bool>()
        {
            { Windows.Main, false },
            { Windows.Editor, false },
            { Windows.SectionEditor, false },
            { Windows.CipherProgress, false }
        };
        #endregion

        #region ScrollBar Brush
        public SolidColorBrush ScrollBarBrush { get; set; } = new SolidColorBrush(Color.FromRgb(222, 184, 135));
        #endregion

        #region ProgressBar Brush
        public SolidColorBrush ProgressBarBrush { get; set; } = Brushes.Green;
        #endregion

        public SettingsProgram() => Language = "english";

        public void Save()
        {
            Directory.CreateDirectory(ProgramResourceFolder);
            using (StreamWriter streamWriter = new StreamWriter($"{ProgramResourceFolder}\\settings.json", false, Encoding.UTF8))
                streamWriter.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        /// <summary>
        /// Получает объект с загружеными настройками
        /// </summary>
        public static SettingsProgram GetInstance()
        {
            string settingsFile = $"{ProgramResourceFolder}\\settings.json";
            if (File.Exists(settingsFile))
                return JsonConvert.DeserializeObject<SettingsProgram>(File.ReadAllText(settingsFile, Encoding.UTF8));
            return new SettingsProgram();
        }
      
        public bool Reset()
        {
            string settings = $"{ProgramResourceFolder}\\settings.json";
            if (File.Exists(settings))
            {
                File.Delete(settings);
                return true;
            }
            return false;
        }

        public void ProgramResourceFolderClearAsync()
        {
            if (ClearProgramResourceFolder)
            {
                Task.Run(() =>
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(ProgramResourceFolder);
                    if (directoryInfo.Exists)
                    {
                        IEnumerable<FileInfo> files = directoryInfo.EnumerateFiles().Where(x => x.Name != BackgroundFile || x.Name != "settings.json");
                        foreach (FileInfo file in files)
                            file.Delete();
                    }
                });
            }
        }

    }
}