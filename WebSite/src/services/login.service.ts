import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { LoginResonse, ResponseResult } from 'src/models/models';
import { appSettings } from '../appSettings/appSettings';
//import * as jwt_decode from 'jwt-decode';
import jwt_decode from 'jwt-decode';
@Injectable({
  providedIn: 'root',
})
export class LoginService {
  constructor(private http: HttpClient, private router: Router) {}

  Login(body: any): Observable<ResponseResult<LoginResonse>> {
    const url = appSettings.webApiUrl + 'api/login/login';
    return this.http.post<ResponseResult<LoginResonse>>(url, body);
  }

  setStorage(item: any) {
    window.localStorage.setItem('currentUser', JSON.stringify(item.data));

    window.localStorage.setItem('userId', item.data.id.toString());
    window.localStorage.setItem('username', item.data.name);
    window.localStorage.setItem('usertype', item.data.type.toString());
    window.localStorage.setItem('token', item.data.token);
  }

  getToken(): string | null {
    return window.localStorage.getItem('token');
  }

  isAdmin(): boolean {
    let token = localStorage.getItem('token');
    if (token != undefined && token != null && token != '') {
      let decodedToken:any = jwt_decode(token);
      if (decodedToken.iss > '2') {
        return false;
      } else {
        return true;
      }
    }
    return false;
  }

  userType() : number {
    let token = localStorage.getItem('token');
    if (token != undefined && token != null && token != '') {
      let decodedToken:any = jwt_decode(token);
      return decodedToken.iss;
    }
    return 0;
  }

  logout() {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('userId');
    localStorage.removeItem('username');
    localStorage.removeItem('usertype');
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }

}
