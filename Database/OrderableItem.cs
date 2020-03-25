using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace expenseTrackerCli.Database
{
    public partial class Database
    {
	public OrderableItem[] GetOrderables()
	{
	    if(!File.Exists("orderable.dat"))
	    {
		return new List<OrderableItem>().ToArray();
	    }
	    BinaryFormatter formatter = new BinaryFormatter();
	    FileStream fs = File.Open("orderable.dat", FileMode.Open);
	    object obj = formatter.Deserialize(fs);
	    fs.Flush();
	    fs.Close();
	    fs.Dispose();
	    return (OrderableItem[])obj;
	}
        public void SaveNewOrderableItem(OrderableItem item) => SaveNew<OrderableItem>(item,
            GetOrderables(),
            "orderable.dat");

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
