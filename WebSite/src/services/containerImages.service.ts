import { Injectable } from '@angular/core';
import { ResponseResult, Container } from '../models/models';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { appSettings } from 'src/appSettings/appSettings';

@Injectable({
  providedIn: 'root',
})
export class ContainerImagesService {
  constructor(private http: HttpClient, private router: Router) {}

  getImagesByContainer(id: number): Observable<ResponseResult<any[]>> {
    const url = appSettings.webApiUrl + 'api/ContainerImages/GetImagesByContainer/' + id;
    return this.http.get<ResponseResult<any[]>>(url);
  }

  DeleteContainerImage(id: any): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/ContainerImages/DeleteContainerImage/' + id;
    return this.http.get<ResponseResult<any>>(url);
  }

  DeleteAllImages(id: any): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/ContainerImages/DeleteAllImages/' + id;
    return this.http.get<ResponseResult<any>>(url);
  }

  DeleteContainersImages(model: Container[]): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/ContainerImages/DeleteContainersImages';
    return this.http.post<ResponseResult<any>>(url, model);
  }
}
