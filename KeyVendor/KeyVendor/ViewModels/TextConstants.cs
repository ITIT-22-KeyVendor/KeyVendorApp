
namespace KeyVendor.ViewModels
{
    public static class TextConstants
    {
        public static string BluetoothUnavailable { get;  } = 
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

        public static string ErrorTryAgain { get; } =
            "Сталась помилка, спробуйте ще раз";
        public static string ErrorUserBlocked { get; } =
            "Вас було заблоковано адміністратором. Тепер Ви не зможете підключатись до цієї системи";
        public static string ErrorApplicationFail { get; } =
            "Не вдалось надіслати заявку на реєстрацію";
        public static string ErrorGetKeyListFail { get; } =
            "Не вдалось отримати список ключів";
        public static string ErrorGetKeyFail { get; } =
            "Сталась помилка. Спробуйте перепідключитися та повторити операцію";

        public static string SuccessApplicationSent { get; } =
            "Заявку на реєстрацію надіслано. Після підтвердження адміністратором, Ви зможете увійти у цю систему";

        public static string ButtonClose { get; } =
            "Закрити";
        public static string ButtonStartRefreshing { get; } =
            "Розпочати пошук";
        public static string ButtonStopRefreshing { get; } =
            "Зупинити пошук";

        public static string DefaultDeviceName { get; } =
            "KeyVendor";
    }
}
