using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using BTrainDemoApp.Data;
using BTrainDemoApp.Models;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;

namespace BTrainDemoApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region field

        private INavigationService _navigationService;

        private VoiceCommandReceiver _commandReceiver;
        private TrainController _trainController;

        private Windows.ApplicationModel.Resources.ResourceLoader _resLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

        private EControlMode _mode;
        private int _trainPosition;
        private bool _modeIsVoice;
        private bool _canSwitch;
        private bool _isRunning;
        private bool _isOuterLoop;
        private bool _isInitialized;
        private ETrainStatus _trainStatus;
        private LoopStationInfo _stationInfo;

        #endregion

        #region property

        private static CoreDispatcher Dispatcher => CoreApplication.MainView.Dispatcher;

        public EControlMode Mode
        {
            get => _mode;
            set
            {
                SetProperty(ref _mode, value);
                if (_mode == EControlMode.Demo)
                    DemoController.Start();
                else
                {
                    DemoController.Stop();
                }
                RaisePropertyChanged(nameof(SwitchDirOffContent));
                RaisePropertyChanged(nameof(SwitchDirOnContent));
            }
        }

        public int TrainPosition
        {
            get => _trainPosition;
            set => SetProperty(ref _trainPosition, value);
        }

        public bool ModeIsVoice
        {
            get => _modeIsVoice;
            set
            {
                SetProperty(ref _modeIsVoice, value);
                if (IsInitialized)
                {
                    Mode = _modeIsVoice ? EControlMode.VoiceCommand : EControlMode.Demo;
                }
            }
        }

        public bool CanSwitch
        {
            get => _canSwitch;
            set => SetProperty(ref _canSwitch, value);
        }

        public bool IsOuterLoop
        {
            get => _isOuterLoop;
            set
            {
                SetProperty(ref _isOuterLoop, value);
                DemoController.IsLoopOuter = value;
            }
        }

        public bool IsInitialized
        {
            get => _isInitialized;
            set
            {
                SetProperty(ref _isInitialized, value);
                RaisePropertyChanged(nameof(ModeIsVoice));
            }
        }

        public string SwitchDirOnContent
        {
            get
            {
                if (Mode == EControlMode.VoiceCommand)
                {
                    return _resLoader.GetString("LoopDirLeftVoice");
                }

                return _resLoader.GetString("LoopDirLeftDemo");
            }
        }

        public string SwitchDirOffContent
        {
            get
            {
                if (Mode == EControlMode.VoiceCommand)
                {
                    return _resLoader.GetString("LoopDirRightVoice");
                }

                return _resLoader.GetString("LoopDirRightDemo");
            }
        }

        public DemoController DemoController { get; }

        #endregion

        #region constructor

        public MainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            _commandReceiver = new VoiceCommandReceiver();
            _commandReceiver.VoiceCommandReceived += OnReceivedVoiceCommand;

            _trainController = new TrainController();
            _trainController.ConnectionChagned += OnChangedTrainConnection;
            _trainController.PositionUpdated += OnUpdatedTrainPosition;

            DemoController = new DemoController(_trainController);
        }

        #endregion

        #region method

        public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            

            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
            {
                _trainController.Connect();
                await _commandReceiver.Initialize();
            });
        }

        #region Train Controller

        private async void OnUpdatedTrainPosition(object sender, int position)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => TrainPosition = position);
        }

        private async void OnChangedTrainConnection(object sender, bool isConnected)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                if (isConnected)
                {
                    Mode = ModeIsVoice ? EControlMode.VoiceCommand : EControlMode.Demo;
                    IsInitialized = true;
                }
                else
                {
                    Mode = EControlMode.Connecting;
                    IsInitialized = false;
                    _trainController.Connect();
                }
            });
        }

        private async void OnChangedTrainStatus(object sender, bool isRunning)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                CanSwitch = !isRunning;
            });
        }

        #endregion

        #region VoiceCommand

        private async void OnReceivedVoiceCommand(object sender, EVoiceCommand command)
        {
            if (!_trainController.IsConnected && !_modeIsVoice) return;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                switch (command)
                {
                    case EVoiceCommand.Go:
                        if (_isRunning) break;
                        _trainController.SetSpeed(IsOuterLoop ? EDirection.Outside : EDirection.Inside, 0x60);
                        break;
                    case EVoiceCommand.Stop:
                        if (!_isRunning) break;
                        _trainController.SetSpeed(EDirection.Stop);
                        break;
                }
            });
        }

        #endregion

        #endregion
    }
}
