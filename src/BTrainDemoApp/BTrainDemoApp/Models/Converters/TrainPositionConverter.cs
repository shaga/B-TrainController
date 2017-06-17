using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BTrainDemoApp.Models.Converters
{
    class TrainPositionConverter : IValueConverter
    {
        private static readonly SolidColorBrush InactiveBrush = new SolidColorBrush(Colors.White);
        private static readonly SolidColorBrush ActiveBrush = new SolidColorBrush(Colors.Red);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var current = (int)value;
            var paramStr = parameter as string;

            if (!int.TryParse(paramStr, out int position)) return InactiveBrush;

            return current == position ? ActiveBrush : InactiveBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
