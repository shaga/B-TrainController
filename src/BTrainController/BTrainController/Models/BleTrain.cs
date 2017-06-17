using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.UI.Core;

namespace BTrainController.Models
{
    internal enum EBleState
    {
        Disconnected,
        Connecting,
        Connected,
        Disconnecting,
    }

    internal enum EDirection
    {
        Right,
        Left,
        Stop,
    }

    internal class BleTrain : IDisposable
    {
        #region const

        #region Device Name

        private const string LocalShortenedName = "BTrain";
        private const string LocalCompleteName = "B-Train Controller";

        #endregion

        #region Characteristic value

        public const int PosUnknown = 0;
        public const int PosFront = 5;
        public const int PosFrontLeft = 4;
        public const int PosLeft = 3;
        public const int PosBackLeft = 2;
        public const int PosBack = 1;
        public const int PosBackRight = 8;
        public const int PosRight = 7;
        public const int PosFrontRight = 6;

        public const int SpeedStop = 0x00;
        public const int SpeedMin = 0x60;
        public const int SpeedMax = 0xa0;

        private const int IdxControllerDirection = 0;
        private const int IdxControllerSpeed = 1;
        private const int IdxPosition = 0;

        #endregion

        #region GATT UUID

        private static readonly Guid ControllerServiceGuid = Guid.Parse("237D1D00-61AF-4B22-AE78-399533CBA7C8");
        private static readonly Guid ControllerCharacteristicGuid = Guid.Parse("237D1D01-61AF-4B22-AE78-399533CBA7C8");
        private static readonly Guid PositionCharacteristicGuid = Guid.Parse("237D1D02-61AF-4B22-AE78-399533CBA7C8");

        private static readonly string ControllerServiceSelector =
            GattDeviceService.GetDeviceSelectorFromUuid(ControllerServiceGuid);

        #endregion

        #endregion

        #region field

        private GattDeviceService _gattDeviceService;
        private GattCharacteristic _controllerCharacteristic;
        private GattCharacteristic _positionCharacteristic;
        private BluetoothLEAdvertisementWatcher _bleWatcher;
        private DeviceInformationCollection _controllerCollection;

        #endregion

        #region property

        private static CoreDispatcher Dispatcher = CoreApplication.MainView.Dispatcher;

        public EBleState ConnectionState { get; private set; }

        #endregion

        #region event

        public event EventHandler<EBleState> ConnectionStateChanged;

        public event EventHandler<int> TrainPositionUpdated;

        #endregion

        #region method

        public void Dispose()
        {
            switch (ConnectionState)
            {
                case EBleState.Connecting:
                    ConnectionState = EBleState.Disconnected;
                    StopScan();
                    break;
                case EBleState.Connected:
                    Disconnect();
                    break;
            }
        }

        public void Connect()
        {
            if (ConnectionState != EBleState.Disconnected) return;

            StartScan();
        }

        public void Disconnect(bool isNotify = true)
        {
            if (ConnectionState != EBleState.Connecting && ConnectionState != EBleState.Connected) return;

            if (ConnectionState == EBleState.Connecting)
            {
                StopScan();
            }

            _gattDeviceService?.Dispose();
            _gattDeviceService = null;
            _controllerCharacteristic = null;
            _positionCharacteristic = null;
            ConnectionState = EBleState.Disconnected;

            if (!isNotify) return;

            ConnectionStateChanged?.Invoke(this, EBleState.Disconnected);
        }

        public async void SetSpeed(EDirection direction, int speed = SpeedStop)
        {
            if(ConnectionState != EBleState.Connected || _controllerCharacteristic == null) return;

            if (direction >= EDirection.Stop) speed = 0;
            else if (speed <= 0 || speed < SpeedMin)
            {
                speed = 0;
                direction = EDirection.Stop;
            }
            else if (speed > SpeedMax)
            {
                speed = SpeedMax;
            }

            var data = new [] {(byte) direction, (byte) speed};

            await _controllerCharacteristic.WriteValueAsync(data.AsBuffer());
        }

        public async Task<int> GetPosition()
        {
            if (ConnectionState != EBleState.Connected || _positionCharacteristic == null) return 0;

            var data = await _positionCharacteristic.ReadValueAsync();

            return data.Value.ToArray()[0];
        }

        private async void StartScan()
        {
            if (_controllerCollection == null)
                _controllerCollection = await DeviceInformation.FindAllAsync(ControllerServiceSelector);

            if (!(_controllerCollection?.Any() ?? false)) return;

            ConnectionState = EBleState.Connecting;

            if (_bleWatcher == null)
            {
                _bleWatcher = new BluetoothLEAdvertisementWatcher();
                _bleWatcher.AdvertisementFilter.Advertisement.LocalName = LocalCompleteName;
                _bleWatcher.Received += OnReceivedAdvertisement;
            }

            _bleWatcher.Start();
        }

        private void StopScan()
        {
            _bleWatcher.Stop();
        }

        private async void OnReceivedAdvertisement(BluetoothLEAdvertisementWatcher watcher,
            BluetoothLEAdvertisementReceivedEventArgs e)
        {
            StopScan();

            if (ConnectionState != EBleState.Connecting) return;

            var deviceInformation = _controllerCollection?.FirstOrDefault();

            if (deviceInformation == null)
            {
                StartScan();
                return;
            }

            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
            {

                _gattDeviceService = await
                    GattDeviceService.FromIdAsync(deviceInformation.Id);

                if (_gattDeviceService == null)
                {
                    ConnectionState = EBleState.Disconnected;
                    return;
                }

                ConnectionState = EBleState.Connected;

                var characteristics = _gattDeviceService.GetAllCharacteristics();

                _controllerCharacteristic =
                    characteristics.FirstOrDefault(c => c.Uuid.Equals(ControllerCharacteristicGuid));
                _positionCharacteristic = characteristics.FirstOrDefault(c => c.Uuid.Equals(PositionCharacteristicGuid));
                await _positionCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.Notify);
                _positionCharacteristic.ValueChanged += OnUpdatedPosition;

                ConnectionState = EBleState.Connected;

                ConnectionStateChanged?.Invoke(this, EBleState.Connected);
            });
        }

        private void OnUpdatedPosition(GattCharacteristic characteristic, GattValueChangedEventArgs e)
        {
            var data = e.CharacteristicValue.ToArray();

            if (data == null || data.Length < 1) return;

            TrainPositionUpdated?.Invoke(this, data[0]);
        }

        #endregion
    }
}
