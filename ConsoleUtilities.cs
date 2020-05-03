using System;
using System.Collections.Generic;
using System.Linq;
using expenseTrackerCli.Database;

namespace expenseTrackerCli
{
    internal static class ConsoleUtilities
    {
        public static bool AskUserBool(string v)
        {
            while (true)
            {
                var resp = AskUser($"{v} (true/false) or (1/0)");
                if (resp == "0")
                {
                    return false;
                }
                else if (resp == "1")
                {
                    return true;
                }
                if (bool.TryParse(resp, out var result))
                {
                    return result;
                }
            }
        }

        public static string AskUser(string prompt)
        {
            Console.Write($"{prompt}: ");
            return Console.ReadLine();
        }

        public static int AskUserNumber(string prompt)
        {
            while (true)
            {
                var resp = AskUser(prompt);
                if (int.TryParse(resp, out var k))
                {
                    return k;
                }
            }
        }

    }
}
