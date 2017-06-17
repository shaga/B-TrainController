using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using BTrainController.Models;
using BTrainController.Views;
using Prism.Mvvm;

namespace BTrainController
{
    class MainViewModel : BindableBase, IDisposable
    {
        private readonly CamFaceDetector _faceDetector;

        private readonly VoiceCommand _voiceCommand;

        private readonly BleTrain _bleTrain;

        private readonly TrainSound _trainSound;

        private bool _isInitalizedTrainSound;

        private bool _isInitializedFaceDetector;

        private bool _isInitializedVoiceCommand;

        private bool _isConnected;
        private int _displayPosition;

        private bool _isFaceExist;

        private bool _isRunning;

        private ESignalStatus _signalStatus;

        private ESpeedLevel _speedLevel;

        private bool _isPlayingBell;

        private bool _isPlayingDoor;

        private ETrainStatus _trainStatus;

        public static CoreDispatcher Dispatcher => CoreApplication.MainView.Dispatcher;

        public bool IsInitialized => _isConnected && _isInitializedFaceDetector && _isInitializedVoiceCommand && _isInitalizedTrainSound;

        public int DisplayPosition
        {
            get { return _displayPosition; }
            set { SetProperty(ref _displayPosition, value); }
        }

        public ESignalStatus SignalStatus
        {
            get { return _signalStatus; }
            set { SetProperty(ref _signalStatus, value); }
        }

        public ESpeedLevel SpeedLevel
        {
            get { return _speedLevel; }
            set { SetProperty(ref _speedLevel, value); }
        }

        public ETrainStatus TrainStatus
        {
            get { return _trainStatus; }
            set { SetProperty(ref _trainStatus, value); }
        }

        public MainViewModel()
        {
            SignalStatus = ESignalStatus.Stop;

            _faceDetector = new CamFaceDetector();
            _faceDetector.IsExistFaceChanged += OnIsFaceExsitChanged;
            _faceDetector.InitalizedStateChanged += OnFaceDetectorInitalizedStateChanged;

            _voiceCommand = new VoiceCommand();
            _voiceCommand.InitializedStateChanged += OnVoiceCommandInitalizedStateChanged;
            _voiceCommand.ReceivedCommand += OnReceivedVoiceCommand;

            _bleTrain = new BleTrain();
            _bleTrain.ConnectionStateChanged += OnChangedConnectionState;
            _bleTrain.TrainPositionUpdated += OnUpdatedTrainPosition;

            _trainSound = new TrainSound();
            _trainSound.Initizlized += OnTrainSoundInitailized;
            _trainSound.SoundPlayEnded += OnSoundPlayEnded;
        }

        public void Dispose()
        {
            _faceDetector.IsExistFaceChanged -= OnIsFaceExsitChanged;
            _faceDetector.InitalizedStateChanged -= OnFaceDetectorInitalizedStateChanged;

            _voiceCommand.InitializedStateChanged -= OnVoiceCommandInitalizedStateChanged;
            _voiceCommand.ReceivedCommand -= OnReceivedVoiceCommand;

            _bleTrain.ConnectionStateChanged -= OnChangedConnectionState;
            _bleTrain.TrainPositionUpdated -= OnUpdatedTrainPosition;
        }

        public void Initialize(CaptureElement element, MediaElement mediaElement)
        {
            _trainSound.Initialise(mediaElement);

            _faceDetector.Initialize(element);
            
            _voiceCommand.Initialize();

            _bleTrain.Connect();
        }

        private void OnChangedConnectionState(object sender, EBleState state)
        {
            _isConnected = state == EBleState.Connected;

            UpdateSignalState();
            OnInitialized();
        }

        private async void OnUpdatedTrainPosition(object sender, int position)
        {
            switch (position)
            {
                case BleTrain.PosFront:
                    SetDisplayPosition(TrainPositionView.PositionFront);
                    SetTrainSpeed(ESpeedLevel.Stop);
                    UpdateTrainStatus(ETrainStatus.Stopping);
                    _isPlayingDoor = true;
                    await Task.Delay(1000);
                    _trainSound.PlayAsync(ESoundType.DoorOpen);
                    break;
                case BleTrain.PosFrontRight:
                    SetDisplayPosition(TrainPositionView.PositionFrontRight);
                    SetTrainSpeed(ESpeedLevel.Lv2);
                    break;
                case BleTrain.PosRight:
                    SetDisplayPosition(TrainPositionView.PositionBackRight);
                    SetTrainSpeed(ESpeedLevel.Lv3);
                    break;
                case BleTrain.PosBackRight:
                    SetDisplayPosition(TrainPositionView.PositionBack);
                    SetTrainSpeed(ESpeedLevel.Lv4);
                    UpdateTrainStatus(ETrainStatus.GoingMax);
                    break;
                case BleTrain.PosBack:
                    break;
                case BleTrain.PosBackLeft:
                    SetDisplayPosition(TrainPositionView.PositionBackLeft);
                    SetTrainSpeed(ESpeedLevel.Lv3);
                    UpdateTrainStatus(ETrainStatus.Going);
                    break;
                case BleTrain.PosLeft:
                    SetDisplayPosition(TrainPositionView.PositionFrontLeft);
                    SetTrainSpeed(ESpeedLevel.Lv2);
                    break;
                case BleTrain.PosFrontLeft:
                    SetTrainSpeed(ESpeedLevel.Lv1);
                    UpdateTrainStatus(ETrainStatus.BeforeStop);
                    break;
            }
        }

        private void OnIsFaceExsitChanged(object sender, bool isFaceExist)
        {
            _isFaceExist = isFaceExist;

            UpdateSignalState();
        }

        private void OnFaceDetectorInitalizedStateChanged(object sender, bool isInitialized)
        {
            _isInitializedFaceDetector = isInitialized;
            OnInitialized();
        }

        private void OnReceivedVoiceCommand(object sender, ECommand command)
        {
            if (command == ECommand.Go)
            {
                if (!IsInitialized) return;

                if (_signalStatus != ESignalStatus.CanGo) return;

                _isPlayingBell = true;
                UpdateTrainStatus(ETrainStatus.BeforeGo);
                _trainSound.PlayAsync(ESoundType.Bell);
                //SetTrainSpeed(ESpeedLevel.Lv1);
            }
            else if (command == ECommand.Stop)
            {
                if (!_isConnected) return;

                SetTrainSpeed(0);
            }
        }

        private void OnVoiceCommandInitalizedStateChanged(object sender, bool isInitialized)
        {
            _isInitializedVoiceCommand = isInitialized;

            OnInitialized();
        }

        private async void SetDisplayPosition(int position)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => DisplayPosition = position);
        }

        private async void SetTrainSpeed(ESpeedLevel speedLevel)
        {
            _isRunning = speedLevel > ESpeedLevel.Stop;
            UpdateSignalState();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                SpeedLevel = speedLevel;
                _bleTrain.SetSpeed(EDirection.Right, (int)speedLevel);
            });
        }

        private async void SetTrainSpeed(int speed)
        {
            _isRunning = speed > 0;
            UpdateSignalState();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => _bleTrain.SetSpeed(EDirection.Right, speed));
        }

        private async void UpdateSignalState()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                if (!_isConnected || !_isInitalizedTrainSound)
                {
                    SignalStatus = ESignalStatus.Stop;
                }
                else if (_isRunning)
                {
                    SignalStatus = ESignalStatus.None;
                }
                else if (_isPlayingBell)
                {
                    SignalStatus = ESignalStatus.CanGo;
                }
                else if (_isPlayingDoor)
                {
                    SignalStatus = ESignalStatus.Stop;
                }
                else
                {
                    SignalStatus = _isFaceExist ? ESignalStatus.CanGo : ESignalStatus.Stop;

                    UpdateTrainStatus(_isFaceExist ? ETrainStatus.CanGo : ETrainStatus.Stop);
                }
            });
        }

        private void OnTrainSoundInitailized(object sender, EventArgs e)
        {
            _isInitalizedTrainSound = true;
            OnInitialized();
        }

        private async void OnInitialized()
        {
            UpdateTrainStatus(IsInitialized ? ETrainStatus.Initializing : ETrainStatus.Stop);
        }

        private async void UpdateTrainStatus(ETrainStatus status)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                TrainStatus = status;
            });
        }

        private async void OnSoundPlayEnded(object sender, ESoundType type)
        {
            if (type == ESoundType.Bell)
            {
                await Task.Delay(500);
                _trainSound.PlayAsync(ESoundType.DoorClose);
            }
            else if (type == ESoundType.DoorOpen)
            {
                _isPlayingDoor = false;
                UpdateSignalState();
                UpdateTrainStatus(_isFaceExist ? ETrainStatus.CanGo : ETrainStatus.Stop);
            }
            else if (type == ESoundType.DoorClose)
            {
                _isPlayingBell = false;
                await Task.Delay(500);
                SetTrainSpeed(ESpeedLevel.Lv1);
                _trainSound.PlayAsync(ESoundType.Drive);
                UpdateTrainStatus(ETrainStatus.Going);
            }
        }
    }
}
