using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace expenseTrackerCli.Database
{
    public partial class Database
    {
	private bool SaveNew<T>(T serializeThing, T[] oldThings, string fileName)
	{
	    if(oldThings.Contains(serializeThing))
		return false;
	    BinaryFormatter formatter = new BinaryFormatter();
	    var newOrders = oldThings.Append(serializeThing).ToArray(); 
	    Stream ms = File.OpenWrite(fileName);
	    formatter.Serialize(ms, newOrders);
	    ms.Flush();
	    ms.Close();
	    ms.Dispose();
	    return true;
	}
    }
}
