using KeyVendor.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace KeyVendor.ViewModels
{
    public class KeyManagementPageViewModel : ViewModelBase
    {
        public KeyManagementPageViewModel(KeyVendorUser user, IBluetoothManager bluetooth)
        {
            InitCommands();
        }

        public async void SetKeyList()
        {

        }

        public ICommand SetKeyListCommand { get; protected set; }

        private void InitCommands()
        {
            SetKeyListCommand = new Command(
                () => { SetKeyList(); },
                () => { return !IsMessageVisible/* && !IsActivityIndicationVisible*/; });
        }
        public override void UpdateCommands()
        {
            ((Command)SetKeyListCommand).ChangeCanExecute();
        }
    }
}
