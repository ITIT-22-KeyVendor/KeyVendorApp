using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace KeyVendor.Models
{
    public interface IBluetoothManager
    {
        void StartRefreshing();
        void StopRefreshing();
        void CreateBond(string address);
        void RemoveBond();
        void OpenConnection();
        void CloseConnection();
        string Read();
        void Write(string data);

        bool IsBluetoothOn { get; set; }
        bool IsBluetoothAvailable { get; }
        bool IsRefreshing { get; }
        bool IsBonded { get; }
        bool IsConnected { get; }

        ObservableCollection<KeyVendor.Models.BluetoothDevice> DeviceList { get; set; }
    }
}