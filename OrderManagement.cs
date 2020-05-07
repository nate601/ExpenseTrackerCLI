using System;
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


            return retVal;
        }


    }
}
