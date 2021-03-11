using PasswordManager.Classes;
using PasswordManager.Classes.Settings;
using PasswordManager.Enums;
using PasswordManager.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для DeCoderProgressWindows.xaml
    /// </summary>
    public partial class CipherProgressWindow : Window
    {
        /// <summary>
        /// Успешность процесса шифрования/дешифрования
        /// </summary>
        public bool IsSuccessfully { get; private set; } = false;

        private readonly WindowSettings settings = new WindowSettings(Windows.CipherProgress);
        private readonly ResourceDictionary localization = App.Current.Resources.MergedDictionaries[0];
        private bool close = true;
        private readonly bool isOpen;
        private readonly string fullFileName;

        public CipherProgressWindow(bool isOpen, string fullFileName)
        {
            InitializeComponent();
            DataContext = settings;
            this.isOpen = isOpen;
            this.fullFileName = fullFileName;
            if (isOpen)
                grid_Decrypt.Visibility = Visibility.Visible;
            else
                grid_Encrypt.Visibility = Visibility.Visible;
        }

        private async void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            string password;
            if (isOpen)
            {
                password = passwordBox_Password.Password;
                passwordBox_Password.Password = "";
            }
            else
            {
                password = textBox_Password.Text;
                textBox_Password.Text = "";
            }
            int i = 0;
            while (password.Length != 32)
            {
                password += password[i];
                i++;
            }
            bool writePasswordHash = checkBox_WritePasswordHash.IsChecked.Value;
            if (isOpen)
                await Task.Run(() => DecryptorVoidThread(new CipherData(fullFileName, password)));
            else
                await Task.Run(() => EncryptorVoidThread(new CipherData(fullFileName, password, writePasswordHash)));
        }

        private void Button_GeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            string password = "";
            Random random = new Random();
            while (password.Length != 32)
                password += (char)random.Next(33, 127);
            textBox_Password.Text = password;
        }

        #region Cipher
        /// <summary>
        /// Шифрование
        /// </summary>
        private void EncryptorVoidThread(CipherData cipherData)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    grid_Encrypt.Visibility = Visibility.Hidden;
                    grid_Progress.Visibility = Visibility.Visible;
                    textBlock_Progress.Text = (string)localization["cpw_InfoPreparation"];
                    progressBar_Progress.IsIndeterminate = true;
                    close = false;
                });
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, true))
                    {
                        binaryWriter.Write(PasswordStorage.Count);
                        foreach (KeyValuePair<string, SectionManager> section in PasswordStorage.GetItems())
                        {
                            Dispatcher.Invoke((Action<string, int>)((string parSectionName, int parSectionItemCount) =>
                            {
                                textBlock_Progress.Text = string.Format((string)localization["cpw_InfoSectionProcessing"], parSectionName);
                                progressBar_Progress.Maximum = parSectionItemCount;
                                progressBar_Progress.Value = 0;
                                progressBar_Progress.IsIndeterminate = false;
                            }), section.Key, section.Value.Count);
                            binaryWriter.Write(section.Key);
                            binaryWriter.Write(section.Value.Count);
                            foreach (KeyValuePair<string, string> sectionItem in section.Value.GetItems())
                            {
                                binaryWriter.Write(sectionItem.Key);
                                binaryWriter.Write(sectionItem.Value);
                                Dispatcher.Invoke(() =>
                                {
                                    progressBar_Progress.Value++;
                                });
                            }
                        }
                    }
                    using (FileStream fileStream = new FileStream(cipherData.FullFileName, FileMode.Create, FileAccess.Write))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            textBlock_Progress.Text = (string)localization["cpw_InfoPasswordProcessing"];
                            progressBar_Progress.IsIndeterminate = true;
                        });
                        if (cipherData.WritePasswordHash)
                        {
                            fileStream.WriteByte(1);
                            byte[] hash;
                            using (SHA512Managed sHA512Managed = new SHA512Managed())
                                hash = sHA512Managed.ComputeHash(Encoding.ASCII.GetBytes(cipherData.FullPassword));
                            fileStream.Write(hash, 0, hash.Length);
                        }
                        else
                            fileStream.WriteByte(0);
                        ICryptoTransform cryptoTransform;
                        using (RijndaelManaged rijndaelManaged = new RijndaelManaged
                        {
                            BlockSize = 256,
                            KeySize = 256,
                            Mode = CipherMode.CBC,
                            Padding = PaddingMode.ISO10126
                        })
                        {
                            byte[] passwordByte = Encoding.ASCII.GetBytes(cipherData.FullPassword);
                            rijndaelManaged.Key = passwordByte;
                            rijndaelManaged.IV = passwordByte.Reverse().ToArray();
                            cryptoTransform = rijndaelManaged.CreateEncryptor();
                        }
                        using (CryptoStream cryptoStream = new CryptoStream(fileStream, cryptoTransform, CryptoStreamMode.Write))
                        {
                            byte[] buffer = new byte[64];
                            int size;
                            memoryStream.Position = 0;
                            Dispatcher.Invoke((Action<long, long>)((long parPosition, long parLength) =>
                            {
                                textBlock_Progress.Text = string.Format((string)localization["cpw_InfoEncryptionProgress"], parPosition, parLength);
                                progressBar_Progress.Maximum = parLength;
                                progressBar_Progress.Value = 0;
                                progressBar_Progress.IsIndeterminate = false;
                            }), memoryStream.Position, memoryStream.Length);
                            while ((size = memoryStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                cryptoStream.Write(buffer, 0, size);
                                Dispatcher.Invoke((Action<long, long>)((long parPosition, long parLength) =>
                                {
                                    textBlock_Progress.Text = string.Format((string)localization["cpw_InfoEncryptionProgress"], parPosition, parLength);
                                    progressBar_Progress.Value = parPosition;
                                }), memoryStream.Position, memoryStream.Length);
                            }
                            PasswordStorage.IsChanged = false;
                        }
                    }
                }
                Dispatcher.Invoke(() => IsSuccessfully = true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    close = true;
                    Close();
                });
            }
        }

        /// <summary>
        /// ДеШифрование
        /// </summary>
        private void DecryptorVoidThread(CipherData cipherData)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    grid_Decrypt.Visibility = Visibility.Hidden;
                    grid_Progress.Visibility = Visibility.Visible;
                    textBlock_Progress.Text = (string)localization["cpw_InfoPreparation"];
                    progressBar_Progress.IsIndeterminate = true;
                    close = false;
                });
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    ICryptoTransform cryptoTransform;
                    using (RijndaelManaged rijndaelManaged = new RijndaelManaged
                    {
                        BlockSize = 256,
                        KeySize = 256,
                        Mode = CipherMode.CBC,
                        Padding = PaddingMode.ISO10126
                    })
                    {
                        byte[] passwordByte = Encoding.ASCII.GetBytes(cipherData.FullPassword);
                        rijndaelManaged.Key = passwordByte;
                        rijndaelManaged.IV = passwordByte.Reverse().ToArray();
                        cryptoTransform = rijndaelManaged.CreateDecryptor();
                    }
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                    {
                        using (FileStream fileStream = new FileStream(cipherData.FullFileName, FileMode.Open, FileAccess.Read))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                textBlock_Progress.Text = (string)localization["cpw_InfoPasswordVerification"];
                            });
                            long excessByte = 1;
                            if (fileStream.ReadByte() == 1)
                            {
                                byte[] readHash = new byte[64];
                                fileStream.Read(readHash, 0, readHash.Length);
                                excessByte = fileStream.Position;
                                byte[] currentHash = Encoding.ASCII.GetBytes(cipherData.FullPassword);
                                using (SHA512Managed sHA512Managed = new SHA512Managed())
                                    currentHash = sHA512Managed.ComputeHash(currentHash);
                                if (readHash != currentHash)
                                {
                                    MessageBox.Show((string)localization["cpw_InfoWrongPassword"], App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                            }
                            byte[] buffer = new byte[64];
                            int size;
                            Dispatcher.Invoke((Action<long, long>)((long parPosition, long parLength) =>
                            {
                                textBlock_Progress.Text = string.Format((string)localization["cpw_InfoDecryptionProgress"], parPosition, parLength);
                                progressBar_Progress.Maximum = parLength;
                                progressBar_Progress.Value = 0;
                                progressBar_Progress.IsIndeterminate = false;
                            }), fileStream.Position - excessByte, fileStream.Length - excessByte);
                            while ((size = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                cryptoStream.Write(buffer, 0, size);
                                Dispatcher.Invoke((Action<long, long>)((long parPosition, long parLength) =>
                                {
                                    textBlock_Progress.Text = string.Format((string)localization["cpw_InfoDecryptionProgress"], parPosition, parLength);
                                    progressBar_Progress.Value = parPosition;
                                }), fileStream.Position - excessByte, fileStream.Length - excessByte);
                            }
                            cryptoStream.FlushFinalBlock();
                        }
                        memoryStream.Position = 0;
                        using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8, false))
                        {
                            PasswordStorage.Clear();
                            Dispatcher.Invoke(() =>
                            {
                                progressBar_Progress.IsIndeterminate = true;
                            });
                            int sectionCount = binaryReader.ReadInt32();
                            for (int i = 0; i < sectionCount; i++)
                            {
                                string sectionName = binaryReader.ReadString();
                                int sectionItemCount = binaryReader.ReadInt32();
                                Dispatcher.Invoke((Action<string>)((string parSectionName) =>
                                {
                                    textBlock_Progress.Text = string.Format((string)localization["cpw_InfoSectionProcessing"], parSectionName);
                                }), sectionName);
                                SectionManager section = new SectionManager();
                                string key, item;
                                for (int j = 0; j < sectionItemCount; j++)
                                {
                                    key = binaryReader.ReadString();
                                    item = binaryReader.ReadString();
                                    section.AddItemNoChange(key, item);
                                }
                                PasswordStorage.AddSectionNoChange(sectionName, section);
                            }
                        }
                    }
                }
                Dispatcher.Invoke(() => IsSuccessfully = true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                PasswordStorage.Clear();
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    close = true;
                    Close();
                });
            }
        }
        #endregion

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (grid_Progress.Visibility != Visibility.Visible)
            {
                if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Escape) Close();
                if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Enter) Button_Start_Click(null, null);
                if (isOpen)
                    if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.P) Button_GeneratePassword_Click(null, null);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) =>
            e.Cancel = !close;
    }
}