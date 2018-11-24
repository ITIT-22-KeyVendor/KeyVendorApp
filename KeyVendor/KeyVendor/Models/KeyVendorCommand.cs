using System;
using System.Collections.Generic;

namespace KeyVendor.Models
{
    public class KeyVendorCommand
    {
        public KeyVendorCommand()
        {
            UserUUID = Data = "";
            Time = new DateTime();
            CommandType = KeyVendorCommandType.UserLogin;
        }

        public string GenerateCommandString()
        {
            string resultingCommand = "$";
            string uuid = UserUUID;
            string dateTime = Time.ToString("yyyy/MM/dd,hh:mm:ss");
            string commandType = ((int)CommandType).ToString();

            resultingCommand += uuid + "@" + dateTime + "@" + commandType;

            if (!String.IsNullOrEmpty(Data))
                resultingCommand += "@" + Data;

            resultingCommand += "$";

            return resultingCommand;
        }
        public List<string> SplitCommandString(string command, uint partLength)
        {
            List<string> commandStringList = new List<string>();
            string commandPart = "";
            int partNumber = 1;

            for (int i = 0; i < command.Length; i++)
            {
                if (i >= partLength * partNumber)
                {
                    partNumber++;
                    commandStringList.Add(commandPart);
                    commandPart = "";
                }

                commandPart += command[i];
            }

            commandStringList.Add(commandPart);
            return commandStringList;
        }

        public string UserUUID { get; set; }
        public DateTime Time { get; set; }
        public KeyVendorCommandType CommandType { get; set; }
        public string Data { get; set; }
    }
}
