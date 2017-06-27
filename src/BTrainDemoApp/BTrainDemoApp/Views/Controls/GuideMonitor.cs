using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using BTrainDemoApp.Annotations;
using BTrainDemoApp.Models;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace BTrainDemoApp.Views.Controls
{
    public sealed class GuideMonitor : Control, INotifyPropertyChanged
    {
        #region implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public static readonly DependencyProperty StatusMotionProperty =
            DependencyProperty.Register(nameof(StatusMotion), typeof(string), typeof(GuideMonitor), new PropertyMetadata(null));

        public string StatusMotion
        {
            get => GetValue(StatusMotionProperty) as string;
            set => SetValue(StatusMotionProperty, value);
        }

        public static readonly DependencyProperty TrainDirectionProperty =
            DependencyProperty.Register(nameof(TrainDirection), typeof(string), typeof(GuideMonitor), new PropertyMetadata(null));

        public string TrainDirection
        {
            get => GetValue(TrainDirectionProperty) as string;
            set => SetValue(TrainDirectionProperty, value);
        }

        public static readonly DependencyProperty StationNameProperty =
            DependencyProperty.Register(nameof(StationName), typeof(string), typeof(GuideMonitor), new PropertyMetadata(null));

        public string StationName
        {
            get => GetValue(StationNameProperty) as string;
            set => SetValue(StationNameProperty, value);
        }

        public static readonly DependencyProperty IsOuterLoopProperty =
            DependencyProperty.Register(nameof(IsOuterLoop), typeof(bool), typeof(GuideMonitor), new PropertyMetadata(false, OnChangedIsOuterLoop));

        private static void OnChangedIsOuterLoop(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var monitor = obj as GuideMonitor;

            monitor?.UpdateVisibility();
        }

        public bool IsOuterLoop
        {
            get => (bool)GetValue(IsOuterLoopProperty);
            set => SetValue(IsOuterLoopProperty, value);
        }

        public static readonly DependencyProperty TrainStatusProperty =
            DependencyProperty.Register(nameof(TrainStatus), typeof(ETrainStatus), typeof(GuideMonitor), new PropertyMetadata(ETrainStatus.Stop, OnChangedTrainStatus));

        private static void OnChangedTrainStatus(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var monitor = obj as GuideMonitor;

            if (monitor == null) return;

            monitor.UpdateVisibility();

            switch (monitor.TrainStatus)
            {
                case ETrainStatus.Go:
                    monitor.StatusMotion = "つぎは";
                    break;
                case ETrainStatus.Arriving:
                    monitor.StatusMotion = "まもなく";
                    break;
                default:
                    monitor.StatusMotion = "ただいま";
                    break;
            }
        }

        public ETrainStatus TrainStatus
        {
            get => (ETrainStatus) GetValue(TrainStatusProperty);
            set => SetValue(TrainStatusProperty, value);
        }

        public static readonly DependencyProperty LoopStationInfoProperty =
            DependencyProperty.Register(nameof(LoopStationInfo), typeof(LoopStationInfo), typeof(GuideMonitor), new PropertyMetadata(null, OnChangedLoopStationInfo));

        private static void OnChangedLoopStationInfo(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var monitor = obj as GuideMonitor;

            if (monitor == null) return;

            var info = monitor.LoopStationInfo;

            monitor.NextCircleRow = info?.Station.Row ?? 0;
            monitor.NextCircleCol = info?.Station.Col ?? 0;

            monitor.ArrowRightRow = info?.ArrowOuter.Row ?? 0;
            monitor.ArrowRightCol = info?.ArrowOuter.Col ?? 0;

            monitor.ArrowLeftRow = info?.ArrowInner.Row ?? 0;
            monitor.ArrowLeftCol = info?.ArrowInner.Col ?? 0;

            monitor.TrainDirection = monitor.IsOuterLoop ? info?.DirOuter : info?.DirInner;

            monitor.StationName = info?.StationName;

            monitor.UpdateVisibility();
        }

        public LoopStationInfo LoopStationInfo
        {
            get => GetValue(LoopStationInfoProperty) as LoopStationInfo;
            set => SetValue(LoopStationInfoProperty, value);
        }
        #region field

        private Visibility _nextCircleCircle;
        private Visibility _arrowRightStopVisibility;
        private Visibility _arrowLeftStopVisibility;
        private Visibility _arrowRightVisibility;
        private Visibility _arrowLeftVisibility;
        private int _nextCircleRow;
        private int _nextCircleCol;
        private int _arrowRightRow;
        private int _arrowLeftCol;
        private int _arrowRightCol;
        private int _arrowLeftRow;

        #endregion

        #region property

        #region next circle

        public Visibility NextCircleVisibility
        {
            get => _nextCircleCircle;
            set
            {
                if (_nextCircleCircle == value) return;
                _nextCircleCircle = value;
                RaisePropertyChanged();
            }
        }

        public int NextCircleRow
        {
            get => _nextCircleRow;
            set
            {
                if (_nextCircleRow == value) return;
                _nextCircleRow = value;
                RaisePropertyChanged();
            }
        }

        public int NextCircleCol
        {
            get => _nextCircleCol;
            set
            {
                if (_nextCircleCol == value) return;
                _nextCircleCol = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region current station right arrow

        public Visibility ArrowRightStopVisibility
        {
            get => _arrowRightStopVisibility;
            set
            {
                if (_arrowRightStopVisibility == value) return;;
                _arrowRightStopVisibility = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region current station left arrow

        public Visibility ArrowLeftStopVisibility
        {
            get => _arrowLeftStopVisibility;
            set
            {
                if (_arrowLeftStopVisibility == value) return;
                _arrowLeftStopVisibility = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region direction arrow right

        public Visibility ArrowRightVisibility
        {
            get => _arrowRightVisibility;
            set
            {
                if (_arrowRightVisibility == value) return;
                _arrowRightVisibility = value;
                RaisePropertyChanged();
            }
        }

        public int ArrowRightRow
        {
            get => _arrowRightRow;
            set
            {
                if (_arrowRightRow == value) return;
                _arrowRightRow = value;
                RaisePropertyChanged();
            }
        }

        public int ArrowRightCol
        {
            get => _arrowRightCol;
            set
            {
                if (_arrowRightCol == value) return;
                _arrowRightCol = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region direction arrow inner

        public Visibility ArrowLeftVisibility
        {
            get => _arrowLeftVisibility;
            set
            {
                if (_arrowLeftVisibility == value) return;
                _arrowLeftVisibility = value;
                RaisePropertyChanged();
            }
        }

        public int ArrowLeftRow
        {
            get => _arrowLeftRow;
            set
            {
                if (_arrowLeftRow == value) return;
                _arrowLeftRow = value;
                RaisePropertyChanged();
            }
        }

        public int ArrowLeftCol
        {
            get => _arrowLeftCol;
            set
            {
                if (_arrowLeftCol == value) return;
                _arrowLeftCol = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        public GuideMonitor()
        {
            this.DefaultStyleKey = typeof(GuideMonitor);

            StatusMotion = "つぎは";
            TrainDirection = "大阪・西九条";
        }

        private void UpdateVisibility()
        {
            if (LoopStationInfo == null) return;

            var isStop = TrainStatus == ETrainStatus.Stop;

            NextCircleVisibility = !isStop ? Visibility.Visible : Visibility.Collapsed;

            if (LoopStationInfo.IsUpper)
            {
                ArrowLeftVisibility = !isStop && !IsOuterLoop ? Visibility.Visible : Visibility.Collapsed;
                ArrowRightVisibility = !isStop && IsOuterLoop ? Visibility.Visible : Visibility.Collapsed;
                ArrowLeftStopVisibility = isStop && !IsOuterLoop ? Visibility.Visible : Visibility.Collapsed;
                ArrowRightStopVisibility = isStop && IsOuterLoop ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                ArrowLeftVisibility = !isStop && IsOuterLoop ? Visibility.Visible : Visibility.Collapsed;
                ArrowRightVisibility = !isStop && !IsOuterLoop ? Visibility.Visible : Visibility.Collapsed;
                ArrowLeftStopVisibility = isStop && IsOuterLoop ? Visibility.Visible : Visibility.Collapsed;
                ArrowRightStopVisibility = isStop && !IsOuterLoop ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
