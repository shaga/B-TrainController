using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace BTrainController.Models
{
    internal class CamFaceDetector : IDisposable
    {
        #region const

        private static readonly TimeSpan IntervalCameraUpdate = TimeSpan.FromMilliseconds(66);

        private const int CameraWidth = 320;
        private const int CameraHeight = 240;


        #endregion

        #region field

        private CaptureElement _captureElement;
        private MediaCapture _mediaCapture;
        private bool _isPreviewing;
        private FaceTracker _faceTracker;
        private ThreadPoolTimer _timer;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private bool _isFaceExist;

        #endregion

        #region property

        private static CoreDispatcher Dispatcher => CoreWindow.GetForCurrentThread().Dispatcher;

        #endregion

        #region event

        public event EventHandler<bool> InitalizedStateChanged;

        public event EventHandler<bool> IsExistFaceChanged;

        #endregion

        #region constructor

        public CamFaceDetector()
        {

        }

        #endregion

        #region method

        public async void Dispose()
        {
            if (_isPreviewing && _mediaCapture != null)
            {
                await _mediaCapture.StopPreviewAsync();
            }

            _mediaCapture?.Dispose();
            _mediaCapture = null;
        }

        public async void Initialize(CaptureElement element)
        {
            _captureElement = element;

            if (_faceTracker == null) _faceTracker = await FaceTracker.CreateAsync();

            _timer?.Cancel();
            _timer = null;

            try
            {
                if (_isPreviewing)
                {
                    if (_mediaCapture != null) await _mediaCapture.StopPreviewAsync();
                }

                _mediaCapture?.Dispose();
                _mediaCapture = null;

                var camId = await GetCameraDeviceId();

                var setting = new MediaCaptureInitializationSettings()
                {
                    VideoDeviceId = camId,
                    StreamingCaptureMode = StreamingCaptureMode.Video,
                };

                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync(setting);

                var vp = new VideoEncodingProperties
                {
                    Width = CameraWidth,
                    Height = CameraHeight,
                    Subtype = "NV12"
                };

                await _mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, vp);

                _captureElement.Source = _mediaCapture;

                await _mediaCapture.StartPreviewAsync();

                _isPreviewing = true;

                _timer = ThreadPoolTimer.CreatePeriodicTimer(OnCaptureFrame, IntervalCameraUpdate);

                InitalizedStateChanged?.Invoke(this, true);
            }
            catch (Exception e)
            {
                _timer?.Cancel();
                _timer = null;

                InitalizedStateChanged?.Invoke(this, false);
                Debug.WriteLine(e.Message);
            }
        }

        private async Task<string> GetCameraDeviceId(string deviceName = null)
        {
            var cams = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            if (!(cams?.Any() ?? false)) throw new Exception("Camera not found");

            var cam = null as DeviceInformation;

            if (string.IsNullOrEmpty(deviceName) || !cams.Any(d => d.Name.Equals(deviceName)))
            {
                cam = cams.LastOrDefault();
            }
            else
            {
                cam = cams.LastOrDefault(d => d.Name.Equals(deviceName));
            }

            return cam.Id;
        }

        private async void OnCaptureFrame(ThreadPoolTimer timer)
        {
            if (!_semaphore.Wait(0)) return;

            try
            {
                using (var frame = new VideoFrame(BitmapPixelFormat.Nv12, CameraWidth, CameraHeight))
                {
                    await _mediaCapture.GetPreviewFrameAsync(frame);


                    if (!FaceDetector.IsBitmapPixelFormatSupported(frame.SoftwareBitmap.BitmapPixelFormat)) return;

                    var faces = await _faceTracker.ProcessNextFrameAsync(frame);

                    var foundFace = faces?.Any() ?? false;

                    if (_isFaceExist == foundFace) return;

                    _isFaceExist = foundFace;
                    IsExistFaceChanged?.Invoke(this, _isFaceExist);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}/{e.InnerException?.Message ?? "inner null"}");
            }
            finally
            {
                _semaphore.Release();
            }
            
        }

        #endregion
    }
}
