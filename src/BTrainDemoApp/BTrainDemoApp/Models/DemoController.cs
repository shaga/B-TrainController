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

        private const int StationStopInterval = 5000;

        #endregion

        #region field

        private readonly TrainController _trainController;
        private readonly OsakaLoopLineStations _stations;

        private bool _isDemoRunning = false;
        private bool _isDemoStop = false;
        private LoopStationInfo _stationInfo;

        private int _loopCount;

        private int _position;
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
            _trainController.PositionUpdated += OnTrainPositionChanged;

            TrainStatus = ETrainStatus.Stop;
            _stations = new OsakaLoopLineStations();
            StationInfo = _stations[OsakaLoopLineStations.DefaultKey];
        }

        #endregion

        #region method

        public async void Start()
        {
            if (_isDemoRunning) return;

            _isDemoRunning = true;
            _isDemoStop = false;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                var direction = IsLoopOuter ? EDirection.Outside : EDirection.Inside;
                TrainStatus = ETrainStatus.Go;
                _trainController.SetSpeed(direction, 0x60);
            });
        }

        public void Stop()
        {
            if (!_isDemoRunning || _isDemoStop) return;

            _isDemoStop = true;
        }

        public async void SetPosition(int position)
        {
            _position = position;

            var direction = IsLoopOuter ? EDirection.Outside : EDirection.Inside;
            if (_position == TrainController.PosFront)
            {
                _loopCount++;

                if (_loopCount >= 2)
                {
                    _loopCount = 0;

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
                    {
                        TrainStatus = ETrainStatus.Stop;
                        _trainController.SetSpeed(EDirection.Stop);
                        await Task.Delay(StationStopInterval);
                        if (_isDemoStop)
                        {
                            _isDemoRunning = false;
                            _isDemoStop = false;
                            return;
                        }
                        StationInfo = _stations[IsLoopOuter ? StationInfo.NextOuter : StationInfo.NextInner];
                        TrainStatus = ETrainStatus.Go;
                        _trainController.SetSpeed(direction, 0x60);
                    });
                }
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    switch (_position)
                    {
                        case TrainController.PosFrontLeft:
                            if (_loopCount == 0 && IsLoopOuter)
                                _trainController.SetSpeed(direction, 0x80);
                            else if (_loopCount == 1 && !IsLoopOuter)
                                _trainController.SetSpeed(direction, 0x60);
                            break;
                        case TrainController.PosLeft:
                            if (_loopCount == 0 && IsLoopOuter)
                                _trainController.SetSpeed(direction, 0xa0);
                            else if (_loopCount == 1 && !IsLoopOuter)
                                _trainController.SetSpeed(direction, 0x80);
                            break;
                        case TrainController.PosBackLeft:
                            if (_loopCount == 0 && IsLoopOuter)
                                _trainController.SetSpeed(direction, 0xc0);
                            else if (_loopCount == 1 && !IsLoopOuter)
                            {
                                TrainStatus = ETrainStatus.Arriving;
                                _trainController.SetSpeed(direction, 0xa0);
                            }
                            break;
                        case TrainController.PosFrontRight:
                            if (_loopCount == 0 && !IsLoopOuter)
                                _trainController.SetSpeed(direction, 0x80);
                            else if (_loopCount == 1 && IsLoopOuter)
                                _trainController.SetSpeed(direction, 0x60);
                            break;
                        case TrainController.PosRight:
                            if (_loopCount == 0 && !IsLoopOuter)
                                _trainController.SetSpeed(direction, 0xa0);
                            else if (_loopCount == 1 && IsLoopOuter)
                                _trainController.SetSpeed(direction, 0x80);
                            break;
                        case TrainController.PosBackRight:
                            if (_loopCount == 0 && !IsLoopOuter)
                                _trainController.SetSpeed(direction, 0xc0);
                            else if (_loopCount == 1 && IsLoopOuter)
                            {
                                TrainStatus = ETrainStatus.Arriving;
                                _trainController.SetSpeed(direction, 0x60);
                            }
                            break;
                    }
                });
            }
        }

        private void OnTrainPositionChanged(object sender, int position)
        {
            if (!_isDemoRunning) return;

            SetPosition(position);
        }

        #endregion
    }
}
