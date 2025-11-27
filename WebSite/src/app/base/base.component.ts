import { DOCUMENT } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { LoginService, ResourcesService } from 'src/services';

@Component({
  selector: 'app-base',
  templateUrl: './base.component.html',
  styleUrls: ['./base.component.css'],
})
export class BaseComponent implements OnInit {
  hasPermission = true;
  

  constructor(
    public loginService: LoginService,
    public translate: TranslateService,
    public resource: ResourcesService,
    @Inject(DOCUMENT) public document: Document
  ) {

    translate.addLangs(['en', 'ar']);
    translate.setDefaultLang('ar');

    const browserLang = translate.getBrowserLang();
    translate.use('ar');

    if (
      localStorage.getItem('currantLang') == 'ar' ||
      localStorage.getItem('currantLang') == 'en'
    ) {
      translate.use(localStorage.getItem('currantLang') || '{}');
    }

    this.resource.currentCulture.subscribe((res) => {
      translate.use(res);
      window.location.reload();
    });

    this.PermissionCheck();
  }

  ngOnInit() {
  }

  PermissionCheck() {
    
    if (!this.loginService.isAdmin()) {
      this.hasPermission = false;
    } else {
      this.hasPermission = true;
    }
  }

  RemoveBlackScreen(){
    this.document.body.classList.remove('modal-open');
    this.document.body.style.removeProperty('overflow');           
    const elements = this.document.documentElement.getElementsByClassName('modal-backdrop');
    while(elements.length > 0){
        elements[0].parentNode?.removeChild(elements[0]);
    }
  }
}
