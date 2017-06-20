using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BTrainDemoApp.Data;
using BTrainDemoApp.Models;
using Prism.Windows.Mvvm;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace BTrainDemoApp.Views
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : SessionStateAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
    }
}
