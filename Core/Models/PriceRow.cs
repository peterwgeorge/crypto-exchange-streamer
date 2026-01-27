using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models;
public class PriceRow
{
    public string Exchange;
    public decimal Price;
    public decimal Delta;
    public decimal Percent;

    public PriceRow(string exchange, decimal price, decimal delta, decimal percent)
    {
        Exchange = exchange;
        Price = price;
        Delta = delta;
        Percent = percent;
    }
}
