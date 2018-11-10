using KeyVendor.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace KeyVendor.ViewModels
{
    public class ConnectionPageViewModel : ViewModelBase
    {
        public ConnectionPageViewModel(KeyVendorUser user, IBluetoothManager bluetooth)
        {
            _user = user;
            _bluetooth = bluetooth;

            if (!_bluetooth.IsBluetoothAvailable)
            {
                MessageText = "На вашому пристрої Bluetooth не доступний! На жаль, ви не зможете користуватись цим застосунком :(";
                IsMessageVisible = true;
            }
            else
            {
                StartRefreshing();
            }

            DeviceList = _bluetooth.DeviceList;
            InitializeCommands();
        }

        public void StartRefreshing()
        {
            if (_bluetooth.IsRefreshing)
                return;

            if (!_bluetooth.IsBluetoothOn)
                _bluetooth.IsBluetoothOn = true;

            int iteration = 0;

            Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                if (_bluetooth.IsBluetoothOn)
                {
                    _bluetooth.StartRefreshing();

                    Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
                    {
                        if (SelectedDevice == null || !DeviceList.Contains(SelectedDevice))
                            foreach (var item in DeviceList)
                                if ((_user.SavedAddress == "" && item.Name == _defaultDeviceName) ||
                                        item.Address == _user.SavedAddress)
                                {
                                    SelectedDevice = item;
                                    break;
                                }

                        if (!_bluetooth.IsRefreshing)
                            ButtonText = _buttonStartRefreshingText;

                        return _bluetooth.IsRefreshing;
                    });

                    return false;
                }
                else if (iteration >= 200)
                {
                    return false;
                }

                iteration++;
                return true;
            });

            ButtonText = _buttonStopRefreshingText;
        }
        public void StopRefreshing()
        {
            if (_bluetooth.IsRefreshing)
                _bluetooth.StopRefreshing();

            ButtonText = _buttonStartRefreshingText;
        }

        public ObservableCollection<BluetoothDevice> DeviceList
        {
            get { return _deviceList; }
            set { SetProperty(ref _deviceList, value); }
        }
        public BluetoothDevice SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                if (SetProperty(ref _selectedDevice, value))
                    _user.SavedAddress = _selectedDevice.Address;
            }
        }
        public string ButtonText
        {
            get { return _buttonText; }
            set { SetProperty(ref _buttonText, value); }
        }

        public ICommand SwitchRefreshingCommand { get; protected set; }

        private void InitializeCommands()
        {
            SwitchRefreshingCommand = new Command(() => 
            {
                if (_bluetooth.IsRefreshing)
                    StopRefreshing();
                else
                    StartRefreshing();
            });
        }

        IBluetoothManager _bluetooth;
        private ObservableCollection<BluetoothDevice> _deviceList;
        private BluetoothDevice _selectedDevice;
        private string _buttonText;
        private KeyVendorUser _user;

        private const string _buttonStartRefreshingText = "Розпочати пошук";
        private const string _buttonStopRefreshingText = "Зупинити пошук";
        private const string _defaultDeviceName = "KeyVendor";
    }
}