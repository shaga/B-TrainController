using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BTrainDemoApp.Models.Converters
{
    class ControlModeVisivilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var paramStr = parameter as string;

            if (string.IsNullOrEmpty(paramStr)) return Visibility.Collapsed;

            var current = (EControlMode) value;

            EControlMode mode;

            if (!Enum.TryParse(paramStr, true, out mode)) return Visibility.Collapsed;

            return mode == current ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
