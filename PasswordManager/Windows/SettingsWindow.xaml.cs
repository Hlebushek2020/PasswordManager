using PasswordManager.Classes;
using PasswordManager.Classes.Settings;
using PasswordManager.Enums;
using PasswordManager.Structures;
using SergeyRegistryExtension;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PasswordManager
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        /// <summary>
        /// Указывает были изменены настройки или нет
        /// </summary>
        public bool IsChanged { get; private set; } = false;

        private readonly WindowSettings settings = new WindowSettings(Windows.Settings);
        private readonly ResourceDictionary localization = App.Current.Resources.MergedDictionaries[0];
        private bool closeMessageEnabled = true;

        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = settings;
            // MAIN
            checkBox_WBM.IsChecked = App.Settings.BackgroundFlags[Windows.Main];
            checkBox_WBE.IsChecked = App.Settings.BackgroundFlags[Windows.Editor];
            checkBox_WBSE.IsChecked = App.Settings.BackgroundFlags[Windows.SectionEditor];
            checkBox_WBCP.IsChecked = App.Settings.BackgroundFlags[Windows.CipherProgress];
            // MENU
            button_SelectMenuBackground.Tag = App.Settings.MenuBackground.Color;
            slider_MenuBackgroundTransparent.Value = 255 - App.Settings.MenuBackground.Color.A;
            checkBox_MBM.IsChecked = App.Settings.MenuBackgroundFlags[Windows.Main];
            checkBox_MBE.IsChecked = App.Settings.MenuBackgroundFlags[Windows.Editor];
            checkBox_MBSE.IsChecked = App.Settings.MenuBackgroundFlags[Windows.SectionEditor];
            checkBox_MBCP.IsChecked = App.Settings.MenuBackgroundFlags[Windows.CipherProgress];
            //checkBox_MBS.IsChecked = App.Settings.MenuBackgroundFlags[Windows.Settings];
            // ELEMENT
            button_SelectElementBackground.Tag = App.Settings.ElementBackground.Color;
            slider_ElementBackgroundTransparent.Value = 255 - App.Settings.ElementBackground.Color.A;
            checkBox_EBM.IsChecked = App.Settings.ElementBackgroundFlags[Windows.Main];
            checkBox_EBE.IsChecked = App.Settings.ElementBackgroundFlags[Windows.Editor];
            checkBox_EBSE.IsChecked = App.Settings.ElementBackgroundFlags[Windows.SectionEditor];
            checkBox_EBCP.IsChecked = App.Settings.ElementBackgroundFlags[Windows.CipherProgress];
            button_SelectScrollBarColor.Tag = App.Settings.ScrollBarBrush.Color;
            slider_ScrollBarTransparent.Value = 255 - App.Settings.ScrollBarBrush.Color.A;
            button_SelectProgressBarColor.Tag = App.Settings.ProgressBarBrush.Color;
            slider_ProgressBarTransparent.Value = 255 - App.Settings.ProgressBarBrush.Color.A;
            // TEXT
            button_SelectTextBrush.Tag = App.Settings.TextBrush.Color;
            checkBox_TBM.IsChecked = App.Settings.TextBrushFlags[Windows.Main];
            checkBox_TBE.IsChecked = App.Settings.TextBrushFlags[Windows.Editor];
            checkBox_TBSE.IsChecked = App.Settings.TextBrushFlags[Windows.SectionEditor];
            checkBox_TBCP.IsChecked = App.Settings.TextBrushFlags[Windows.CipherProgress];
            checkBox_TBS.IsChecked = App.Settings.TextBrushFlags[Windows.Settings];
            // ADDITIONALLY
            if (Directory.Exists($"{Path.GetDirectoryName(App.ResourceAssembly.Location)}\\Language"))
            {
                string[] languages = Directory.GetFiles($"{Path.GetDirectoryName(App.ResourceAssembly.Location)}\\Language", "*.xaml");
                for (int i = 0; i < languages.Length; i++)
                    comboBox_Language.Items.Add(Path.GetFileNameWithoutExtension(languages[i]).ToLower());
            }
            comboBox_Language.SelectedItem = App.Settings.Language;
            //textBlock_ResourceFolder.Text = App.Settings.ResourceFolder;
        }

        private void Button_SelectBackground_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show((string)localization["sw_InfoSelectBackground"], App.ProgramName, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog
                {
                    Filter = $"{localization["sw_InfoImageExtensionName"]}|*.jpg;*.png",
                    DefaultExt = "jpg"
                })
                {
                    if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        button_SelectBackground.Tag = openFileDialog.FileName;
                }
            }
            if (messageBoxResult == MessageBoxResult.No)
            {
                Color color = Colors.White;
                if (button_SelectBackground.Tag != null)
                    if (button_SelectBackground.Tag.GetType() == typeof(Color))
                        color = (Color)button_SelectBackground.Tag;
                SelectColor selectColor = ShellSelectColor(color);
                if (selectColor.IsSelected)
                    button_SelectBackground.Tag = selectColor.Color;
            }
        }

        private void Button_SelectScrollBarColor_Click(object sender, RoutedEventArgs e) =>
            button_SelectScrollBarColor.Tag = ShellSelectColor((Color)button_SelectScrollBarColor.Tag).Color;

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) =>
            ((Slider)sender).SelectionEnd = e.NewValue;

        private void Button_SelectMenuBackground_Click(object sender, RoutedEventArgs e) =>
            button_SelectMenuBackground.Tag = ShellSelectColor((Color)button_SelectMenuBackground.Tag).Color;

        private void Button_SelectElementBackground_Click(object sender, RoutedEventArgs e) =>
            button_SelectElementBackground.Tag = ShellSelectColor((Color)button_SelectElementBackground.Tag).Color;

        private void Button_SelectTextBrush_Click(object sender, RoutedEventArgs e) =>
            button_SelectTextBrush.Tag = ShellSelectColor((Color)button_SelectTextBrush.Tag).Color;

        private void Button_SelectProgressBarColor_Click(object sender, RoutedEventArgs e) =>
            button_SelectProgressBarColor.Tag = ShellSelectColor((Color)button_SelectProgressBarColor.Tag).Color;

        private void Button_SetExtension_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string pathIco = Path.GetDirectoryName(Application.ResourceAssembly.Location) + "\\Extension.ico";
                if (!File.Exists(pathIco))
                    pathIco = string.Empty;
                Registry.AssociateExtension(".sgpm", App.ServiceProgramName, Application.ResourceAssembly.Location, pathIco);
                MessageBox.Show((string)localization["sw_InfoDone"], App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_RemoveExtension_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Registry.RemoveAssociateExtension(".sgpm", App.ServiceProgramName);
                MessageBox.Show((string)localization["sw_InfoDone"], App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Grid_Additionally_Loaded(object sender, RoutedEventArgs e) =>
            comboBox_Language.Margin = new Thickness(textBlock_Language.ActualWidth + 15, comboBox_Language.Margin.Top, 0, 0);

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (IsChangedSettings())
                SaveSettings();
            Close();
        }

        private void Button_NoSave_Click(object sender, RoutedEventArgs e)
        {
            closeMessageEnabled = false;
            Close();
        }

        private void Button_SettingsReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (App.Settings.Reset())
                {
                    MessageBox.Show((string)localization["sw_InfoSettingsReset"], App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Information);
                    closeMessageEnabled = false;
                    Close();
                }
            }
            catch
            {
                MessageBox.Show((string)localization["sw_InfoSettingsResetFail"], App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (closeMessageEnabled)
                if (IsChangedSettings())
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show((string)localization["sw_InfoSave"], App.ProgramName, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (messageBoxResult == MessageBoxResult.Yes)
                        SaveSettings();
                    else
                    {
                        if (messageBoxResult == MessageBoxResult.Cancel)
                            e.Cancel = true;
                    }
                }

        }

        private SelectColor ShellSelectColor(Color color)
        {
            bool isChanged = false;
            using (System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog
            {
                Color = System.Drawing.Color.FromArgb(color.R, color.G, color.B)
            })
            {
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    color = Color.FromArgb(color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                    isChanged = true;
                }
            }
            return new SelectColor(isChanged, color);
        }

        private void SaveSettings()
        {
            if (button_SelectBackground.Tag != null)
            {
                object backgroundTag = button_SelectBackground.Tag;
                if (backgroundTag.GetType() == typeof(string))
                {
                    string newImage = (string)backgroundTag;
                    string fileName = Guid.NewGuid() + Path.GetExtension(newImage);
                    Directory.CreateDirectory(SettingsProgram.ProgramResourceFolder);
                    File.Copy(newImage, $"{SettingsProgram.ProgramResourceFolder}\\{fileName}", true);
                    App.Settings.BackgroundFile = fileName;
                }
                else
                {
                    App.Settings.BackgroundColor = (Color)backgroundTag;
                    App.Settings.BackgroundFile = string.Empty;
                }
            }
            if (comboBox_Language.SelectedItem != null)
                App.Settings.Language = comboBox_Language.SelectedItem.ToString();
            Color color = (Color)button_SelectScrollBarColor.Tag;
            color.A = (byte)(255 - slider_ScrollBarTransparent.Value);
            App.Settings.ScrollBarBrush = new SolidColorBrush(color);
            color = (Color)button_SelectMenuBackground.Tag;
            color.A = (byte)(255 - slider_MenuBackgroundTransparent.Value);
            if (App.Settings.MenuBackground.Color != color)
            {
                App.Settings.MenuBackground = new SolidColorBrush(color);
                App.Settings.SetMenuBackgroundForEvents();
            }
            color = (Color)button_SelectElementBackground.Tag;
            color.A = (byte)(255 - slider_ElementBackgroundTransparent.Value);
            App.Settings.ElementBackground = new SolidColorBrush(color);
            color = (Color)button_SelectProgressBarColor.Tag;
            color.A = (byte)(255 - slider_ProgressBarTransparent.Value);
            App.Settings.ProgressBarBrush = new SolidColorBrush(color);
            App.Settings.TextBrush = new SolidColorBrush((Color)button_SelectTextBrush.Tag);
            App.Settings.BackgroundFlags[Windows.Main] = checkBox_WBM.IsChecked.Value;
            App.Settings.BackgroundFlags[Windows.Editor] = checkBox_WBE.IsChecked.Value;
            App.Settings.BackgroundFlags[Windows.SectionEditor] = checkBox_WBSE.IsChecked.Value;
            App.Settings.BackgroundFlags[Windows.CipherProgress] = checkBox_WBCP.IsChecked.Value;
            App.Settings.SetBackground();
            App.Settings.MenuBackgroundFlags[Windows.Main] = checkBox_MBM.IsChecked.Value;
            App.Settings.MenuBackgroundFlags[Windows.Editor] = checkBox_MBE.IsChecked.Value;
            App.Settings.MenuBackgroundFlags[Windows.SectionEditor] = checkBox_MBSE.IsChecked.Value;
            App.Settings.MenuBackgroundFlags[Windows.CipherProgress] = checkBox_MBCP.IsChecked.Value;
            //App.Settings.MenuBackgroundFlags[Windows.Settings] = checkBox_MBS.IsChecked.Value;
            App.Settings.ElementBackgroundFlags[Windows.Main] = checkBox_EBM.IsChecked.Value;
            App.Settings.ElementBackgroundFlags[Windows.Editor] = checkBox_EBE.IsChecked.Value;
            App.Settings.ElementBackgroundFlags[Windows.SectionEditor] = checkBox_EBSE.IsChecked.Value;
            App.Settings.ElementBackgroundFlags[Windows.CipherProgress] = checkBox_EBCP.IsChecked.Value;
            App.Settings.TextBrushFlags[Windows.Main] = checkBox_TBM.IsChecked.Value;
            App.Settings.TextBrushFlags[Windows.Editor] = checkBox_TBE.IsChecked.Value;
            App.Settings.TextBrushFlags[Windows.SectionEditor] = checkBox_TBSE.IsChecked.Value;
            App.Settings.TextBrushFlags[Windows.CipherProgress] = checkBox_TBCP.IsChecked.Value;
            App.Settings.TextBrushFlags[Windows.Settings] = checkBox_TBS.IsChecked.Value;
            App.Settings.Save();
            IsChanged = true;
        }

        private bool IsChangedSettings()
        {
            object backgroundTag = button_SelectBackground.Tag;
            bool change;
            if (backgroundTag != null)
            {
                if (backgroundTag.GetType() == typeof(string))
                    change = (string)backgroundTag != App.Settings.BackgroundFile;
                else
                    change = (Color)backgroundTag != App.Settings.BackgroundColor;
                if (change)
                    return change;
            }

            Color color = (Color)button_SelectScrollBarColor.Tag;
            color.A = (byte)(255 - slider_ScrollBarTransparent.Value);
            change = color != App.Settings.ScrollBarBrush.Color;
            color = (Color)button_SelectMenuBackground.Tag;
            color.A = (byte)(255 - slider_MenuBackgroundTransparent.Value);
            change = change || color != App.Settings.MenuBackground.Color;
            color = (Color)button_SelectElementBackground.Tag;
            color.A = (byte)(255 - slider_ElementBackgroundTransparent.Value);
            change = change || color != App.Settings.ElementBackground.Color;
            color = (Color)button_SelectTextBrush.Tag;
            change = change || color != App.Settings.TextBrush.Color;
            color = (Color)button_SelectProgressBarColor.Tag;
            color.A = (byte)(255 - slider_ProgressBarTransparent.Value);
            change = change || color != App.Settings.ProgressBarBrush.Color;
            
            if (change)
                return change;

            string language = App.Settings.Language;
            if (comboBox_Language.SelectedItem != null)
                language = (string)comboBox_Language.SelectedItem;

            change = language != App.Settings.Language ||
                checkBox_WBM.IsChecked != App.Settings.BackgroundFlags[Windows.Main] ||
                checkBox_WBE.IsChecked != App.Settings.BackgroundFlags[Windows.Editor] ||
                checkBox_WBSE.IsChecked != App.Settings.BackgroundFlags[Windows.SectionEditor] ||
                checkBox_WBCP.IsChecked != App.Settings.BackgroundFlags[Windows.CipherProgress] ||
                checkBox_MBM.IsChecked != App.Settings.MenuBackgroundFlags[Windows.Main] ||
                checkBox_MBE.IsChecked != App.Settings.MenuBackgroundFlags[Windows.Editor] ||
                checkBox_MBSE.IsChecked != App.Settings.MenuBackgroundFlags[Windows.SectionEditor] ||
                checkBox_MBCP.IsChecked != App.Settings.MenuBackgroundFlags[Windows.CipherProgress] ||
                //checkBox_MBS.IsChecked != App.Settings.MenuBackgroundFlags[Windows.Settings] ||
                checkBox_EBM.IsChecked != App.Settings.ElementBackgroundFlags[Windows.Main] ||
                checkBox_EBE.IsChecked != App.Settings.ElementBackgroundFlags[Windows.Editor] ||
                checkBox_EBSE.IsChecked != App.Settings.ElementBackgroundFlags[Windows.SectionEditor] ||
                checkBox_EBCP.IsChecked != App.Settings.ElementBackgroundFlags[Windows.CipherProgress] ||
                checkBox_TBM.IsChecked != App.Settings.TextBrushFlags[Windows.Main] ||
                checkBox_TBE.IsChecked != App.Settings.TextBrushFlags[Windows.Editor] ||
                checkBox_TBSE.IsChecked != App.Settings.TextBrushFlags[Windows.SectionEditor] ||
                checkBox_TBCP.IsChecked != App.Settings.TextBrushFlags[Windows.CipherProgress] ||
                checkBox_TBS.IsChecked != App.Settings.TextBrushFlags[Windows.Settings];

            return change;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Escape) Close();
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.R) Button_SettingsReset_Click(null, null);
        }

    }
}