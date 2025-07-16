import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginCreds, RegisterCreds, User } from '../../Types/user';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LikesService } from './likes-service';
import { PresenceService } from './presence-service';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);
  private likesService = inject(LikesService);
  private presenceService = inject(PresenceService);
  private baseUrl = environment.apiUrl;
  private interval: number = 0;

  currentUser = signal<User | null>(null);

  register(creds: RegisterCreds) {
    return this.http
      .post<User>(this.baseUrl + 'account/register', creds, {
        withCredentials: true,
      })
      .pipe(
        tap((user) => {
          if (user) {
            this.setCurrentUser(user);
            this.startTokenRefreshInterval();
          }
        })
      );
  }

  login(creds: LoginCreds) {
    return this.http
      .post<User>(this.baseUrl + 'account/login', creds, {
        withCredentials: true,
      })
      .pipe(
        tap((user) => {
          if (user) {
            this.setCurrentUser(user);
            this.startTokenRefreshInterval();
          }
        })
      );
  }

  logout() {
    this.http
      .post(this.baseUrl + 'account/logout', {}, { withCredentials: true })
      .subscribe({
        next: () => {
          localStorage.removeItem('filters');
          this.currentUser.set(null);
          this.likesService.clearLikeIds();
          this.presenceService.stopHubConnection();
          this.stopTokenRefreshInterval();
        },
      });
  }

  setCurrentUser(user: User) {
    user.roles = this.getRolesFromToken(user);
    this.currentUser.set(user);
    this.likesService.getLikeIds();

    if (!this.presenceService.isConnected()) {
      this.presenceService.createHubConnection(user);
    }
  }

  refreshToken() {
    return this.http.post<User>(
      this.baseUrl + 'account/refresh-token',
      {},
      { withCredentials: true }
    );
  }

  startTokenRefreshInterval() {
    this.interval = setInterval(() => {
      this.http
        .post<User>(
          this.baseUrl + 'account/refresh-token',
          {},
          { withCredentials: true }
        )
        .subscribe({
          next: (user) => {
            this.setCurrentUser(user);
          },
          error: () => {
            this.logout();
          },
        });
    }, 14 * 24 * 60 * 60 * 1000); // 14 days
  }

  stopTokenRefreshInterval() {
    clearInterval(this.interval);
  }

  private getRolesFromToken(user: User): string[] {
    const payload = user.token.split('.')[1];
    const decoded = atob(payload);
    const jsonPayload = JSON.parse(decoded);

    return Array.isArray(jsonPayload.role)
      ? jsonPayload.role
      : [jsonPayload.role];
  }
}
