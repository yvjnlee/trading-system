using System.Collections.Generic;

namespace TradingEngineServer.Orders
{
    public class Limit
    {
        public Limit(long price)
        {
            Price = price;
        }
        
        public long Price { get; set; }
        public OrderbookEntry Head { get; set; }
        public OrderbookEntry Tail { get; set; }

        public uint GetLevelOrderCount()
        {
            uint orderCount = 0;
            OrderbookEntry head = Head;

            while (head != null)
            {
                if (head.CurrentOrder.CurrentQuantity != 0)
                    orderCount++;
                head = head.Next;
            }
            
            return orderCount;
        }

        public uint GetLevelOrderCountQuantity()
        {
            uint orderQuantity = 0;
            OrderbookEntry head = Head;

            while (head != null)
            {
                orderQuantity += head.CurrentOrder.CurrentQuantity;
                head = head.Next;
            }
            
            return orderQuantity;
        }
        
        public bool IsEmpty
        {
            get
            {
                return Head == null && Tail == null;
            }
        }
        
        public Side Side
        {
            get
            {
                if (IsEmpty)
                    return Side.Unknown;
                else
                    return Head.CurrentOrder.IsBuySide ? Side.Bid : Side.Ask;
            }
        }

        public List<OrderRecord> GetLevelOrderRecords()
        {
            List<OrderRecord> orderRecords = new List<OrderRecord>();
            OrderbookEntry head = Head;
            uint theoreticalQueuePosition = 0;

            while (head != null)
            {
                var currentOrder = head.CurrentOrder;
                
                if (currentOrder.CurrentQuantity != 0)
                    orderRecords.Add(new OrderRecord(currentOrder.OrderId, currentOrder.CurrentQuantity,
                        Price, currentOrder.IsBuySide, currentOrder.Username, currentOrder.SecurityId,
                        theoreticalQueuePosition));
                
                theoreticalQueuePosition++;
                head = head.Next;
            }
            
            return orderRecords;
        }
    }
}