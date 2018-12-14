﻿
namespace KeyVendor.ViewModels
{
    public static class TextConstants
    {
        public static string BluetoothUnavailable { get; } =
            "На вашому пристрої Bluetooth не доступний! На жаль, ви не зможете користуватись цим застосунком :(";
        public static string BluetoothTurnOnFail { get; } =
            "Не вдалось увімкнути Bluetooth";
        public static string BluetoothDeviceSearchFail { get; } =
            "Не вдалось знайти пристрій обліку ключів. Переконайтесь що Ви знаходитесь достатньо близько до нього. Також Ви можете перейти на сторінку вибору з'єднання та вибрати Bluetooth-пристрій, який відповідатиме пристрою обліку ключів";
        public static string BluetoothBondFail { get; } =
            "Не вдалось утворити пару з системою видачі ключів або вийшов час на її утворення";
        public static string BluetoothConnectionFail { get; } =
            "Не вдалось підключитися";

        public static string ActivityConnection { get; } =
            "Іде підключення. Будь ласка зачекайте...";
        public static string ActivityRegistration { get; } =
            "Надсилається заявка на реєстрацію...";
        public static string ActivitySettingKeyList { get; } =
            "Іде процес задання нового списку ключів...";
        public static string ActivityGettingLog { get; } =
            "Іде процес отримання облікових даних...";
        public static string ActivityClearingLog { get; } =
            "Іде процес очищення облікових даних...";
        public static string ActivityGettingUsers { get; } =
            "Іде процес отримання списку користувачів...";
        public static string ActivityGettingUserInfo { get; } =
            "Іде процес отримання персональних даних користувача...";
        public static string ActivityUserAction { get; } =
            "Іде процес виконання операції над користувачем...";

        public static string ErrorTryAgain { get; } =
            "Сталась помилка, спробуйте ще раз";
        public static string ErrorUserBlocked { get; } =
            "Вас було заблоковано адміністратором. Тепер Ви не зможете підключатись до цієї системи";
        public static string ErrorApplicationFail { get; } =
            "Не вдалось надіслати заявку на реєстрацію";
        public static string ErrorGetKeyListFail { get; } =
            "Не вдалось отримати список ключів";
        public static string ErrorSetKeyListFail { get; } =
            "Не вдалось задати новий список ключів";
        public static string ErrorGetKeyFail { get; } =
            "Сталась помилка. Спробуйте перепідключитися та повторити операцію";
        public static string ErrorGetLogFail { get; } =
            "Не вдалось отримати облікові дані";
        public static string ErrorClearLogFail { get; } =
            "Не вдалось очистити облікові дані";
        public static string ErrorGetUserListFail { get; } =
            "Не вдалось отримати список користувачів";
        public static string ErrorGetInfoFail { get; } =
            "Не вдалось отримати інформацію про користувача";
        public static string ErrorUserConfirmFail { get; } =
            "Не вдалось підтвердити заявку користувача";
        public static string ErrorUserDenyFail { get; } =
            "Не вдалось відхилити заявку користувача";
        public static string ErrorUserBanFail { get; } =
            "Не вдалось заблокувати користувача";
        public static string ErrorUserUnbanFail { get; } =
            "Не вдалось розблокувати користувача";
        public static string ErrorUserPromoteFail { get; } =
            "Не вдалось підвищити користувача до адміністратора";
        public static string ErrorUserDemoteFail { get; } =
            "Не вдалось забрати у користувача права адміністратора";

        public static string SuccessApplicationSent { get; } =
            "Заявку на реєстрацію надіслано. Після підтвердження адміністратором, Ви зможете увійти у цю систему";
        public static string SuccessKeyListSet { get; } =
            "Новий список ключів задано. Оновіть сторінку видачі ключів щоб його побачити";
        public static string SuccessLogCleared { get; } =
            "Облікові дані успішно очищено";

        public static string ButtonClose { get; } =
            "Закрити";
        public static string ButtonStartRefreshing { get; } =
            "Розпочати пошук";
        public static string ButtonStopRefreshing { get; } =
            "Зупинити пошук";

        public static string ApplicationListTitle { get; } =
            "Заявки";
        public static string UserListTitle { get; } =
            "Користувачі";
        public static string AdminListTitle { get; } =
            "Адміністратори";
        public static string BanListTitle { get; } =
            "Заблоковані";

        public static string DefaultDeviceName { get; } =
            "KeyVendor";
    }
}
