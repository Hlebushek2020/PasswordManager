using PasswordManager.Classes;
using PasswordManager.Classes.Settings;
using PasswordManager.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PasswordManager
{
    /// <summary>
    /// Логика взаимодействия для AddEditWindow.xaml
    /// </summary>
    public partial class SectionEditorWindow : Window
    {
        public bool IsSectionNameChanged { get; private set; } = false;

        private readonly WindowSettings settings = new WindowSettings(Windows.SectionEditor);
        private readonly ResourceDictionary localization = App.Current.Resources.MergedDictionaries[0];
        private readonly SectionManager sectionManager;
        private readonly string sectionName;
        private bool isItemEdit;

        public SectionEditorWindow()
        {
            InitializeComponent();
            DataContext = settings;
            sectionManager = new SectionManager();
            listBox_ShowSectionItemKeys.ItemsSource = sectionManager.GetItemKeys();
        }

        public SectionEditorWindow(string sectionName)
        {
            InitializeComponent();
            DataContext = settings;
            this.sectionName = sectionName;
            textBox_SectionName.Text = sectionName;
            sectionManager = PasswordStorage.GetItem(sectionName);
            listBox_ShowSectionItemKeys.ItemsSource = sectionManager.GetItemKeys();
        }

        private void Button_AddSectionItem_Click(object sender, RoutedEventArgs e)
        {
            grid_SectionEditor.Visibility = Visibility.Hidden;
            grid_EditSectionItem.Visibility = Visibility.Visible;
            textBox_Parameter.IsReadOnly = false;
            isItemEdit = false;
        }

        private void Button_DeleteSectionItem_Click(object sender, RoutedEventArgs e)
        {
            object currentItem = listBox_ShowSectionItemKeys.SelectedItem;
            if (currentItem != null)
                if (MessageBox.Show(string.Format((string)localization["sew_InfoDelete"], currentItem), App.ProgramName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    sectionManager.DeleteItem((string)currentItem);
                    listBox_ShowSectionItemKeys.Items.Refresh();
                }
        }

        private void Button_EditSectionItem_Click(object sender, RoutedEventArgs e)
        {
            object currentItem = listBox_ShowSectionItemKeys.SelectedItem;
            if (currentItem != null)
            {
                grid_SectionEditor.Visibility = Visibility.Hidden;
                grid_EditSectionItem.Visibility = Visibility.Visible;
                string strCurrentItem = (string)currentItem;
                textBox_Parameter.Text = strCurrentItem;
                textBox_Parameter.IsReadOnly = true;
                textBox_Value.Text = sectionManager.GetItemValue(strCurrentItem);
                isItemEdit = true;
            }
        }

        private void Button_SaveAndCloseSectionItemEditor_Click(object sender, RoutedEventArgs e)
        {
            if (isItemEdit)
            {
                if (string.IsNullOrEmpty(textBox_Value.Text))
                {
                    MessageBox.Show((string)localization["sew_InfoValueEmpty"], App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (textBox_Value.Text != sectionManager.GetItemValue(textBox_Parameter.Text))
                    sectionManager.UpdateItem(textBox_Parameter.Text, textBox_Value.Text);
            }
            else
            {
                if (string.IsNullOrEmpty(textBox_Value.Text) || string.IsNullOrEmpty(textBox_Parameter.Text))
                {
                    MessageBox.Show((string)localization["sew_InfoSectionItemNull"], App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (textBox_Parameter.Text.Contains('='))
                {
                    MessageBox.Show((string)localization["sew_InfoInvalidCharacter"], App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (sectionManager.ContainsItem(textBox_Parameter.Text))
                {
                    MessageBox.Show((string)localization["sew_InfoParameterContains"], App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                sectionManager.AddItem(textBox_Parameter.Text, textBox_Value.Text);
                listBox_ShowSectionItemKeys.Items.Refresh();
            }
            textBox_Parameter.Text = string.Empty;
            textBox_Value.Text = string.Empty;
            grid_SectionEditor.Visibility = Visibility.Visible;
            grid_EditSectionItem.Visibility = Visibility.Hidden;
        }

        private void Button_NewPassword_Click(object sender, RoutedEventArgs e)
        {
            string password = "";
            Random random = new Random();
            while (password.Length != 10)
                password += (char)random.Next(33, 127);
            textBox_Value.Text = password;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (grid_EditSectionItem.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrEmpty(textBox_Parameter.Text) && string.IsNullOrEmpty(textBox_Value.Text))
                {
                    Button_СloseSectionItemEditor_Click(null, null);
                    e.Cancel = true;
                }
                else
                {
                    if (isItemEdit)
                        if (sectionManager.GetItemValue(textBox_Parameter.Text) == textBox_Value.Text)
                            return;
                    if (MessageBox.Show((string)localization["sew_InfoSaveChanges"], App.ProgramName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Button_SaveAndCloseSectionItemEditor_Click(null, null);
                        e.Cancel = true;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(sectionName) == false)
                {
                    if (string.IsNullOrEmpty(textBox_SectionName.Text))
                    {
                        MessageBox.Show((string)localization["sew_InfoSectionNameEmpty"], App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Cancel = true;
                        return;
                    }
                    if (sectionName != textBox_SectionName.Text)
                    {
                        if (PasswordStorage.Contains(textBox_SectionName.Text))
                        {
                            MessageBox.Show((string)localization["sew_InfoSectionContains"], App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Warning);
                            e.Cancel = true;
                            return;
                        }
                        PasswordStorage.RemoveSection(sectionName);
                        PasswordStorage.AddSection(textBox_SectionName.Text, sectionManager);
                        IsSectionNameChanged = true;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(textBox_SectionName.Text) && sectionManager.Count == 0)
                        return;
                    if (string.IsNullOrEmpty(textBox_SectionName.Text) && sectionManager.Count != 0)
                    {
                        MessageBox.Show((string)localization["sew_InfoSectionNameEmpty"], App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Cancel = true;
                        return;
                    }
                    if (PasswordStorage.Contains(textBox_SectionName.Text))
                    {
                        MessageBox.Show((string)localization["sew_InfoSectionContains"], App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Warning);
                        e.Cancel = true;
                        return;
                    }
                    PasswordStorage.AddSection(textBox_SectionName.Text, sectionManager);
                    IsSectionNameChanged = true;
                }
            }
        }

        private void Button_СloseSectionItemEditor_Click(object sender, RoutedEventArgs e)
        {
            textBox_Parameter.Text = string.Empty;
            textBox_Value.Text = string.Empty;
            grid_SectionEditor.Visibility = Visibility.Visible;
            grid_EditSectionItem.Visibility = Visibility.Hidden;
        }

        private void ListBox_ShowSectionItemKeys_MouseDoubleClick(object sender, MouseButtonEventArgs e) => Button_EditSectionItem_Click(null, null);

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Escape) Close();
            if (grid_EditSectionItem.Visibility == Visibility.Visible)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.P) Button_NewPassword_Click(null, null);
            }
            else
            {
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.A) Button_AddSectionItem_Click(null, null);
                if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.E) Button_EditSectionItem_Click(null, null);
                if ((Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.D) || e.Key == Key.Delete) Button_DeleteSectionItem_Click(null, null);
            }
        }

    }
}