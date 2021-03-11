using PasswordManager.Classes;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace PasswordManager
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Настройки программы
        /// </summary>
        public static SettingsProgram Settings { get; private set; }
        /// <summary>
        /// Название программы
        /// </summary>
        public const string ProgramName = "Password Manager";
        /// <summary>
        /// Название программы (СЛУЖЕБНОЕ)
        /// </summary>
        public const string ServiceProgramName = "PasswordManager";
        /// <summary>
        /// Точка входа
        /// </summary>
        [STAThread]
        public static void Main()
        {
            try
            {
                App application = new App();
                application.InitializeComponent();
                application.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Подгружаем ресурсы и выполняем прочие операции
        /// </summary>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //try
            //{
            Settings = SettingsProgram.GetInstance();
            Settings.ProgramResourceFolderClearAsync();
            Settings.SetBackground();
            MainWindow = new MainWindow();
            if (e.Args.Length != 0)
            {
                if (File.Exists(e.Args[0]))
                {
                    CipherProgressWindow cipherProgressWindow = new CipherProgressWindow(true, e.Args[0]);
                    cipherProgressWindow.ShowDialog();
                    if (cipherProgressWindow.IsSuccessfully == false)
                        Shutdown();
                    else
                        ((MainWindow)MainWindow).UpdateSectionList();
                }
                else
                    Shutdown();
            }
            MainWindow.Show();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
            //    Shutdown();
            //}
        }
    }
}
