using System;

namespace expenseTrackerCli
{
    internal static class Program
    {
        private static void Main()
        {
            while (true)
            {
                Database.Database db = new Database.Database();
                NMFRecordPrompt(db);
            }
        }

        private static void NMFRecordPrompt(Database.Database db)
        {
            Console.WriteLine("NMF Record Generator");
            Console.WriteLine("1: Add new Orderable");
            Console.WriteLine("2: Add new Order");
            Console.WriteLine("3: Edit orderable database");
            Console.WriteLine("4: Edit order");
            Console.WriteLine("5: Receipt");
            Console.WriteLine("6: Print Reports");
            switch (Console.ReadLine())
            {
                case "1":
                    // Add New Orderable
                    Creation.OrderablePrompt(db);
                    break;
                case "2":
                    // Add New Order
                    Creation.OrderPrompt(db);
                    break;
                case "3":
                    OrderableManagement.DisplayOrderables(db);
                    break;
                case "4":
                    Database.ExpenseOrder editOrderOrder = OrderManagement.ChooseOrder(db);
                    if (editOrderOrder != null)
                    {
                        editOrderOrder = OrderManagement.EditOrder(editOrderOrder);
                        db.OverwriteOrder(editOrderOrder);
                    }
                    break;
                case "5":
                    Database.ExpenseOrder receiptOrder = OrderManagement.ChooseOrderResolved(db);
                    if (receiptOrder != null)
                    {
                        receiptOrder = OrderManagement.ResolveOrder(receiptOrder);
                        if (receiptOrder == null)
                        {
                            break;
                        }

                        db.OverwriteOrder(receiptOrder);
                    }
                    break;
                case "6":
                    ReportGen.CreateNotReceiptReport(db, DateTime.Today);
                    break;
                default:
                    Console.WriteLine("Invalid Entry");
                    break;
            }
        }
    }
}
