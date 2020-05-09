using System;
using System.Linq;
using expenseTrackerCli.Database;

namespace expenseTrackerCli
{
    internal static class OrderManagement
    {

        public static ExpenseOrder ChooseOrder(Database.Database db)
        {
            ExpenseOrder retVal = null;
            if (db is null)
            {
                throw new System.ArgumentNullException(nameof(db));
            }
            ExpenseOrder[] orders = db.GetOrders();
            Console.Clear();

            const int numberPerPage = 5;
            int numberOfPages = orders.Length / numberPerPage;
            int currentPageIndex = 0;

            while (true)
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

        }


    }
}
