using KeyVendor.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace KeyVendor.ViewModels
{
    public class LogPageViewModel : ViewModelBase
    {
        public LogPageViewModel(KeyVendorUser user, IBluetoothManager bluetooth)
        {
            _bluetooth = bluetooth;
            _user = user;

            LogList = new ObservableCollection<LogInfo>();
            Indexer = 0;

            GetLogAsync(Indexer);
        }

        public ObservableCollection<LogInfo> LogList
        {
            get { return _logList; }
            set { SetProperty(ref _logList, value); }
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

        public ICommand GetLogCommand { get; protected set; }
        public ICommand NextLogCommand { get; protected set; }
        public ICommand PreviousLogCommand { get; protected set; }
        public ICommand ClearLogCommand { get; protected set; }

        protected override void InitializeCommands()
        {
            GetLogCommand = new Command(
                () => 
                {
                    Indexer = 0;
                    GetLogAsync(Indexer);
                },
                () => { return !IsMessageVisible && !IsActivityIndicationVisible; });
            PreviousLogCommand = new Command(
                () => 
                {
                    Indexer++;
                    GetLogAsync(Indexer);
                },
                () => { return !IsMessageVisible && !IsActivityIndicationVisible && LogList != null && LogList.Count == 10; });
            NextLogCommand = new Command(
                () => 
                {
                    Indexer--;
                    GetLogAsync(Indexer);
                },
                () => { return !IsMessageVisible && !IsActivityIndicationVisible && Indexer > 0; });
            ClearLogCommand = new Command(
                () => { ClearLogAsync(); },
                () => { return !IsMessageVisible && !IsActivityIndicationVisible; });
        }
        protected override void UpdateCommands()
        {
            ((Command)GetLogCommand).ChangeCanExecute();
            ((Command)NextLogCommand).ChangeCanExecute();
            ((Command)PreviousLogCommand).ChangeCanExecute();
            ((Command)ClearLogCommand).ChangeCanExecute();
        }

        private async void GetLogAsync(int indexer)
        {
            StartActivityIndication(TextConstants.ActivityGettingLog);
            LogList.Clear();

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

                KeyVendorCommand getLogCommand = new KeyVendorCommand
                {
                    UserUUID = _user.UUID,
                    Time = DateTime.Now,
                    CommandType = KeyVendorCommandType.GetLog,
                    Data = indexer.ToString()
                };
                KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
                KeyVendorAnswer answer = await terminal.ExecuteCommandAsync(getLogCommand, 15000, 100);

                if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
                {
                    ShowMessage(TextConstants.ErrorGetLogFail, TextConstants.ButtonClose);
                    return;
                }

                string answerData = answer.Data;
                string[] dataArray = answerData.Split('@');

                for (int i = 0; i < dataArray.Length; i += 6)
                {
                    LogInfo logInfo = new LogInfo
                    {
                        UUID = dataArray[i],
                        Time = dataArray[i + 1],
                        Command = ((KeyVendorCommandType)(int.Parse(dataArray[i + 2]))).ToString(),
                        Answer = ((KeyVendorAnswerType)(int.Parse(dataArray[i + 3]))).ToString(),
                        Data = dataArray[i + 4],
                        UserName = dataArray[i + 5]
                    };

                    LogList.Add(logInfo);
                }
            });

            UpdateCommands();
            StopActivityIndication();
        }
        private async void ClearLogAsync()
        {
            StartActivityIndication(TextConstants.ActivityClearingLog);

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

                KeyVendorCommand clearLogCommand = new KeyVendorCommand
                {
                    UserUUID = _user.UUID,
                    Time = DateTime.Now,
                    CommandType = KeyVendorCommandType.ClearLog
                };
                KeyVendorTerminal terminal = new KeyVendorTerminal(_bluetooth);
                KeyVendorAnswer answer = await terminal.ExecuteCommandAsync(clearLogCommand, 15000, 100);

                if (!answer.IsCorrect || answer.AnswerType != KeyVendorAnswerType.Success)
                {
                    ShowMessage(TextConstants.ErrorClearLogFail, TextConstants.ButtonClose);
                    return;
                }

                LogList.Clear();
                Indexer = 0;
                ShowMessage(TextConstants.SuccessLogCleared, TextConstants.ButtonClose);
            });

            UpdateCommands();
            StopActivityIndication();
        }

        private IBluetoothManager _bluetooth;
        private KeyVendorUser _user;

        private ObservableCollection<LogInfo> _logList;
        private int _indexer;

        public class LogInfo
        {
            public string UUID { get; set; }
            public string Time { get; set; }
            public string Command { get; set; }
            public string Answer { get; set; }
            public string Data { get; set; }
            public string UserName { get; set; }
        }
    }
}
