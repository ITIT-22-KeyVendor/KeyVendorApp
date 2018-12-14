
namespace KeyVendor.Models
{
    public enum KeyVendorCommandType
    {
        UserLogin          = 0,
        UserRegister       = 1,
        UserConfirm        = 2,
        UserDeny           = 3,
        UserBan            = 4,
        UserUnban          = 5,
        UserPromote        = 6,
        UserDemote         = 7,
        GetApplicationList = 8,
        GetUserList        = 9,
        GetAdminList       = 10,
        GetBanList         = 11,
        GetInfo            = 12,
        UpdateInfo         = 13,
        GetKeyList         = 14,
        SetKeyList         = 15,
        GetKey             = 16,
        GetLog             = 17,
        ClearLog           = 18,
        AdminCheck         = 19
    }
}
