using KeyVendor.Models;
using System.Windows.Input;
using Xamarin.Forms;

namespace KeyVendor.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        public ProfilePageViewModel(KeyVendorUser user)
        {
            InitializeCommands();

            _user = user;

            UserName = user.Name;
            UserDescription = user.Description;
            InfoChanged = false;
        }

        public void SaveChanges()
        {
            _user.Name = UserName;
            _user.Description = UserDescription;
            InfoChanged = false;
        }

        public string UUID
        {
            get { return _user.UUID; }
        }
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (SetProperty(ref _userName, value) && _userName != _user.Name)
                    InfoChanged = true;
            }
        }
        public string UserDescription
        {
            get { return _userDescription; }
            set
            {
                if (SetProperty(ref _userDescription, value) && _userDescription != _user.Description)
                    InfoChanged = true;
            }
        }        
        public bool InfoChanged
        {
            get { return _infoChanged; }
            set
            {
                if (SetProperty(ref _infoChanged, value))
                    UpdateCommands();
            }
        }

        public ICommand SaveChangesCommand { get; protected set; }

        private void InitializeCommands()
        {
            SaveChangesCommand = new Command(
                () => { SaveChanges(); },
                () => { return InfoChanged; });
        }
        public override void UpdateCommands()
        {
            ((Command)SaveChangesCommand).ChangeCanExecute();
        }

        private string _userName;
        private string _userDescription;
        private bool _infoChanged;

        private KeyVendorUser _user;
    }
}
