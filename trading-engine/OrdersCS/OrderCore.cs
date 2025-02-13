namespace TradingEngineServer.Orders
{
    public class OrderCore : IOrderCore
    {
        public OrderCore(long orderId, string username, int securityId)
        {
            OrderId = orderId;
            Username = username;
            SecurityId = securityId;
        }
        
        // private set it not needed but there for clarity
        public long OrderId { get; private set; }
        public string Username { get; private set; }
        public int SecurityId { get; private set; }
    }
}