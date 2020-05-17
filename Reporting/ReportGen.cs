using System;
using System.Collections.Generic;
using System.Linq;
using expenseTrackerCli.Database;
using expenseTrackerCli.Reporting;

namespace expenseTrackerCli
{
    internal static class ReportGen
    {
        internal static void CreateNotReceiptReport(Database.Database db, DateTime reportAsOf)
        {
            if (db is null)
            {
                throw new ArgumentNullException(nameof(db));
            }

            Report rpt = new Report("PendingReceipt");
            ExpenseOrder[] orders = db.GetOrders();
            // Get list of orders that have at least one item that has not been received
            orders = orders.Where(currentOrder => currentOrder.orderedItems.Any(currentOrderedItem => currentOrderedItem.Value.Resolution?.received != true)).ToArray();
            int totalNumberOfMissingItems = 0;
            orders.ToList().ForEach(x => x.orderedItems.ToList().ForEach(y =>
            {
                if (y.Value.Resolution?.received != true)
                {
                    totalNumberOfMissingItems++;
                }
            }));

            rpt.AddHeader("Introduction");
            rpt.AddBlank();
            rpt.AddLine($"This file describes the pending status of pharmacy expense items.  This file is accurate as of {reportAsOf.ToShortDateString()}.  There are {orders.Length} orders with pending items.  There are {totalNumberOfMissingItems} items that are pending.  These items are critical to the operation of the pharmacy.");
            rpt.AddBlank();
            rpt.AddHeader("Orders with Pending Items");
            rpt.AddLine("The following orders have pending items.");
            List<string> data = new List<string>();
            orders.ToList().ForEach(x =>
            {
                data.Add(x.OrderDate.ToShortDateString());
                data.Add(x.orderedItems.Count(y => y.Value.Resolution?.received != true).ToString());
            });
            rpt.AddTable(new string[] { "Order Date", "Number of Pending Items" }, data.ToArray());
            rpt.AddBlank();
            rpt.AddHeader("Number of Pending Items per Wic");
            rpt.AddLine("The following are the number of items for each wic that are pending");
            rpt.AddBlank();
            data.Clear();
            Dictionary<string, int> wicToPendingItemCountMap = new Dictionary<string, int>();
            orders.ToList().ForEach(x => x.orderedItems.ToList().ForEach(y =>
            {
                if (y.Value.Resolution?.received != true)
                {
                    if (wicToPendingItemCountMap.ContainsKey(y.Key.Wic.ToString()))
                    {
                        wicToPendingItemCountMap[y.Key.Wic.ToString()]++;
                    }
                    else
                    {
                        wicToPendingItemCountMap.Add(y.Key.Wic.ToString(), 1);
                    }

                }
            }));
            wicToPendingItemCountMap.ToArray().ToList().ForEach(x =>
            {
                data.Add(x.Key);
                data.Add(x.Value.ToString());
            });
            rpt.AddTable(new string[] { "Wic", "Number of Pending Items" }, data.ToArray());
            rpt.DebugPrintToConsole();
            rpt.SaveReport();


        }

        internal static void CreateOrderReport(ExpenseOrder order)
        {

        }
        internal static void CreateOrderReceiptReport(ExpenseOrder order)
        {

        }

    }
}
