using System;
using System.Collections.Generic;
using System.Linq;
using expenseTrackerCli.Database;

namespace expenseTrackerCli
{
    internal static class Program
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

        private static void Main()
        {
            while (true)
            {
                var db = new Database.Database();
                Console.WriteLine("NMF Record Generator");
                Console.WriteLine("1: Add new Orderable");
                Console.WriteLine("2: Add new Order");
                Console.WriteLine("3: Edit orderable database");
                switch (Console.ReadLine())
                {
                    case "1":
                        // Add New Orderable
                        OrderablePrompt(db);
                        break;
                    case "2":
                        // Add New Order
                        OrderPrompt(db);
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


        private static void OrderPrompt(Database.Database db)
        {

            OrderableItem[] orderableItems = db.GetOrderables();
            ExpenseOrder order = new ExpenseOrder
            {
                OrderDate = new DateTime(
                    AskUserNumber("Year"),
                    AskUserNumber("Month"),
                    AskUserNumber("Day")),
            };
            int daysUntilThursday = ((int)DayOfWeek.Thursday - (int)order.OrderDate.DayOfWeek + 7) % 7;
            order.ExpectedDateOneCycle = order.OrderDate.AddDays(daysUntilThursday);
            order.ExpectedDateTwoCycle = order.ExpectedDateOneCycle.AddDays(7);

            order.orderedItems = new Dictionary<OrderableItem, OrderedItemInfo>();
            while (true)
            {
                DisplayOrderPreorder(order);
                Console.WriteLine("Add item by (w)ic, or search by (n)ame.");
                Console.WriteLine("(s)uggested Order");
                Console.WriteLine("(f)inish order");
                var resp = AskUser("");
                if (resp == "w")
                {
                    var wic = AskUserNumber("Wic");
                    if (orderableItems.Any((x) => x.Wic == wic))
                    {
                        if (order.orderedItems.ContainsKey(orderableItems.First((x) => x.Wic == wic)))
                        {
                            Console.WriteLine("That item is already in the order.");
                            OrderedItemInfo oldItem = order.orderedItems.First((x) => x.Key.Wic == wic).Value;
                            oldItem.onHand = AskUserNumber($"Modify on hand amount ({oldItem.onHand})");
                            oldItem.orderedAmount = AskUserNumber($"Modify ordered amount ({oldItem.orderedAmount})");
                        }
                        else
                        {
                            order.orderedItems.Add(orderableItems.First((x) => x.Wic == wic), new Database.OrderedItemInfo(AskUserNumber("On hand amount"), AskUserNumber("Ordered Amount")));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Wic");
                        if (AskUserBool("Add new item?"))
                        {
                            OrderablePrompt(db);
                            orderableItems = db.GetOrderables();
                        }
                    }
                }
                else if (resp == "f")
                {
                    break;
                }
                else if (resp == "s")
                {
                    foreach (KeyValuePair<int, int> k in new Dictionary<int, int>()
            {
                { 957754, 3 },
                { 828188, 4 },
                { 829838, 4 },
                { 829839, 1 },
                { 829841, 1 },
                { 962843, 8 },
                { 958293, 5 },
                { 964586, 2 },
                { 958292, 1 },
                { 964588, 1 },
                { 475071, 1 },
                { 475070, 1 },
                { 475069, 1 },
                { 475068, 1 },
                { 475067, 1 },
                { 475066, 1 },
                { 475065, 1 },
                { 959737, 1 },
                { 958843, 1 },
                { 345150, 3 },
                { 963233, 4 },
                { 216735, 1 },
                { 219561, 1 },
                { 225610, 3 },
                { 964880, 3 },
                { 961510, 3 },
                { 274002, 1 },
                { 151913, 1 },
                { 957507, 1 },
            }
                    .Where(k => orderableItems.Any((x) => x.Wic == k.Key)))
                    {
                        OrderableItem item = orderableItems.First((x) => x.Wic == k.Key);
                        Console.WriteLine($"{item.Wic} : {item.ItemName}");
                        int onHand = AskUserNumber("On Hand");
                        int orderThis = onHand < k.Value ? k.Value - onHand : 0;
                        Console.WriteLine("Suggested on hand value should be " + k.Value);
                        if (orderThis != 0)
                        {
                            if (!AskUserBool("Order this amount? (" + orderThis + ")"))
                            {
                                order.orderedItems.Add(item, new OrderedItemInfo(onHand, AskUserNumber("Quantity")));
                            }
                            else
                            {
                                order.orderedItems.Add(item, new OrderedItemInfo(onHand, orderThis));
                            }
                        }
                        else if (AskUserBool("Current amount is sufficient. Order more?"))
                        {
                            order.orderedItems.Add(item, new OrderedItemInfo(onHand, AskUserNumber("Quantity")));
                        }
                        else
                        {
                            order.orderedItems.Add(item, new OrderedItemInfo(onHand, 0));
                        }
                    }
                }
            }
            db.SaveNewOrder(order);
        }

        private static void DisplayOrderPreorder(ExpenseOrder order)
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

        private static void OrderablePrompt(Database.Database db)
        {
            OrderableItem item = new OrderableItem(
                AskUserNumber("Wic"),
                AskUser("Item Name"),
                AskUserNumber("Package Size"),
                AskUserBool("Two Week Cycle"));
            db.SaveNewOrderableItem(item);
        }

    }
}
