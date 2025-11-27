import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { appSettings } from 'src/appSettings/appSettings';
import { Auto, ResponseResult, SearchAuto } from 'src/models/models';

@Injectable({
  providedIn: 'root',
})
export class AutoService {
  constructor(private http: HttpClient, private router: Router) {}

  GetClients(): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Users/GetClients';
    return this.http.get<ResponseResult<any>>(url);
  }

  SaveAuto(model: Auto): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Autos/SaveAuto';
    return this.http.post<ResponseResult<any>>(url, model);
  }

  GetAllByUser(search: SearchAuto): Observable<ResponseResult<any>> {
    let body = {
      isSearch: search.isSearch,
      vinNumber: search.vinNumber,
      lotNumber: search.lotNumber,
      client: search.client,
      auction: search.auction,
      buyAccount: search.buyAccount,
      container: search.container,
      loadPort: search.loadPort,
      destination: search.destination,
      city: search.city,
      carId: search.carId,
      deliveryFromDate: search.deliveryFromDate?.toString() == '' ? null : search.deliveryFromDate,
      deliveryToDate: search.deliveryToDate?.toString() == '' ? null : search.deliveryToDate,
      Status: search.Status,
      purchaseFromDate: search.purchaseFromDate?.toString() == '' ? null : search.purchaseFromDate,
      purchaseToDate: search.purchaseToDate?.toString() == '' ? null : search.purchaseToDate,
    };

    

    const url = appSettings.webApiUrl + 'api/Autos/GetAllByUser';
    return this.http.post<ResponseResult<any>>(url, body);
  }

  GetArchiveAllByUser(search: SearchAuto): Observable<ResponseResult<any>> {
    let body = {
      isSearch: search.isSearch,
      vinNumber: search.vinNumber,
      lotNumber: search.lotNumber,
      client: search.client,
      auction: search.auction,
      buyAccount: search.buyAccount,
      container: search.container,
      loadPort: search.loadPort,
      destination: search.destination,
      city: search.city,
      carId: search.carId,
      deliveryFromDate: search.deliveryFromDate?.toString() == '' ? null : search.deliveryFromDate,
      deliveryToDate: search.deliveryToDate?.toString() == '' ? null : search.deliveryToDate,
      Status: search.Status,
      purchaseFromDate: search.purchaseFromDate?.toString() == '' ? null : search.purchaseFromDate,
      purchaseToDate: search.purchaseToDate?.toString() == '' ? null : search.purchaseToDate,
    };

    const url = appSettings.webApiUrl + 'api/Autos/GetArchiveAllByUser';
    return this.http.post<ResponseResult<any>>(url, body);
  }

  DeleteAuto(id: number): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Autos/DeleteAuto/' + id;
    return this.http.post<ResponseResult<any>>(url, id);
  }

  DeleteAutos(model: Auto[]): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Autos/DeleteAutos/' + model;
    return this.http.post<ResponseResult<any>>(url, model);
  }

  ArchiveAuto(id: number): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Autos/ArchiveAuto/' + id;
    return this.http.post<ResponseResult<any>>(url, id);
  }

  ArchiveAutos(model: Auto[]): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Autos/ArchiveAutos/' + model;
    return this.http.post<ResponseResult<any>>(url, model);
  }

  UnArchiveAutos(model: Auto[]): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Autos/UnArchiveAutos/' + model;
    return this.http.post<ResponseResult<any>>(url, model);
  }

  DeleteImages(id: number): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Autos/DeleteImages/' + id;
    return this.http.post<ResponseResult<any>>(url, id);
  }

  GetAutoById(id: number): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Autos/GetAutoById/' + id;
    return this.http.get<ResponseResult<any>>(url);
  }

  GetCarName(): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Autos/GetCarName';
    return this.http.get<ResponseResult<any>>(url);
  }

  GetClientNameByUser(): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Autos/GetClientNameByUser';
    return this.http.get<ResponseResult<any>>(url);
  }

  GetSideMenuByUser(isArchive: number): Observable<any> {
    const url = appSettings.webApiUrl + 'api/Autos/GetSideMenuByUser/' + isArchive;
    return this.http.get<any>(url);
  }
}
