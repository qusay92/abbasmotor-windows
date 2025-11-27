import { Payment, PaymentDetails, ResponseResult, SearchPayment } from '../models/models';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { appSettings } from 'src/appSettings/appSettings';



@Injectable({
  providedIn: 'root',
})
export class PaymentService {

  constructor(private http: HttpClient, private router: Router) {}

  GetPayment(
    autoId: number
  ): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Payments/GetPayment/' + autoId;
    return this.http.get<ResponseResult<any>>(url);
  }

  GetPayments(search: SearchPayment){
    let body = {
      isSearch: search.isSearch,
      autoId: search.autoId,
      vinNo: search.vinNo,
      purchaseDate: search.purchaseDate?.toString() == '' ? null : search.purchaseDate,
      clientId: search.clientId
    };

    const url = appSettings.webApiUrl + 'api/Payments/GetPayments';
    return this.http.post<ResponseResult<any>>(url, body);
  }

  SavePayment(model: Payment): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Payments/SavePayment';
    return this.http.post<ResponseResult<any>>(url, model);
  }

  savePaymentDetails(model: PaymentDetails): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Payments/savePaymentDetails';
    return this.http.post<ResponseResult<any>>(url, model);
  }

  Delete(model: PaymentDetails[]): Observable<ResponseResult<any>> {
    const url = appSettings.webApiUrl + 'api/Payments/Delete';
    return this.http.post<ResponseResult<any>>(url, model);
  }
}
