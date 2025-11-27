import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { AutoById, Container, ContainerById, Payment, SearchAuto, SearchContainer, StatusType } from 'src/models/models';
import { AutoImagesService, AutoService, ContainerImagesService, ContainerService, LoginService, LookupService, PaymentService, ResourcesService, UploadService } from 'src/services';
import { Galleria } from 'primeng/galleria';
import * as JSZip from 'jszip';
import { saveAs } from "file-saver";
import * as jSZipUtils from '../../assets/js/jszip-utils.js';
import { BaseComponent } from '../base/base.component';
import { TranslateService } from '@ngx-translate/core';
import { DOCUMENT } from '@angular/common';

//For compressing images
import { NgxImageCompressService } from 'ngx-image-compress';

@Component({
  selector: 'app-containers',
  templateUrl: './containers.component.html',
  styleUrls: ['./containers.component.css'],
})
export class ContainersComponent extends BaseComponent implements OnInit {
  searchForm: FormGroup;
  searchmodel: SearchContainer = new SearchContainer();
  clients: any;
  lookup_loadPort: any;
  lookup_destination: any;
  lookup_shippingCompany: any;
  page: any;
  pagepayment: any;
  sideMenuList: any[] = [];
  showArchive = 0;
  containers: any[] = [];
  showDialog = false;
  showDeleteAllDialog = false;
  showArchiveAllDialog = false;
  showArchiveDialog = false;
  showDeleteDialog = false;
  showDeleteImagesDialog = false;
  model: Container = new Container();
  ngAddUpdateForm: FormGroup;
  files: File[] = [];
  formData = new FormData();
  isSubmitted: boolean = false;
  autoList: any[] = [];
  selectContainer: ContainerById = new ContainerById();
  images: any[] = [];
  autoImages: any[] = [];
  @ViewChild('galleria') galleria!: Galleria;
  @ViewChild('galleriaCont') galleriaCont!: Galleria;
  responsiveOptions: any[] = [
    {
      breakpoint: '1024px',
      numVisible: 5,
    },
    {
      breakpoint: '768px',
      numVisible: 3,
    },
    {
      breakpoint: '560px',
      numVisible: 1,
    },
  ];
  activeIndex: number = 0;
  activeIndex_auto :number= 0;
  showThumbnails: boolean = true;
  fullscreen: boolean = false;
  onFullScreenListener: any;
  selectAuto: AutoById = new AutoById();
  paymentmodel: Payment = new Payment();
  totaldebit = 0;
  totalcredit = 0;
  required = 0;
  paymentDetails: any;
  paymentType = 0;
  checkedAutos: number = 0;
  showClear: boolean = false;
  departurePortIdValidation = false;
  destinationIdValidation = false;
  archiveItem : any;
  deleteItem : any;
  deleteImagesItem : any;


  constructor(
    fb: FormBuilder,
    public containerService: ContainerService,
    private autoService: AutoService,
    private lookupService: LookupService,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
    private uploadService: UploadService,
    private containerImagesService: ContainerImagesService,
    private paymentService: PaymentService,
    private autoImageService:AutoImagesService,
    public translate: TranslateService,
    public loginService: LoginService,
    public resource: ResourcesService,
    //for compressing images
    private imageCompress: NgxImageCompressService,

    @Inject(DOCUMENT) public document: Document
  ) {
    super(loginService,translate, resource,document);
    this.searchForm = fb.group({
      containerNo: [],
      bookingNo: [],
      loadingFromDate: [],
      loadingToDate: [],
      loadPortId: [],
      destinationId: [],
      shippingLineId: [],
      clientId: [],
      arrivalFromDate: [],
      arrivalToDate: [],
    });

    this.ngAddUpdateForm = fb.group({
      id: [],
      creationUserId: [],
      creationDate: [],
      serialNumber: ['', Validators.required],
      bookNo: ['', [Validators.required]],
      departurePortId: ['', [Validators.required]],
      destinationId: ['', [Validators.required]],
      shippingCompanyId: ['', [Validators.required]],
      departureDate: ['', [Validators.required]],
      arrivalDate: ['', [Validators.required]],
      autoIds: ['',[Validators.required]]    
    });
  }

  ngOnInit(): void {
    this.LoadContainers(true, this.searchmodel);
    this.LoadClients();
    this.LoadLookups();
    this.LoadSideMenu();
    this.LoadAutos();
  }

  searchContainer() {
    this.showClear = true;
    this.searchmodel = Object.assign({}, this.searchForm.value);
    this.searchmodel.isSearch = true;
    this.LoadContainers(true, this.searchmodel);
  }

  clearSearchForm() {
    this.showClear = false;
    this.searchForm.reset();
    this.LoadContainers(true, new SearchContainer());
  }

  LoadContainers(showPinner = true, search: SearchContainer) {
    if (showPinner) {
      this.spinner.show();
    }

    this.containerService.getAllByUser(search).subscribe(
      (res) => {
        this.containers = res.data;
        this.spinner.hide();
      },
      (err) => {
        this.spinner.hide();
      }
    );
  }

  LoadClients() {
    this.autoService.GetClients().subscribe(
      (res) => {
        if (res && res.status == StatusType.Success) {
          this.clients = res.data;
        }
      },
      (err) => {}
    );
  }

  LoadAutos() {
    let search: SearchAuto = new SearchAuto();
    this.autoService.GetAllByUser(search).subscribe(res => {
      for (let i = 0 ; i < res.data.length ; i++) {
        this.autoList.push({label: (res.data[i].vinNo), value: res.data[i].id});
      }
    }, err => {
    });
  }

  LoadLookups() {
    this.lookupService.GetAllLookupValues().subscribe((res) => {
      this.lookup_loadPort = res.data.filter((x: any) => x.lookupId == 2);
      this.lookup_destination = res.data.filter((x: any) => x.lookupId == 5);
      this.lookup_shippingCompany = res.data.filter(
        (x: any) => x.lookupId == 6
      );
    });
  }

  LoadSideMenu() {
    let isArchive =
      <any>this.showArchive == true || this.showArchive == 1 ? 1 : 0;
      debugger;
    this.containerService.GetSideMenuByUser(isArchive).subscribe(
      (res) => {
        if (res) {
          this.sideMenuList = [];
          this.sideMenuList.push({
            label: 'All',
            labeltext: 'All',
            awaitingload: res.all.awaitingload,
            departured: res.all.departured,
            arrived: res.all.arrived,
          });
          for (let i = 0; i < res.filteredContainers.length; i++) {
            this.sideMenuList.push({
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

  onSideItemClick(item: any, status: any) {
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

  openImagePopup(id: number) {
    this.images = [];
    this.activeIndex = 0;
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

  openDialog(){
    this.showDialog = true;
    this.model = new Container();
    this.model.status = 1;
  }

  hideDialog(){
    this.showDialog = false;
    this.ngAddUpdateForm.reset();
    super.RemoveBlackScreen();
  }

  openDeleteAllDialog() {
    this.showDeleteAllDialog = true;
  }
  openArchiveAllDialog() {
    this.showArchiveAllDialog = true;
  }
  openArchiveDialog(item: any) {
    this.archiveItem = item;
    this.showArchiveDialog = true;
  }
  openDeleteDialog(item: any) {
    this.deleteItem = item;
    this.showDeleteDialog = true;
  }
  openDeleteImagesDialog(item: any) {
    this.deleteImagesItem = item;
    this.showDeleteImagesDialog = true;
  }

  openAutoPopup(id: number) {
    this.spinner.show();
    this.autoImages = [];
    this.activeIndex_auto = 0;
    this.autoService.GetAutoById(id).subscribe(
      (res) => {
        this.selectAuto = res.data;

        for (let i = 0; i < res.data.images.length; i++) {
          let toBeReplaced = '../assets/uploaded/';
          res.data.images[i].previewImageSrc =
            toBeReplaced + res.data.images[i].title;
          res.data.images[i].thumbnailImageSrc =
            toBeReplaced + res.data.images[i].title;
        }

        this.autoImages = res.data.images;
        this.spinner.hide();
      },
      (err) => {
        this.spinner.hide();
      }
    );

    this.GetPayment(id);
  }

  GetPayment(autoId: number) {
    this.paymentService.GetPayment(autoId).subscribe(
      (res) => {
        if (res && res.status == StatusType.Success && res.data != null) {
          this.totaldebit = res.data.debitAmount;
          this.totalcredit = res.data.creditAmount;
          this.paymentDetails = res.data.paymentDetails;
          this.required = this.totaldebit - this.totalcredit;
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
      (err) => {
        this.toastr.error('Error', '');
      }
    );
  }

  deleteAutoImage(image: any,autoId:number){
    if (image && image.id && image.id > 0) {
      this.autoImageService.DeleteAutoImage(image.id).subscribe(
        (res) => {
          if (res && res.status == StatusType.Success) {
            //this.getImages(image.autoid);
            this.translate.get(['ArchiveComponent.DeleteSuccessfully']).subscribe(res => {
              this.toastr.success('', res['ArchiveComponent.DeleteSuccessfully']);
            });
            this.getAutoImages(autoId);
            //super.RemoveBlackScreen();
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
  getAutoImages(autoId: number) {
    this.autoImageService.getImagesByAuto(autoId).subscribe(
      (response) => {
        this.spinner.hide();
        if (
          response &&
          response.status == StatusType.Success &&
          response.data
        ) {
          for (let i = 0; i < response.data.length; i++) {
            let toBeReplaced = 'assets/uploaded/';
            response.data[i].previewImageSrc =
              toBeReplaced + response.data[i].title;
            response.data[i].thumbnailImageSrc =
              toBeReplaced + response.data[i].title;
          }

          this.autoImages = response.data;
        }
      },
      (err) => {
        this.spinner.hide();
      }
    );
  }


  deleteAutoImages(id:number){
    this.autoService.DeleteImages(id).subscribe(
      (res) => {
        this.spinner.hide();
        if (res && res.status == StatusType.Success) {
          this.showDialog = false;
          this.ngAddUpdateForm.reset();
          this.translate.get(['ArchiveComponent.DeleteSuccessfully']).subscribe(res => {
            this.toastr.success('', res['ArchiveComponent.DeleteSuccessfully']);
          });
          //super.RemoveBlackScreen();
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


  editContainer(container: any)
  {
    this.model = Object.assign({}, container);
    this.model.isArchive = ( this.model.isArchive == 1) ? 1 : 0;
    this.showDialog = true;
  }

  DeleteAllContainers() {
    const selectedContainers = this.containers.filter((cont) => cont.checked);
    if (selectedContainers && selectedContainers.length > 0) {
      this.containerService.DeleteContainers(selectedContainers).subscribe(
        (res) => {
          if (res && res.status == StatusType.Success) {
            this.LoadContainers(true, this.searchmodel);
            this.showDeleteAllDialog = false;
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
    } else {
      this.translate.get(['ArchiveComponent.SelectAtLeastOneContainer']).subscribe(res => {
        this.toastr.warning('', res['ArchiveComponent.SelectAtLeastOneContainer']);
      });
    }
  }

  archiveAllContainers() {
    const selectedcontainers = this.containers.filter((cont) => cont.checked);
    if (selectedcontainers && selectedcontainers.length > 0) {
      this.containerService.ArchiveContainers(selectedcontainers).subscribe(
        (res) => {
          if (res && res.status == StatusType.Success) {
            this.LoadContainers(true, this.searchmodel);
            this.LoadSideMenu();
            this.showArchiveAllDialog = false;
            this.translate.get(['ArchiveComponent.ArchiveSuccessfully']).subscribe(res => {
              this.toastr.success('', res['ArchiveComponent.ArchiveSuccessfully']);
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

  archiveContainer() {
    this.containerService.ArchiveContainer(this.archiveItem.id).subscribe(
      (res) => {
        this.spinner.hide();
        if (res && res.status == StatusType.Success) {
          this.LoadContainers(false, this.searchmodel);
          this.LoadSideMenu();
          this.showArchiveDialog = false;
          this.translate.get(['ArchiveComponent.ArchiveSuccessfully']).subscribe(res => {
            this.toastr.success('', res['ArchiveComponent.ArchiveSuccessfully']);
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

  deleteContainer() {
    this.containerService.DeleteContainer(this.deleteItem.id).subscribe(
      (res) => {
        this.spinner.hide();
        if (res && res.status == StatusType.Success) {
          this.LoadContainers(false, this.searchmodel);
          this.LoadSideMenu();
          this.showDeleteDialog = false;
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

  deleteImages(){
    this.containerService.DeleteImages(this.deleteImagesItem.id).subscribe(
      (res) => {
        this.spinner.hide();
        if (res && res.status == StatusType.Success) {
          this.LoadContainers(false, this.searchmodel);
          this.showDeleteImagesDialog = false;
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
          }
        }
      },
      (err) => {
        this.spinner.hide();
      }
    );
  }

  saveContainer(){
    this.isSubmitted = true;
    this.validationCheck();
    if (this.ngAddUpdateForm.valid) {
      this.spinner.show();
      this.model = this.ngAddUpdateForm.value;
      this.containerService.SaveContainer(this.model).subscribe( (res) => {
          let emptyAttachments = this.files.length < 1;
          if (this.files.length < 1) {
            this.spinner.hide();
          }

          if (res && res.status == StatusType.Success) {
            this.LoadContainers(true, this.searchmodel);
            this.LoadSideMenu();
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
            let containerId = this.model.id < 1 || this.model.id == null || this.model.id == undefined ? res.data[0].id : this.model.id;
            this.UploadHandler(containerId);
            this.LoadContainers(true, this.searchmodel);
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

  // replace it with compress function
  /*onFileSelect(event: any) {
    this.files = event.files;
    if (this.files.length > 0) {
      this.formData = new FormData();
      this.formData.delete('files');
      Array.from(this.files).forEach((file) => {
        this.formData.append('files', file, file.name);
      });
    }
  }*/

    //Compressin images
  async onFileSelect(event: any) {
    this.files = event.files;

    if (this.files.length > 0) {
      this.spinner.show();      // Show loading indicator
      this.formData = new FormData();
      this.formData.delete('files');

      for (const file of this.files) {
        if (file.type.startsWith('image/')) {
          try {
            console.log(`Compressing image: ${file.name}`);
            const compressedFile = await this.compressImage(file);
            this.formData.append('files', compressedFile, file.name);
            console.log(`Image compressed successfully: ${file.name}`);
          } catch (error) {
            console.error(`Failed to compress image: ${file.name}`, error);
            // Append the original image if compression fails
            this.formData.append('files', file, file.name);
          }
        } else {
          console.log(`Non-image file detected: ${file.name}`);
          this.formData.append('files', file, file.name); // Non-image files are appended as-is
        }
      }
      this.spinner.hide();          // Hide loading indicator once compression is complete

    }
  }

//Compressin images
  async compressImage(file: File): Promise<Blob> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);

      reader.onload = () => {
        const base64Image = reader.result as string;

        this.imageCompress.compressFile(base64Image, -1, 50, 50).then((compressedBase64) => {
          const byteString = atob(compressedBase64.split(',')[1]);
          const arrayBuffer = new ArrayBuffer(byteString.length);
          const uint8Array = new Uint8Array(arrayBuffer);
          for (let i = 0; i < byteString.length; i++) {
            uint8Array[i] = byteString.charCodeAt(i);
          }
          const compressedBlob = new Blob([arrayBuffer], { type: file.type });
          resolve(compressedBlob);
        }).catch(error => reject(error));
      };

      reader.onerror = (error) => reject(error);
    });
  }


  onFileDelete(event: any) {
    var files = this.formData.getAll('files');
    this.formData.delete('files');
    files.forEach((value: any) => {
      if (value.name != event.file.name) {
        this.formData.append('files', value, value.name);
      }
    });
  }

  onUploadCancel() {
    this.files = [];
    this.formData = new FormData();
  }

  UploadHandler(containerId: number) {
    this.uploadService.uploadFile(this.formData, 0, 2 , containerId).subscribe(
      (res) => {
        this.spinner.hide();
        this.onUploadCancel();
      },
      (err) => {
        this.spinner.hide();
      }
    );
  }

  deleteImage(image: any,containerId: number) {
    if (image && image.id && image.id > 0) {
      this.containerImagesService.DeleteContainerImage(image.id).subscribe(
        (res) => {
          if (res && res.status == StatusType.Success) {
            this.translate.get(['ArchiveComponent.DeleteSuccessfully']).subscribe(res => {
              this.toastr.success('', res['ArchiveComponent.DeleteSuccessfully']);
            });
            this.getImages(containerId);
           // this.toastr.success('', 'Delete Successfully');
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

  getImages(containerId: number) {
    this.containerImagesService.getImagesByContainer(containerId).subscribe(
      (response) => {
        this.spinner.hide();
        if (
          response &&
          response.status == StatusType.Success &&
          response.data
        ) {
          for (let i = 0; i < response.data.length; i++) {
            let toBeReplaced = '../assets/uploaded/';
            response.data[i].previewImageSrc =
              toBeReplaced + response.data[i].title;
            response.data[i].thumbnailImageSrc =
              toBeReplaced + response.data[i].title;
          }

          this.images = response.data;
        }
      },
      (err) => {
        this.spinner.hide();
      }
    );
  }


  galleriaClass() {
    return `custom-galleria ${this.fullscreen ? 'fullscreen' : ''}`;
  }

  onThumbnailButtonClick() {
    this.showThumbnails = !this.showThumbnails;
  }

  fullScreenIcon() {
    return `pi ${
      this.fullscreen ? 'pi-window-minimize' : 'pi-window-maximize'
    }`;
  }

  toggleFullScreen() {

    if (this.fullscreen) {
      // this.closePreviewFullScreen();
    } else {
      this.openPreviewFullScreen();
    }
  }

  toggleContainerFullScreen() {

    if (this.fullscreen) {
      // this.closePreviewFullScreen();
    } else {
      this.openPreviewFullScreenContainer();
    }
  }

  onFullScreenChange() {
    this.fullscreen = !this.fullscreen;
  }

  openPreviewFullScreen() {
    let elem = this.galleria.element.nativeElement.querySelector('.p-galleria');
    if (elem.requestFullscreen) {
      elem.requestFullscreen();
    } else if (elem['mozRequestFullScreen']) {
      /* Firefox */
      elem['mozRequestFullScreen']();
    } else if (elem['webkitRequestFullscreen']) {
      /* Chrome, Safari & Opera */
      elem['webkitRequestFullscreen']();
    } else if (elem['msRequestFullscreen']) {
      /* IE/Edge */
      elem['msRequestFullscreen']();
    }
  }

  openPreviewFullScreenContainer(){
    let elem = this.galleriaCont.element.nativeElement.querySelector('.p-galleria');
    if (elem.requestFullscreen) {
      elem.requestFullscreen();
    } else if (elem['mozRequestFullScreen']) {
      /* Firefox */
      elem['mozRequestFullScreen']();
    } else if (elem['webkitRequestFullscreen']) {
      /* Chrome, Safari & Opera */
      elem['webkitRequestFullscreen']();
    } else if (elem['msRequestFullscreen']) {
      /* IE/Edge */
      elem['msRequestFullscreen']();
    }
  }

  onCheckboxChange(event: any) {
    if (event.target.checked) {
      this.checkedAutos = this.checkedAutos + 1;
    } else {
      this.checkedAutos = this.checkedAutos - 1;
    }
  }

  validationCheck() {
    if (this.ngAddUpdateForm.value.departurePortId == '') {
      this.departurePortIdValidation = true;
    }

    if (this.ngAddUpdateForm.value.destinationId == '') {
      this.destinationIdValidation = true;
    }
  }

  onChangeDeparturePort(event: any) {
    if ((this.model.departurePortId == null) || (this.model.departurePortId == 0)){
      this.departurePortIdValidation = true;
    } else this.departurePortIdValidation = false;
  }

  onChangeDestination(event: any) {
    if ((this.model.destinationId == null) || (this.model.destinationId == 0)){
      this.destinationIdValidation = true;
    } else this.destinationIdValidation = false;
  }


  
  DownlaodAll(){
    const zip = new JSZip();
    const name = this.selectContainer.serialNumber + '.zip';  
    
    let count = 0;

    this.images.forEach((url) => {
      const filename = url['title'];

    jSZipUtils.getBinaryContent(url['previewImageSrc'], (err:any, data:any) => {
      if (err) {
        throw err;
      }

      zip.file(filename, data, {binary: true});
      count++;

      if (count === this.images.length) {
        zip.generateAsync({type: 'blob'}).then((content) => {
          const objectUrl: string = URL.createObjectURL(content);
          const link: any = document.createElement('a');

          link.download = name;
          link.href = objectUrl;
          link.click();
        });
      }
    });
  });
}


  
DownlaodAllAutoImages(){
  const zip = new JSZip();
  const name = this.selectAuto.vinNo + '.zip';  
  
  let count = 0;

  this.autoImages.forEach((url) => {
    const filename = url['title'];

  jSZipUtils.getBinaryContent(url['previewImageSrc'], (err:any, data:any) => {
    if (err) {
      throw err;
    }

    zip.file(filename, data, {binary: true});
    count++;

    if (count === this.autoImages.length) {
      zip.generateAsync({type: 'blob'}).then((content) => {
        const objectUrl: string = URL.createObjectURL(content);
        const link: any = document.createElement('a');

        link.download = name;
        link.href = objectUrl;
        link.click();
      });
    }
  });
});
}


}
