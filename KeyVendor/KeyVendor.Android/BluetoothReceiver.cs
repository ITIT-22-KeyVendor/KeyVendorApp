using Android.Content;
using System.Collections.ObjectModel;

namespace KeyVendor.Droid
{
    [BroadcastReceiver(Enabled = true, Label = "receiver")]
    public class BluetoothReceiver : BroadcastReceiver
    {
        public BluetoothReceiver()
        {
            DeviceList = new ObservableCollection<Models.BluetoothDevice>();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            System.String action = intent.Action;
            if (Android.Bluetooth.BluetoothDevice.ActionFound.Equals(action))
            {
                Android.Bluetooth.BluetoothDevice device =
                    (Android.Bluetooth.BluetoothDevice)intent.GetParcelableExtra(Android.Bluetooth.BluetoothDevice.ExtraDevice);

                var btDevice = new KeyVendor.Models.BluetoothDevice()
                {
                    Name = device.Name,
                    Address = device.Address
                };

                bool itemAlreadyAdded = false;

                foreach (var item in DeviceList)
                {
                    if (item.Address == btDevice.Address)
                    {
                        itemAlreadyAdded = true;
                        break;
                    }
                }

                if (!itemAlreadyAdded)
                    DeviceList.Add(btDevice);
            }
        }
        public void ClearDeviceList()
        {
            DeviceList.Clear();
        }

        public ObservableCollection<KeyVendor.Models.BluetoothDevice> DeviceList { get; private set; }
    }
}