import { Injectable } from '@angular/core';
import { Container, ResponseResult, SearchContainer } from '../models/models';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { appSettings } from 'src/appSettings/appSettings';

@Injectable({
  providedIn: 'root'
})
export class ContainerService {

  constructor(private http: HttpClient, private router: Router) { }

  getAllByUser(search: SearchContainer): Observable<ResponseResult<any>> {
    let body = {
        isSearch: search.isSearch,
        containerNo: search.containerNo,
        bookingNo: search.bookingNo,
        statusId: search.StatusId,    
        loadingFromDate: search.loadingFromDate?.toString() == '' ? null : search.loadingFromDate,
        loadingToDate: search.loadingToDate,
        loadPortId: search.loadPortId,
        destinationId: search.destinationId,
        shippingLineId: search.shippingLineId,
        clientId: search.clientId,
        arrivalFromDate: search.arrivalFromDate?.toString() == '' ? null : search.arrivalFromDate,
        arrivalToDate: search.arrivalToDate?.toString() == '' ? null : search.arrivalToDate
      };
    const url = appSettings.webApiUrl + 'api/Containers/getAllByUser';
    return this.http.post<ResponseResult<any>>(url, body);
  }

  getAllArchiveByUser(search: SearchContainer): Observable<ResponseResult<any>> {
    let body = {
        isSearch: search.isSearch,
        containerNo: search.containerNo,
        bookingNo: search.bookingNo,
        statusId: search.StatusId,    
        loadingFromDate: search.loadingFromDate?.toString() == '' ? null : search.loadingFromDate,
        loadingToDate: search.loadingToDate,
        loadPortId: search.loadPortId,
        destinationId: search.destinationId,
        shippingLineId: search.shippingLineId,
        clientId: search.clientId,
        arrivalFromDate: search.arrivalFromDate?.toString() == '' ? null : search.arrivalFromDate,
        arrivalToDate: search.arrivalToDate?.toString() == '' ? null : search.arrivalToDate
      };
    const url = appSettings.webApiUrl + 'api/Containers/GetAllArchiveByUser';
    return this.http.post<ResponseResult<any>>(url, body);
  }

  SaveContainer(model: Container): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Containers/SaveContainer';
    return this.http.post<ResponseResult<any>>(url, model);
  }

  GetSideMenuByUser( isArchive:number): Observable<any> {
    const url = appSettings.webApiUrl + 'api/Containers/GetSideMenuByUser/' + isArchive;
    return this.http.get<any>(url);
  }

  DeleteContainers(model: Container[]): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Containers/DeleteContainers';
    return this.http.post<ResponseResult<any>>(url, model);
  }

  DeleteContainer(id: number): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Containers/DeleteContainer/' + id;
    return this.http.post<ResponseResult<any>>(url, id);
  }
  
  ArchiveContainer(id: number): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Containers/ArchiveContainer/' + id;
    return this.http.post<ResponseResult<any>>(url, id);
  }

  ArchiveContainers(model: Container[]): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Containers/ArchiveContainers';
    return this.http.post<ResponseResult<any>>(url, model);
  }

  GetContainerById(id:number): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Containers/GetContainerById/' + id;
    return this.http.get<ResponseResult<any>>(url);
  }

  DeleteImages(id: number): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Containers/DeleteImages/' + id;
    return this.http.post<ResponseResult<any>>(url, id);
  }
  
  UnArchiveContainers(model: Container[]): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Containers/UnArchiveContainers';
    return this.http.post<ResponseResult<any>>(url, model);
  }

}
