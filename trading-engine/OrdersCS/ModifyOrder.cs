namespace TradingEngineServer.Orders
{
    public class ModifyOrder : IOrderCore
    {
        public ModifyOrder(IOrderCore orderCore, long price, uint quantity, bool isBuySide)
        {
            // properties
            Price = price;
            Quantity = quantity;
            IsBuySide = isBuySide;
            
            // fields
            _orderCore = orderCore;
        }
        
        // properties
        public long Price { get; private set; }
        public uint Quantity { get; private set; }
        public bool IsBuySide { get; private set; }
        public long OrderId => _orderCore.OrderId;
        public string Username => _orderCore.Username;
        public int SecurityId => _orderCore.SecurityId;
        
        // methods
        public CancelOrder ToCancelOrder()
        {
            return new CancelOrder(this);
        }

        public Order ToNewOrder()
        {
            return new Order(this);
        }
        
        // fields
        private readonly IOrderCore _orderCore;
    }
}