using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models;
public class PriceUpdate
{
    public string Exchange {  get; set; }
    public string Symbol { get; set; }
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; }

    public PriceUpdate(string exchange, string symbol, decimal price, DateTime timestamp)
    {
        Exchange = exchange;
        Symbol = symbol;
        Price = price;
        Timestamp = timestamp;
    }
}
