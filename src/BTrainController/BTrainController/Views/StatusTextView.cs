using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.AllJoyn;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using BTrainController.Models;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace BTrainController.Views
{
    public sealed class StatusTextView : Control
    {
        public static readonly DependencyProperty TrainStatuProperty =
            DependencyProperty.Register(nameof(TrainStatus), typeof(ETrainStatus), typeof(StatusTextView),
                new PropertyMetadata(ETrainStatus.Initializing, OnChangedTrainStatus));

        public ETrainStatus TrainStatus
        {
            get { return (ETrainStatus) GetValue(TrainStatuProperty); }
            set { SetValue(TrainStatuProperty, value); }
        }

        private static void OnChangedTrainStatus(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var view = obj as StatusTextView;

            if (view == null) return;

            view.StatusText = _statusTextDictionary[view.TrainStatus];
        }

        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register(nameof(StatusText), typeof(string), typeof(StatusTextView), new PropertyMetadata(string.Empty));

        public string StatusText
        {
            get { return GetValue(StatusTextProperty) as string; }
            set { SetValue(StatusTextProperty, value); }
        }

        private static Dictionary<ETrainStatus, string> _statusTextDictionary;

        public StatusTextView()
        {
            this.DefaultStyleKey = typeof(StatusTextView);

            if (_statusTextDictionary == null)
            {
                _statusTextDictionary = new Dictionary<ETrainStatus, string>()
                {
                    {ETrainStatus.Initializing, "準備中"},
                    {ETrainStatus.Stop, "停車中"},
                    {ETrainStatus.CanGo, "発車待ち"},
                    {ETrainStatus.BeforeGo, "発車"},
                    {ETrainStatus.Going, "運行中"},
                    {ETrainStatus.GoingMax, "運行中(最高速)"},
                    {ETrainStatus.BeforeStop, "もうすぐ停車"},
                    {ETrainStatus.Stopping, "停車"},
                };
            }

            StatusText = _statusTextDictionary[TrainStatus];
        }
    }
}
