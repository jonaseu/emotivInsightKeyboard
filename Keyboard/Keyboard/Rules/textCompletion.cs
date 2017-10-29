using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using NHunspell;

namespace Keyboard.Rules
{
    class textCompletion
    {
        public static List<string> suggestWords(string wordToCheck, string language)
        {
            string affFile;
            string dicFile;

            if (language == "pt_BR")
            {

                affFile = "pt_BR.aff";
                dicFile = "pt_BR.dic";
            }
            else
            {
                if (language != "pt_BR")
                    Console.WriteLine("Unknow language, english defined.");
                affFile = "en_us.aff";
                dicFile = "en_us.dic";
            }

            using (Hunspell hunspell = new Hunspell(affFile, dicFile))
            {
                List<string> suggestions = hunspell.Suggest(wordToCheck);
                hunspell.Suggest(wordToCheck);
                Console.WriteLine("There are " + suggestions.Count.ToString() + " suggestions");
                foreach (string suggestion in suggestions)
                {
                    Console.WriteLine("Suggestion is: " + suggestion);
                }
                return suggestions;
            }
        }

        public static List<string> suggestWordsSimple(string wordToCheck, string language)
        {
            string dicFile;

            if (language == "pt_BR")
                dicFile = "pt_BR_simple.dic";
            else
            {
                if (language != "pt_BR")
                    Console.WriteLine("Unknow language, english defined.");
                dicFile = "en_simple.dic";
            }

            StreamReader sr = new StreamReader(dicFile, Encoding.GetEncoding("iso-8859-1"));

            //Read the first line of text
            string line = sr.ReadLine();
            
            List<string> suggestions = new List<string>();
            //Continue to read until you reach end of file
            while (line != null && wordToCheck != " " && wordToCheck != "")
            {
                int value = CalcLevenshteinDistance(wordToCheck, line);
                if (line.StartsWith(wordToCheck) || CalcLevenshteinDistance(wordToCheck.ToLower(),line) < 2)
                {
                    if (line.Length >= wordToCheck.Length && line != wordToCheck)
                        suggestions.Add(line);
                    if (suggestions.Count() >= 5)
                        break;
                }
                line = sr.ReadLine();
            }
            sr.Close();
            return suggestions;
        }

        private static int CalcLevenshteinDistance(string a, string b)
        {
            if (String.IsNullOrEmpty(a) || String.IsNullOrEmpty(b)) return 0;

            int lengthA = a.Length;
            int lengthB = b.Length;
            var distances = new int[lengthA + 1, lengthB + 1];
            for (int i = 0; i <= lengthA; distances[i, 0] = i++) ;
            for (int j = 0; j <= lengthB; distances[0, j] = j++) ;

            for (int i = 1; i <= lengthA; i++)
                for (int j = 1; j <= lengthB; j++)
                {
                    int cost = b[j - 1] == a[i - 1] ? 0 : 1;
                    distances[i, j] = Math.Min
                        (
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                        );
                }
            return distances[lengthA, lengthB];
        }
    }
}
