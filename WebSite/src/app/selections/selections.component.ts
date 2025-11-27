import { DOCUMENT } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { LookupValue, StatusType } from 'src/models/models';
import { LoginService, LookupService, ResourcesService } from '../../services/index';
import { BaseComponent } from '../base/base.component';

@Component({
  selector: 'app-selections',
  templateUrl: './selections.component.html',
  styleUrls: ['./selections.component.css'],
})
export class SelectionsComponent extends BaseComponent implements OnInit {
  lookups: any[] = [];
  lookupvalues: any[] = [];
  page: any;
  search: string = '';
  lookupId: number = 0;
  showDialog: boolean = false;
  model: LookupValue = new LookupValue();
  ngForm: FormGroup;
  isSubmitted: boolean = false;
  selectedLookupvalues: any[] = [];
  showDeleteDialog: boolean = false;
  typeValidation = false;


  constructor(
    private lookupService: LookupService,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
    fb: FormBuilder,
    public translate: TranslateService,
    public loginService: LoginService,
    public resource: ResourcesService,
    @Inject(DOCUMENT) public document: Document
  ) {
    super(loginService,translate, resource,document);
    this.ngForm = fb.group({
      id: [],
      name: ['', Validators.required],
      lookupId: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.GetLookups();
    this.GetLookupValues();
  }

  GetLookups() {
    this.lookupService.GetLookups().subscribe(
      (res) => {
        if (
          res &&
          res.status == StatusType.Success &&
          res.data &&
          res.data.length > 0
        ) {
          this.lookups = res.data;
        }
      },
      (err) => {}
    );
  }

  GetLookupValues(search = '', showPinner = true, lookupId = 0) {
    if (showPinner) {
      this.spinner.show();
    }
    this.lookupService.GetLookupValues(search, lookupId).subscribe(
      (res) => {
        this.spinner.hide();
        if (res && res.status == StatusType.Success) {
          this.lookupvalues = res.data;
        }
      },
      (err) => {
        this.spinner.hide();
      }
    );
  }

  filterGlobal(evt: any) {
    this.GetLookupValues(evt.target.value, false, this.lookupId);
  }
  Changelookups(evt: any) {
    this.GetLookupValues(this.search, false, evt.target.value);
  }

  openDialog() {
    this.showDialog = true;
    this.model = new LookupValue();
  }

  openDeleteDialog(item: any) {
    this.selectedLookupvalues.push(item);
    this.showDeleteDialog = true;
  }

  onChangeType(){
    this.typeValidation = false;
  }
  saveLookupValue() {
    this.isSubmitted = true;

    if(this.ngForm.value.lookupId == '')
    {
      this.typeValidation = true
      return;
    }

    if (this.ngForm.valid) {
      this.spinner.show();
      this.model = this.ngForm.value;
      this.lookupService.Save(this.model).subscribe(
        (res) => {
          if (res && res.status == StatusType.Success) {
            this.GetLookupValues(this.search, false, this.lookupId);
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

  editLookup(container: any) {
    this.model = Object.assign({}, container);
    this.showDialog = true;
  }

  deleteLookupValue() {
    this.lookupService.Delete(this.selectedLookupvalues).subscribe(
      (res) => {
        if (res && res.status == StatusType.Success) {
          this.lookupvalues = res.data;
          this.selectedLookupvalues = [];
          this.GetLookupValues(this.search, false, this.lookupId);
          this.translate.get(['ArchiveComponent.DeleteSuccessfully']).subscribe(res => {
            this.toastr.success('', res['ArchiveComponent.DeleteSuccessfully']);
          });
          super.RemoveBlackScreen();
        } else if (
          res.status == StatusType.Failed &&
          res.errors != null &&
          res.errors.length > 0
        ) {
          for (let i = 0; i < res.errors.length; i++) {
            this.toastr.error('', res.errors[i]);
          }
        }
      },
      (err) => {}
    );
  }
}
