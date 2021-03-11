using PasswordManager.Classes;
using PasswordManager.Classes.Settings;
using PasswordManager.Enums;
using System.Windows;
using System.Windows.Input;

namespace PasswordManager
{
    /// <summary>
    /// Логика взаимодействия для RedactorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        public bool IsSectionNameChanged { get; private set; } = false;

        private readonly WindowSettings settings = new WindowSettings(Windows.Editor);
        private readonly ResourceDictionary localization = App.Current.Resources.MergedDictionaries[0];

        public EditorWindow()
        {
            InitializeComponent();
            DataContext = settings;
            listBox_ShowSectionNames.ItemsSource = PasswordStorage.GetItemKeys();
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            SectionEditorWindow sectionEditorWindow = new SectionEditorWindow();
            sectionEditorWindow.ShowDialog();
            if (sectionEditorWindow.IsSectionNameChanged)
            {
                listBox_ShowSectionNames.Items.Refresh();
                IsSectionNameChanged = true;
            }
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            object sectionName = listBox_ShowSectionNames.SelectedItem;
            if (sectionName != null)
            {
                SectionEditorWindow sectionEditorWindow = new SectionEditorWindow((string)sectionName);
                sectionEditorWindow.ShowDialog();
                if (sectionEditorWindow.IsSectionNameChanged)
                {
                    listBox_ShowSectionNames.Items.Refresh();
                    IsSectionNameChanged = true;
                }
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            object sectionName = listBox_ShowSectionNames.SelectedItem;
            if (sectionName != null)
                if (MessageBox.Show(string.Format((string)localization["ew_InfoDelete"], sectionName), App.ProgramName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    PasswordStorage.RemoveSection((string)sectionName);
                    listBox_ShowSectionNames.Items.Refresh();
                    IsSectionNameChanged = true;
                }
        }

        private void Button_Open_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordStorage.IsChanged)
                if (MessageBox.Show((string)localization["mw_InfoSaveChanges"], App.ProgramName, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
            using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = $"{App.ProgramName}|*.sgpm",
                DefaultExt = "sgpm",
                RestoreDirectory = false
            })
            {
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    CipherProgressWindow cipherProgressWindow = new CipherProgressWindow(true, openFileDialog.FileName);
                    cipherProgressWindow.ShowDialog();
                    if (cipherProgressWindow.IsSuccessfully)
                    {
                        listBox_ShowSectionNames.Items.Refresh();
                        IsSectionNameChanged = true;
                    }
                }
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e) =>
            Close();

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = $"{App.ProgramName}|*.sgpm",
                DefaultExt = "sgpm",
                RestoreDirectory = false
            })
            {
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    CipherProgressWindow cipherProgressWindow = new CipherProgressWindow(false, saveFileDialog.FileName);
                    cipherProgressWindow.ShowDialog();
                }
            }
        }
        private void ListBox_ShowSectionNames_MouseDoubleClick(object sender, MouseButtonEventArgs e) => Button_Edit_Click(null, null);

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Escape) Close();
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S) Button_Save_Click(null, null);
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O) Button_Open_Click(null, null);
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.A) Button_Add_Click(null, null);
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.E) Button_Edit_Click(null, null);
            if ((Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.D) || e.Key == Key.Delete) Button_Delete_Click(null, null);
        }

    }
}