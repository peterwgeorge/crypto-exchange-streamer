// src/components/PriceChart.tsx
import React, { useEffect, useState } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { webSocketService } from '../services/webSocketService';
import { createAlignedPriceSeries } from '../utils/dataAlignment';
import { retainRecentPrices } from '../utils/history';
import { PricePoint } from '../types/price-point';
import ConnectionStatus from './ConnectionStatus';
import PriceChartLegend from './PriceChartLegend';

export const PriceChart: React.FC = () => {
  const [isConnected, setIsConnected] = useState(false);
  const [exchangeData, setExchangeData] = useState<Map<string, PricePoint[]>>(new Map());

  const combinedData = React.useMemo(() => {
    return createAlignedPriceSeries(exchangeData);
  }, [exchangeData]);
  
  useEffect(() => {
    webSocketService.addListener("priceChart", {onMessage, onOpen, onClose});
    webSocketService.connect();
    
    return () => {
      webSocketService.removeListener("priceChart");
    };
  }, []);

  return (
    <div className="price-chart">
      <div>
        <ConnectionStatus isConnected={isConnected} />
      </div>

      <div style={{ display: 'flex', height: 400 }}>
        <div style={{ flex: 1 }}>
          <ResponsiveContainer width="100%" height="100%">
            <LineChart data={combinedData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis
                dataKey="timestamp"
                domain={['auto', 'auto']}
                tickFormatter={(unixTime) => new Date(unixTime).toLocaleTimeString()}
              />
              <YAxis domain={['auto', 'auto']} />
              <Tooltip
                labelFormatter={(label) => new Date(label).toLocaleString()}
                formatter={(value) => [`$${value}`, 'Price']}
              />
              <Line
                type="monotone"
                dataKey="coinbase"
                stroke="#0052FF"
                dot={true}
                isAnimationActive={false}
                name="Coinbase"
              />
              <Line
                type="monotone"
                dataKey="binance"
                stroke="#f7931a"
                dot={true}
                isAnimationActive={false}
                name="Binance"
              />
              <Line
                type="monotone"
                dataKey="kraken"
                stroke="#5841D8"
                dot={true}
                isAnimationActive={false}
                name="Kraken"
              />
              <Line
                type="monotone"
                dataKey="btcc"
                stroke="#008000"
                dot={true}
                isAnimationActive={false}
                name="BTCC"
              />
            </LineChart>
          </ResponsiveContainer>
        </div>
        <PriceChartLegend />
      </div>
    </div>

  );

  function onOpen(){
    setIsConnected(true);
  }

  function onClose(){
    setIsConnected(false);
  }

  function onMessage(data: PricePoint): void {
    if (!data || isNaN(data.price)) return;
  
    setExchangeData(prev => {
      const updated = new Map(prev);
      const history = updated.get(data.exchange) ?? [];
      const newHistory = retainRecentPrices(history, data, 30_000);
      updated.set(data.exchange, newHistory);
      return updated;
    });
  }
}


