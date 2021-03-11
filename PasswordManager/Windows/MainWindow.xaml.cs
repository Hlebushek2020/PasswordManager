using PasswordManager.Classes;
using PasswordManager.Classes.Settings;
using PasswordManager.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PasswordManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WindowSettings settings = new WindowSettings(Windows.Main);
        private readonly List<object> showSection = new List<object>();
        private int detailSectionIndex = -1;

        public void UpdateSectionList()
        {
            showSection.Clear();
            showSection.AddRange(PasswordStorage.GetItemKeys());
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = settings;
            listBox_ShowSection.ItemsSource = showSection;
            showSection.AddRange(PasswordStorage.GetItemKeys());
        }

        private void TextBox_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string currentText = textBox_Search.Text;
            showSection.Clear();
            showSection.AddRange(PasswordStorage.GetItemKeys()
                .Where(x => x.IndexOf(currentText, StringComparison.CurrentCultureIgnoreCase) >= 0));
            listBox_ShowSection.Items.Refresh();
            detailSectionIndex = -1;
        }

        private void ListBox_ShowSection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selected = listBox_ShowSection.SelectedItem;
            if (selected == null || selected.GetType() == typeof(TextBox))
                return;
            int selectedIndex = listBox_ShowSection.SelectedIndex;
            if (detailSectionIndex - 1 == selectedIndex)
                return;
            if (detailSectionIndex != -1)
            {
                showSection.RemoveAt(detailSectionIndex);
                if (detailSectionIndex < selectedIndex)
                    selectedIndex--;
            }
            string sectionName = (string)selected;
            IEnumerable<string> sectionItems = PasswordStorage.GetItem(sectionName).GetItems()
                .Select(x => $"{x.Key}: {x.Value}");
            TextBox section = new TextBox
            {
                BorderThickness = new Thickness(0, 0, 0, 0),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(15, 0, 0, 0),
                Background = Brushes.Transparent,
                Foreground = listBox_ShowSection.Foreground,
                IsReadOnly = true,
                Text = string.Join(Environment.NewLine, sectionItems)
            };
            showSection.Insert(selectedIndex + 1, section);
            detailSectionIndex = selectedIndex + 1;
            listBox_ShowSection.Items.Refresh();
        }

        private void Button_Editor_Click(object sender, RoutedEventArgs e)
        {
            EditorWindow editorWindow = new EditorWindow();
            editorWindow.ShowDialog();
            if (editorWindow.IsSectionNameChanged)
            {
                UpdateSectionList();
                listBox_ShowSection.Items.Refresh();
            }
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
            if (settingsWindow.IsChanged)
                settings.ChangedAll();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (PasswordStorage.IsChanged)
                if (MessageBox.Show((string)App.Current.Resources.MergedDictionaries[0]["mw_InfoSaveChanges"], App.ProgramName, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    e.Cancel = true;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Escape) Close();
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.E) Button_Editor_Click(null, null);
        }
    }
}