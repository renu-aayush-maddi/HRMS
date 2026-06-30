import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toastr = inject(ToastrService);

  // Check if we should bypass this interceptor
  const bypass = req.headers.has('X-Bypass-Error-Interceptor');
  let cleanReq = req;
  if (bypass) {
    cleanReq = req.clone({ headers: req.headers.delete('X-Bypass-Error-Interceptor') });
  }

  return next(cleanReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (!bypass) {
        let message = 'An unexpected error occurred.';
        if (error.error) {
          if (typeof error.error === 'string') {
            message = error.error;
          } else if (error.error.Message) {
            message = error.error.Message;
          } else if (error.error.message) {
            message = error.error.message;
          } else if (error.error.Errors && Array.isArray(error.error.Errors)) {
            message = error.error.Errors.join(', ');
          }
        } else if (error.statusText) {
          message = error.statusText;
        }

        if (error.status === 401) {
          message = 'Session expired. Please log in again.';
        } else if (error.status === 403) {
          message = 'You do not have permission to perform this action.';
        } else if (error.status === 0) {
          message = 'Network error. Please check your internet connection.';
        }

        toastr.error(message, 'Error');
      }
      return throwError(() => error);
    })
  );
};
