using System.Windows.Media;

namespace PasswordManager.Classes.Settings
{
    public class DefaultSettings
    {
        public Brush Background { get; } = new SolidColorBrush(Color.FromRgb(250, 235, 215));
        public SolidColorBrush ElementBackground { get; } = new SolidColorBrush(Color.FromRgb(255, 248, 220));
        public SolidColorBrush TextBrush { get; } = Brushes.Black;
        public SolidColorBrush MenuBackground { get; } = Brushes.Transparent;
        public SolidColorBrush MenuBackgroundMouseOver { get; } = new SolidColorBrush(Color.FromArgb(127, 255, 255, 255));
        public SolidColorBrush MenuBackgroundPressed { get; } = new SolidColorBrush(Color.FromArgb(127, 255, 255, 255));
    }
}
