using KeyVendor.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace KeyVendor.ViewModels
{
    public class UserPageViewModel : ViewModelBase
    {
        public UserPageViewModel(KeyVendorUser user, IBluetoothManager bluetooth, string uuid, UserListType listType)
        {
            _bluetooth = bluetooth;
            _user = user;
            _listType = listType;

            SelectedUser = new KeyVendorUser()
            {
                UUID = uuid
            };

            IsConfirmButtonVisible = false;
            IsDenyButtonVisible = false;
            IsBanButtonVisible = false;
            IsUnbanButtonVisible = false;
            IsPromoteButtonVisible = false;
            IsDemoteButtonVisible = false;

            switch (listType)
            {
                case UserListType.Applications:
                    IsConfirmButtonVisible = true;
                    IsDenyButtonVisible = true;
                    break;
                case UserListType.Users:
                    IsBanButtonVisible = true;
                    IsPromoteButtonVisible = true;
                    break;
                case UserListType.Admins:
                    if (_user.UUID != uuid)
                        IsDemoteButtonVisible = true;
                    break;
                case UserListType.Bans:
                    IsUnbanButtonVisible = true;
                    break;
            }

            GetUserInfoAsync();
        }

        public bool IsConfirmButtonVisible
        {
            get { return _isConfirmButtonVisible; }
            set { SetProperty(ref _isConfirmButtonVisible, value); }
        }
        public bool IsDenyButtonVisible
        {
            get { return _isDenyButtonVisible; }
            set { SetProperty(ref _isDenyButtonVisible, value); }
        }
        public bool IsBanButtonVisible
        {
            get { return _isBanButtonVisible; }
            set { SetProperty(ref _isBanButtonVisible, value); }
        }
        public bool IsUnbanButtonVisible
        {
            get { return _isUnbanButtonVisible; }
            set { SetProperty(ref _isUnbanButtonVisible, value); }
        }
        public bool IsPromoteButtonVisible
        {
            get { return _isPromoteButtonVisible; }
            set { SetProperty(ref _isPromoteButtonVisible, value); }
        }
        public bool IsDemoteButtonVisible
        {
            get { return _isDemoteButtonVisible; }
            set { SetProperty(ref _isDemoteButtonVisible, value); }
        }
        
        public ICommand UserConfirmCommand { get; protected set; }
        public ICommand UserDenyCommand { get; protected set; }
        public ICommand UserBanCommand { get; protected set; }
        public ICommand UserUnbanCommand { get; protected set; }
        public ICommand UserPromoteCommand { get; protected set; }
        public ICommand UserDemoteCommand { get; protected set; }
        
        protected override void InitializeCommands()
        {
            UserConfirmCommand = new Command(
                () => UserConfirmAsync());
            UserDenyCommand = new Command(
                () => UserDenyAsync());
            UserBanCommand = new Command(
                () => UserBanAsync());
            UserUnbanCommand = new Command(
                () => UserUnbanAsync());
            UserPromoteCommand = new Command(
                () => UserPromoteAsync());
            UserDemoteCommand = new Command(
                () => UserDemoteAsync());
        }

        public KeyVendorUser SelectedUser
        {
            get { return _selectedUser; }
            set { SetProperty(ref _selectedUser, value); }
        }

        private async void GetUserInfoAsync()
        {
            StartActivityIndication(TextConstants.ActivityGettingUserInfo);

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

                KeyVendorCommand getUserInfoCommand = new KeyVendorCommand
                {
                    UserUUID = _user.UUID,
                    Time = DateTime.Now,
                    CommandType = KeyVendorCommandType.GetInfo,
                    Data = SelectedUser.UUID
                };
                KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
                KeyVendorAnswer answer = await terminal.ExecuteCommandAsync(getUserInfoCommand, 10000, 100);

                if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
                {
                    Device.BeginInvokeOnMainThread(() => SelectedUser.Name = SelectedUser.Description = "<>");
                    ShowMessage(TextConstants.ErrorGetInfoFail, TextConstants.ButtonClose);
                    return;
                }
                
                string[] answerData = answer.Data.Split('@');
                string uuid = SelectedUser.UUID;

                if (answerData.Length == 2)
                {
                    SelectedUser = new KeyVendorUser()
                    {
                        UUID = uuid,
                        Name = answerData[0],
                        Description = answerData[1]
                    };
                }
            });

            switch (_listType)
            {
                case UserListType.Applications:
                    IsConfirmButtonVisible = true;
                    IsDenyButtonVisible = true;
                    break;
                case UserListType.Users:
                    IsBanButtonVisible = true;
                    IsPromoteButtonVisible = true;
                    break;
                case UserListType.Admins:
                    if (_user.UUID != SelectedUser.UUID)
                        IsDemoteButtonVisible = true;
                    break;
                case UserListType.Bans:
                    IsUnbanButtonVisible = true;
                    break;
            }

            UpdateCommands();
            StopActivityIndication();
        }
        private async void UserConfirmAsync()
        {
            StartActivityIndication(TextConstants.ActivityUserAction);

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

                KeyVendorCommand command = new KeyVendorCommand
                {
                    UserUUID = _user.UUID,
                    Time = DateTime.Now,
                    CommandType = KeyVendorCommandType.UserConfirm,
                    Data = SelectedUser.UUID
                };
                KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
                KeyVendorAnswer answer = await terminal.ExecuteCommandAsync(command, 10000, 100);

                if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
                {
                    Device.BeginInvokeOnMainThread(() => SelectedUser.Name = SelectedUser.Description = "<>");
                    ShowMessage(TextConstants.ErrorUserConfirmFail, TextConstants.ButtonClose);
                    return;
                }
            });
            
            StopActivityIndication();
        }
        private async void UserDenyAsync()
        {
            StartActivityIndication(TextConstants.ActivityUserAction);

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

                KeyVendorCommand command = new KeyVendorCommand
                {
                    UserUUID = _user.UUID,
                    Time = DateTime.Now,
                    CommandType = KeyVendorCommandType.UserDeny,
                    Data = SelectedUser.UUID
                };
                KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
                KeyVendorAnswer answer = await terminal.ExecuteCommandAsync(command, 10000, 100);

                if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
                {
                    Device.BeginInvokeOnMainThread(() => SelectedUser.Name = SelectedUser.Description = "<>");
                    ShowMessage(TextConstants.ErrorUserDenyFail, TextConstants.ButtonClose);
                    return;
                }
            });

            StopActivityIndication();
        }
        private async void UserBanAsync()
        {
            StartActivityIndication(TextConstants.ActivityUserAction);

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

                KeyVendorCommand command = new KeyVendorCommand
                {
                    UserUUID = _user.UUID,
                    Time = DateTime.Now,
                    CommandType = KeyVendorCommandType.UserBan,
                    Data = SelectedUser.UUID
                };
                KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
                KeyVendorAnswer answer = await terminal.ExecuteCommandAsync(command, 10000, 100);

                if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
                {
                    Device.BeginInvokeOnMainThread(() => SelectedUser.Name = SelectedUser.Description = "<>");
                    ShowMessage(TextConstants.ErrorUserBanFail, TextConstants.ButtonClose);
                    return;
                }
            });

            StopActivityIndication();
        }
        private async void UserUnbanAsync()
        {
            StartActivityIndication(TextConstants.ActivityUserAction);

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

                KeyVendorCommand command = new KeyVendorCommand
                {
                    UserUUID = _user.UUID,
                    Time = DateTime.Now,
                    CommandType = KeyVendorCommandType.UserUnban,
                    Data = SelectedUser.UUID
                };
                KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
                KeyVendorAnswer answer = await terminal.ExecuteCommandAsync(command, 10000, 100);

                if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
                {
                    Device.BeginInvokeOnMainThread(() => SelectedUser.Name = SelectedUser.Description = "<>");
                    ShowMessage(TextConstants.ErrorUserUnbanFail, TextConstants.ButtonClose);
                    return;
                }
            });

            StopActivityIndication();
        }
        private async void UserPromoteAsync()
        {
            StartActivityIndication(TextConstants.ActivityUserAction);

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

                KeyVendorCommand command = new KeyVendorCommand
                {
                    UserUUID = _user.UUID,
                    Time = DateTime.Now,
                    CommandType = KeyVendorCommandType.UserPromote,
                    Data = SelectedUser.UUID
                };
                KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
                KeyVendorAnswer answer = await terminal.ExecuteCommandAsync(command, 10000, 100);

                if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
                {
                    Device.BeginInvokeOnMainThread(() => SelectedUser.Name = SelectedUser.Description = "<>");
                    ShowMessage(TextConstants.ErrorUserPromoteFail, TextConstants.ButtonClose);
                    return;
                }
            });

            StopActivityIndication();
        }
        private async void UserDemoteAsync()
        {
            StartActivityIndication(TextConstants.ActivityUserAction);

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

                KeyVendorCommand command = new KeyVendorCommand
                {
                    UserUUID = _user.UUID,
                    Time = DateTime.Now,
                    CommandType = KeyVendorCommandType.UserDemote,
                    Data = SelectedUser.UUID
                };
                KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
                KeyVendorAnswer answer = await terminal.ExecuteCommandAsync(command, 10000, 100);

                if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
                {
                    Device.BeginInvokeOnMainThread(() => SelectedUser.Name = SelectedUser.Description = "<>");
                    ShowMessage(TextConstants.ErrorUserDemoteFail, TextConstants.ButtonClose);
                    return;
                }
            });

            StopActivityIndication();
        }

        private IBluetoothManager _bluetooth;
        private KeyVendorUser _user;
        private UserListType _listType;
        private KeyVendorUser _selectedUser;

        private bool _isBanButtonVisible;
        private bool _isUnbanButtonVisible;
        private bool _isPromoteButtonVisible;
        private bool _isDemoteButtonVisible;
        private bool _isConfirmButtonVisible;
        private bool _isDenyButtonVisible;
    }
}
