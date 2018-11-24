﻿using KeyVendor.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
                StartRefreshingAsync();
            }

            DeviceList = _bluetooth.DeviceList;
            InitializeCommands();
        }

        public async void StartRefreshingAsync()
        {
            if (_bluetooth.IsDiscovering)
                return;

            ButtonText = _buttonStopRefreshingText;

            await _bluetooth.TurnOnBluetoothAsync(1000, 25);
            await _bluetooth.StartDeviceDiscoveryAsync(1000, 25);

            await Task.Run(async () =>
            {
                while (_bluetooth.IsDiscovering)
                {
                    if (SelectedDevice == null || !DeviceList.Contains(SelectedDevice))
                    {
                        foreach (var item in DeviceList)
                        {
                            if ((_user.SavedAddress == "" && item.Name == _defaultDeviceName) ||
                                    item.Address == _user.SavedAddress)
                            {
                                SelectedDevice = item;
                                break;
                            }
                        }
                    }

                    await Task.Delay(25);
                }
            });

            ButtonText = _buttonStartRefreshingText;
        }
        public void StopRefreshing()
        {
            if (_bluetooth.IsDiscovering)
                _bluetooth.StopDiscovering();
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
                if (_bluetooth.IsDiscovering)
                    StopRefreshing();
                else
                    StartRefreshingAsync();
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