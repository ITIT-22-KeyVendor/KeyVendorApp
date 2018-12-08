using KeyVendor.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace KeyVendor.ViewModels
{
    public class KeyManagementPageViewModel : ViewModelBase
    {
        public KeyManagementPageViewModel(KeyVendorUser user, IBluetoothManager bluetooth)
        {
            _bluetooth = bluetooth;
            _user = user;

            KeyList = "";
        }

        public async void SetKeyList()
        {
            StartActivityIndication(TextConstants.ActivitySettingKeyList);

            await Task.Run(async () =>
            {
                if (!await _bluetooth.TurnOnBluetoothAsync(1000, 25))
                {
                    ShowMessage(TextConstants.BluetoothTurnOnFail, TextConstants.ButtonClose);
                    return;
                }
                if (!await _bluetooth.CreateConnectionAsync(5000, 50))
                {
                    ShowMessage(TextConstants.BluetoothConnectionFail, TextConstants.ButtonClose);
                    return;
                }

                var splittedData = KeyList.Split('\n').Where(key => 
                    key != "" && key != "\n" && !String.IsNullOrWhiteSpace(key));
                string data = String.Join("@", splittedData);

                KeyVendorCommand setKeyListCommand = new KeyVendorCommand
                {
                    UserUUID = _user.UUID,
                    Time = DateTime.Now,
                    CommandType = KeyVendorCommandType.SetKeyList,
                    Data = data
                };
                KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
                KeyVendorAnswer answer = await terminal.ExecuteCommandAsync(setKeyListCommand, 10000, 100);

                if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
                    ShowMessage(TextConstants.ErrorSetKeyListFail, TextConstants.ButtonClose);
                else
                    ShowMessage(TextConstants.SuccessKeyListSet, TextConstants.ButtonClose);
            });

            StopActivityIndication();
        }
        
        public string KeyList
        {
            get { return _keyList; }
            set
            {
                if (SetProperty(ref _keyList, value))
                    UpdateCommands();
            }
        }

        public ICommand SetKeyListCommand { get; protected set; }
        public ICommand ClearKeyListCommand { get; protected set; }

        protected override void InitializeCommands()
        {
            SetKeyListCommand = new Command(
                () => { SetKeyList(); },
                () => { return KeyList != ""; });
            ClearKeyListCommand = new Command(
                () => { KeyList = ""; },
                () => { return KeyList != "" && !IsMessageVisible && !IsActivityIndicationVisible; });
        }
        protected override void UpdateCommands()
        {
            ((Command)SetKeyListCommand).ChangeCanExecute();
            ((Command)ClearKeyListCommand).ChangeCanExecute();
        }
                
        private string _keyList;
        private IBluetoothManager _bluetooth;
        private KeyVendorUser _user;
    }
}
