using KeyVendor.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace KeyVendor.ViewModels
{
    public class UserManagementPageViewModel : ViewModelBase
    {
        public UserManagementPageViewModel(KeyVendorUser user, IBluetoothManager bluetooth)
        {
            _bluetooth = bluetooth;
            _user = user;

            UserList = new ObservableCollection<KeyVendorUser>();
            Indexer = 0;

            GetUserListAsync(Indexer, UserListType.Users);
        }

        public void OpenUserPage()
        {
            OnOpenUserPage(this, new UserPageViewModel(_user, _bluetooth, SelectedUser.UUID, _currentListType));
        }

        public event EventHandler<UserPageViewModel> OnOpenUserPage;

        public ObservableCollection<KeyVendorUser> UserList
        {
            get { return _userList; }
            set { SetProperty(ref _userList, value); }
        }
        public int Indexer
        {
            get { return _indexer; }
            set
            {
                if (SetProperty(ref _indexer, value))
                    Device.BeginInvokeOnMainThread(() => UpdateCommands());
            }
        }
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public KeyVendorUser SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (SetProperty(ref _selectedUser, value))
                    Device.BeginInvokeOnMainThread(() => UpdateCommands());
            }
        }

        public ICommand GetApplicationListCommand { get; protected set; }
        public ICommand GetUserListCommand { get; protected set; }
        public ICommand GetAdminListCommand { get; protected set; }
        public ICommand GetBanListCommand { get; protected set; }
        public ICommand NextUserListCommand { get; protected set; }
        public ICommand PreviousUserListCommand { get; protected set; }
        public ICommand OpenUserPageCommand { get; protected set; }

        protected override void InitializeCommands()
        {
            GetApplicationListCommand = new Command(
                () =>
                {
                    Indexer = 0;
                    GetUserListAsync(Indexer, UserListType.Applications);
                },
                () => { return !IsMessageVisible && !IsActivityIndicationVisible; });
            GetUserListCommand = new Command(
                () =>
                {
                    Indexer = 0;
                    GetUserListAsync(Indexer, UserListType.Users);
                },
                () => { return !IsMessageVisible && !IsActivityIndicationVisible; });
            GetAdminListCommand = new Command(
                () =>
                {
                    Indexer = 0;
                    GetUserListAsync(Indexer, UserListType.Admins);
                },
                () => { return !IsMessageVisible && !IsActivityIndicationVisible; });
            GetBanListCommand = new Command(
                () =>
                {
                    Indexer = 0;
                    GetUserListAsync(Indexer, UserListType.Bans);
                },
                () => { return !IsMessageVisible && !IsActivityIndicationVisible; });
            NextUserListCommand = new Command(
                () =>
                {
                    Indexer++;
                    GetUserListAsync(Indexer, _currentListType);
                },
                () => { return !IsMessageVisible && !IsActivityIndicationVisible && UserList != null && UserList.Count == 10; });
            PreviousUserListCommand = new Command(
                () =>
                {
                    Indexer--;
                    GetUserListAsync(Indexer, _currentListType);
                },
                () => { return !IsMessageVisible && !IsActivityIndicationVisible && Indexer > 0; });
            OpenUserPageCommand = new Command(
                () => { OpenUserPage(); },
                () => { return !IsMessageVisible && !IsActivityIndicationVisible && SelectedUser != null; });
        }
        protected override void UpdateCommands()
        {
            ((Command)GetApplicationListCommand).ChangeCanExecute();
            ((Command)GetUserListCommand).ChangeCanExecute();
            ((Command)GetAdminListCommand).ChangeCanExecute();
            ((Command)GetBanListCommand).ChangeCanExecute();
            ((Command)NextUserListCommand).ChangeCanExecute();
            ((Command)PreviousUserListCommand).ChangeCanExecute();
            ((Command)OpenUserPageCommand).ChangeCanExecute();
        }

        private async void GetUserListAsync(int indexer, UserListType listType)
        {
            StartActivityIndication(TextConstants.ActivityGettingUsers);
            UserList.Clear();

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

                KeyVendorCommand getUserListCommand = new KeyVendorCommand
                {
                    UserUUID = _user.UUID,
                    Time = DateTime.Now,
                    Data = indexer.ToString()
                };

                switch (listType)
                {
                    case UserListType.Applications:
                        getUserListCommand.CommandType = KeyVendorCommandType.GetApplicationList;
                        Device.BeginInvokeOnMainThread(() => Title = TextConstants.ApplicationListTitle);
                        break;
                    case UserListType.Users:
                        getUserListCommand.CommandType = KeyVendorCommandType.GetUserList;
                        Device.BeginInvokeOnMainThread(() => Title = TextConstants.UserListTitle);
                        break;
                    case UserListType.Admins:
                        getUserListCommand.CommandType = KeyVendorCommandType.GetAdminList;
                        Device.BeginInvokeOnMainThread(() => Title = TextConstants.AdminListTitle);
                        break;
                    case UserListType.Bans:
                        getUserListCommand.CommandType = KeyVendorCommandType.GetBanList;
                        Device.BeginInvokeOnMainThread(() => Title = TextConstants.BanListTitle);
                        break;
                    default:
                        getUserListCommand.CommandType = KeyVendorCommandType.GetUserList;
                        Device.BeginInvokeOnMainThread(() => Title = TextConstants.UserListTitle);
                        break;
                }

                KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
                KeyVendorAnswer answer = await terminal.ExecuteCommandAsync(getUserListCommand, 15000, 100);

                if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
                {
                    ShowMessage(TextConstants.ErrorGetUserListFail, TextConstants.ButtonClose);
                    return;
                }

                _currentListType = listType;
                string answerData = answer.Data;
                string[] dataArray = answerData.Split('@');

                for (int i = 0; i < dataArray.Length; i++)
                {
                    //if (i + 2 >= dataArray.Length)
                    //    break;

                    if (dataArray[i] != "" && dataArray[i] != "\n")
                    {
                        KeyVendorUser user = new KeyVendorUser
                        {
                            UUID = dataArray[i]
                        };

                        UserList.Add(user);
                    }
                }
            });

            UpdateCommands();
            StopActivityIndication();
        }

        private IBluetoothManager _bluetooth;
        private KeyVendorUser _user;

        private ObservableCollection<KeyVendorUser> _userList;
        private int _indexer;
        private string _title;
        private UserListType _currentListType;
        private KeyVendorUser _selectedUser;
    }
}
