using System;

namespace TradingEngineServer.Orders
{
    public class OrderbookEntry
    {
        public OrderbookEntry(Order currentOrder, Limit parentLimit)
        {
            CreationTime = DateTime.Now;
            ParentLimit = parentLimit;
            CurrentOrder = currentOrder;
        }
        
        // properties
        public OrderbookEntry Next { get; set; }
        public OrderbookEntry Previous { get; set; }
        
        // fields
        public DateTime CreationTime { get; private set; }
        public Order CurrentOrder { get; private set; }
        public Limit ParentLimit { get; private set; }
    }
}