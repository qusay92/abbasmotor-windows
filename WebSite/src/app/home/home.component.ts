import { DOCUMENT } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import {
  LoginService,
  NGTranslateHelperService,
  ResourcesService,
} from 'src/services';
import { BaseComponent } from '../base/base.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent extends BaseComponent implements OnInit {
  constructor(
    private router: Router,
    public resource: ResourcesService,
    public translate: TranslateService,
    public loginService: LoginService,
    @Inject(DOCUMENT) public document: Document
  ) {
    super(loginService, translate, resource, document);
    this.HandleResources();
  }

  selectedCulture = 'ar';
  ActionName: string | null = '';
  ABOUTUS: string = '';
  RoadTransport: string = '';
  OceanFreight: string = '';
  StorageService: string = '';
  ABOUTUSDesc: string = '';
  OceanFreightDesc: string = '';
  RoadTransportDesc: string = '';
  StorageServiceDesc: string = '';
  OurServices: string = '';
  AllShippingServices: string = '';
  AllShippingServicesDesc: string = '';
  Warehousing: string = '';
  WarehousingDesc: string = '';
  AutoExport: string = '';
  AutoExportDesc: string = '';
  StorageSolutions: string = '';
  StorageSolutionsDesc: string = '';
  WhyAbbasMotor: string = '';
  WhyAbbasMotorDesc: string = '';
  OnlineTrackingManagement: string = '';
  OnceYouRegister: string = '';
  MonitorAllExpenses: string = '';
  CarTrackingService: string = '';
  AllPicturesOfTheCar: string = '';
  ConstantCommunication: string = '';
  AnswerAllQuestions: string = '';
  Safety: string = '';
  SafetyDesc: string = '';
  Speed: string = '';
  SpeedDesc: string = '';
  EndToEndServices: string = '';
  EndToEndServicesDesc: string = '';

  ngOnInit(): void {
    let culture = localStorage.getItem('currantLang');
    if (culture != null && culture != undefined && culture != '') {
      localStorage.setItem('currantLang', culture);
      this.selectedCulture = culture;
    }

    if (
      window.localStorage.getItem('username') != null &&
      window.localStorage.getItem('username') != ''
    ) {
      this.ActionName = window.localStorage.getItem('username');
    } else {
      if (this.selectedCulture == 'en') this.ActionName = 'Login';
      else this.ActionName = 'تسجيل الدخول';
    }
  }

  loginroterlink() {
    let username = window.localStorage.getItem('username');
    if (username != null && username != '') {
      this.router.navigate(['/dashboard']);
    } else {
      this.router.navigate(['/login']);
    }
  }

  HandleResources() {
    this.resource.GetHomePageResources().subscribe(
      (res) => {
        this.ABOUTUS = res.data.filter((x: any) => x.key == 'ABOUTUS')[0].text;
        this.RoadTransport = res.data.filter(
          (x: any) => x.key == 'RoadTransport'
        )[0].text;
        this.OceanFreight = res.data.filter(
          (x: any) => x.key == 'RoadTransport'
        )[0].text;
        this.StorageService = res.data.filter(
          (x: any) => x.key == 'StorageService'
        )[0].text;
        this.ABOUTUSDesc = res.data.filter(
          (x: any) => x.key == 'ABOUTUSDesc'
        )[0].text;
        this.OceanFreightDesc = res.data.filter(
          (x: any) => x.key == 'OceanFreightDesc'
        )[0].text;
        this.RoadTransportDesc = res.data.filter(
          (x: any) => x.key == 'RoadTransportDesc'
        )[0].text;
        this.StorageServiceDesc = res.data.filter(
          (x: any) => x.key == 'StorageServiceDesc'
        )[0].text;
        this.OurServices = res.data.filter(
          (x: any) => x.key == 'OurServices'
        )[0].text;
        this.AllShippingServices = res.data.filter(
          (x: any) => x.key == 'AllShippingServices'
        )[0].text;
        this.AllShippingServicesDesc = res.data.filter(
          (x: any) => x.key == 'AllShippingServicesDesc'
        )[0].text;
        this.Warehousing = res.data.filter(
          (x: any) => x.key == 'Warehousing'
        )[0].text;
        this.WarehousingDesc = res.data.filter(
          (x: any) => x.key == 'WarehousingDesc'
        )[0].text;
        this.AutoExport = res.data.filter(
          (x: any) => x.key == 'AutoExport'
        )[0].text;
        this.AutoExportDesc = res.data.filter(
          (x: any) => x.key == 'AutoExportDesc'
        )[0].text;
        this.StorageSolutions = res.data.filter(
          (x: any) => x.key == 'StorageSolutions'
        )[0].text;
        this.StorageSolutionsDesc = res.data.filter(
          (x: any) => x.key == 'StorageSolutionsDesc'
        )[0].text;
        this.WhyAbbasMotor = res.data.filter(
          (x: any) => x.key == 'WhyAbbasMotor'
        )[0].text;
        this.WhyAbbasMotorDesc = res.data.filter(
          (x: any) => x.key == 'WhyAbbasMotorDesc'
        )[0].text;
        this.OnlineTrackingManagement = res.data.filter(
          (x: any) => x.key == 'OnlineTrackingManagement'
        )[0].text;
        this.OnceYouRegister = res.data.filter(
          (x: any) => x.key == 'OnceYouRegister'
        )[0].text;
        this.MonitorAllExpenses = res.data.filter(
          (x: any) => x.key == 'MonitorAllExpenses'
        )[0].text;
        this.CarTrackingService = res.data.filter(
          (x: any) => x.key == 'CarTrackingService'
        )[0].text;
        this.AllPicturesOfTheCar = res.data.filter(
          (x: any) => x.key == 'AllPicturesOfTheCar'
        )[0].text;
        this.ConstantCommunication = res.data.filter(
          (x: any) => x.key == 'ConstantCommunication'
        )[0].text;
        this.AnswerAllQuestions = res.data.filter(
          (x: any) => x.key == 'AnswerAllQuestions'
        )[0].text;
        this.Safety = res.data.filter((x: any) => x.key == 'Safety')[0].text;
        this.SafetyDesc = res.data.filter(
          (x: any) => x.key == 'SafetyDesc'
        )[0].text;
        this.Speed = res.data.filter((x: any) => x.key == 'Speed')[0].text;
        this.SpeedDesc = res.data.filter(
          (x: any) => x.key == 'SpeedDesc'
        )[0].text;
        this.EndToEndServices = res.data.filter(
          (x: any) => x.key == 'EndToEndServices'
        )[0].text;
        this.EndToEndServicesDesc = res.data.filter(
          (x: any) => x.key == 'EndToEndServicesDesc'
        )[0].text;
      },
      (err) => {}
    );
  }

  translateLanguageTo(lang: string) {
    localStorage.setItem('currantLang', lang);
    this.translate.use(lang);
    this.HandleResources();

    if (this.selectedCulture == 'en') this.ActionName = 'Login';
    else this.ActionName = 'تسجيل الدخول';
  }
}
