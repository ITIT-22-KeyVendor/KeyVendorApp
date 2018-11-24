using Android.App;
using Android.Bluetooth;
using Android.Content;
using Java.IO;
using Java.Lang;
using Java.Lang.Reflect;
using KeyVendor.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(KeyVendor.Droid.BluetoothManager))]

namespace KeyVendor.Droid
{
    public class BluetoothManager : IBluetoothManager
    {
        public BluetoothManager()
        {
            receiver = new BluetoothReceiver();
            DeviceList = receiver.DeviceList;
            
            IntentFilter filter = new IntentFilter(Android.Bluetooth.BluetoothDevice.ActionFound);
            var mainActivity = ((Activity)Forms.Context).Window.DecorView;
            Forms.Context.RegisterReceiver(receiver, filter);
        }

        public void StartDiscovering()
        {
            if (bluetoothAdapter == null)
                return;

            receiver.ClearDeviceList();

            foreach (var item in bluetoothAdapter.BondedDevices)
            {
                DeviceList.Add(new KeyVendor.Models.BluetoothDevice()
                {
                    Name = item.Name,
                    Address = item.Address
                });
            }

            bluetoothAdapter.StartDiscovery();
        }
        public void StopDiscovering()
        {
            if (bluetoothAdapter == null)
                return;

            bluetoothAdapter.CancelDiscovery();
        }
        public void CreateBond(string address)
        {
            StopDiscovering();

            if (bluetoothAdapter == null)
                return;

            if (device == null || device.Address != address)
                device = bluetoothAdapter.GetRemoteDevice(address);

            if (device.BondState != Bond.Bonded)
                device.CreateBond();
        }
        public void RemoveBond()
        {
            StopDiscovering();

            if (device != null && device.BondState == Bond.Bonded)
            {
                Method m = device.Class.GetMethod("removeBond", (Class[])null);
                m.Invoke(device, (Java.Lang.Object[])null);
            }
        }
        public void OpenConnection()
        {
            StopDiscovering();

            if (bluetoothAdapter == null)
                return;

            if (device == null || device.BondState != Bond.Bonded)
                return;

            try
            {
                var deviceClass = device.Class;
                Class[] paramTypes = new Class[] { Integer.Type };
                Method m = deviceClass.GetMethod("createRfcommSocket", paramTypes);
                Java.Lang.Object[] parameters = new Java.Lang.Object[] { Integer.ValueOf(1) };

                socket = (BluetoothSocket)m.Invoke(device, parameters);
                socket.Connect();
            }
            catch (System.Exception ex)
            {
                CloseConnection();
                Debug.WriteLine(ex.Message);
            }
        }
        public void CloseConnection()
        {
            StopDiscovering();

            if (socket != null)
                socket.Close();
        }
        public void Write(string data)
        {
            if (socket == null || !socket.IsConnected)
                return;

            using (var stream = socket.OutputStream)
                using (var writer = new BufferedWriter(new OutputStreamWriter(stream)))
                {
                    try
                    {
                       writer.Write(data);
                       writer.Flush();
                       Debug.Write("DATA HAS BEEN WRITTEN");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.WriteLine("EXCEPTION: " + ex.Message);
                    }
                }
        }
        public string Read()
        {
            string data = "";

            if (socket == null || !socket.IsConnected)
                return data;

            using (var stream = socket.InputStream)
            using (var reader = new BufferedReader(new InputStreamReader(stream)))
            {
                if (reader.Ready())
                {
                    try
                    {
                        Debug.WriteLine("TRYING TO READ DATA");

                        System.String line = "";
                        while (line != null && reader.Ready())
                        {
                            line = reader.ReadLine();
                            if (line != null)
                            {
                                Debug.WriteLine(line);
                                data += line;
                            }
                        }
                        
                        if (data == null) data = "";
                        Debug.WriteLine("DATA: " + data);
                    }
                    catch (Java.Lang.Exception ex)
                    {
                        Debug.WriteLine("java EXCEPTION: " + ex.Message);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.WriteLine("c# EXCEPTION: " + ex.Message);
                    }
                }
            }

            return data;
        }        

        public bool IsBluetoothAvailable
        {
            get
            {
                if (bluetoothAdapter == null)
                    return false;
                else
                    return true;
            }
        }
        public bool IsDiscovering
        {
            get
            {
                return bluetoothAdapter.IsDiscovering;
            }
        }
        public bool IsBluetoothOn
        {
            get
            {
                if (bluetoothAdapter == null)
                    return false;
                else
                    return bluetoothAdapter.IsEnabled;
            }
            set
            {
                if (bluetoothAdapter == null)
                    return;
                else
                {
                    if (value == true)
                        bluetoothAdapter.Enable();
                    else
                        bluetoothAdapter.Disable();
                }
            }
        }
        public bool IsBonded
        {
            get
            {
                if (device != null && device.BondState == Bond.Bonded)
                    return true;

                return false;
            }
        }
        public bool IsConnected
        {
            get
            {
                if (socket != null && socket.IsConnected)
                    return true;

                return false;
            }
        }

        public ObservableCollection<KeyVendor.Models.BluetoothDevice> DeviceList { get; set; }

        private BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
        private Android.Bluetooth.BluetoothDevice device;
        private BluetoothReceiver receiver;
        private BluetoothSocket socket;
    };
}