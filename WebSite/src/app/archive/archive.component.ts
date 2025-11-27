import { DOCUMENT } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { AutoById, ContainerById, SearchAuto, SearchContainer, StatusType } from 'src/models/models';
import { AutoService, ContainerService, LoginService, ResourcesService } from 'src/services';
import { BaseComponent } from '../base/base.component';

@Component({
  selector: 'app-archive',
  templateUrl: './archive.component.html',
  styleUrls: ['./archive.component.css'],
})
export class ArchiveComponent extends BaseComponent implements OnInit {
  autos: any[] = [];
  searchmodel: SearchAuto = new SearchAuto();
  searchContianermodel: SearchContainer = new SearchContainer();
  checkedAutos: number = 0;
  checkedCont: number = 0;
  deleteauto: any[] = [];
  deletedList: any[] = [];
  unarchiveauto: any[] = [];
  unarchivedList: any[] = [];
  showDeleteDialog: boolean = false;
  showUnArchiveDialog: boolean = false;
  selectAuto: AutoById = new AutoById();
  images: any[] = [];
  page: any;
  pageCont: any;
  showArchive = 1;
  sideMenuList: any[] = [];
  showDialog = false;
  sideMenuContainerList : any[] = [];
  containers: any[] = [];
  selectContainer: ContainerById = new ContainerById();
  deleteCont: any[] = [];
  deletedContList: any[] = [];
  showDeleteContDialog: boolean = false;
  unarchiveCont: any[] = [];
  unarchivedContList: any[] = [];
  showUnArchiveContDialog: boolean = false;

  constructor(
    public translate: TranslateService,
    public loginService: LoginService,
    public resource: ResourcesService,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
    private autoService: AutoService,
    private containerService: ContainerService,
    @Inject(DOCUMENT) public document: Document
  ) {
    super(loginService, translate, resource,document);
  }

  ngOnInit(): void {
    this.LoadAutos(true, this.searchmodel);
    this.LoadSideMenu();
    this.LoadContainers(true, this.searchContianermodel);
    this.LoadContainerSideMenu();
  }

  LoadAutos(showPinner = true, search: SearchAuto) {
    if (showPinner) {
      this.spinner.show();
    }
    this.autoService.GetArchiveAllByUser(search).subscribe(
      (res) => {
        this.autos = res.data;
        this.spinner.hide();
      },
      (err) => {
        this.spinner.hide();
      }
    );
  }

  onCheckboxChange(event: any) {
    if (event.target.checked) {
      this.checkedAutos = this.checkedAutos + 1;
    } else {
      this.checkedAutos = this.checkedAutos - 1;
    }
  }

  openDeleteDialog(item: any) {
    this.deleteauto.push(item);
    this.showDeleteDialog = true;
  }

  openDeleteAllDialog() {
    this.showDeleteDialog = true;
  }

  openImagePopup(id: number) {
    this.spinner.show();
    this.autoService.GetAutoById(id).subscribe(
      (res) => {
        this.selectAuto = res.data;

        for (let i = 0; i < res.data.images.length; i++) {
          let toBeReplaced = 'assets/uploaded/';
          res.data.images[i].previewImageSrc =
            toBeReplaced + res.data.images[i].title;
          res.data.images[i].thumbnailImageSrc =
            toBeReplaced + res.data.images[i].title;
        }

        this.images = res.data.images;
        this.spinner.hide();
      },
      (err) => {
        this.spinner.hide();
      }
    );
  }

  deleteAuto() {
    const selectedAutos = this.autos.filter((auto) => auto.checked);
    if (
      (selectedAutos && selectedAutos.length > 0) ||
      this.deleteauto.length > 0
    ) {
      if (selectedAutos.length > 0) this.deletedList = selectedAutos;
      if (this.deleteauto.length > 0) this.deletedList = this.deleteauto;

      this.autoService.DeleteAutos(this.deletedList).subscribe(
        (res) => {
          this.spinner.hide();
          if (res && res.status == StatusType.Success) {
            this.LoadAutos(false, this.searchmodel);
            this.LoadSideMenu();
            this.showDeleteDialog = false;
            this.translate.get(['ArchiveComponent.DeleteSuccessfully']).subscribe(res => {
              this.toastr.success('', res['ArchiveComponent.DeleteSuccessfully']);
            });
            this.checkedAutos = 0;
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
              this.showDeleteDialog = false;
            }
          }
        },
        (err) => {
          this.spinner.hide();
          this.showDeleteDialog = false;
        }
      );
    }
  }

  LoadSideMenu() {
    let isArchive =
      <any>this.showArchive == true || this.showArchive == 1 ? 1 : 0;
    this.autoService.GetSideMenuByUser(isArchive).subscribe(
      (res) => {
        if (res) {
          this.sideMenuList = [];
          this.sideMenuList.push({
            label: 'All',
            labeltext: 'All',
            boughtNew: res.all.boughtNew,
            loaded: res.all.loaded,
            arrived: res.all.arrived,
          });
          for (let i = 0; i < res.filteredAutos.length; i++) {
            this.sideMenuList.push({
              label: res.filteredAutos[i].loadPort,
              labeltext: res.filteredAutos[i].loadPort.replace(/\s/g, ''),
              loadPortId: res.filteredAutos[i].loadPortId,
              boughtNew: res.filteredAutos[i].boughtNew,
              loaded: res.filteredAutos[i].loaded,
              arrived: res.filteredAutos[i].arrived,
            });
          }
        }
      },
      (err) => {}
    );
  }

  onSideItemClick(item: any, status: any) {
    let search = new SearchAuto();
    search.Status = status;
    if (item != null) {
      if (item.label == 'All') {
        this.LoadAutos(false, search);
      } else {
        search.loadPort = item.loadPortId;
        this.LoadAutos(false, search);
      }
    } else {
      this.LoadAutos(false, search);
    }
  }

  openUnArchiveDialog(item: any) {
    this.unarchiveauto.push(item);
    this.showUnArchiveDialog = true;
  }

  openUnArchiveAllDialog() {
    this.showUnArchiveDialog = true;
  }

  UnArchiveAllAutos() {
    const selectedAutos = this.autos.filter((auto) => auto.checked);
    if ((selectedAutos && selectedAutos.length > 0) ||this.unarchiveauto.length > 0) {
      if (selectedAutos.length > 0) this.unarchivedList = selectedAutos;
      if (this.unarchiveauto.length > 0) this.unarchivedList = this.unarchiveauto;

      this.autoService.UnArchiveAutos(this.unarchivedList).subscribe(
        (res) => {
          if (res && res.status == StatusType.Success) {
            this.LoadAutos(true, this.searchmodel);
            this.LoadSideMenu();
            this.showUnArchiveDialog = false;
            this.translate.get(['ArchiveComponent.UnArchiveSuccessfully']).subscribe(res => {
              this.toastr.success('', res['ArchiveComponent.UnArchiveSuccessfully']);
            });
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
        },
        (err) => {
          this.spinner.hide();
        }
      );
    } else {
      this.translate.get(['ArchiveComponent.SelectAtLeastOneAuto']).subscribe(res => {
        this.toastr.warning('', res['ArchiveComponent.SelectAtLeastOneAuto']);
      });
    }
  }



  // Container

  LoadContainerSideMenu() {
    let isArchive = <any>this.showArchive == true || this.showArchive == 1 ? 1 : 0;
    this.containerService.GetSideMenuByUser(isArchive).subscribe(
      (res) => {
        if (res) {
          this.sideMenuContainerList = [];
          this.sideMenuContainerList.push({
            label: 'All',
            labeltext: 'All',
            awaitingload: res.all.awaitingload,
            departured: res.all.departured,
            arrived: res.all.arrived,
          });
          for (let i = 0; i < res.filteredContainers.length; i++) {
            this.sideMenuContainerList.push({
              label: res.filteredContainers[i].departurePort,
              labeltext: res.filteredContainers[i].departurePort.replace(/\s/g, ""),
              departurePortId: res.filteredContainers[i].departurePortId,
              awaitingload: res.filteredContainers[i].awaitingload,
              departured: res.filteredContainers[i].departured,
              arrived: res.filteredContainers[i].arrived,
            });
          }
        }
      },
      (err) => {}
    );
  }

  onContainerSideItemClick(item: any, status: any) {
    let search = new SearchContainer();
    search.StatusId = status;
    if (item != null) {
      if (item.label == 'All') {
        this.LoadContainers(false, search);
      } else {
        search.loadPortId = item.loadPortId;
        this.LoadContainers(false, search);
      }
    } else {
      this.LoadContainers(false, search);
    }
  }

  LoadContainers(showPinner = true, search: SearchContainer) {
    if (showPinner) {
      this.spinner.show();
    }

    this.containerService.getAllArchiveByUser(search).subscribe(
      (res) => {
        this.containers = res.data;
        this.spinner.hide();
      },
      (err) => {
        this.spinner.hide();
      }
    );
  }

  onContCheckboxChange(event: any) {
    if (event.target.checked) {
      this.checkedCont = this.checkedCont + 1;
    } else {
      this.checkedCont = this.checkedCont - 1;
    }
  }

  openImageContPopup(id: number){
    this.spinner.show();
    this.containerService.GetContainerById(id).subscribe(
      (res) => {
        this.selectContainer = res.data;
        for (let i = 0; i < res.data.images.length; i++) {
          let toBeReplaced = '../assets/uploaded/';
          res.data.images[i].previewImageSrc =
            toBeReplaced + res.data.images[i].title;
          res.data.images[i].thumbnailImageSrc =
            toBeReplaced + res.data.images[i].title;
        }
        this.images = res.data.images;
        this.spinner.hide();
      },
      (err) => {
        this.spinner.hide();
      }
    );
  }

  openDeleteContDialog(item: any) {
    this.deleteCont.push(item);
    this.showDeleteContDialog = true;
  }

  openDeleteAllContDialog() {
    this.showDeleteContDialog = true;
  }

  deleteContainer(){
    const selectedCont = this.containers.filter((cont) => cont.checked);
    if (
      (selectedCont && selectedCont.length > 0) ||
      this.deleteCont.length > 0
    ) {
      if (selectedCont.length > 0) this.deletedContList = selectedCont;
      if (this.deleteCont.length > 0) this.deletedContList = this.deleteCont;
    
      this.containerService.DeleteContainers(this.deletedContList).subscribe(
        (res) => {
          if (res && res.status == StatusType.Success) {
            this.LoadContainers(true, this.searchContianermodel);
            this.showDeleteContDialog = false;
            this.translate.get(['ArchiveComponent.DeleteSuccessfully']).subscribe(res => {
              this.toastr.success('', res['ArchiveComponent.DeleteSuccessfully']);
            });
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
        },
        (err) => {
          this.spinner.hide();
        }
      );
    }
  }

  openUnArchiveContDialog(item: any) {
    this.unarchiveCont.push(item);
    this.showUnArchiveContDialog = true;
  }

  openUnArchiveAllContDialog() {
    this.showUnArchiveContDialog = true;
  }

  UnArchiveContainers() {
    const selectedCont = this.containers.filter((cont) => cont.checked);
    if ((selectedCont && selectedCont.length > 0) ||this.unarchiveCont.length > 0) {
      if (selectedCont.length > 0) this.unarchivedContList = selectedCont;
      if (this.unarchiveCont.length > 0) this.unarchivedContList = this.unarchiveCont;

      this.containerService.UnArchiveContainers(this.unarchivedContList).subscribe(
        (res) => {
          if (res && res.status == StatusType.Success) {
            this.LoadContainers(true, this.searchContianermodel);
            this.LoadContainerSideMenu();
            this.showUnArchiveContDialog = false;
            this.translate.get(['ArchiveComponent.UnArchiveSuccessfully']).subscribe(res => {
              this.toastr.success('', res['ArchiveComponent.UnArchiveSuccessfully']);
            });
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
        },
        (err) => {
          this.spinner.hide();
        }
      );
    } else {
      this.translate.get(['ArchiveComponent.SelectAtLeastOneContainer']).subscribe(res => {
        this.toastr.warning('', res['ArchiveComponent.SelectAtLeastOneContainer']);
      });
    }
  }


}
