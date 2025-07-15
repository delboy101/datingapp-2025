import { Component, inject, OnInit, signal } from '@angular/core';
import { AdminService } from '../../../core/services/admin-service';
import { Photo } from '../../../Types/member';
import { ToastService } from '../../../core/services/toast-service';

@Component({
  selector: 'app-photo-management',
  imports: [],
  templateUrl: './photo-management.html',
  styleUrl: './photo-management.css',
})
export class PhotoManagement implements OnInit {
  private adminService = inject(AdminService);
  private toast = inject(ToastService);

  protected photos = signal<Photo[]>([]);

  ngOnInit(): void {
    this.getPhotosForApproval();
  }

  getPhotosForApproval() {
    this.adminService.getPhotosForApproval().subscribe({
      next: (photos) => this.photos.set(photos),
    });
  }

  approvePhoto(id: number) {
    this.adminService.approvePhoto(id).subscribe({
      next: () => {
        this.photos.update((prev) => prev.filter((x) => x.id !== id));
        this.toast.success('Photo approved');
      },
    });
  }

  rejectPhoto(id: number) {
    this.adminService.rejectPhoto(id).subscribe({
      next: () => {
        this.photos.update((prev) => prev.filter((x) => x.id !== id));
        this.toast.success('Photo rejected');
      },
    });
  }
}
