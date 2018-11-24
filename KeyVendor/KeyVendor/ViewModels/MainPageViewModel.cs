using KeyVendor.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace KeyVendor.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            InitializeCommands();
            _bluetooth = DependencyService.Get<IBluetoothManager>();
        }

        public void OpenConnectionPage()
        {
            OnOpenConnectionPage(this, new ConnectionPageViewModel(_user, _bluetooth));
        }
        public void OpenProfilePage()
        {
            IsNewUser = false;
            OnOpenProfilePage(this, new ProfilePageViewModel(_user));
        }
        public void OpenHelpPage()
        {
            OnOpenHelpPage(this, new HelpPageViewModel());
        }
        public async void ConnectAsync()
        {
            // omit sign in process
            /*
            _user.HasAdminRights = true;
            OnOpenVendingPage(this, new VendingPageViewModel(_user, _bluetoothManager));
            return;
            */
            if (!_bluetooth.IsBluetoothAvailable)
            {
                ShowMessage(TextConstants.BluetoothUnavailable, TextConstants.ButtonClose);
                return;
            }

            StartActivityIndication(TextConstants.ActivityConnection);
            await Task.Delay(50);

            if (!await _bluetooth.TurnOnBluetoothAsync(2000, 25))
            {
                StopActivityIndication();
                ShowMessage(TextConstants.BluetoothTurnOnFail, TextConstants.ButtonClose);
                return;
            }

            if (_user.SavedAddress == "")
            {
                var device = await _bluetooth.FindBluetoothDeviceByNameAsync(TextConstants.DefaultDeviceName, 25);

                if (device == null)
                {
                    StopActivityIndication();
                    ShowMessage(TextConstants.BluetoothDeviceSearchFail, TextConstants.ButtonClose);
                    return;
                }
                else
                {
                    _user.SavedAddress = device.Address;
                }
            }
            else
            {
                var device = await _bluetooth.FindBluetoothDeviceByAddressAsync(_user.SavedAddress, 25);

                if (device == null)
                {
                    StopActivityIndication();
                    ShowMessage(TextConstants.BluetoothDeviceSearchFail, TextConstants.ButtonClose);
                    return;
                }
            }

            if (!await _bluetooth.BondWithBluetoothDeviceAsync(_user.SavedAddress, 25000, 50))
            {
                StopActivityIndication();
                ShowMessage(TextConstants.BluetoothBondFail, TextConstants.ButtonClose);
                return;
            }
            if (!await _bluetooth.CreateConnectionAsync(5000, 50))
            {
                StopActivityIndication();
                ShowMessage(TextConstants.BluetoothConnectionFail, TextConstants.ButtonClose);
                return;
            }
            
            KeyVendorAnswer loginAnswer = await LogIn(3000, 50);

            if (!loginAnswer.IsCorrect || loginAnswer.AnswerType == KeyVendorAnswerType.InvalidCommand)
            {
                StopActivityIndication();
                ShowMessage(TextConstants.ErrorTryAgain, TextConstants.ButtonClose);
                return;
            }
            else if (loginAnswer.AnswerType == KeyVendorAnswerType.AccessDenied)
            {
                StopActivityIndication();
                ShowMessage(TextConstants.ErrorUserBlocked, TextConstants.ButtonClose);
                return;
            }
            else if (loginAnswer.AnswerType == KeyVendorAnswerType.Failure)
            {
                StopActivityIndication();
                IsRegistrationOverlayVisible = true;
                return;
            }

            KeyVendorAnswer checkForAdminRightsAnswer = await CheckForAdminRights(3000, 50);
            _user.HasAdminRights = checkForAdminRightsAnswer.AnswerType == KeyVendorAnswerType.Success;
            
            StopActivityIndication();
            OnOpenVendingPage(this, new VendingPageViewModel(_user, _bluetooth));
        }
        public async void RegisterAsync()
        {
            StartActivityIndication(TextConstants.ActivityRegistration);

            if (!await _bluetooth.TurnOnBluetoothAsync(1000, 25))
            {
                StopActivityIndication();
                ShowMessage(TextConstants.BluetoothTurnOnFail, TextConstants.ButtonClose);
                return;
            }
            
            KeyVendorAnswer answer = await SendApplication(3000, 100);

            if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
            {
                StopActivityIndication();
                ShowMessage(TextConstants.ErrorApplicationFail, TextConstants.ButtonClose);
                return;
            }

            StopActivityIndication();
            ShowMessage(TextConstants.SuccessApplicationSent, TextConstants.ButtonClose);
        }

        public void SaveState(IDictionary<string, object> dictionary)
        {
            dictionary["IsNewUser"] = _user.IsNewUser;
            dictionary["UUID"] = _user.UUID;
            dictionary["UserName"] = _user.Name;
            dictionary["UserDescription"] = _user.Description;
            dictionary["SavedAddress"] = _user.SavedAddress;
        }
        public void RestoreState(IDictionary<string, object> dictionary)
        {
            _user.IsNewUser = GetDictionaryEntry(dictionary, "IsNewUser", true);
            _user.UUID = GetDictionaryEntry(dictionary, "UUID", KeyVendorUser.GenerateUUID());
            _user.Name = GetDictionaryEntry(dictionary, "UserName", "");
            _user.Description = GetDictionaryEntry(dictionary, "UserDescription", "");
            _user.SavedAddress = GetDictionaryEntry(dictionary, "SavedAddress", "");
        }
        public void OnSleep()
        {
            if (_bluetooth != null)
            {
                if (_bluetooth.IsDiscovering)
                    _bluetooth.StopDiscovering();
                if (_bluetooth.IsConnected)
                    _bluetooth.CloseConnection();
                _bluetooth.IsBluetoothOn = false;
            }
        }
        public void OnResume()
        {
            if (_bluetooth != null)
                _bluetooth.IsBluetoothOn = true;
        }

        public bool IsNewUser
        {
            get { return _user.IsNewUser; }
            protected set
            {
                if (_user.IsNewUser != value)
                {
                    _user.IsNewUser = value;
                    OnPropertyChanged();
                    UpdateCommands();
                }                
            }
        }
        public bool IsActivityIndicationVisible
        {
            get { return _isActivityIndicationVisible; }
            set { SetProperty(ref _isActivityIndicationVisible, value); }
        }
        public string ActivityIndicationText
        {
            get { return _activityIndicationText; }
            set { SetProperty(ref _activityIndicationText, value); }
        }
        public bool IsRegistrationOverlayVisible
        {
            get { return _isRegistrationOverlayVisible; }
            set { SetProperty(ref _isRegistrationOverlayVisible, value); }
        }
        
        public event EventHandler<ConnectionPageViewModel> OnOpenConnectionPage;
        public event EventHandler<ProfilePageViewModel> OnOpenProfilePage;
        public event EventHandler<HelpPageViewModel> OnOpenHelpPage;
        public event EventHandler<VendingPageViewModel> OnOpenVendingPage;
        
        public ICommand OpenConnectionPageCommand { get; protected set; }
        public ICommand OpenProfilePageCommand { get; protected set; }
        public ICommand OpenHelpPageCommand { get; protected set; }
        public ICommand ConnectCommand { get; protected set; }
        public ICommand CloseNewUserOverlayCommand { get; protected set; }
        public ICommand RegisterCommand { get; protected set; }
        public ICommand CloseRegistrationOverlayCommand { get; protected set; }

        private async Task<KeyVendorAnswer> LogIn(uint timeout, uint delay)
        {
            KeyVendorCommand loginCommand = new KeyVendorCommand
            {
                UserUUID = _user.UUID,
                Time = DateTime.Now,
                CommandType = KeyVendorCommandType.UserLogin
            };
            KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
            return await terminal.ExecuteCommandAsync(loginCommand, timeout, delay);
        }
        private async Task<KeyVendorAnswer> SendApplication(uint timeout, uint delay)
        {
            KeyVendorCommand registerCommand = new KeyVendorCommand
            {
                UserUUID = _user.UUID,
                Time = DateTime.Now,
                CommandType = KeyVendorCommandType.UserRegister,
                Data = _user.Name + "@" + _user.Description
            };
            KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
            return await terminal.ExecuteCommandAsync(registerCommand, timeout, delay);
        }
        private async Task<KeyVendorAnswer> CheckForAdminRights(uint timeout, uint delay)
        {
            KeyVendorCommand adminCheckCommand = new KeyVendorCommand
            {
                UserUUID = _user.UUID,
                Time = DateTime.Now,
                CommandType = KeyVendorCommandType.AdminCheck
            };
            KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
            return await terminal.ExecuteCommandAsync(adminCheckCommand, timeout, delay);
        }

        private void InitializeCommands()
        {
            OpenConnectionPageCommand = new Command(
                () => { OpenConnectionPage(); },
                () => { return !IsNewUser && !IsActivityIndicationVisible; });
            OpenProfilePageCommand = new Command(
                () => { OpenProfilePage(); },
                () => { return !IsActivityIndicationVisible; });
            OpenHelpPageCommand = new Command(
                () => { OpenHelpPage(); },
                () => { return !IsNewUser && !IsActivityIndicationVisible; });
            ConnectCommand = new Command(
                () => { ConnectAsync(); },
                () => { return !IsNewUser && !IsActivityIndicationVisible; });
            CloseNewUserOverlayCommand = new Command(
                () => { IsNewUser = false; });
            RegisterCommand = new Command(
                () => { RegisterAsync();
                        IsRegistrationOverlayVisible = false; });
            CloseRegistrationOverlayCommand = new Command(
                () => { IsRegistrationOverlayVisible = false; });
        }
        public override void UpdateCommands()
        {
            ((Command)OpenConnectionPageCommand).ChangeCanExecute();
            ((Command)OpenProfilePageCommand).ChangeCanExecute();
            ((Command)OpenHelpPageCommand).ChangeCanExecute();
            ((Command)ConnectCommand).ChangeCanExecute();
        }
        private void StartActivityIndication(string text)
        {
            IsActivityIndicationVisible = true;
            ActivityIndicationText = text;
            UpdateCommands();
        }
        private void StopActivityIndication()
        {
            IsActivityIndicationVisible = false;
            ActivityIndicationText = "";
            UpdateCommands();
        }
        private T GetDictionaryEntry<T>(IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            if (dictionary.ContainsKey(key))
                return (T)dictionary[key];

            return defaultValue;
        }

        private bool _isActivityIndicationVisible;
        private string _activityIndicationText;
        private bool _isRegistrationOverlayVisible;

        private KeyVendorUser _user = new KeyVendorUser();
        private IBluetoothManager _bluetooth;
    }
}
