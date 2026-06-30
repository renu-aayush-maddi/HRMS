import { Injectable, signal, effect, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { AuthStore } from '../../stores/auth/auth.store';
import { Observable } from 'rxjs';

export interface NotificationDto {
  id: string;
  title: string;
  message: string;
  isRead: boolean;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private http = inject(HttpClient);
  private authStore = inject(AuthStore);
  
  private hubConnection: signalR.HubConnection | null = null;
  readonly notifications = signal<NotificationDto[]>([]);
  readonly unreadCount = signal<number>(0);

  constructor() {
    // Monitor auth state to connect/disconnect SignalR dynamically
    effect(() => {
      const user = this.authStore.currentUser();
      const token = this.authStore.token();
      
      if (user && token) {
        this.loadNotifications(user.userId);
        this.startSignalRConnection(user.userId, token);
      } else {
        this.stopSignalRConnection();
        this.notifications.set([]);
        this.unreadCount.set(0);
      }
    });
  }

  private loadNotifications(userId: string) {
    this.getNotifications(userId).subscribe({
      next: (list) => {
        this.notifications.set(list);
        this.unreadCount.set(list.filter(n => !n.isRead).length);
      },
      error: (err) => console.error('Failed to load notifications', err)
    });
  }

  getNotifications(userId: string): Observable<NotificationDto[]> {
    return this.http.get<NotificationDto[]>(`${environment.apiUrl}/notifications/${userId}`);
  }

  markAsRead(notificationId: string): Observable<any> {
    return this.http.put(`${environment.apiUrl}/notifications/${notificationId}/read`, {});
  }

  markAsReadLocal(id: string) {
    this.markAsRead(id).subscribe({
      next: () => {
        this.notifications.update(list => 
          list.map(n => n.id === id ? { ...n, isRead: true } : n)
        );
        this.unreadCount.update(c => Math.max(0, c - 1));
      },
      error: (err) => console.error('Failed to mark notification as read', err)
    });
  }

  private startSignalRConnection(userId: string, token: string) {
    this.stopSignalRConnection();

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.signalRUrl}/notificationHub?userId=${userId}`, {
        accessTokenFactory: () => token,
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => console.log('SignalR NotificationHub connected.'))
      .catch(err => console.error('Error starting SignalR connection:', err));

    this.hubConnection.on('ReceiveNotification', (notification: NotificationDto) => {
      this.notifications.update(list => [notification, ...list]);
      this.unreadCount.update(c => c + 1);
    });
  }

  private stopSignalRConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop()
        .then(() => console.log('SignalR NotificationHub stopped.'))
        .catch(err => console.error('Error stopping SignalR connection:', err));
      this.hubConnection = null;
    }
  }
}
