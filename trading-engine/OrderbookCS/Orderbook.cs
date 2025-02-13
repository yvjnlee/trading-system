using System.Collections.Generic;
using System.Linq;
using TradingEngineServer.Orders;
using TradingEngineServer.Instrument;

namespace TradingEngineServer.Orderbook
{
    public class Orderbook : IRetrievalOrderbook
    {
        // private fields //
        private readonly Security _instrument;
        private readonly Dictionary<long, OrderbookEntry> _orders = new Dictionary<long, OrderbookEntry>();
        private readonly SortedSet<Limit> _askLimits = new SortedSet<Limit>(AskLimitComparer.Comparer);
        private readonly SortedSet<Limit> _bidLimits = new SortedSet<Limit>(BidLimitComparer.Comparer);
        
        public Orderbook(Security instrument)
        {
            _instrument = instrument;
        }

        public int Count => _orders.Count;
        
        public void AddOrder(Order order)
        {
            var baseLimit = new Limit(order.Price);
            AddOrder(order, baseLimit, order.IsBuySide ? _bidLimits : _askLimits, _orders);
        }

        private static void AddOrder(Order order, Limit baseLimit, SortedSet<Limit> limitLevels, Dictionary<long, OrderbookEntry> internalBook)
        {
            limitLevels.Add(baseLimit);
            OrderbookEntry orderbookEntry = new OrderbookEntry(order, baseLimit);
            
            if (limitLevels.TryGetValue(baseLimit, out Limit limit))
            {
                if (limit.Head == null)
                {
                    limit.Head = orderbookEntry;
                    limit.Tail = orderbookEntry;
                }
                else
                {
                    OrderbookEntry tail = limit.Tail;
                    tail.Next = orderbookEntry;
                    orderbookEntry.Previous = tail;
                    limit.Tail = tail;
                }
            }
            else
            {
                baseLimit.Head = orderbookEntry;
                baseLimit.Tail = orderbookEntry;
            }
            
            internalBook.Add(order.OrderId, orderbookEntry);
        }

        public void ChangeOrder(ModifyOrder modifyOrder)
        {
            if (_orders.TryGetValue(modifyOrder.OrderId, out OrderbookEntry orderbookEntry))
            {
                RemoveOrder(modifyOrder.ToCancelOrder());
                AddOrder(modifyOrder.ToNewOrder(), orderbookEntry.ParentLimit, modifyOrder.IsBuySide ? _bidLimits : _askLimits, _orders);
            }
        }

        public void RemoveOrder(CancelOrder cancelOrder)
        {
            if (_orders.TryGetValue(cancelOrder.OrderId, out OrderbookEntry orderbookEntry))
            {
                RemoveOrder(cancelOrder.OrderId, orderbookEntry, _orders);
            }
        }

        private static void RemoveOrder(long orderId, OrderbookEntry orderbookEntry, Dictionary<long, OrderbookEntry> internalBook)
        {
            // dealing with orderbookentry location in linked list
            if (orderbookEntry.Previous != null && orderbookEntry.Next != null)
            {
                orderbookEntry.Next.Previous = orderbookEntry.Previous;
                orderbookEntry.Previous.Next = orderbookEntry.Next;
            }
            else if (orderbookEntry.Previous != null)
            {
                orderbookEntry.Previous.Next = null;
            }
            else
            {
                orderbookEntry.Next.Previous = null;
            }
            
            // dealing with orderbookentry on limit level
            if (orderbookEntry.ParentLimit.Head == orderbookEntry && orderbookEntry.ParentLimit.Tail == orderbookEntry)
            {
                // one order on this level
                orderbookEntry.ParentLimit.Head = null;
                orderbookEntry.ParentLimit.Tail = null;
            } 
            else if (orderbookEntry.ParentLimit.Head == orderbookEntry)
            {
                // more than one order, but orderbookentry is first on level
                orderbookEntry.ParentLimit.Head = orderbookEntry.Next;
            }
            else
            {
                // more than one order, but orderbookentry is last on level
                orderbookEntry.ParentLimit.Tail = orderbookEntry.Previous;
            }

            internalBook.Remove(orderId);
        }

        public bool ContainsOrder(long orderId)
        {
            return _orders.ContainsKey(orderId);
        }

        public List<OrderbookEntry> GetAskOrders()
        {
            List<OrderbookEntry> orderOrderbookEntries = new List<OrderbookEntry>();

            foreach (var askLimit in _askLimits)
            {
                if (askLimit.IsEmpty)
                    continue;
                else
                {
                    OrderbookEntry askLimitPointer = askLimit.Head;
                    while (askLimitPointer != null)
                    {
                        orderOrderbookEntries.Add(askLimitPointer);
                        askLimitPointer = askLimitPointer.Next;
                    }
                }
            }
            
            return orderOrderbookEntries;
        }

        public List<OrderbookEntry> GetBidOrders()
        {
            List<OrderbookEntry> orderOrderbookEntries = new List<OrderbookEntry>();

            foreach (var bidLimit in _bidLimits)
            {
                if (bidLimit.IsEmpty)
                    continue;
                else
                {
                    OrderbookEntry bidLimitPointer = bidLimit.Head;
                    while (bidLimitPointer != null)
                    {
                        orderOrderbookEntries.Add(bidLimitPointer);
                        bidLimitPointer = bidLimitPointer.Next;
                    }
                }
            }
            
            return orderOrderbookEntries;
        }

        public OrderbookSpread GetSpread()
        {
            long? bestAsk = null;
            long? bestBid = null;

            if (_askLimits.Any() && !_askLimits.Min.IsEmpty)
            {
                bestAsk = _askLimits.Min.Price;
            }

            if (_bidLimits.Any() && !_bidLimits.Max.IsEmpty)
            {
                bestBid = _bidLimits.Max.Price;
            }
            
            return new OrderbookSpread(bestAsk, bestBid);
        }
    }
}