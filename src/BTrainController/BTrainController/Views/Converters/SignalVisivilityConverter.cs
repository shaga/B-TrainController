using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using BTrainController.Models;

namespace BTrainController.Views.Converters
{
    internal class SignalVisivilityConverter : IValueConverter
    {
        private static readonly SolidColorBrush OffColor = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
        private static readonly SolidColorBrush StopColor = new SolidColorBrush(Colors.Red);
        private static readonly SolidColorBrush CanGoColor = new SolidColorBrush(Colors.Green);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var status = (ESignalStatus) value;

            ESignalStatus param;

            if (!Enum.TryParse(parameter as string, out param))
            {
                param = ESignalStatus.None;
            }

            if (status != param || param == ESignalStatus.None) return OffColor;

            return param == ESignalStatus.CanGo ? CanGoColor : StopColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
