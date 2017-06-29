using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using BTrainDemoApp.Annotations;
using BTrainDemoApp.Data;
using Prism.Windows.Mvvm;

namespace BTrainDemoApp.Models
{
    public class DemoController : INotifyPropertyChanged
    {
        #region implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region const

        private const int StationStopInterval = 10000;
        #endregion

        #region field

        private readonly TrainController _trainController;
        private readonly OsakaLoopLineStations _stations;

        private bool _isDemoRunning = false;
        private bool _isDemoStop = false;
        private LoopStationInfo _stationInfo;

        private ETrainStatus _trainStatus;

        #endregion

        #region property

        private static CoreDispatcher Dispatcher => CoreApplication.MainView.Dispatcher;

        public LoopStationInfo StationInfo
        {
            get => _stationInfo;
            set
            {
                if (_stationInfo?.StationName == value?.StationName) return;
                _stationInfo = value;
                RaisePropertyChanged();
            }
        }

        public ETrainStatus TrainStatus
        {
            get => _trainStatus;
            set
            {
                if (_trainStatus == value) return;
                _trainStatus = value;
                RaisePropertyChanged();
            }
        }

        public bool IsLoopOuter { get; set; }

        #endregion

        #region constructor

        public DemoController(TrainController controller)
        {
            _trainController = controller;
            _trainController.TrainStatusChanged += OnChangedTrainStatus;

            TrainStatus = ETrainStatus.Stop;
            _stations = new OsakaLoopLineStations();
            StationInfo = _stations[OsakaLoopLineStations.DefaultKey];
        }

        #endregion

        #region method

        public void Start()
        {
            if (_isDemoRunning) return;

            _isDemoRunning = true;
            _isDemoStop = false;

            GoNextStation();
        }

        public void Stop()
        {
            if (!_isDemoRunning || _isDemoStop) return;

            _isDemoStop = true;
        }

        private async void OnChangedTrainStatus(object sender, ETrainStatus status)
        {
            if (!_isDemoRunning) return;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                TrainStatus = status;
            });

            if (status != ETrainStatus.Stop) return;

            await Task.Delay(StationStopInterval);

            if (_isDemoStop)
            {
                _isDemoRunning = false;
                _isDemoStop = false;
                return;
            }

            GoNextStation();
        }

        private async void GoNextStation()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                StationInfo = _stations[IsLoopOuter ? StationInfo.NextOuter : StationInfo.NextInner];
            });

            _trainController.StartLoop(IsLoopOuter);
        }

        #endregion
    }
}
