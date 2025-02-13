using TradingEngineServer.Orders;

namespace TradingEngineServer.Rejects
{
    public class Rejection : IOrderCore
    {
        public Rejection(IOrderCore rejectedOrder, RejectionReason reason)
        {
            // properties
            RejectionReason = reason;
            
            // fields
            _orderCore = rejectedOrder;
        }
        
        // properties
        public RejectionReason RejectionReason { get; private set; }
        public long OrderId => _orderCore.OrderId;
        public string Username => _orderCore.Username;
        public int SecurityId => _orderCore.SecurityId;
        
        // fields
        private readonly IOrderCore _orderCore;
    }
}