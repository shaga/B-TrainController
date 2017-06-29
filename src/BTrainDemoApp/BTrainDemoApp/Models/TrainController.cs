﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI.Core;

namespace BTrainDemoApp.Models
{
    public enum EBleState
    {
        Disconnected,
        Connecting,
        Connected,
        Disconnecting,
    }

    public enum EDirection
    {
        Left,
        Right,
        Stop,
    }

    public class TrainController : IDisposable
    {
        #region const

        #region Device Name

        private const string LocalShortenedName = "BTrain";
        private const string LocalCompleteName = "B-Train Controller";

        #endregion

        #region Characteristic value

        #region train position

        public const int PosUnknown = 0;
        public const int PosFront = 5;
        public const int PosFrontLeft = 4;
        public const int PosLeft = 3;
        public const int PosBackLeft = 2;
        public const int PosBack = 1;
        public const int PosBackRight = 8;
        public const int PosRight = 7;
        public const int PosFrontRight = 6;

        #endregion

        #region train Control

        private const int IdxControlDirection = 0;
        private const int IdxControlSpeed = 1;

        public const int SpeedStop = 0x00;
        public const int SpeedMin = 0x60;
        public const int SpeedMax = 0xff;

        public const int SpeedMidLow = 0x70;
        public const int SpeedMidHigh = 0x85;
        public const int SpeedHigh = 0xb0;

        public const int MaxLoopCount = 2;

        #endregion

        #endregion
        
        #region GATT UUID

        private static readonly Guid ControllerServiceGuid = Guid.Parse("237D1D00-61AF-4B22-AE78-399533CBA7C8");
        private static readonly Guid ControllerCharacteristicGuid = Guid.Parse("237D1D01-61AF-4B22-AE78-399533CBA7C8");
        private static readonly Guid PositionCharacteristicGuid = Guid.Parse("237D1D02-61AF-4B22-AE78-399533CBA7C8");

        #endregion

        #endregion

        #region field

        private readonly BluetoothLEAdvertisementWatcher _watcher;
        private BluetoothLEDevice _bleDevice;
        private GattDeviceService _controllerService;
        private GattCharacteristic _controllerCharacteristic;
        private GattCharacteristic _positionCharacteristic;

        private int _loopCount = 0;

        private bool _isRunning;

        #endregion

        #region property

        private static CoreDispatcher Dispatcher => CoreApplication.MainView.Dispatcher;

        public bool IsConnected => (_bleDevice?.ConnectionStatus ?? BluetoothConnectionStatus.Disconnected) ==
                                   BluetoothConnectionStatus.Connected;

        public int Position { get; set; }

        public bool IsLoopRight { get; set; }

        #endregion

        #region event

        public event EventHandler<int> PositionUpdated;

        public event EventHandler<bool> ConnectionChagned;

        public event EventHandler<ETrainStatus> TrainStatusChanged;

        #endregion

        #region constructor

        public TrainController()
        {
            _watcher = new BluetoothLEAdvertisementWatcher();
            _watcher.AdvertisementFilter.Advertisement.LocalName = LocalCompleteName;
            _watcher.Received += OnReceivedAdvertisement;
        }

        #endregion

        #region method

        public void Dispose()
        {
            Disconnect();
        }

        public void Connect()
        {
            _watcher.Start();
        }

        public void Disconnect()
        {
            
        }

        public void StartLoop(bool isLoopRight)
        {
            if (_isRunning) return;

            var direction = isLoopRight ? EDirection.Right : EDirection.Left;
            IsLoopRight = isLoopRight;
            SetSpeed(direction, SpeedMin);
            TrainStatusChanged?.Invoke(this, ETrainStatus.Go);
        }

        private async void SetSpeed(EDirection direction, int speed = SpeedStop)
        {
            if (!IsConnected || _controllerCharacteristic == null) return;

            if (direction == EDirection.Stop) speed = SpeedStop;
            else if (speed < 0 || speed < SpeedMin)
            {
                speed = SpeedStop;
                direction = EDirection.Stop;
            }
            else if (speed > SpeedMax)
            {
                speed = SpeedMax;
            }

            var data = new[] {(byte) direction, (byte) speed};

            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
            {
                await _controllerCharacteristic.WriteValueAsync(data.AsBuffer(), GattWriteOption.WriteWithoutResponse);
            });
        }

        private void OnReceivedAdvertisement(BluetoothLEAdvertisementWatcher watcher,
            BluetoothLEAdvertisementReceivedEventArgs e)
        {
            _watcher.Stop();

            ConnectDevice(e.BluetoothAddress);
        }

        private async void ConnectDevice(ulong address)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
            {
                _bleDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(address);

                _bleDevice.ConnectionStatusChanged += OnConnectionChanged;

                var servicesResult = await _bleDevice.GetGattServicesAsync();

                if (servicesResult.Status != GattCommunicationStatus.Success)
                {
                    _bleDevice?.Dispose();
                    _bleDevice = null;

                    _watcher.Start();
                    return;
                }

                _controllerService = servicesResult.Services.FirstOrDefault(s => s.Uuid.Equals(ControllerServiceGuid));

                if (_controllerService == null)
                {
                    _bleDevice?.Dispose();
                    _bleDevice = null;

                    _watcher.Start();
                    return;
                }

                var characteristicResult = await _controllerService.GetCharacteristicsAsync();

                if (characteristicResult.Status != GattCommunicationStatus.Success)
                {
                    _bleDevice?.Dispose();
                    _bleDevice = null;

                    _controllerService?.Dispose();
                    _controllerService = null;

                    _watcher.Start();
                    return;
                }

                _controllerCharacteristic =
                    characteristicResult.Characteristics.FirstOrDefault(
                        c => c.Uuid.Equals(ControllerCharacteristicGuid));

                _positionCharacteristic =
                    characteristicResult.Characteristics.FirstOrDefault(c => c.Uuid.Equals(PositionCharacteristicGuid));

                await _positionCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.Notify);
                _positionCharacteristic.ValueChanged += OnUpdatedPosition;

                ConnectionChagned?.Invoke(this, true);
            });
        }

        private void OnConnectionChanged(BluetoothLEDevice device, object args)
        {
            if (device == null) return;

            var isConnected = device.ConnectionStatus == BluetoothConnectionStatus.Connected;

            if (isConnected) return;

            ConnectionChagned?.Invoke(this, false);
        }

        private void OnUpdatedPosition(GattCharacteristic characteristic, GattValueChangedEventArgs e)
        {
            var data = e.CharacteristicValue?.ToArray();

            if (data == null || data.Length < 1) return;

            var position = data[0];

            Position = position;
            PositionUpdated?.Invoke(this, position);
            SetNextAction();
        }

        private void SetNextAction()
        {
            switch (Position)
            {
                case PosFront:
                    _loopCount++;
                    if (_loopCount >= MaxLoopCount)
                    {
                        _isRunning = false;
                        _loopCount = 0;
                        SetSpeed(EDirection.Stop);
                        TrainStatusChanged?.Invoke(this, ETrainStatus.Stop);
                    }
                    break;
                case PosFrontLeft:
                    if (_loopCount == 0 && IsLoopRight)
                        SetSpeed(EDirection.Right, SpeedMidLow);
                    else if (_loopCount == MaxLoopCount - 1 && !IsLoopRight)
                        SetSpeed(EDirection.Left, SpeedMin);
                    break;
                case PosLeft:
                    if (_loopCount == 0 && IsLoopRight)
                        SetSpeed(EDirection.Right, SpeedMidHigh);
                    else if (_loopCount == MaxLoopCount - 1 && !IsLoopRight)
                        SetSpeed(EDirection.Left, SpeedMidLow);
                    break;
                case PosBackLeft:
                    if (_loopCount == 0 && IsLoopRight)
                        SetSpeed(EDirection.Right, SpeedHigh);
                    else if (_loopCount == MaxLoopCount - 1 && !IsLoopRight)
                    {
                        SetSpeed(EDirection.Left, SpeedMidHigh);
                        TrainStatusChanged?.Invoke(this, ETrainStatus.Arriving);
                    }
                    break;
                case PosFrontRight:
                    if (_loopCount == 0 && !IsLoopRight)
                        SetSpeed(EDirection.Left, SpeedMidLow);
                    else if (_loopCount == MaxLoopCount - 1 && IsLoopRight)
                        SetSpeed(EDirection.Right, SpeedMin);
                    break;
                case PosRight:
                    if (_loopCount == 0 && !IsLoopRight)
                        SetSpeed(EDirection.Left, SpeedMidHigh);
                    else if (_loopCount == MaxLoopCount - 1 && IsLoopRight)
                        SetSpeed(EDirection.Right, SpeedMidLow);
                    break;
                case PosBackRight:
                    if (_loopCount == 0 && !IsLoopRight)
                        SetSpeed(EDirection.Left, SpeedHigh);
                    else if (_loopCount == MaxLoopCount - 1 && IsLoopRight)
                    {
                        SetSpeed(EDirection.Right, SpeedMidHigh);
                        TrainStatusChanged?.Invoke(this, ETrainStatus.Arriving);
                    }
                    break;
            }
        }

        #endregion
    }
}
