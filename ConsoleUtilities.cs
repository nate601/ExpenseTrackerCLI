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
                string resp = AskUser(prompt);
                if (int.TryParse(resp, out int k))
                {
                    return k;
                }
            }
        }

        public static void DisplayOrderPreorder(ExpenseOrder order)
        {
            Console.Clear();
            Console.WriteLine($"Order for {order.OrderDate.ToShortDateString()}");
            Console.WriteLine($"Expected Arrival {order.ExpectedDateOneCycle.ToShortDateString(),6} ({order.ExpectedDateTwoCycle.ToShortDateString():d2})");
            Console.WriteLine();
            Console.WriteLine("Items: ");
            Console.WriteLine();
            Console.WriteLine($"|Wic   |Item Name      |On Hand|Ordered|");
            Console.WriteLine();
            foreach (KeyValuePair<OrderableItem, OrderedItemInfo> s in order.orderedItems)
            {
                Console.WriteLine($"|{s.Key.Wic,6:d6}|{s.Key.ItemName,-15}|{s.Value.onHand,7:d3}|{s.Value.orderedAmount,7:d3}|");
            }
            Console.WriteLine();
        }
    }
}
