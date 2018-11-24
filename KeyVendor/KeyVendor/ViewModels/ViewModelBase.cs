﻿using System;
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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void UpdateCommands()
        {

        }
        public void ShowMessage(string message, string button)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                MessageText = message;
                MessageButtonText = button;
                IsMessageVisible = true;
            });
        }

        public bool IsMessageVisible
        {
            get { return _isMessageVisible; }
            set
            {
                if (SetProperty(ref _isMessageVisible, value))
                    UpdateCommands();
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
    }
}
