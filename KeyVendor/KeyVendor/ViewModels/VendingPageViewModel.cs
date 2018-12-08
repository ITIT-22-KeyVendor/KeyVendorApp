using KeyVendor.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace KeyVendor.ViewModels
{
    public class VendingPageViewModel : ViewModelBase
    {
        public VendingPageViewModel(KeyVendorUser user, IBluetoothManager bluetooth)
        {
            _bluetooth = bluetooth;
            _user = user;

            IsToolbarVisible = _user.HasAdminRights;

            KeyList = new ObservableCollection<string>();

            GetKeyListAsync();
        }

        public void OpenKeyManagementPage()
        {
            OnOpenKeyManagementPage(this, new KeyManagementPageViewModel(_user, _bluetooth));
        }
        public void OpenUserManagementPage()
        {
            OnOpenUserManagementPage(this, new UserManagementPageViewModel(_user, _bluetooth));
        }
        public void OpenLogPage()
        {
            OnOpenLogPage(this, new LogPageViewModel(_user, _bluetooth));
        }

        public ObservableCollection<string> KeyList
        {
            get { return _keyList; }
            set { SetProperty(ref _keyList, value); }
        }
        public string SelectedKey
        {
            get { return _selectedKey; }
            set
            {
                if (SetProperty(ref _selectedKey, value))
                    UpdateCommands();
            }
        }
        public bool GettingKey
        {
            get { return _gettingKey; }
            set
            {
                if (SetProperty(ref _gettingKey, value))
                    Device.BeginInvokeOnMainThread(() => UpdateCommands());
            }
        }
        public bool IsToolbarVisible
        {
            get { return _isToolbarVisible; }
            set { SetProperty(ref _isToolbarVisible, value); }
        }

        public event EventHandler<KeyManagementPageViewModel> OnOpenKeyManagementPage;
        public event EventHandler<UserManagementPageViewModel> OnOpenUserManagementPage;
        public event EventHandler<LogPageViewModel> OnOpenLogPage;

        public ICommand OpenKeyManagementPageCommand { get; protected set; }
        public ICommand OpenUserManagementPageCommand { get; protected set; }
        public ICommand OpenLogPageCommand { get; protected set; }
        public ICommand RefreshCommand { get; protected set; }
        public ICommand GetKeyCommand { get; protected set; }
        
        private async void GetKeyListAsync()
        {
            StartActivityIndication();

            await Task.Run(async () =>
            {
                KeyList.Clear();

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

                KeyVendorCommand getKeyListCommand = new KeyVendorCommand
                {
                    UserUUID = _user.UUID,
                    Time = DateTime.Now,
                    CommandType = KeyVendorCommandType.GetKeyList,
                    Data = ""
                };
                KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
                KeyVendorAnswer answer = await terminal.ExecuteCommandAsync(getKeyListCommand, 5000, 100);

                if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
                {
                    ShowMessage(TextConstants.ErrorGetKeyListFail, TextConstants.ButtonClose);
                    return;
                }

                string answerData = answer.Data;
                string[] dataArray = answerData.Split('@');

                foreach (var item in dataArray)
                    KeyList.Add(item);
            });

            StopActivityIndication();
        }
        private async void GetKeyAsync()
        {
            GettingKey = true;

            await Task.Run(async () =>
            {
                if (!await _bluetooth.TurnOnBluetoothAsync(1000, 25))
                {
                    GettingKey = false;
                    ShowMessage(TextConstants.BluetoothTurnOnFail, TextConstants.ButtonClose);
                    return;
                }
                if (!await _bluetooth.CreateConnectionAsync(5000, 50))
                {
                    GettingKey = false;
                    ShowMessage(TextConstants.BluetoothConnectionFail, TextConstants.ButtonClose);
                    return;
                }

                KeyVendorCommand getKeyCommand = new KeyVendorCommand
                {
                    UserUUID = _user.UUID,
                    Time = DateTime.Now,
                    CommandType = KeyVendorCommandType.GetKey,
                    Data = SelectedKey
                };
                KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
                KeyVendorAnswer answer = await terminal.ExecuteCommandAsync(getKeyCommand, 3000, 100);

                if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
                    ShowMessage(TextConstants.ErrorGetKeyFail, TextConstants.ButtonClose);
            });

            GettingKey = false;
        }

        protected override void InitializeCommands()
        {
            OpenKeyManagementPageCommand = new Command(
                () => { OpenKeyManagementPage(); },
                () => { return !IsMessageVisible && !GettingKey && !IsActivityIndicationVisible; });
            OpenUserManagementPageCommand = new Command(
                () => { OpenUserManagementPage(); },
                () => { return !IsMessageVisible && !GettingKey && !IsActivityIndicationVisible; });
            OpenLogPageCommand = new Command(
                () => { OpenLogPage(); },
                () => { return !IsMessageVisible && !GettingKey && !IsActivityIndicationVisible; });
            RefreshCommand = new Command(
                () => { GetKeyListAsync(); },
                () => { return !IsMessageVisible && !GettingKey && !IsActivityIndicationVisible; });
            GetKeyCommand = new Command(
                () => { if (SelectedKey != null) GetKeyAsync(); },
                () => { return SelectedKey != null && !GettingKey; });
        }
        protected override void UpdateCommands()
        {
            ((Command)OpenKeyManagementPageCommand).ChangeCanExecute();
            ((Command)OpenUserManagementPageCommand).ChangeCanExecute();
            ((Command)OpenLogPageCommand).ChangeCanExecute();
            ((Command)RefreshCommand).ChangeCanExecute();
            ((Command)GetKeyCommand).ChangeCanExecute();
        }

        private IBluetoothManager _bluetooth;
        private KeyVendorUser _user;
        private ObservableCollection<string> _keyList;
        private string _selectedKey;

        private bool _gettingKey;
        private bool _isToolbarVisible;
    }
}
