import { Component, Inject, OnInit } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { ClientService } from '../../services/client.service';
import { StatusType, User, UserType } from '../../models/models';
import { ToastrService } from 'ngx-toastr';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { LoginService, ResourcesService } from 'src/services';
import { BaseComponent } from '../base/base.component';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-clients',
  templateUrl: './clients.component.html',
  styleUrls: ['./clients.component.css'],
})
export class ClientsComponent extends BaseComponent implements OnInit {
  clients: any[] = [];
  page: any;
  search: string = '';
  showDialog = false;
  model: User = new User();
  disableSave = true;
  enableEditPassword = false;
  originalPassword = null;
  editPasswordValue = false;
  ngForm: FormGroup;
  isSubmitted: boolean = false;
  showPassword: boolean = false;
  checkedClients: number = 0;
  showDeleteAllDialog: boolean = false;
  deleteclient: any[] = [];
  deletedList: any[] = [];

  constructor(
    private clientService: ClientService,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
    fb: FormBuilder,
    public translate: TranslateService,
    public loginService: LoginService,
    public resource: ResourcesService,
    @Inject(DOCUMENT) public document: Document
  ) {
    super(loginService, translate, resource, document);
    this.ngForm = fb.group({
      id: [],
      name: ['', Validators.required],
      userName: ['', Validators.required],
      password: ['', Validators.required],
      mobile: [],
      company: [],
      address: [],
    });
  }

  ngOnInit(): void {
    this.LoadClients();
  }

  LoadClients(showPinner = true) {
    if (showPinner) {
      this.spinner.show();
    }

    this.clientService.GetClients(this.search).subscribe(
      (res) => {
        this.spinner.hide();
        if (res && res.status == StatusType.Success) {
          this.clients = res.data;
        }
      },
      (err) => {
        this.spinner.hide();
      }
    );
  }

  filterGlobal(evt: any) {
    this.search = evt.target.value;
    this.LoadClients();
  }

  openDialog() {
    this.enableEditPassword = true;
    this.model = new User();
    this.showDialog = true;
    this.disableSave = true;
  }

  hideDialog() {
    this.showDialog = false;
  }

  saveClient() {
    this.isSubmitted = true;
    if (this.ngForm.valid) {
      this.spinner.show();
      //this.ngForm.controls['password'].enable();
      this.model = this.ngForm.value;
      this.model.type = UserType.Client;
      this.model.groupId = 1;

      this.clientService
        .SaveClient(this.model, this.editPasswordValue)
        .subscribe(
          (res) => {
            this.spinner.hide();
            if (res && res.status == StatusType.Success) {
              this.LoadClients(false);
              this.showDialog = false;
              this.translate
                .get([
                  'ArchiveComponent.SavedSuccessfully',
                  'ArchiveComponent.UpdateSuccessfully',
                ])
                .subscribe((res) => {
                  this.toastr.success(
                    '',
                    this.model.id < 1 ||
                      this.model.id == null ||
                      this.model.id == undefined
                      ? res['ArchiveComponent.SavedSuccessfully']
                      : res['ArchiveComponent.UpdateSuccessfully']
                  );
                });
                super.RemoveBlackScreen();
              this.spinner.hide();
            } else {
              if (
                res.status == StatusType.Failed &&
                res.errors &&
                res.errors.length > 0
              ) {
                for (let i = 0; i < res.errors.length; i++) {
                  this.toastr.error('', res.errors[i]);
                }
                //this.ngForm.controls['password'].disable();
                this.spinner.hide();
              }
            }
          },
          (err) => {
            // this.ngForm.controls['password'].disable();
            this.spinner.hide();
          }
        );
    }
  }

  editRow(row: any) {
    this.editPasswordValue = false;
    this.enableEditPassword = true;
    //this.ngForm.controls['password'].disable();
    this.model = Object.assign({}, row);
    this.showDialog = true;
  }

  openDeleteAllDialog() {
    this.showDeleteAllDialog = true;
  }
  openDeleteDialog(item: any) {
    this.deleteclient.push(item);
    this.showDeleteAllDialog = true;
  }

  deleteClient() {
    const selectedpaymentDetails = this.clients.filter((cont) => cont.checked);

    if (
      (selectedpaymentDetails && selectedpaymentDetails.length > 0) ||
      this.deleteclient.length > 0
    ) {
      if (selectedpaymentDetails.length > 0)
        this.deletedList = selectedpaymentDetails;
      if (this.deleteclient.length > 0) this.deletedList = this.deleteclient;

      this.clientService.DeleteClient(this.deletedList).subscribe(
        (res) => {
          this.spinner.hide();
          if (res && res.status == StatusType.Success) {
            this.LoadClients(false);
            this.showDeleteAllDialog = false;
            this.translate
              .get(['ArchiveComponent.DeleteSuccessfully'])
              .subscribe((res) => {
                this.toastr.success(
                  '',
                  res['ArchiveComponent.DeleteSuccessfully']
                );
              });
              this.checkedClients = 0;
              super.RemoveBlackScreen();
          } else {
            if (
              res.status == StatusType.Failed &&
              res.errors &&
              res.errors.length > 0
            ) {
              for (let i = 0; i < res.errors.length; i++) {
                this.toastr.error('', res.errors[i]);
              }
              this.spinner.hide();
            }
          }
          this.deleteclient = [];
        },
        (err) => {
          this.spinner.hide();
        }
      );
    }
  }

  showHidePassword() {
    this.showPassword = !this.showPassword;
  }

  onCheckboxChange(event: any) {
    if (event.target.checked) {
      this.checkedClients = this.checkedClients + 1;
    } else {
      this.checkedClients = this.checkedClients - 1;
    }
  }
}
