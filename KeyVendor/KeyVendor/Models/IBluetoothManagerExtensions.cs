using System.Diagnostics;
using System.Threading.Tasks;

namespace KeyVendor.Models
{
    public static class IBluetoothManagerExtensions
    {
        public static async Task<bool> TurnOnBluetoothAsync(
            this IBluetoothManager bluetooth, uint timeout, uint delay)
        {
            return await Task.Run(async () =>
            {
                if (!bluetooth.IsBluetoothOn)
                {
                    bluetooth.IsBluetoothOn = true;

                    for (int i = 0; i * delay < timeout; i++)
                    {
                        if (!bluetooth.IsBluetoothOn)
                            await Task.Delay((int)delay);
                        else break;

                        Debug.WriteLine("turning BT on...");
                    }
                }

                return bluetooth.IsBluetoothOn;
            });
        }
        public static async Task<bool> StartDeviceDiscoveryAsync(
            this IBluetoothManager bluetooth, uint timeout, uint delay)
        {
            return await Task.Run(async () =>
            {
                if (!bluetooth.IsDiscovering)
                {
                    bluetooth.StartDiscovering();

                    for (int i = 0; i * delay < timeout; i++)
                    {
                        if (!bluetooth.IsDiscovering)
                            await Task.Delay((int)delay);
                        else break;

                        Debug.WriteLine("starting BT discovering...");
                    }
                }

                return bluetooth.IsDiscovering;
            });
        }
        public static async Task<BluetoothDevice> FindBluetoothDeviceByNameAsync(
            this IBluetoothManager bluetooth, string name, uint delay)
        {
            return await Task.Run(async () =>
            {
                await bluetooth.StartDeviceDiscoveryAsync(1000, delay);

                while (bluetooth.IsDiscovering)
                {
                    foreach (var device in bluetooth.DeviceList)
                    {
                        if (device.Name == name)
                        {
                            bluetooth.StopDiscovering();
                            return new BluetoothDevice() { Name = device.Name, Address = device.Address };
                        }
                    }

                    Debug.WriteLine("looking fot device with name...");
                    await Task.Delay((int)delay);
                }

                return null;
            });
        }
        public static async Task<BluetoothDevice> FindBluetoothDeviceByAddressAsync(
            this IBluetoothManager bluetooth, string address, uint delay)
        {
            return await Task.Run(async () =>
            {
                await bluetooth.StartDeviceDiscoveryAsync(1000, delay);

                while (bluetooth.IsDiscovering)
                {
                    foreach (var device in bluetooth.DeviceList)
                    {
                        if (device.Address == address)
                        {
                            bluetooth.StopDiscovering();
                            return new BluetoothDevice() { Name = device.Name, Address = device.Address };
                        }
                    }

                    Debug.WriteLine("looking fot device with address...");
                    await Task.Delay((int)delay);
                }

                return null;
            });
        }
        public static async Task<bool> BondWithBluetoothDeviceAsync(
            this IBluetoothManager bluetooth, string address, int timeout, uint delay)
        {
            return await Task.Run(async () =>
            {
                if (!bluetooth.IsBonded)
                {
                    bluetooth.CreateBond(address);

                    for (int i = 0; i * delay < timeout; i++)
                    {
                        if (!bluetooth.IsBonded)
                            await Task.Delay((int)delay);
                        else break;

                        Debug.WriteLine("trying to bond...");
                    }
                }

                return bluetooth.IsBonded;
            });
        }
        public static async Task<bool> CreateConnectionAsync(
            this IBluetoothManager bluetooth, uint timeout, uint delay)
        {
            return await Task.Run(async () =>
            {
                if (!bluetooth.IsConnected)
                {
                    bluetooth.OpenConnection();

                    for (int i = 0; i * delay < timeout; i++)
                    {
                        if (!bluetooth.IsConnected)
                            await Task.Delay((int)delay);
                        else break;

                        Debug.WriteLine("trying to connect...");
                    }
                }

                return bluetooth.IsConnected;
            });
        }
    }
}
