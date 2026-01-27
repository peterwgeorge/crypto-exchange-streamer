using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Desktop.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IPriceEngine _engine;

    public ObservableCollection<PriceRowViewModel> Rows { get; } = new();

    private decimal _average;
    public decimal Average
    {
        get => _average;
        private set { _average = value; OnPropertyChanged(); }
    }

    private decimal _coinbaseWeight = 1;
    public decimal CoinbaseWeight
    {
        get => _coinbaseWeight;
        set
        {
            _coinbaseWeight = value;
            OnPropertyChanged();
            PushWeights();
        }
    }

    private decimal _krakenWeight = 1;
    public decimal KrakenWeight
    {
        get => _krakenWeight;
        set
        {
            _krakenWeight = value;
            OnPropertyChanged();
            PushWeights();
        }
    }

    private decimal _binanceWeight = 1;
    public decimal BinanceWeight
    {
        get => _binanceWeight;
        set
        {
            _binanceWeight = value;
            OnPropertyChanged();
            PushWeights();
        }
    }


    private decimal _globalOffset = 0;
    public decimal GlobalOffset
    {
        get => _globalOffset;
        set
        {
            _globalOffset = value;
            OnPropertyChanged();
            PushOffsets();
        }
    }

    public decimal _coinbaseOffset = 0;
    public decimal CoinbaseOffset
    {
        get => _coinbaseOffset;
        set
        {
            _coinbaseOffset = value;
            OnPropertyChanged();
            PushOffsets();
        }
    }

    private decimal _krakenOffset = 0;
    public decimal KrakenOffset
    {
        get => _krakenOffset;
        set
        {
            _krakenOffset = value;
            OnPropertyChanged();
            PushOffsets();
        }
    }

    private decimal _binanceOffset = 0;
    public decimal BinanceOffset
    {
        get => _binanceOffset;
        set
        {
            _binanceOffset = value;
            OnPropertyChanged();
            PushOffsets();
        }
    }

    private decimal _btccOffset = 0;
    public decimal BtccOffset
    {
        get => _btccOffset;
        set
        {
            _btccOffset = value;
            OnPropertyChanged();
            PushOffsets();
        }
    }

    public MainViewModel(IPriceEngine engine)
    {
        _engine = engine;

        _engine.SnapshotUpdated += snapshot =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Average = snapshot.Average;
                UpdateRows(snapshot.Rows);
            });
        };

        PushWeights();
        PushOffsets();
    }

    private void PushWeights()
    {
        _engine.SetWeights(new Dictionary<string, decimal>
        {
            ["Coinbase"] = CoinbaseWeight,
            ["Kraken"] = KrakenWeight,
            ["Binance"] = BinanceWeight
        });
    }

    private void PushOffsets()
    {
        _engine.SetGlobalOffset(GlobalOffset);

        _engine.SetExchangeOffset("Coinbase", CoinbaseOffset);
        _engine.SetExchangeOffset("Kraken", KrakenOffset);
        _engine.SetExchangeOffset("Binance", BinanceOffset);
        _engine.SetExchangeOffset("BTCC", BtccOffset);
    }


    private void UpdateRows(IEnumerable<PriceRow> rows)
    {
        Rows.Clear();
        foreach (var row in rows)
            Rows.Add(new PriceRowViewModel(row));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
