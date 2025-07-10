import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/account-service';
import { ToastService } from '../services/toast-service';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toast = inject(ToastService);

  var roles = accountService.currentUser()?.roles;

  if (!roles) return false;

  if (roles.includes('Admin') || roles.includes('Moderator')) {
    return true;
  } else {
    toast.error('Enter this area, you cannot');
    return false;
  }
};
