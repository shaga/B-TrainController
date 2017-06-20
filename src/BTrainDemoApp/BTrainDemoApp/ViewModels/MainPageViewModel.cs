using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
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

        private EControlMode _mode;
        private int _trainPosition;

        #endregion

        #region property

        private static CoreDispatcher Dispatcher => CoreApplication.MainView.Dispatcher;

        public EControlMode Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }

        public int TrainPosition
        {
            get => _trainPosition;
            set => SetProperty(ref _trainPosition, value);
        }

        #endregion


        public MainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            _commandReceiver = new VoiceCommandReceiver();
            _trainController = new TrainController();
        }

        public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            

            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
            {
                Mode = EControlMode.VoiceCommand;
                _trainController.Connect();
                await _commandReceiver.Initialize();
            });
        }
    }
}
