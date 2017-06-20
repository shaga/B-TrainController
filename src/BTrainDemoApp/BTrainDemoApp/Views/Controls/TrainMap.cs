using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace BTrainDemoApp.Views.Controls
{
    public sealed class TrainMap : Control
    {
        private const double TrackAspectRate = 320 / 200.0;

        public static readonly DependencyProperty SideMarginWidthProperty =
            DependencyProperty.Register(nameof(SideMarginWidth), typeof(double), typeof(TrainMap), new PropertyMetadata(0));

        public double SideMarginWidth
        {
            get => (double)GetValue(SideMarginWidthProperty);
            set => SetValue(SideMarginWidthProperty, value);
        }

        public static readonly DependencyProperty MiddleMarginWidthProperty =
            DependencyProperty.Register(nameof(MiddleMarginWidth), typeof(double), typeof(TrainMap), new PropertyMetadata(0));

        public double MiddleMarginWidth
        {
            get => (double)GetValue(MiddleMarginWidthProperty);
            set => SetValue(MiddleMarginWidthProperty, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(int), typeof(TrainMap), new PropertyMetadata(0));

        public int Position
        {
            get => (int)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty TrainStatusProperty =
            DependencyProperty.Register(nameof(TrainStatus), typeof(string), typeof(TrainMap), new PropertyMetadata(null));

        public string TrainStatus { get; set; }

        public TrainMap()
        {
            this.DefaultStyleKey = typeof(TrainMap);

            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var height = ActualHeight;
            var width = ActualWidth;

            var w = (height - 50) * TrackAspectRate;

            SideMarginWidth = (width - w) / 2 - 23;

            MiddleMarginWidth = (height - 50) / 2 - 50;
        }
    }
}
