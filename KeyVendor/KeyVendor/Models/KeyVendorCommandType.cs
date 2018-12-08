
namespace KeyVendor.Models
{
    public enum KeyVendorCommandType
    {
        UserLogin    = 0,
        UserRegister = 1,
        UserConfirm  = 2,
        UserDeny     = 3,
        UserBan      = 4,
        UserUnban    = 5,
        UserPromote  = 6,
        UserDemote   = 7,
        GetUserList  = 8,
        GetKeyList   = 9,
        SetKeyList   = 10,
        GetKey       = 11,
        GetLog       = 12,
        ClearLog     = 13,
        AdminCheck   = 14,
        UpdateInfo   = 15
    }
}
