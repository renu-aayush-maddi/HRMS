import { Injectable, signal, computed } from '@angular/core';
import { CurrentUser } from '../../core/models/current-user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthStore {

  readonly token = signal<string | null>(null);

  readonly currentUser =
    signal<CurrentUser | null>(null);

  readonly isAuthenticated =
    computed(() => !!this.token());

  setToken(token: string) {
    this.token.set(token);
  }

  setCurrentUser(user: CurrentUser) {
    this.currentUser.set(user);
  }

  clear() {
    this.token.set(null);
    this.currentUser.set(null);
  }
}