using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace expenseTrackerCli.Database
{
    public partial class Database
    {
        public ExpenseOrder[] GetOrders()
        {
            if (!File.Exists("orders.dat"))
            {
                return new List<ExpenseOrder>().ToArray();
            }
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = File.Open("orders.dat", FileMode.Open);
            object obj = formatter.Deserialize(fs);
            fs.Flush();
            fs.Close();
            fs.Dispose();
            return (ExpenseOrder[])obj;
        }
        public void SaveNewOrder(ExpenseOrder order)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            var newOrders = GetOrders().Append(order).ToArray();
            File.Delete("orders.dat");
            Stream ms = File.OpenWrite("orders.dat");
            formatter.Serialize(ms, newOrders);
            ms.Flush();
            ms.Close();
            ms.Dispose();
        }
        private void OverwriteOrders(ExpenseOrder[] orders)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            File.Delete("orders.dat");
            Stream ms = File.OpenWrite("orders.dat");
            formatter.Serialize(ms, orders);
            ms.Flush();
            ms.Close();
            ms.Dispose();
        }
        public void DeleteOrder(ExpenseOrder[] orders, int index)
        {
            var k = orders.ToList();
            k.RemoveAt(index);
            OverwriteOrders(orders);
        }
    }

    [Serializable]
    public class ExpenseOrder
    {
        public DateTime OrderDate;
        public DateTime ExpectedDateOneCycle;
        public DateTime ExpectedDateTwoCycle;
        public Dictionary<OrderableItem, OrderedItemInfo> orderedItems;

        [Serializable]
        public class OrderedItemInfo
        {
            public int onHand, orderedAmount;

            public OrderedItemInfo(int onHand, int orderedAmount)
            {
                this.onHand = onHand;
                this.orderedAmount = orderedAmount;
            }
        }

        public ExpenseOrder()
        {
        }

        public ExpenseOrder(DateTime orderDate, DateTime expectedDateOneCycle, DateTime expectedDateTwoCycle, Dictionary<OrderableItem, OrderedItemInfo> orderedItems)
        {
            OrderDate = orderDate;
            ExpectedDateOneCycle = expectedDateOneCycle;
            ExpectedDateTwoCycle = expectedDateTwoCycle;
            this.orderedItems = orderedItems;
        }
    }
}
