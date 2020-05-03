using System;
using System.Collections.Generic;
using System.Linq;
using expenseTrackerCli.Database;
using static expenseTrackerCli.ConsoleUtilities;

namespace expenseTrackerCli
{
    internal static class Program
    {

        private static void Main()
        {
            while (true)
            {
                Database.Database db = new Database.Database();
                Console.WriteLine("NMF Record Generator");
                Console.WriteLine("1: Add new Orderable");
                Console.WriteLine("2: Add new Order");
                Console.WriteLine("3: Edit orderable database");
                switch (Console.ReadLine())
                {
                    case "1":
                        // Add New Orderable
                        Creation.OrderablePrompt(db);
                        break;
                    case "2":
                        // Add New Order
                        Creation.OrderPrompt(db);
                        break;
                    case "3":
                        OrderableManagement.DisplayOrderables(db);
                        break;
                    default:
                        Console.WriteLine("Invalid Entry");
                        break;
                }
            }
        }


        internal static void DisplayOrderPreorder(ExpenseOrder order)
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

        private static void DisplayOrderResolve(Database.ExpenseOrder order)
        {
            Console.Clear();
            Console.WriteLine($"Order for {order.OrderDate.ToShortDateString()}");
            Console.WriteLine($"Expected Arrival {order.ExpectedDateOneCycle.ToShortDateString(),6} ({order.ExpectedDateTwoCycle.ToShortDateString():d2})");
            Console.WriteLine();
            Console.WriteLine("Items: ");
            Console.WriteLine();
            Console.WriteLine($"|Wic   |Item Name      |On Hand|Ordered|New On Hand|Recieved|Deficit|");
            Console.WriteLine();
            foreach (KeyValuePair<OrderableItem, OrderedItemInfo> s in order.orderedItems)
            {
                Console.WriteLine($"|{s.Key.Wic,6:d6}|{s.Key.ItemName,-15}|{s.Value.onHand,7:d3}|{s.Value.orderedAmount,7:d3}|");
            }
            Console.WriteLine();
        }


    }
}
