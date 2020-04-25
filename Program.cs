﻿using System;
using System.Collections.Generic;
using System.Linq;
using expenseTrackerCli.Database;

namespace expenseTrackerCli
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var db = new Database.Database();
                Console.WriteLine("NMF Record Generator");
                Console.WriteLine("1: Add new Orderable");
                Console.WriteLine("2: Add new Order");
                Console.WriteLine("3: Edit orderable database");
                var userSelection = Console.ReadLine();
                switch (userSelection)
                {
                    case "1":
                        //Add New Orderable	
                        orderablePrompt(db);
                        break;
                    case "2":
                        //Add New Order
                        orderPrompt(db);
                        break;
                    case "3":
                        displayOrderables(db);
                        break;
                    default:
                        Console.WriteLine("Invalid Entry");
                        break;
                }
            }
        }

        private static void displayOrderables(Database.Database db)
        {
            var orderables = db.GetOrderables().OrderBy((x) => x.Wic).ToArray();

            int numberPerPage = 5;
            int numberOfPages = orderables.Length / numberPerPage;
            int currentPageIndex = 0;

            while (true)
            {
                if (currentPageIndex < 0) currentPageIndex = 0;
                if (currentPageIndex > numberOfPages) currentPageIndex = numberOfPages;
                Console.Clear();
                var currentPage = orderables.Skip((currentPageIndex) * numberPerPage).Take(numberPerPage).ToArray();
                Console.WriteLine($"Displaying {currentPage.Count()} of {orderables.Length} items. Page {currentPageIndex + 1} / {numberOfPages + 1 }.");
                Console.WriteLine();
		Console.WriteLine($"|Index|Wic   |Name             |Cycle|");
                for (var i = 0; i < currentPage.Count(); i++)
                {
                    var k = currentPage[i];
                    Console.WriteLine($"{i + 1,5:d2}|{k.Wic,6:d6}|{k.ItemName,-17}");
                }
                var resp = AskUser("(n)ext page, (p)rev page, or press a number to edit.");
                if (resp == "n")
                {
                    currentPageIndex++;
                }
                else if (resp == "p")
                {
                    currentPageIndex--;
                }
                else if (int.TryParse(resp, out var selectedItemIndex)
                         && selectedItemIndex > 0
                         && selectedItemIndex <= currentPage.Count())
                {
                    var selectedItem = currentPage.ToArray()[selectedItemIndex - 1];
                    while (true)
                    {
                        resp = AskUser("(d)elete, (c)hange wic, change (n)ame, change c(y)cle, (f)inish)");
			switch(resp)
			{
			    case "d":
				selectedItem = null;
				break;
			    case "c":
				selectedItem.Wic = int.Parse(new string(AskUserNumber("New wic?").ToString().ToArray().Take(6).ToArray()));
				break;
			    case "n":
				selectedItem.ItemName = AskUser("New name?");
				break;
			    case "y":
				selectedItem.twoWeekCycle = !selectedItem.twoWeekCycle;
				break;
			    case "f":
				goto finishEdit;
			    default:
				break;
			}
                    }
finishEdit:
		    if(selectedItem == null)
			break;
                    var newOrderables = orderables.Where((x) => x.Wic != selectedItem.Wic).Append(selectedItem);
                    db.OverwriteOrderableItems(newOrderables.ToArray());
                }
            }
        }

        private static void orderPrompt(Database.Database db)
        {

            var orderableItems = db.GetOrderables();
            Database.ExpenseOrder order = new Database.ExpenseOrder();
            order.OrderDate = new DateTime(AskUserNumber("Year"),
                                           AskUserNumber("Month"),
                                           AskUserNumber("Day"));
            //order.OrderDate = DateTime.Now;
            int daysUntilThursday = ((int)DayOfWeek.Thursday - (int)order.OrderDate.DayOfWeek + 7) % 7;
            order.ExpectedDateOneCycle = order.OrderDate.AddDays(daysUntilThursday);
            order.ExpectedDateTwoCycle = order.ExpectedDateOneCycle.AddDays(7);

            order.orderedItems = new Dictionary<Database.OrderableItem, Database.OrderedItemInfo>();
            while (true)
            {
                displayOrderPreorder(order);
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
                            Database.OrderedItemInfo oldItem = order.orderedItems.First((x) => x.Key.Wic == wic).Value;
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
                            orderablePrompt(db);
                            orderableItems = db.GetOrderables();
                            continue;
                        }
                    }
                }
                else if (resp == "f")
                {
                    break;
                }
                else if (resp == "s")
                {
                    foreach (var k in new Dictionary<int, int>()
            {
                {957754, 3},
                {828188, 4},
                {829838, 4},
                {829839, 1},
                {829841, 1},
                {962843, 8},
                {958293, 5},
                {964586, 2},
                {958292, 1},
                {964588, 1},
                {475071, 1},
                {475070, 1},
                {475069, 1},
                {475068, 1},
                {475067, 1},
                {475066, 1},
                {475065, 1},
                {959737, 1},
                {958843, 1},
        {345150, 3},
        {963233, 4},
        {216735, 1},
        {219561, 1},
        {225610, 3},
        {964880, 3},
        {961510, 3},
        {274002, 1},
        {151913, 1},
        {957507, 1}
            }
                    .Where(k => orderableItems.Any((x) => x.Wic == k.Key)))
                    {
                        var item = orderableItems.First((x) => x.Wic == k.Key);
                        Console.WriteLine($"{item.Wic} : {item.ItemName}");
                        var onHand = AskUserNumber("On Hand");
                        int orderThis;
                        if (onHand < k.Value)
                        {
                            orderThis = k.Value - onHand;
                        }
                        else { orderThis = 0; }

                        Console.WriteLine("Suggested on hand value should be " + k.Value);
                        if (orderThis != 0)
                        {
                            if (!AskUserBool("Order this amount? (" + orderThis + ")"))
                            {
                                order.orderedItems.Add(item, new Database.OrderedItemInfo(onHand, AskUserNumber("Quantity")));
                            }
                            else
                            {
                                order.orderedItems.Add(item, new Database.OrderedItemInfo(onHand, orderThis));
                            }
                        }
                        else
                        {
                            if (AskUserBool("Current amount is sufficient. Order more?"))
                            {
                                order.orderedItems.Add(item, new Database.OrderedItemInfo(onHand, AskUserNumber("Quantity")));
                            }
                            else
                            {
                                order.orderedItems.Add(item, new Database.OrderedItemInfo(onHand, 0));
                            }
                        }
                    }
                }
            }
            db.SaveNewOrder(order);

        }
        private static void displayOrderPreorder(Database.ExpenseOrder order)
        {
            Console.Clear();
            Console.WriteLine($"Order for {order.OrderDate.ToShortDateString()}");
            Console.WriteLine($"Expected Arrival {order.ExpectedDateOneCycle.ToShortDateString(),6} ({order.ExpectedDateTwoCycle.ToShortDateString():d2})");
            Console.WriteLine();
            Console.WriteLine("Items: ");
            Console.WriteLine();
            Console.WriteLine($"|Wic   |Item Name      |On Hand|Ordered|");
            Console.WriteLine();
            foreach (var s in order.orderedItems)
            {
                Console.WriteLine($"|{s.Key.Wic,6:d6}|{s.Key.ItemName,-15}|{s.Value.onHand,7:d3}|{s.Value.orderedAmount,7:d3}|");
            }
            Console.WriteLine();
        }
        private static void displayOrderResolve(Database.ExpenseOrder order)
        {
            Console.Clear();
            Console.WriteLine($"Order for {order.OrderDate.ToShortDateString()}");
            Console.WriteLine($"Expected Arrival {order.ExpectedDateOneCycle.ToShortDateString(),6} ({order.ExpectedDateTwoCycle.ToShortDateString():d2})");
            Console.WriteLine();
            Console.WriteLine("Items: ");
            Console.WriteLine();
            Console.WriteLine($"|Wic   |Item Name      |On Hand|Ordered|New On Hand|Recieved|Deficit|");
            Console.WriteLine();
            foreach (var s in order.orderedItems)
            {
                Console.WriteLine($"|{s.Key.Wic,6:d6}|{s.Key.ItemName,-15}|{s.Value.onHand,7:d3}|{s.Value.orderedAmount,7:d3}|");
            }
            Console.WriteLine();
        }

        private static void orderablePrompt(Database.Database db)
        {
            Database.OrderableItem item = new Database.OrderableItem(
                AskUserNumber("Wic"),
                AskUser("Item Name"),
                AskUserNumber("Package Size"),
                AskUserBool("Two Week Cycle")
                );
            db.SaveNewOrderableItem(item);
        }

        private static bool AskUserBool(string v)
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

        private static string AskUser(string prompt)
        {
            Console.Write($"{prompt}: ");
            return Console.ReadLine();
        }
        private static int AskUserNumber(string prompt)
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
