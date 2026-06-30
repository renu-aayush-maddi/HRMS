import {
  HttpInterceptorFn
} from '@angular/common/http';

import { inject } from '@angular/core';

import { AuthStore } from '../../stores/auth/auth.store';

export const authInterceptor: HttpInterceptorFn = (
  req,
  next
) => {

  const authStore = inject(AuthStore);

  const token = authStore.token();

  if (!token) {
    return next(req);
  }

  const clonedRequest = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });

  return next(clonedRequest);
};