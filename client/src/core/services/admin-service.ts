import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../../Types/user';
import { Photo } from '../../Types/member';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getUsersWithRoles() {
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(userId: string, roles: string[]) {
    return this.http.post<string[]>(
      this.baseUrl + 'admin/edit-roles/' + userId + '?roles=' + roles,
      {}
    );
  }

  getPhotosForApproval() {
    return this.http.get<Photo[]>(this.baseUrl + 'admin/photos-to-moderate');
  }

  approvePhoto(id: number) {
    return this.http.post(this.baseUrl + 'admin/approve-photo/' + id, {});
  }

  rejectPhoto(id: number) {
    return this.http.post(this.baseUrl + 'admin/reject-photo/' + id, {});
  }
}
