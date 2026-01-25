import { PricePoint } from "../types/price-point";

export class WebSocketService {
  private socket: WebSocket | null = null;
  private listeners: Map<string, {
    onMessage?: (data: any) => void;
    onOpen?: () => void;
    onClose?: (event: CloseEvent) => void;
  }> = new Map();

  private url: string;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 10;
  private reconnectTimeout: ReturnType<typeof setTimeout> | null = null;

  constructor(url: string) {
    this.url = url;
  }

  public connect(): void {
    if (this.isConnected()) return;

    this.socket = new WebSocket(this.url);

    this.socket.onopen = () => {
      console.log('WebSocket connected');
      this.reconnectAttempts = 0; // Reset
      this.listeners.forEach(listener => listener.onOpen?.());
    };

    this.socket.onmessage = (event) => {
      try {

        if (event.data === "Connected to CryptoExchange WebSocket relay")
          return;

        const data = JSON.parse(event.data) as PricePoint;
        this.listeners.forEach(listener => listener.onMessage?.(data));
      } catch (error) {
        console.error('Failed to parse WebSocket message:', error);
      }
    };

    this.socket.onclose = (event) => {
      console.log(`WebSocket disconnected. Code: ${event.code}, Reason: ${event.reason}`);
      this.listeners.forEach(listener => listener.onClose?.(event));
      if (event.code !== 1000) {
        this.tryReconnect();
      }
    };


    this.socket.onerror = (error) => {
      console.error('WebSocket error:', error);
      this.socket?.close();
    };
  }

  public isConnected(): boolean {
    return this.socket?.readyState === WebSocket.OPEN;
  }

  public disconnect(): void {
    if (this.reconnectTimeout) {
      clearTimeout(this.reconnectTimeout);
      this.reconnectTimeout = null;
    }
    this.reconnectAttempts = 0;

    if (this.socket) {
      this.socket.close(1000, "Manual disconnect");
      this.socket = null;
    }
  }


  public addListener(
    id: string,
    listener: {
      onMessage?: (data: any) => void;
      onOpen?: () => void;
      onClose?: (event: CloseEvent) => void;
    }
  ): void {
    this.listeners.set(id, listener);
  }

  public removeListener(id: string): void {
    this.listeners.delete(id);
  }

  private tryReconnect(): void {
    if (this.reconnectAttempts >= this.maxReconnectAttempts) {
      console.warn("Max reconnect attempts reached.");
      return;
    }

    const delay = Math.pow(2, this.reconnectAttempts) * 1000; // Exponential backoff
    console.log(`Attempting to reconnect in ${delay / 1000}s...`);

    this.reconnectTimeout = setTimeout(() => {
      this.reconnectAttempts++;
      this.socket = null;
      this.connect();
    }, delay);
  }

}

// Create an instance with your local WebSocket URL
export const webSocketService = new WebSocketService(import.meta.env.VITE_RELAY_SERVER_URL);