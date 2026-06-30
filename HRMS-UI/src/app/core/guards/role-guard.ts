import { inject } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivateFn,
  Router
} from '@angular/router';

import { AuthStore } from '../../stores/auth/auth.store';

function getDashboard(role: string): string {

  switch (role) {

    case 'Admin':
      return '/admin/dashboard';

    case 'HR':
      return '/hr/dashboard';

    case 'Manager':
      return '/manager/dashboard';

    case 'Employee':
      return '/employee/dashboard';

    default:
      return '/login';
  }
}

export const roleGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot
) => {

  const authStore = inject(AuthStore);
  const router = inject(Router);

  const currentUser = authStore.currentUser();

  if (!currentUser) {
    router.navigate(['/login']);
    return false;
  }

  const allowedRoles = route.data['roles'] as string[];

  if (allowedRoles.includes(currentUser.role)) {
    return true;
  }

  router.navigate([
    getDashboard(currentUser.role)
  ]);

  return false;
};