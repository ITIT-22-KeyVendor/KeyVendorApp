using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace KeyVendor.Models
{
    public class KeyVendorTerminal
    {
        public KeyVendorTerminal(IBluetoothManager bluetoothManager)
        {
            _bluetoothManager = bluetoothManager;

            SplittingEnabled = true;
            PartLenght = 32;
            TimeToReadPart = 300;
        }

        public async Task<KeyVendorAnswer> ExecuteCommandAsync(KeyVendorCommand command, uint timeout, uint delay)
        {
            return await Task.Run<KeyVendorAnswer>(async () =>
            {
                string commandString = command.GenerateCommandString();

                if (SplittingEnabled && commandString.Length > PartLenght)
                {
                    List<string> commandParts = command.SplitCommandString(commandString, PartLenght);

                    foreach (var part in commandParts)
                    {
                        _bluetoothManager.Write(part);

                        if (part != commandParts[commandParts.Count - 1])
                            await Task.Delay((int)TimeToReadPart);
                    }
                }
                else
                {
                    _bluetoothManager.Write(commandString);
                }

                string answerString = "";
                KeyVendorAnswer answer = new KeyVendorAnswer(answerString);

                for (int i = 0; i * delay < timeout; i++)
                {
                    answerString += _bluetoothManager.Read();
                    answer.SetAnswerString(answerString);

                    if (answerString == "" || !answer.IsComplete)
                        await Task.Delay((int)delay);
                    else if (answer.IsComplete)
                        break;
                }

                return answer;
            });
        }

        public bool SplittingEnabled { get; set; }
        public uint PartLenght { get; set; }
        public uint TimeToReadPart { get; set; }
        private IBluetoothManager _bluetoothManager;
    }
}