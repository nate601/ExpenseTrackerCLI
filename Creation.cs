using System;
using System.Collections.Generic;
using System.Linq;
using expenseTrackerCli.Database;
using static expenseTrackerCli.ConsoleUtilities;

namespace expenseTrackerCli
{
    internal class Creation
    {
        internal static void OrderablePrompt(Database.Database db)
        {
            OrderableItem[] currentOrders = db.GetOrderables();
            bool isOrderableValid = true;
            int AskValidWic()
            {
                var newWic = AskUserNumber("Wic");
                if (currentOrders.Any(x => x.Wic == newWic))
                {
                    Console.WriteLine("Wic is not unique!");
                    isOrderableValid = false;
                }
                return newWic;
            }

            OrderableItem item = new OrderableItem(
                AskValidWic(),
                AskUser("Item Name"),
                AskUserNumber("Package Size"),
                AskUserBool("Two Week Cycle"));
            if (!isOrderableValid)
            {
                Console.WriteLine("Unable to save. Item is invalid!");
            }
            db.SaveNewOrderableItem(item);
        }

        internal static void OrderPrompt(Database.Database db)
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

    }
}
