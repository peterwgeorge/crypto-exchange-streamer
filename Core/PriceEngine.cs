using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core;
public class PriceEngine : IPriceEngine
{
    private readonly object _lock = new();

    private readonly Dictionary<string, decimal> _latestPrices = new();
    private Dictionary<string, decimal> _weights = new();
    private readonly Dictionary<string, decimal> _exchangeOffsets = new();

    private decimal _globalOffset;

    public event Action<PriceSnapshot>? SnapshotUpdated;

    public void Publish(PriceUpdate update)
    {
        lock (_lock)
        {
            _latestPrices[update.Exchange] = update.Price;

            var avg = ComputeWeightedAverage();
            var rows = BuildRows();

            SnapshotUpdated?.Invoke(new PriceSnapshot(rows, avg));
        }
    }

    public void SetWeights(Dictionary<string, decimal> weights)
    {
        lock (_lock)
            _weights = new Dictionary<string, decimal>(weights);
    }

    public void SetGlobalOffset(decimal offset)
    {
        lock (_lock)
            _globalOffset = offset;
    }

    public void SetExchangeOffset(string exchange, decimal offset)
    {
        lock (_lock)
            _exchangeOffsets[exchange] = offset;
    }

    private decimal ComputeWeightedAverage()
    {
        decimal weightedSum = 0;
        decimal weightTotal = 0;

        foreach (var (exchange, price) in _latestPrices)
        {
            if (!_weights.TryGetValue(exchange, out var weight))
                continue;

            weightedSum += price * weight;
            weightTotal += weight;
        }

        return weightTotal == 0 ? 0 : weightedSum / weightTotal;
    }

    private IReadOnlyList<PriceRow> BuildRows()
    {
        var rows = new List<PriceRow>();

        if (!_latestPrices.TryGetValue("btcc", out var btccPrice))
            return rows; // nothing to compare yet

        if (!_exchangeOffsets.TryGetValue("btcc", out var btccOffset))
            return rows;

        foreach (var (exchange, price) in _latestPrices)
        {
            var exchangeOffset = _exchangeOffsets.TryGetValue(exchange, out var o) ? o : 0;

            decimal delta;
            decimal percent;

            if (exchange == "btcc")
            {
                delta = 0;
                percent = 0;
            }
            else
            {
                delta = (price + exchangeOffset) - (btccPrice + btccOffset);
                percent = btccPrice == 0 ? 0 : delta / (btccPrice + btccOffset + _globalOffset) * 100;
            }

            rows.Add(new PriceRow(
                exchange,
                price + _globalOffset + exchangeOffset,
                delta,
                percent
            ));
        }

        return rows;
    }

}
