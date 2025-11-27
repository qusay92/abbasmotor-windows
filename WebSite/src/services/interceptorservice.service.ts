import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class InterceptorService implements HttpInterceptor {
  constructor(
    private router: Router,
    private spinnerService: NgxSpinnerService
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    let lang = localStorage.getItem('currantLang') == null ? 'ar' : localStorage.getItem('currantLang');
    request = request.clone({
      setHeaders: {
       // 'Content-Type': 'application/json; charset=utf-8',
        Accept: 'application/json',
        Authorization: `Bearer ${localStorage.getItem('token')}`,
        'Accept-Language': `${lang }`,
      },
    });
    
    return next
      .handle(request)
      .pipe(catchError((x) => this.handleAuthError(x)));
  }

  private handleAuthError(err: HttpErrorResponse): Observable<any> {
    debugger;
    //handle your auth error or rethrow
    if (err.status === 401 || err.status === 403) {
      //this.spinnerService.hide();
      setTimeout(() => {
        this.spinnerService.hide();
      }, 3000); // 5 seconds

      this.router.navigateByUrl(`/login`);
      return of(err.message);
    }
    return throwError(err);
  }
}
