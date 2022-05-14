using System;
using System.Collections.Generic;
using System.Text;

namespace BitCoinManagerModels
{
    public class Order
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public OrderOperation Operation { get; set; }
    }

    public enum OrderOperation
    {
        Buy,
        Sell
    }
}
