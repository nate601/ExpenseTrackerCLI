namespace expenseTrackerCli.Database
{
    public class OnHandItems
    {
	public OrderableItem orderedFrom;
	public int Count;
	public string UUID;

        public OnHandItems()
        {
        }
        public OnHandItems(OrderableItem orderedFrom, int count, string uUID)
        {
            this.orderedFrom = orderedFrom;
            Count = count;
            UUID = uUID;
        }
    }
}
