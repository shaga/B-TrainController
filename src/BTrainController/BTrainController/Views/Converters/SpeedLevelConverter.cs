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
    internal class SpeedLevelConverter : IValueConverter
    {
        private static readonly SolidColorBrush DeactiveBrush = new SolidColorBrush(Colors.DarkKhaki);
        private static readonly SolidColorBrush ActiveBrush = new SolidColorBrush(Colors.Yellow);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var level = (ESpeedLevel)value;

            var paramStr = parameter as string;

            ESpeedLevel param;

            if (!Enum.TryParse<ESpeedLevel>(paramStr, out param))
            {
                return DeactiveBrush;
            }

            return param <= level ? ActiveBrush : DeactiveBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
