using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDIWindowManagerTestForm
{
    internal static class RandomLoremWords
    {
        private static readonly string[] LoremWords = new string[]
        {
            "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit",
            "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore",
            "magna", "aliqua", "ut", "enim", "ad", "minim", "veniam", "quis", "nostrud",
            "exercitation", "ullamco", "laboris", "nisi", "ut", "aliquip", "ex", "ea",
            "commodo", "consequat"
        };

        private static readonly Random random = new Random();

        public static string Generate(int minWords, int maxWords, int minParagraphs, int maxParagGraphs)
        {
            if (minWords <= 0 || maxWords <= 0 || minParagraphs <= 0 || maxParagGraphs <= 0)
                return string.Empty;

            StringBuilder result = new StringBuilder();

            for (int p = 0; p < random.Next(minParagraphs, maxParagGraphs); p++)
            {
                if (p > 0) result.AppendLine().AppendLine();

                for (int w = 0; w < random.Next(minWords, maxWords); w++)
                {
                    if (w > 0) result.Append(" ");

                    result.Append(LoremWords[random.Next(LoremWords.Length)]);
                }
            }

            return result.ToString();
        }
    }
}
