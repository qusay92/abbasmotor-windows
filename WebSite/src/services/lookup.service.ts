import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Observable } from 'rxjs';
import { appSettings } from '../appSettings/appSettings';
import { ResponseResult } from "../models/models";


@Injectable({
    providedIn: 'root',
  })
  export class LookupService {
    constructor(private http: HttpClient, private router: Router) {}

    GetAllLookupValues(): Observable<ResponseResult<any>> {
        const url = appSettings.webApiUrl + 'api/Lookups/GetAllLookupValues';
        return this.http.get<ResponseResult<any>>(url);
      }

      
  GetLookups(): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl +  'api/Lookups/GetLookups';
    return this.http.get<ResponseResult<any>>(url);
  }

  GetLookupValues(search:any, lookupId:number): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl +  'api/Lookups/GetLookupValues/' + lookupId + '/' + search;
    return this.http.get<ResponseResult<any>>(url);
  }

  Save(model: any): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl +  'api/Lookups/Save';
    return this.http.post<ResponseResult<any>>(url, model);
  }

  Delete(model: any[]): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl +  'api/Lookups/Delete';
    return this.http.post<ResponseResult<any>>(url, model);
  }
  }