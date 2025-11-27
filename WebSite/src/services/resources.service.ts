import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { ResponseResult, LoginResonse, User } from '../models/models';
import { appSettings } from 'src/appSettings/appSettings';

@Injectable({
  providedIn: 'root',
})
export class ResourcesService {
  currentCulture = new Subject<string>();

  constructor(private http: HttpClient) {}

  HandleResources(pageUrl: string): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Resources/HandleResourcesByUrl?url=' + pageUrl;
    return this.http.get<ResponseResult<LoginResonse>>(url);
  }

  GetResources(search: string, resourceId:number): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Resources/GetResources/' + resourceId + '/' + search;
    return this.http.get<ResponseResult<LoginResonse>>(url);
  }

  SaveResource(body: any): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Resources/SaveResource';
    return this.http.post<ResponseResult<LoginResonse>>(url, body);
  }

  GetResourcesKeys(): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Resources/GetResourcesKeys';
    return this.http.get<ResponseResult<LoginResonse>>(url);
  }

  GetHomePageResources():  Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Resources/GetHomePageResources';
    return this.http.get<ResponseResult<LoginResonse>>(url);
  }

}
