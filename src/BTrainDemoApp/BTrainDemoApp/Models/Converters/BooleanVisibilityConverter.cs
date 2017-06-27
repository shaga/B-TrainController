﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BTrainDemoApp.Models.Converters
{
    class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isVisible = (bool)value;

            var isInvert = false;

            if (parameter != null && !bool.TryParse((string) parameter, out isInvert))
            {
                isInvert = false;
            }

            if (!isInvert) return isVisible ? Visibility.Visible : Visibility.Collapsed;
            return isVisible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
