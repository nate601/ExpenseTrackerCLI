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
            var orderables = db.GetOrderables();
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
            rpt.AddBlank();
            rpt.AddTable(new string[] { "Order Date", "Number of Pending Items" }, data.ToArray());
            rpt.AddBlank();
            rpt.AddHeader("Number of Pending Items per Wic");
            rpt.AddLine("The following are the number of items for each wic that are pending");
            rpt.AddBlank();
            data = new List<string>();
            Dictionary<int, int> wicToPendingItemCountMap = new Dictionary<int, int>();
            orders.ToList().ForEach(currentOrder => currentOrder.orderedItems.ToList().ForEach(currentItem =>
            {
                if (currentItem.Value.Resolution?.received != true)
                {
                    if (wicToPendingItemCountMap.ContainsKey(currentItem.Key.Wic))
                    {
                        wicToPendingItemCountMap[currentItem.Key.Wic] += currentItem.Value.orderedAmount;
                    }
                    else
                    {
                        wicToPendingItemCountMap.Add(currentItem.Key.Wic, currentItem.Value.orderedAmount);
                    }
                }
            }));
            foreach (KeyValuePair<int, int> wicToPendingItem in wicToPendingItemCountMap.ToArray())
            {
                int wic = wicToPendingItem.Key;
                int numberOfItemsPending = wicToPendingItem.Value;

                data.Add(wic.ToString());
                data.Add(orderables.First(y => y.Wic == wic).ItemName);
                data.Add(numberOfItemsPending.ToString());
            }
            rpt.AddTable(new string[] { "Wic", "Item Name", "Number of Pending Items" }, data.ToArray());
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
