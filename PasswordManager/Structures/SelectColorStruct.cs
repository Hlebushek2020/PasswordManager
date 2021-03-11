using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PasswordManager.Structures
{
    public struct SelectColor
    {
        public bool IsSelected { get; }
        public Color Color { get; }

        public SelectColor(bool isSelected, Color color)
        {
            IsSelected = isSelected;
            Color = color;
        }
    }
}
