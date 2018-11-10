using KeyVendor.Models;
using System;
using System.Collections.Generic;
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
        public async Task TurnBluetoothOn()
        {
            try
            {
                var message = new MessageDialog("");

                var access = await Radio.RequestAccessAsync();
                message.Content = access.ToString();
                message.ShowAsync();
                Debug.WriteLine("ACCESS: " + access.ToString());
                if (access != RadioAccessStatus.Allowed)
                    return;

                BluetoothAdapter adapter = await BluetoothAdapter.GetDefaultAsync();
                message.Content = adapter.ToString();
                message.ShowAsync();
                Debug.WriteLine("ADAPTER: " + adapter.ToString());
                if (adapter == null)
                    return;

                var radio = await adapter.GetRadioAsync();
                await radio.SetStateAsync(RadioState.On);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task TurnBluetoothOff()
        {
            try
            {
                var access = await Radio.RequestAccessAsync();
                Debug.WriteLine("ACCESS: " + access.ToString());
                if (access != RadioAccessStatus.Allowed)
                    return;

                BluetoothAdapter adapter = await BluetoothAdapter.GetDefaultAsync();
                Debug.WriteLine("ADAPTER: " + adapter.ToString());
                if (adapter == null)
                    return;

                var radio = await adapter.GetRadioAsync();
                await radio.SetStateAsync(RadioState.Off);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void StartWatcher()
        {            
            _deviceWatcher = DeviceInformation.CreateWatcher(
                BluetoothDevice.GetDeviceSelectorFromDeviceName("alcatel"),
                null,
                DeviceInformationKind.AssociationEndpoint);
            _deviceWatcher.Added += DeviceWatcher_Added;
            _deviceWatcher.Removed += DeviceWatcher_Removed;
            _deviceWatcher.Start();
        }
        public void StopWatcher(NavigationEventArgs e)
        {
            _deviceWatcher.Stop();
        }
        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            //throw new NotImplementedException(); 
        }
        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            var message = new MessageDialog("");

            try
            {
                var device = await BluetoothDevice.FromIdAsync(args.Id);
                var services = await device.GetRfcommServicesAsync();
                                
                message.Content += device.BluetoothAddress + "  " + device.BluetoothDeviceId + "  " +
                    device.ClassOfDevice + "  " + device.DeviceInformation;
                 
                foreach (var service in services.Services)
                {
                    Debug.WriteLine("Service:" + service.ServiceId);
                    var characteristics = await service.GetSdpRawAttributesAsync();
                        
                    foreach (var character in characteristics)
                    {
                        Debug.WriteLine("Characteristic: " + character.Key + "  " + character.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            message.ShowAsync();
        }

        private DeviceWatcher _deviceWatcher;
        private RfcommDeviceService _service;
        private StreamSocket _socket;
    }
}
