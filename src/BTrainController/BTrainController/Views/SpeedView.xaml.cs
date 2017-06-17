using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BTrainController.Models;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BTrainController.Views
{
    public sealed partial class SpeedView : UserControl
    {
        public static readonly DependencyProperty SpeedLevelProperty =
            DependencyProperty.Register(nameof(SpeedLevel), typeof(ESpeedLevel), typeof(SpeedView), new PropertyMetadata(ESpeedLevel.Stop));

        public ESpeedLevel SpeedLevel
        {
            get { return (ESpeedLevel) GetValue(SpeedLevelProperty); }
            set { SetValue(SpeedLevelProperty, value); }
        }

        public SpeedView()
        {
            this.InitializeComponent();
        }
    }
}
