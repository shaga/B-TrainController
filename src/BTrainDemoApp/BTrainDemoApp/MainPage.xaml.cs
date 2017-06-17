using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BTrainDemoApp.Data;
using BTrainDemoApp.Models;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace BTrainDemoApp
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Loaded += OnLoaded;
            Unloaded += (s, e) =>
            {
                _timer?.Stop();
            };
        }

        private VoiceCommandReceiver _receiver;
        private DispatcherTimer _timer;
        private StationInfo _info;
        private OsakaLoopLineStations _stations;
        private async  void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _receiver = new VoiceCommandReceiver();
            await _receiver.Initialize();

            //_stations = new OsakaLoopLineStations();
            //_info = _stations[OsakaLoopLineStations.DefaultKey];
            //OsakaLoopLineSign.StationInfo = _info;
            //_timer = new DispatcherTimer();
            //_timer.Tick += async (s, e) =>
            //{
            //    _info = _stations.GetNextOutStationInfo(_info.NameEng);
            //    await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            //    {
            //        OsakaLoopLineSign.StationInfo = _info;
            //    });
            //};
            //_timer.Interval = TimeSpan.FromSeconds(3);
            //_timer.Start();
        }
    }
}
