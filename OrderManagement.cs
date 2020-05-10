using System;
using System.Collections.Generic;
using System.Linq;
using expenseTrackerCli.Database;

namespace expenseTrackerCli
{
    internal static class OrderManagement
    {
        public static ExpenseOrder EditOrder(ExpenseOrder order)
        {
            KeyValuePair<OrderableItem, OrderedItemInfo>[] orderItems = order.orderedItems.ToArray();
            do
            {
                Console.Clear();
                ConsoleUtilities.DisplayOrderPreorder(order);
                int resp = ConsoleUtilities.AskUserNumber("Choose Wic to edit.");
                if (resp.ToString().Length != 6 || !orderItems.Any(x => x.Key.Wic == resp))
                {
                    continue;
                }
                KeyValuePair<OrderableItem, OrderedItemInfo> selectedItem = orderItems.First(x => x.Key.Wic == resp);

                selectedItem.Value.onHand = ConsoleUtilities.AskUserNumber($"The previous order records {selectedItem.Value.onHand} items as being on hand.  New value = ?");
                selectedItem.Value.orderedAmount = ConsoleUtilities.AskUserNumber($"The previous order records {selectedItem.Value.orderedAmount} items as being ordered.  New value = ?");

                order.orderedItems = orderItems.ToDictionary(x => x.Key, x => x.Value);
            }
            while (!ConsoleUtilities.AskUserBool("Finished editing?"));
            return order;
        }

        public static ExpenseOrder ChooseOrder(Database.Database db)
        {
            if (db is null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            ExpenseOrder[] orders = db.GetOrders();
            Console.Clear();

            const int numberPerPage = 5;
            int numberOfPages = orders.Length / numberPerPage;
            int currentPageIndex = 0;

            ExpenseOrder retVal = null;

            while (retVal == null)
            {
                if (currentPageIndex < 0)
                {
                    currentPageIndex = 0;
                }
                if (currentPageIndex > numberOfPages)
                {
                    currentPageIndex = numberOfPages;
                }
                Console.Clear();
                ExpenseOrder[] currentPage = orders.Skip(currentPageIndex * numberPerPage).Take(numberPerPage).ToArray();
                Console.WriteLine($"Displaying {currentPage.Length} of {orders.Length} orders.");

                for (int i = 0; i < currentPage.Length; i++)
                {
                    ExpenseOrder order = currentPage[i];
                    Console.WriteLine($"{i + 1} : Order from {order.OrderDate.ToShortDateString()}");
                }
                string resp = ConsoleUtilities.AskUser("(n)ext page, (p)rev page, (q)uit, or press a number to select an order.");
                if (resp == "n")
                {
                    currentPageIndex++;
                }
                else if (resp == "p")
                {
                    currentPageIndex--;
                }
                else if (resp == "q")
                {
                    return null;
                }
                else if (int.TryParse(resp, out int selectedItemIndex)
                         && selectedItemIndex > 0
                         && selectedItemIndex <= currentPage.Length)
                {
                    return currentPage.ToArray()[selectedItemIndex - 1];
                }
            }
            return null;
        }
    }
}
