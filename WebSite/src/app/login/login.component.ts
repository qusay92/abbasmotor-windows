import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { LoginVM } from 'src/models/loginVM';
import { StatusType, UserType } from 'src/models/models';
import { LoginService, ResourcesService } from '../../services/index';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { BaseComponent } from '../base/base.component';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent extends BaseComponent implements OnInit {
  ngForm: FormGroup;
  isSubmitted: boolean;
  loginVM: LoginVM;

  constructor(
    fb: FormBuilder,
    private spinner: NgxSpinnerService,
    public loginService: LoginService,
    private router: Router,
    private toastr: ToastrService,
    public resource: ResourcesService,
    public translate: TranslateService,
    @Inject(DOCUMENT) public document: Document
  ) {
    super(loginService, translate, resource, document);
    this.isSubmitted = false;
    this.loginVM = new LoginVM();

    this.ngForm = fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  ngOnInit(): void {}

  login() {
    this.isSubmitted = true;

    if (this.ngForm.valid) {
      this.spinner.show();

      this.loginService.Login(this.ngForm.value).subscribe((item) => {
        this.spinner.hide();
        if (item && item.status == StatusType.Success) {
          this.loginService.setStorage(item);

          if (item.data.type == UserType.Admin) {
            this.router.navigate(['/dashboard']);
          } else {
            this.router.navigate(['/dashboard']);
          }
        } else {
          if (item && item.errors && item.errors.length > 0) {
            for (let i = 0; i < item.errors.length; i++) {
              this.toastr.error('', item.errors[0]);
            }
          }
        }
      });
    }
  }
}
