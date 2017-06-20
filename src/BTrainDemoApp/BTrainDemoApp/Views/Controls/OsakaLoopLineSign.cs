using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BTrainDemoApp.Data;
using BTrainDemoApp.Models;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace BTrainDemoApp.Views.Controls
{
    public sealed class OsakaLoopLineSign : Control
    {
        private readonly OsakaLoopLineStations _stations;

        public static readonly DependencyProperty StationInfoProperty =
            DependencyProperty.Register(nameof(StationInfo), typeof(StationInfo), typeof(OsakaLoopLineSign), new PropertyMetadata(null, OnStationInfoChanged));

        public static void OnStationInfoChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = obj as OsakaLoopLineSign;

            if (ctrl == null) return;

            ctrl.NextOutStationInfo = ctrl._stations.GetNextOutStationInfo(ctrl.StationInfo.NameEng);
            ctrl.NextInStationInfo = ctrl._stations.GetNextInStationInfo(ctrl.StationInfo.NameEng);
        }

        public StationInfo StationInfo
        {
            get => GetValue(StationInfoProperty) as StationInfo;
            set => SetValue(StationInfoProperty, value);
        }

        public static readonly DependencyProperty NextOutStationInfoProperty =
            DependencyProperty.Register(nameof(NextOutStationInfo), typeof(StationInfo), typeof(OsakaLoopLineSign), new PropertyMetadata(null));

        public StationInfo NextOutStationInfo
        {
            get => GetValue(NextOutStationInfoProperty) as StationInfo;
            set => SetValue(NextOutStationInfoProperty, value);
        }

        public static readonly DependencyProperty NextInStationInfoProperty =
            DependencyProperty.Register(nameof(NextInStationInfo), typeof(StationInfo), typeof(OsakaLoopLineSign), new PropertyMetadata(null));

        public StationInfo NextInStationInfo
        {
            get => GetValue(NextInStationInfoProperty) as StationInfo;
            set => SetValue(NextInStationInfoProperty, value);
        }

        public OsakaLoopLineSign()
        {
            this.DefaultStyleKey = typeof(OsakaLoopLineSign);

            _stations = new OsakaLoopLineStations();
            StationInfo = _stations[OsakaLoopLineStations.DefaultKey];
        }
    }
}
