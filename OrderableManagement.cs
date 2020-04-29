using System;
using System.Collections.Generic;
using System.Linq;
using expenseTrackerCli.Database;

namespace expenseTrackerCli
{

    public static class OrderableManagement
    {
        public static void DisplayOrderables(Database.Database db)
        {
            OrderableItem[] orderables = db.GetOrderables().OrderBy((x) => x.Wic).ToArray();

            const int numberPerPage = 5;
            int numberOfPages = orderables.Length / numberPerPage;
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
                OrderableItem[] currentPage = orderables.Skip(currentPageIndex * numberPerPage).Take(numberPerPage).ToArray();
                Console.WriteLine($"Displaying {currentPage.Length} of {orderables.Length} items. Page {currentPageIndex + 1} / {numberOfPages + 1 }.");
                Console.WriteLine();
                Console.WriteLine($"|Index|Wic   |Name             |Cycle|");

                for (int i = 0; i < currentPage.Length; i++)
                {
                    OrderableItem k = currentPage[i];
                    Console.WriteLine($"|{i + 1,5:d2}|{k.Wic,6:d6}|{k.ItemName,-17}|{(k.twoWeekCycle ? 2 : 1),5:d2}|");
                }
                string resp = Program.AskUser("(n)ext page, (p)rev page, (q)uit, or press a number to edit.");

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
                    return;
                }
                else if (int.TryParse(resp, out int selectedItemIndex)
                         && selectedItemIndex > 0
                         && selectedItemIndex <= currentPage.Length)
                {
                    OrderableItem selectedItem = currentPage.ToArray()[selectedItemIndex - 1];
                    int oldWic = selectedItem.Wic;
                    while (true)
                    {
                        resp = Program.AskUser("(d)elete, (c)hange wic, change (n)ame, change c(y)cle, (f)inish)");

                        switch (resp)
                        {
                            case "d":
                                selectedItem = null;
                                goto finishEdit;
                            case "c":
                                selectedItem.Wic = int.Parse(new string(Program.AskUserNumber("New wic?").ToString().ToArray().Take(6).ToArray()));
                                goto finishEdit;
                            case "n":
                                selectedItem.ItemName = Program.AskUser("New name?");
                                goto finishEdit;
                            case "y":
                                selectedItem.twoWeekCycle = !selectedItem.twoWeekCycle;
                                goto finishEdit;
                            case "f":
                                goto finishEdit;
                            default:
                                Console.WriteLine("Invalid command");
                                break;
                        }
                    }
                finishEdit:
                    IEnumerable<OrderableItem> newOrderables;
                    newOrderables = selectedItem == null
                        ? orderables.Where((x) => x.Wic != oldWic)
                        : orderables.Where((x) => x.Wic != oldWic).Append(selectedItem);
                    db.OverwriteOrderableItems(newOrderables.ToArray());
                    orderables = db.GetOrderables().OrderBy((x) => x.Wic).ToArray();
                    numberOfPages = orderables.Length / numberPerPage;
                }
            }
        }
    }
}
