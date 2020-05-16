using expenseTrackerCli.Database;

namespace expenseTrackerCli
{
    internal static class Reporting
    {
        internal static void CreateNotReceiptReport(Database.Database db)
        {
            db.GetOrders();
        }

        internal static void CreateOrderReport(ExpenseOrder order)
        {

        }
        internal static void CreateOrderReceiptReport(ExpenseOrder order)
        {

        }
    
    }
}
