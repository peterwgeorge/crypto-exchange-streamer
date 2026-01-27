using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.ViewModels;
public class PriceRowViewModel
{
    public string Exchange { get; }
    public decimal Price { get; }
    public decimal Delta { get; }
    public decimal Percent { get; }

    public PriceRowViewModel(PriceRow row)
    {
        Exchange = row.Exchange;
        Price = row.Price;
        Delta = row.Delta;
        Percent = row.Percent;
    }
}
