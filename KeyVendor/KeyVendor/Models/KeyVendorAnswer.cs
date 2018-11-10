using System;
using System.Diagnostics;
using System.Linq;

namespace KeyVendor.Models
{
    public class KeyVendorAnswer
    {
        public KeyVendorAnswer(string answerString)
        {
            Data = "";
            IsCorrect = false;
            IsComplete = true;

            SetAnswerString(answerString);
        }

        public void SetAnswerString(string answerString)
        {
            IsCorrect = false;
            int answerLength = answerString.Length;

            if (answerLength > 0 && answerString[0] == '$' && (answerLength < 3 || answerString[answerLength - 1] != '$'))
            {
                IsComplete = false;
                Debug.WriteLine("Answer is not complete");
                return;
            }
            if (answerLength < 3 || answerString[0] != '$' || answerString[answerLength-1] != '$')
            {
                Debug.WriteLine("Answer is incorrect");
                return;
            }

            string commandTypeString = "";
            string answerTypeString = "";
            string dataString = "";

            int i = 1;

            while (i < (answerString.Length - 1) && answerString[i] != '@')
            {
                commandTypeString += answerString[i];
                i++;
            }
            i++;
            while (i < (answerString.Length - 1) && answerString[i] != '@')
            {
                answerTypeString += answerString[i];
                i++;
            }
            i++;
            while (i < (answerString.Length - 1) && answerString[i] != '$')
            {
                dataString += answerString[i];
                i++;
            }

            Debug.WriteLine(commandTypeString);
            Debug.WriteLine(answerTypeString);
            Debug.WriteLine(dataString);

            if (commandTypeString == "" || answerTypeString == "")
                return;

            int commandTypeNumber = 0;
            int answerTypeNumber = 0;

            if (!int.TryParse(commandTypeString, out commandTypeNumber) || 
                !int.TryParse(answerTypeString, out answerTypeNumber))
                return;

            var listOfCommandTypes = Enum.GetValues(typeof(KeyVendorCommandType)).Cast<KeyVendorCommandType>().ToList();
            var listOfCommandTypeResults = listOfCommandTypes.FindAll((c) => { return (int)c == commandTypeNumber; });

            var listOfAnswerTypes = Enum.GetValues(typeof(KeyVendorAnswerType)).Cast<KeyVendorAnswerType>().ToList();
            var listOfAnswerTypeResults = listOfAnswerTypes.FindAll((c) => { return (int)c == answerTypeNumber; });

            if (listOfCommandTypeResults.Count == 0 || listOfAnswerTypeResults.Count == 0)
                return;

            CommandType = listOfCommandTypeResults[0];
            AnswerType = listOfAnswerTypeResults[0];
            Data = dataString;

            IsCorrect = true;
        }

        public KeyVendorCommandType CommandType { get; protected set; }
        public KeyVendorAnswerType AnswerType { get; protected set; }
        public string Data { get; protected set; }
        public bool IsCorrect { get; protected set; }
        public bool IsComplete { get; protected set; }
    }
}
