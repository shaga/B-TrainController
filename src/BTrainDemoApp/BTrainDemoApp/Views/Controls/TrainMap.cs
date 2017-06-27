using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace BTrainDemoApp.Views.Controls
{
    public sealed class TrainMap : Control
    {
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
        }
    }
}
