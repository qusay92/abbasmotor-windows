import { DOCUMENT } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { ResourceParamDto, StatusType } from 'src/models/models';
import { LoginService, ResourcesService } from 'src/services';
import { BaseComponent } from '../base/base.component';

@Component({
  selector: 'app-resources',
  templateUrl: './resources.component.html',
  styleUrls: ['./resources.component.css'],
})
export class ResourcesComponent extends BaseComponent implements OnInit {
  page: any;
  search: string = '';
  resourceId: number = 0;
  showDialog: boolean = false;
  model: ResourceParamDto = new ResourceParamDto();
  ngForm: FormGroup;
  isSubmitted: boolean = false;
  resources: any[] = [];
  searchKeys: any[] = [];

  constructor(
    fb: FormBuilder,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
    public translate: TranslateService,
    public loginService: LoginService,
    public resource: ResourcesService,
    @Inject(DOCUMENT) public document: Document
  ) {
    super(loginService,translate, resource,document);
    this.ngForm = fb.group({
      id: [],
      key: ['', Validators.required],
      english: ['', Validators.required],
      arabic: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.LoadResources();
    this.LoadResourcesKeys();
  }

  LoadResources(search = '', showPinner = true, resourceId = 0) {
    search = this.search != null ? this.search : search;
    if (showPinner) {
      this.spinner.show();
    }

    this.resource.GetResources(search, resourceId).subscribe(
      (res) => {
        this.resources = res.data;
        this.spinner.hide();
      },
      (err) => {
        this.spinner.hide();
      }
    );
  }

  LoadResourcesKeys() {
    this.resource.GetResourcesKeys().subscribe(
      (res) => {
        this.searchKeys = res.data;
        this.spinner.hide();
      },
      (err) => {
        this.spinner.hide();
      }
    );
  }

  filterGlobal(evt: any) {
    this.LoadResources(evt.target.value, false, this.resourceId);
  }

  ChangeKey(evt: any) {
    this.LoadResources(this.search, false, this.resourceId);
  }
  ClearSearch(){
    this.LoadResources(this.search);
  }

  openDialog() {
    this.showDialog = true;
    this.model = new ResourceParamDto();
    this.ngForm.controls['key'].enable();
  }

  saveResource() {
    this.isSubmitted = true;
    if (this.ngForm.valid) {
      this.ngForm.controls['key'].enable();
      this.spinner.show();
      this.model = this.ngForm.value;
      this.resource.SaveResource(this.model).subscribe(
        (res) => {
          if (res && res.status == StatusType.Success) {
            this.LoadResources();
            this.LoadResourcesKeys();
            this.ngForm.controls['key'].enable();
            this.translate.get(['ArchiveComponent.SavedSuccessfully','ArchiveComponent.UpdateSuccessfully']).subscribe(res => {
              this.toastr.success(
                '',
                this.model.id < 1 || this.model.id == null || this.model.id == undefined
                  ? res['ArchiveComponent.SavedSuccessfully']
                  : res['ArchiveComponent.UpdateSuccessfully']
              );
            });
            super.RemoveBlackScreen();
            this.showDialog = false;
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
        },
        (err) => {
          this.spinner.hide();
        }
      );
    }
  }


  editResource(resource: any) {
    this.model = Object.assign({}, resource);
    this.showDialog = true;
    this.ngForm.controls['key'].disable();
  }

}
