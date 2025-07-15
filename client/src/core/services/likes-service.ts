import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { LikesParams, Member } from '../../Types/member';
import { PaginatedResult } from '../../Types/pagination';

@Injectable({
  providedIn: 'root',
})
export class LikesService {
  private baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  private likeIds = signal<string[]>([]);

  hasLiked(targetMemberId: string) {
    return this.likeIds().includes(targetMemberId);
  }

  toggleLike(targetMemberId: string) {
    return this.http
      .post(this.baseUrl + 'likes/' + targetMemberId, {})
      .subscribe({
        next: () => {
          if (this.hasLiked(targetMemberId)) {
            this.likeIds.update((ids) =>
              ids.filter((x) => x !== targetMemberId)
            );
          } else {
            this.likeIds.update((ids) => [...ids, targetMemberId]);
          }
        },
      });
  }

  getLikes(likesParams: LikesParams) {
    let params = new HttpParams();

    params = params.append('pageNumber', likesParams.pageNumber);
    params = params.append('pageSize', likesParams.pageSize);
    params = params.append('predicate', likesParams.predicate);

    return this.http.get<PaginatedResult<Member>>(this.baseUrl + 'likes', {
      params,
    });
  }

  getLikeIds() {
    return this.http.get<string[]>(this.baseUrl + 'likes/list').subscribe({
      next: (ids) => this.likeIds.set(ids),
    });
  }

  clearLikeIds() {
    this.likeIds.set([]);
  }
}
