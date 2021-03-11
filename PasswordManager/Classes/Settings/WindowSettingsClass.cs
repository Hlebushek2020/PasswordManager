using PasswordManager.Enums;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace PasswordManager.Classes.Settings
{
    public class WindowSettings : INotifyPropertyChanged
    {
        private readonly Windows window;

        public WindowSettings(Windows window) => this.window = window;

        public void ChangedAll()
        {
            OnPropertyChanged("TextBrush");
            OnPropertyChanged("ElementBackground");
            OnPropertyChanged("ScrollBarBrush");
            OnPropertyChanged("ProgressBarBrush");
            ChangedMenuBackground();
            OnPropertyChanged("Background");
        }

        #region Background
        public Brush Background
        {
            get
            {
                if (App.Settings.BackgroundFlags[window] == false)
                    return App.Settings.Default.Background;
                return App.Settings.Background;
            }
        }
        public void ChangedBackground() => OnPropertyChanged("Background");
        #endregion

        #region Text Brush
        public SolidColorBrush TextBrush
        {
            get
            {
                if (App.Settings.TextBrushFlags[window] == false)
                    return App.Settings.Default.TextBrush;
                return App.Settings.TextBrush;
            }
        }
        public void ChangedTextBrush() => OnPropertyChanged("TextBrush");
        #endregion

        #region Menu Background
        public SolidColorBrush MenuBackground
        {
            get
            {
                if (App.Settings.MenuBackgroundFlags[window] == false)
                    return App.Settings.Default.MenuBackground;
                return App.Settings.MenuBackground;
            }
        }
        public SolidColorBrush MenuBackgroundMouseOver
        {
            get
            {
                if (App.Settings.MenuBackgroundFlags[window] == false)
                    return App.Settings.Default.MenuBackgroundMouseOver;
                return App.Settings.MenuBackgroundMouseOver;
            }
        }
        public SolidColorBrush MenuBackgroundPressed
        {
            get
            {
                if (App.Settings.MenuBackgroundFlags[window] == false)
                    return App.Settings.Default.MenuBackgroundPressed;
                return App.Settings.MenuBackgroundPressed;
            }
        }
        public void ChangedMenuBackground()
        {
            OnPropertyChanged("MenuBackground");
            OnPropertyChanged("MenuBackgroundMauseOver");
            OnPropertyChanged("MenuBackgroundPressed");
        }
        #endregion

        #region Element Background
        public SolidColorBrush ElementBackground
        {
            get
            {
                if (App.Settings.ElementBackgroundFlags[window] == false)
                    return App.Settings.Default.ElementBackground;
                return App.Settings.ElementBackground;
            }
        }
        public void ChangedElementBackground() => OnPropertyChanged("ElementBackground");
        #endregion
      
        #region Scroll Bar Brush
        public SolidColorBrush ScrollBarBrush { get => App.Settings.ScrollBarBrush; }
        public void ChangedScrollBarBrush() => OnPropertyChanged("ScrollBarBrush");
        #endregion

        #region Progress Bar Brush
        public SolidColorBrush ProgressBarBrush { get => App.Settings.ProgressBarBrush; }
        public void ShangedProgressBarBrush() => OnPropertyChanged("ProgressBarBrush");
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

    }
}