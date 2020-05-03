using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace expenseTrackerCli.Database
{
    public partial class Database
    {
        public OrderableItem[] GetOrderables()
        {
            if (!File.Exists("orderable.dat"))
            {
                return new List<OrderableItem>().ToArray();
            }
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = File.Open("orderable.dat", FileMode.Open);
            object obj = formatter.Deserialize(fs);
            fs.Flush();
            fs.Close();
            fs.Dispose();
            return this.CleanOrderableItems((OrderableItem[])obj);
        }
        public void SaveNewOrderableItem(OrderableItem item)
        {
            _ = SaveNew(
                item,
                this.GetOrderables(),
                "orderable.dat");
        }
        public void OverwriteOrderableItems(OrderableItem[] items)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            File.Delete("orderable.dat");
            Stream ms = File.OpenWrite("orderable.dat");
            formatter.Serialize(ms, items);
            ms.Flush();
            ms.Close();
            ms.Dispose();
        }
        public OrderableItem[] CleanOrderableItems(OrderableItem[] items)
        {
            var retVal = items.ToList();
            foreach (var item in items)
            {
                if (items.Count(x => x.Wic == item.Wic) > 1)
                {
                    _ = retVal.Remove(retVal.First(x => x.Wic == item.Wic));
                }
            }
            return retVal.ToArray();
        }
    }

    [Serializable]
    public class OrderableItem
    {
        public int Wic;
        public string ItemName;
        public int PackageSize;
        public bool twoWeekCycle;
        public int onHandAmount;

        public OrderableItem()
        {
        }

        public OrderableItem(int wic, string itemName, int packageSize, bool twoWeekCycle)
        {
            Wic = wic;
            ItemName = itemName;
            PackageSize = packageSize;
            this.twoWeekCycle = twoWeekCycle;
        }

    }
}
