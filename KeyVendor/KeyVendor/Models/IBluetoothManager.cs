using System.Collections.ObjectModel;

namespace KeyVendor.Models
{
    public interface IBluetoothManager
    {
        void StartDiscovering();
        void StopDiscovering();
        void CreateBond(string address);
        void RemoveBond();
        void OpenConnection();
        void CloseConnection();
        string Read();
        void Write(string data);

        bool IsBluetoothOn { get; set; }
        bool IsBluetoothAvailable { get; }
        bool IsDiscovering { get; }
        bool IsBonded { get; }
        bool IsConnected { get; }

        ObservableCollection<KeyVendor.Models.BluetoothDevice> DeviceList { get; set; }
    }
}
