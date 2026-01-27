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
            var rows = BuildRows(avg);

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

    private IReadOnlyList<PriceRow> BuildRows(decimal avg)
    {
        var rows = new List<PriceRow>();

        foreach (var (exchange, price) in _latestPrices)
        {
            var offset = _exchangeOffsets.TryGetValue(exchange, out var o) ? o : 0;
            var delta = (price - avg) + _globalOffset + offset;
            var percent = avg == 0 ? 0 : delta / avg * 100;

            rows.Add(new PriceRow(exchange, price, delta, percent));
        }

        return rows;
    }
}
