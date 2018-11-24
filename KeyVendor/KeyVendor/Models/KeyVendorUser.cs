using System;

namespace KeyVendor.Models
{
    public class KeyVendorUser
    {
        public KeyVendorUser()
        {
            UUID = Name = Description = SavedAddress = "";
            HasAdminRights = false;
            IsNewUser = true;
        }

        public string UUID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SavedAddress { get; set; }
        public bool HasAdminRights { get; set; }
        public bool IsNewUser { get; set; }

        public static string GenerateUUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
