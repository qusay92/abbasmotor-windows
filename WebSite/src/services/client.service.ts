import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ResponseResult, SearchBalance, User } from '../models/models';
import { appSettings } from '../appSettings/appSettings';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ClientService {
  constructor(private http: HttpClient, private router: Router) {}

  GetClients(search: string): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Clients/GetClients/' + search;
    return this.http.get<ResponseResult<any>>(url);
  }

  SaveClient(
    model: User,
    editPassword: boolean
  ): Observable<ResponseResult<any>> {
    const url =
      appSettings.webApiUrl + 'api/Clients/SaveClient/' + editPassword;
    return this.http.post<ResponseResult<any>>(url, model);
  }

  DeleteClient(client: User[]): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Clients/DeleteClient';
    return this.http.post<ResponseResult<any>>(url, client);
  }

  GetClientsInfo(search: SearchBalance): Observable<ResponseResult<any>> {
    let body = {
      isSearch: search.isSearch,
      autoId: search.autoId,
      vinNo: search.vinNo,
      purchaseDate: search.purchaseDate?.toString() == '' ? null : search.purchaseDate,
      clientId: search.clientId,
    };
    const url = appSettings.webApiUrl + 'api/Clients/GetClientsInfo';
    return this.http.post<ResponseResult<any>>(url, body);
  }

  GetClientsBalance(search: string): Observable<ResponseResult<any>> {
    const url =
      appSettings.webApiUrl + 'api/Clients/GetClientsBalance/' + search;
    return this.http.get<ResponseResult<any>>(url);
  }

  GetClientById() {
    const url = appSettings.webApiUrl + 'api/Clients/GetClientById';
    return this.http.get<ResponseResult<any>>(url);
  }

  UpdatePassword(password: string) {
    let body = {
      password: password
    };

    const url = appSettings.webApiUrl + 'api/Clients/UpdatePassword';
    return this.http.post<ResponseResult<any>>(url,body);
  }
}
