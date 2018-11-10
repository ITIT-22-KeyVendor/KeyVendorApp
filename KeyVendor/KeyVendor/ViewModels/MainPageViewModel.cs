using KeyVendor.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            _bluetoothManager = DependencyService.Get<IBluetoothManager>();
        }

        public void OpenConnectionPage()
        {
            OnOpenConnectionPage(this, new ConnectionPageViewModel(_user, _bluetoothManager));
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
            if (!_bluetoothManager.IsBluetoothAvailable)
            {
                ShowMessage("Bluetooth на вашому пристрої не доступний!", "Закрити");
                return;
            }

            StartActivityIndication("Іде підключення. Будь ласка зачекайте...");
            await Task.Delay(50);
                        
            if (!await TurnOnBluetoothAsync(2000, 50))
            {
                StopActivityIndication();
                ShowMessage("Не вдалось увімкнути Bluetooth", "Закрити");
                return;
            }
            if (!await FindBluetoothDeviceAsync(50))
            {
                StopActivityIndication();
                ShowMessage("Не вдалось знайти пристрій обліку ключів. Переконайтесь що Ви знаходитесь достатньо близько до нього. Також Ви можете перейти на сторінку вибору з'єднання та вибрати Bluetooth-пристрій, який відповідатиме системі видачі ключів", "Закрити");
                return;
            }
            if (!await PairWithBluetoothDeviceAsync(25000, 50))
            {
                StopActivityIndication();
                ShowMessage("Не вдалось утворити пару з системою видачі ключів або вийшов час на її утворення", "Закрити");
                return;
            }
            if (!await CreateConnectionAsync(5000, 50))
            {
                StopActivityIndication();
                ShowMessage("Не вдалось підключитися", "Закрити");
                return;
            }
            
            KeyVendorAnswer loginAnswer = await LogIn(3000, 50);

            if (!loginAnswer.IsCorrect || loginAnswer.AnswerType == KeyVendorAnswerType.InvalidCommand)
            {
                StopActivityIndication();
                ShowMessage("Сталась помилка, спробуйте ще раз", "Закрити");
                return;
            }
            else if (loginAnswer.AnswerType == KeyVendorAnswerType.AccessDenied)
            {
                StopActivityIndication();
                ShowMessage("Вас було заблоковано адміністратором. Тепер Ви не зможете підключатись до цієї системи", "Закрити");
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
            OnOpenVendingPage(this, new VendingPageViewModel(_user, _bluetoothManager));
        }
        public async void RegisterAsync()
        {
            StartActivityIndication("Надсилається заявка на реєстрацію");

            KeyVendorAnswer answer = await SendApplication(3000, 100);

            if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
            {
                StopActivityIndication();
                ShowMessage("Не вдалось надіслати заявку на реєстрацію", "Закрити");
                return;
            }

            StopActivityIndication();
            ShowMessage("Заявку на реєстрацію надіслано. Після підтвердження адміністратором, Ви зможете увійти у цю систему", "Закрити");
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
            if (_bluetoothManager != null)
            {
                if (_bluetoothManager.IsRefreshing)
                    _bluetoothManager.StopRefreshing();
                if (_bluetoothManager.IsConnected)
                    _bluetoothManager.CloseConnection();
                _bluetoothManager.IsBluetoothOn = false;
            }
        }
        public void OnResume()
        {
            if (_bluetoothManager != null)
            {
                _bluetoothManager.IsBluetoothOn = true;
            }
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
        public ICommand MessageButtonCommand { get; protected set; }
        public ICommand RegisterCommand { get; protected set; }
        public ICommand CloseRegistrationOverlayCommand { get; protected set; }

        private async Task<bool> TurnOnBluetoothAsync(uint timeout, uint delay)
        {
            return await Task.Run(async () =>
            {
                if (!_bluetoothManager.IsBluetoothOn)
                {
                    _bluetoothManager.IsBluetoothOn = true;

                    for (int i = 0; i * delay < timeout; i++)
                        if (!_bluetoothManager.IsBluetoothOn)
                            await Task.Delay((int)delay);
                        else break;
                }

                return _bluetoothManager.IsBluetoothOn;
            });
        }
        private async Task<bool> FindBluetoothDeviceAsync(uint delay)
        {
            return await Task.Run(async () =>
            {
                if (_user.SavedAddress == "")
                {
                    _bluetoothManager.StartRefreshing();

                    while (_bluetoothManager.IsRefreshing)
                    {
                        foreach (var item in _bluetoothManager.DeviceList)
                            if ((_user.SavedAddress == "" && item.Name == _defaultDeviceName) ||
                                 item.Address == _user.SavedAddress)
                            {
                                _user.SavedAddress = item.Address;
                                break;
                            }

                        if (_user.SavedAddress == "")
                            await Task.Delay((int)delay);
                    }
                }

                return _user.SavedAddress != "";
            });
        }
        private async Task<bool> PairWithBluetoothDeviceAsync(uint timeout, uint delay)
        {
            return await Task.Run(async () =>
            {
                if (!_bluetoothManager.IsBonded)
                {
                    _bluetoothManager.CreateBond(_user.SavedAddress);

                    for (int i = 0; i * delay < timeout; i++)
                        if (!_bluetoothManager.IsBonded)
                            await Task.Delay((int)delay);
                        else break;
                }

                return _bluetoothManager.IsBonded;
            });
        }
        private async Task<bool> CreateConnectionAsync(uint timeout, uint delay)
        {
            return await Task.Run(async () => 
            {
                if (!_bluetoothManager.IsConnected)
                {
                    _bluetoothManager.OpenConnection();

                    for (int i = 0; i * delay < timeout; i++)
                        if (!_bluetoothManager.IsConnected)
                            await Task.Delay((int)delay);
                        else break;
                }

                return _bluetoothManager.IsConnected;
            });
        }
        private async Task<KeyVendorAnswer> LogIn(uint timeout, uint delay)
        {
            KeyVendorCommand loginCommand = new KeyVendorCommand
            {
                UserUUID = _user.UUID,
                Time = DateTime.Now,
                CommandType = KeyVendorCommandType.UserLogin
            };
            KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetoothManager);
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
            KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetoothManager);
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
            KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetoothManager);
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
            MessageButtonCommand = new Command(
                () => { IsMessageVisible = false; });
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
        private IBluetoothManager _bluetoothManager;

        private const string _defaultDeviceName = "KeyVendor";
    }
}