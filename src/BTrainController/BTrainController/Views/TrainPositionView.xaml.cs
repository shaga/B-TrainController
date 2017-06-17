using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BTrainController.Views
{
    public sealed partial class TrainPositionView : UserControl
    {
        private const double SizeRate = 250 / 320.0;

        public const int PostionUnknown = 0;

        public const int PositionBack = 1;

        public const int PositionBackRight = 2;

        public const int PositionFrontRight = 3;

        public const int PositionFront = 4;

        public const int PositionFrontLeft = 5;

        public const int PositionBackLeft = 6;

        public static readonly DependencyProperty TrainPositionProperty =
            DependencyProperty.Register(nameof(TrainPosition), typeof(int), typeof(TrainPositionView), new PropertyMetadata(-1));

        public int TrainPosition
        {
            get { return (int)GetValue(TrainPositionProperty); }
            set { SetValue(TrainPositionProperty, value); }
        }

        public TrainPositionView()
        {
            this.InitializeComponent();

            Loaded += (s, e) => UpdateSize();
            SizeChanged += (s, e) => UpdateSize();
        }

        private void UpdateSize()
        {
            var width = ActualWidth;
            var height = ActualHeight;

            var w = 0.0;
            var h = 0.0;

            if (height > width * SizeRate)
            {
                h = (height - width * SizeRate) / 2;
            }
            else
            {
                w = (width - height / SizeRate) / 2;
            }

            BorderTopLeft.Width = w;
            BorderTopLeft.Height = h;
            BorderBottomRight.Width = w;
            BorderBottomRight.Height = h;
        }
    }
}
