import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { environments } from '../environments/environments';

@Injectable({
  providedIn: 'root'
})
export class WebsocketService {
  private socket!: WebSocket;
  private messageSubject = new Subject<any>();
  private webSocketUrl = environments.WebSocketUrl;

  constructor() { 
    this.connect();
  }

  private connect() {
    console.log('WebSocket connecting...');

    this.socket = new WebSocket(this.webSocketUrl);

    this.socket.onopen = () => {
      console.log('WebSocket connected');
    };

    this.socket.onmessage = (event) => {
      console.log('WebSocket message received:', event.data);
      this.messageSubject.next(JSON.parse(event.data));
    };

    this.socket.onclose = (event) => {
      console.log('WebSocket closed:', event);
      setTimeout(() => this.connect(), 1000); // Reconnect on close
    };

    this.socket.onerror = (error) => {
      console.log('WebSocket error:', error);
      this.socket.close(); // Close on error
    };
  }

  get messages() {
    return this.messageSubject.asObservable();
  }

  sendMessage(message: any) {
    console.log('WebSocket send message:', message);
    if (this.socket.readyState === WebSocket.OPEN) {
      this.socket.send(JSON.stringify(message));
    } else {
      console.error('WebSocket is not open. Ready state:', this.socket.readyState);
    }
  }
}