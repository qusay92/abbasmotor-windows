import { DOCUMENT } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { ClientService, LoginService, ResourcesService } from '../../services/index';
import { BaseComponent } from '../base/base.component';

@Component({
  selector: 'app-admin-layout',
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.css'],
})
export class AdminLayoutComponent extends BaseComponent implements OnInit {
  username: string | null = '';
  userId: number = 0;
  model: any;
  ngForm: FormGroup;
  isSubmitted: boolean = false;
  showDailog: boolean = false;
  showPassword: boolean = false;
  showConfirmedPassword: boolean = false;
  typeValidation = false;
  hasPermission = false;
  superAdmin = false;
  showMenu: boolean = false;

  constructor(
    public loginService: LoginService,
    private clientService: ClientService,
    fb: FormBuilder,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
    public resource: ResourcesService,
    public translate: TranslateService,
    @Inject(DOCUMENT) public document: Document
  ) {
    super(loginService, translate, resource,document);

    this.ngForm = fb.group({
      id: [],
      name: [],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.username = localStorage.getItem('username');
    this.PermissionCheck();
  }

  logout() {
    this.loginService.logout();
  }

  getUserLogin() {
    this.clientService.GetClientById().subscribe(
      (res) => {
        this.model = res.data;
      },
      (err) => {}
    );
  }

  editProfileDailog() {
    this.showDailog = true;
    this.getUserLogin();
  }

  onChangeConfirmPassword() {
    this.typeValidation = false;
  }

  editProfile() {
    this.isSubmitted = true;
    if (this.ngForm.value.password != this.ngForm.value.confirmPassword) {
      this.typeValidation = true;
      return;
    }

    if (this.ngForm.valid) {
      this.clientService.UpdatePassword(this.model.actualPass).subscribe(
        (res) => {
          this.showDailog = false;
          super.RemoveBlackScreen();
        },
        (err) => {}
      );
    }
  }

  showHidePassword() {
    this.showPassword = !this.showPassword;
  }

  showHideConfirmedPassword() {
    this.showConfirmedPassword = !this.showConfirmedPassword;
  }

  PermissionCheck() {
    if (this.loginService.userType() == 1) {
      this.hasPermission = true;
    } else if (this.loginService.userType() == 2) {
      this.hasPermission = true;
      this.superAdmin = true;
    } else this.hasPermission = false;
  }

  toggleMenu(){
    this.showMenu = !this.showMenu;
  }

  disableMenu(){
    this.showMenu = false;
  }

  // constructor(private ngTranslateHelperService: NGTranslateHelperService) {
  //   this.ngTranslateHelperService.addTranslationJsonFile(PortalLayoutComponent.name);
  //   if (this.ngTranslateHelperService.getCurrentLang() == LanguagesEnum[LanguagesEnum.Ar])
  //     loadCSS("../../assets/css/rtl.css");
  //    // loadCSS("../../assets/css/ltr.css");
  //   else
  //     loadCSS("../../assets/css/ltr.css");
  // }
}
