using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Interfaces;
public interface IPriceEngine
{
    void Publish(PriceUpdate update);

    event Action<PriceSnapshot>? SnapshotUpdated;

    void SetWeights(Dictionary<string, decimal> weights);
    void SetGlobalOffset(decimal offset);
    void SetExchangeOffset(string exchange, decimal offset);
}
