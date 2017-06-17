using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BTrainController.Models
{
    public enum ESoundType
    {
        None,
        Bell,
        Drive,
        DoorOpen,
        DoorClose,
    }

    class TrainSound
    {
        private const string SoundFilePathFmt = "ms-appx:///Sounds/{0}.mp3";

        private static readonly Uri SoundPathBell = new Uri(string.Format(SoundFilePathFmt, "train-bell1"));
        private static readonly Uri SoundPathDrive = new Uri(string.Format(SoundFilePathFmt, "train-driving2"));
        private static readonly Uri SoundPathDoorOpen = new Uri(string.Format(SoundFilePathFmt, "train-door1"));
        private static readonly Uri SoundPathDoorClose = new Uri(string.Format(SoundFilePathFmt, "traindoor_close"));

        private ESoundType _typePlaying;

        private MediaElement _mediaElement;

        private StorageFile _soundBell;

        private StorageFile _soundDrive;

        private StorageFile _soundDoorOpen;

        private StorageFile _soundDoorClose;

        private static CoreDispatcher Dispatcher => CoreApplication.MainView.Dispatcher;

        public event EventHandler Initizlized;

        public event EventHandler<ESoundType> SoundPlayEnded;

        public async void Initialise(MediaElement mediaElement)
        {
            _mediaElement = mediaElement;
            _mediaElement.MediaEnded += OnMediaEnded;

            _soundBell = await StorageFile.GetFileFromApplicationUriAsync(SoundPathBell);
            _soundDrive = await StorageFile.GetFileFromApplicationUriAsync(SoundPathDrive);
            _soundDoorOpen = await StorageFile.GetFileFromApplicationUriAsync(SoundPathDoorOpen);
            _soundDoorClose = await StorageFile.GetFileFromApplicationUriAsync(SoundPathDoorClose);

            Initizlized?.Invoke(this, EventArgs.Empty);
        }

        public async void PlayAsync(ESoundType type)
        {
            if (type == _typePlaying) return;

            _typePlaying = type;
            await _mediaElement.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                _mediaElement.Stop();
                StorageFile file;
                switch (type)
                {
                    case ESoundType.Bell:
                        _mediaElement.IsLooping = false;
                        file = _soundBell;
                        break;
                    case ESoundType.Drive:
                        _mediaElement.IsLooping = true;
                        file = _soundDrive;
                        break;
                    case ESoundType.DoorOpen:
                        _mediaElement.IsLooping = false;
                        file = _soundDoorOpen;
                        break;
                    case ESoundType.DoorClose:
                        _mediaElement.IsLooping = false;
                        file = _soundDoorClose;
                        break;
                    default:
                        return;
                }

                var stream = await file.OpenAsync(FileAccessMode.Read);
                _mediaElement.SetSource(stream, file.ContentType);
                _mediaElement.Play();
            });
        }

        public async Task PlayBellAsync()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(SoundPathBell);
                var stream = await file.OpenAsync(FileAccessMode.Read);
                _mediaElement.SetSource(stream, file.ContentType);
                Debug.WriteLine("start");
                _mediaElement.Play();
            });
        }

        private void OnMediaEnded(object sender, RoutedEventArgs e)
        {
            if (_typePlaying == ESoundType.Drive || _typePlaying == ESoundType.None) return;

            SoundPlayEnded?.Invoke(this, _typePlaying);
            _typePlaying = ESoundType.None;
        }
    }
}
