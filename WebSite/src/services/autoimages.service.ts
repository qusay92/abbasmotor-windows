import { Injectable } from '@angular/core';
import { Auto, ResponseResult } from '../models/models';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { appSettings } from '../appSettings/appSettings';

@Injectable({
  providedIn: 'root',
})
export class AutoImagesService {
  constructor(private http: HttpClient, private router: Router) {}

  getImagesByAuto(id: number): Observable<ResponseResult<any[]>> {
    const url = appSettings.webApiUrl + 'api/AutoImages/GetImagesByAuto/' + id;
    return this.http.get<ResponseResult<any[]>>(url);
  }

  DeleteAutoImage(id: any): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/AutoImages/DeleteAutoImage/' + id;
    return this.http.get<ResponseResult<any>>(url);
  }

  DeleteAllImages(id: any): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/AutoImages/DeleteAllImages/' + id;
    return this.http.get<ResponseResult<any>>(url);
  }

  DeleteAutosImages(model: Auto[]): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/AutoImages/DeleteAutosImages';
    return this.http.post<ResponseResult<any>>(url, model);
  }
}
