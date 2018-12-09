using KeyVendor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Devices.Radios;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.UI.Popups;
using Xamarin.Forms;

[assembly: Dependency(typeof(KeyVendor.UWP.BluetoothManager))]

namespace KeyVendor.UWP
{
    public class BluetoothManager : IBluetoothManager
    {
        public bool IsBluetoothOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsBluetoothAvailable => throw new NotImplementedException();

        public bool IsDiscovering => throw new NotImplementedException();

        public bool IsBonded => throw new NotImplementedException();

        public bool IsConnected => throw new NotImplementedException();

        public ObservableCollection<Models.BluetoothDevice> DeviceList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void CloseConnection()
        {
            throw new NotImplementedException();
        }

        public void CreateBond(string address)
        {
            throw new NotImplementedException();
        }

        public void OpenConnection()
        {
            throw new NotImplementedException();
        }

        public string Read()
        {
            throw new NotImplementedException();
        }

        public void RemoveBond()
        {
            throw new NotImplementedException();
        }

        public void StartDiscovering()
        {
            throw new NotImplementedException();
        }

        public void StopDiscovering()
        {
            throw new NotImplementedException();
        }

        public void Write(string data)
        {
            throw new NotImplementedException();
        }
    }
}
