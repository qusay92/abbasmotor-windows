import { Observable, throwError } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { appSettings } from 'src/appSettings/appSettings';

@Injectable({
  providedIn: 'root'
})
export class UploadService {

  constructor(private http: HttpClient, private router: Router) { }

  uploadFile(formData: FormData, autoId:number, type:number, containerId:number): Observable<any[]> {   
    const url = appSettings.webApiUrl + 'api/Attachments/UploadImages';
    
    return this.http.post<any[]>(url + '/' + type + '/' + autoId + '/' + containerId, formData)
      .pipe(
        catchError(err => {
          return throwError(err);
        })
      )
  }
  // delete file method
  deleteAttachment(deleteList: Array<any>) {
    if (deleteList.length) {
    // for multiple delete do foreach here
    const body = { filename: deleteList[0].uploadname };
    const options = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      responseType: 'blob'
    };
      return this.http.post(`<delete file api>`, body);
    }

    return;
  }
  // Download file 
//   downloadFile = function (list:any, index:any) {
//     const body = { filename: list[index].uploadname };
//     const options = {
//       headers: new HttpHeaders({ 'Content-Type': 'application/json', Accept: 'application/pdf' }),
//       responseType: 'blob'
//     };
//     this.http.post(`<api URL>`, body, options).subscribe(data => {
//     this.saveFile(data, list[index].uploadname);
//     const blob = new Blob([data], { type: 'application/pdf' });
//     let filename =list[index].uploadname;
//     let result = filename.match('.pdf');
//     if (result) {
//       let blobURL = URL.createObjectURL(blob);
//       window.open(blobURL);
//     } else {
//       // saveAs(blob, filename);
//     }
//     });
//   };

}
