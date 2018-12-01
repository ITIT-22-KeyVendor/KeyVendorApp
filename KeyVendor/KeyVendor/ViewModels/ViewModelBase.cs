using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace KeyVendor.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public ViewModelBase()
        {
            MessageButtonCommand = new Command(() => { IsMessageVisible = false; });
            InitializeCommands();            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void InitializeCommands()
        {

        }
        protected virtual void UpdateCommands()
        {

        }

        protected void ShowMessage(string message, string button)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                MessageText = message;
                MessageButtonText = button;
                IsMessageVisible = true;
            });
        }
        protected void StartActivityIndication(string text)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                IsActivityIndicationVisible = true;
                ActivityIndicationText = text;
            });
        }
        protected void StopActivityIndication()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                IsActivityIndicationVisible = false;
                ActivityIndicationText = "";
            });
        }

        public bool IsMessageVisible
        {
            get { return _isMessageVisible; }
            set
            {
                if (SetProperty(ref _isMessageVisible, value))
                    Device.BeginInvokeOnMainThread(() => UpdateCommands());
            }
        }
        public string MessageText
        {
            get { return _messageText; }
            set { SetProperty(ref _messageText, value); }
        }
        public string MessageButtonText
        {
            get { return _messageButtonText; }
            set { SetProperty(ref _messageButtonText, value); }
        }
        public bool IsActivityIndicationVisible
        {
            get { return _isActivityIndicationVisible; }
            set
            {
                if (SetProperty(ref _isActivityIndicationVisible, value))
                    Device.BeginInvokeOnMainThread(() => UpdateCommands());
            }
        }
        public string ActivityIndicationText
        {
            get { return _activityIndicationText; }
            set { SetProperty(ref _activityIndicationText, value); }
        }

        public ICommand MessageButtonCommand { get; protected set; }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetProperty<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(oldValue, newValue))
                return false;

            oldValue = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        private bool _isMessageVisible;
        private string _messageText;
        private string _messageButtonText;
        private bool _isActivityIndicationVisible;
        private string _activityIndicationText;
    }
}
