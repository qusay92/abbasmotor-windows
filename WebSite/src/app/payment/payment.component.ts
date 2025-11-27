import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import {
  AutoById,
  Payment,
  SearchPayment,
  StatusType,
} from 'src/models/models';
import {
  AutoService,
  LoginService,
  LookupService,
  PaymentService,
  ResourcesService,
} from 'src/services';
import { ScriptService } from 'src/services/script.service';
import { DatePipe, DOCUMENT } from '@angular/common';
import { BaseComponent } from '../base/base.component';
import { TranslateService } from '@ngx-translate/core';
import { Galleria } from 'primeng/galleria';

declare let pdfMake: any;

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css'],
})
export class PaymentComponent extends BaseComponent implements OnInit {
  searchForm: FormGroup;
  carNames: any;
  clientNames: any;
  searchmodel: SearchPayment = new SearchPayment();
  paymentDetails: any[] = [];
  pagepayment: any;
  totaldebit = 0;
  totalcredit = 0;
  required = 0;
  lookup_paymentMethod: any;
  paymentForm: FormGroup;
  isSubmitted: boolean = false;
  paymentmodel: Payment = new Payment();
  showInvoiceDialog: boolean = false;
  showDeleteAllDialog: boolean = false;
  deletepayment: any[] = [];
  deletedList: any[] = [];
  selectAuto: AutoById = new AutoById();
  checkedPayments: number = 0;
  showClear: boolean = false;
  showPrint: boolean = false;

  paymentTypes = [
    { name: 'Purchase Price', id: 1 },
    { name: 'Sea Freight', id: 2 },
    { name: 'Internal Shipping', id: 3 },
    { name: 'Fees', id: 4 },
    { name: 'Purchase Transfer', id: 5 },
    { name: 'Customs Clearance', id: 7 },
    { name: 'Storage Fees', id: 8 },
    { name: 'Other', id: 9 },
  ];

  vinNumberValidation = false;
  categoryValidation = false;
  typeValidation = false;
  paymentMethodValidation = false;

  images: any[] = [];
  @ViewChild('galleria') galleria!: Galleria;
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
  showThumbnails: boolean = true;
  fullscreen: boolean = false;
  onFullScreenListener: any;
  showbtn: boolean = false;

  constructor(
    fb: FormBuilder,
    private autoService: AutoService,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
    private paymentService: PaymentService,
    private lookupService: LookupService,
    private scriptService: ScriptService,
    private datePipe: DatePipe,
    public translate: TranslateService,
    public loginService: LoginService,
    public resource: ResourcesService,
    @Inject(DOCUMENT) public document: Document
  ) {
    super(loginService, translate, resource, document);
    this.scriptService.load('pdfMake', 'vfsFonts');

    this.searchForm = fb.group({
      autoId: [],
      vinNo: [],
      purchaseDate: [],
      clientId: [],
    });

    this.paymentForm = fb.group({
      id: [],
      paymentId: [],
      autoId: ['', Validators.required],
      categoryId: ['', Validators.required],
      paymentType: ['', Validators.required],
      amount: ['', Validators.required],
      payDate: ['', Validators.required],
      paymentMethod: ['', Validators.required],
      notes: [],
    });
  }

  ngOnInit(): void {
    super.RemoveBlackScreen();
    this.LoadCarName();
    this.LoadClientName();
    this.LoadPayments(true, this.searchmodel);
    this.LoadLookups();
  }

  LoadCarName() {
    this.autoService.GetCarName().subscribe(
      (res) => {
        if (res && res.status == StatusType.Success) {
          this.carNames = res.data;
        }
      },
      (err) => {}
    );
  }

  LoadClientName() {
    this.autoService.GetClientNameByUser().subscribe(
      (res) => {
        if (res && res.status == StatusType.Success) {
          this.clientNames = res.data;
        }
      },
      (err) => {}
    );
  }

  onChangeVinNumber(event: any) {
    if (this.paymentmodel.autoId == null || this.paymentmodel.autoId == 0) {
      this.vinNumberValidation = true;
    } else this.vinNumberValidation = false;
  }

  onChangeCategory(event: any) {
    if (event.target.value == '') {
      this.categoryValidation = true;
    } else this.categoryValidation = false;
  }

  onChangePaymentType(event: any) {
    if (event.target.value == '') {
      this.typeValidation = true;
    } else this.typeValidation = false;
  }

  onChangePaymentMethod(event: any) {
    if (event.target.value == '') {
      this.paymentMethodValidation = true;
    } else this.paymentMethodValidation = false;
  }

  searchPayment() {
    this.showClear = true;
    this.searchmodel = Object.assign({}, this.searchForm.value);
    this.searchmodel.isSearch = true;

    if (this.searchmodel.autoId > 0) this.showPrint = true;

    this.LoadPayments(true, this.searchmodel);
  }

  clearSearchForm() {
    this.showClear = false;
    this.searchForm.reset();
    this.showPrint = false;
    this.LoadPayments(false, new SearchPayment());
  }

  LoadLookups() {
    this.lookupService.GetAllLookupValues().subscribe((res) => {
      this.lookup_paymentMethod = res.data.filter((x: any) => x.lookupId == 10);
    });
  }

  LoadPayments(showPinner = true, search: SearchPayment) {
    if (showPinner) {
      this.spinner.show();
    }

    this.paymentService.GetPayments(search).subscribe(
      (res) => {
        if (res && res.status == StatusType.Success && res.data != null) {
          this.totaldebit = res.data.debitAmount;
          this.totalcredit = res.data.creditAmount;
          this.paymentDetails = res.data.paymentDetails;
          this.required = this.totaldebit - this.totalcredit;

          if (this.searchmodel.autoId > 0) {
            this.selectAuto.buyerName = res.data.paymentDetails[0]?.client;
            this.selectAuto.vinNo = res.data.paymentDetails[0]?.vinNo;
            this.selectAuto.name = res.data.paymentDetails[0]?.carName;
          }
          this.spinner.hide();
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

  openInvoice() {
    this.paymentForm.reset();
    this.showInvoiceDialog = true;
    this.paymentmodel = new Payment();
    this.paymentForm.controls['autoId'].enable();
    this.paymentForm.controls['paymentType'].enable();
  }

  iserror: Boolean = false;
  validation() {
    if (this.paymentForm.value.autoId == '') {
      this.vinNumberValidation = true;
      this.iserror = false;
    }

    if (this.paymentForm.value.categoryId == '') {
      this.categoryValidation = true;
      this.iserror = false;
    }

    if (this.paymentForm.value.paymentType == '') {
      this.typeValidation = true;
      this.iserror = false;
    }

    if (this.paymentForm.value.paymentMethod == '') {
      this.paymentMethodValidation = true;
      this.iserror = false;
    }
  }

  saveInvoice() {
    this.isSubmitted = true;
    this.validation();

    if (this.iserror) {
      return;
    }

    if (this.paymentForm.valid) {
      this.paymentForm.controls['autoId'].enable();
      this.paymentForm.controls['paymentType'].enable();
      this.paymentmodel = this.paymentForm.value;
      this.paymentmodel.id =
        this.paymentmodel.id != null ? this.paymentmodel.id : 0;

      this.paymentService.SavePayment(this.paymentmodel).subscribe(
        (res) => {
          this.spinner.hide();
          if (res && res.status == StatusType.Success) {
            this.showInvoiceDialog = false;
            this.paymentForm.reset();
            this.translate
              .get(['ArchiveComponent.SavedSuccessfully'])
              .subscribe((res) => {
                this.toastr.success(
                  '',
                  res['ArchiveComponent.SavedSuccessfully']
                );
              });
            super.RemoveBlackScreen();
            this.LoadPayments(false, this.searchmodel);
          } else if (
            res.status == StatusType.Failed &&
            res.errors != null &&
            res.errors.length > 0
          ) {
            for (let i = 0; i < res.errors.length; i++) {
              this.toastr.error('', res.errors[i]);
            }
            this.paymentForm.controls['autoId'].disable();
            this.paymentForm.controls['paymentType'].disable();
          }
        },
        (err) => {
          this.spinner.hide();
          this.paymentForm.controls['autoId'].disable();
          this.paymentForm.controls['paymentType'].disable();
        }
      );

      this.spinner.hide();
    }
  }

  cancelInvoice() {
    this.paymentForm.reset();
    this.showInvoiceDialog = false;
  }

  EditInvoice(payment: any) {
    this.paymentmodel = Object.assign({}, payment);
    this.paymentmodel.categoryId = payment.buyType;
    this.paymentmodel.paymentType = payment.payment.paymentType;
    this.paymentmodel.paymentMethod = payment.cashType;
    this.paymentmodel.payDate = payment.payDate;
    this.paymentForm.controls['autoId'].disable();
    this.paymentmodel.autoId = payment.autoId;
    this.paymentForm.controls['paymentType'].disable();
    this.showInvoiceDialog = true;
  }

  openDeleteAllDialog() {
    this.showDeleteAllDialog = true;
  }
  openDeleteDialog(item: any) {
    this.deletepayment.push(item);
    this.showDeleteAllDialog = true;
  }

  DeleteAllPayments() {
    const selectedpaymentDetails = this.paymentDetails.filter(
      (cont) => cont.checked
    );

    if (
      (selectedpaymentDetails && selectedpaymentDetails.length > 0) ||
      this.deletepayment.length > 0
    ) {
      if (selectedpaymentDetails.length > 0)
        this.deletedList = selectedpaymentDetails;
      if (this.deletepayment.length > 0) this.deletedList = this.deletepayment;

      this.paymentService.Delete(this.deletedList).subscribe(
        (res) => {
          if (res && res.status == StatusType.Success) {
            this.LoadPayments(true, this.searchmodel);
            this.showDeleteAllDialog = false;
            this.translate
              .get(['ArchiveComponent.DeleteSuccessfully'])
              .subscribe((res) => {
                this.toastr.success(
                  '',
                  res['ArchiveComponent.DeleteSuccessfully']
                );
              });
            this.checkedPayments = 0;
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
          this.deletepayment = [];
        },
        (err) => {
          this.spinner.hide();
        }
      );
    } else {
      this.translate
        .get(['ArchiveComponent.SelectAtLeastOnPayment'])
        .subscribe((res) => {
          this.toastr.warning(
            '',
            res['ArchiveComponent.SelectAtLeastOnPayment']
          );
        });
    }
  }

  openImagePopup(autoId: number) {
    this.images = [];
    this.spinner.show();
    this.autoService.GetAutoById(autoId).subscribe(
      (res) => {
        this.selectAuto = res.data;
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

  onCheckboxChange(event: any) {
    if (event.target.checked) {
      this.checkedPayments = this.checkedPayments + 1;
    } else {
      this.checkedPayments = this.checkedPayments - 1;
    }
  }

  async print() {
    const documentDefinition = await this.getDocumentDefinition();
    pdfMake.createPdf(documentDefinition).open();
  }

  async getDocumentDefinition() {
    return {
      content: [
        {
          text: '',
          bold: true,
          fontSize: 20,
          alignment: 'center',
          margin: [0, 0, 0, 20],
        },
        {
          image: await this.getBase64ImageFromURL(
            '../../assets/images/headerphoto.jpg'
          ),
          fit: [600, 500],
          absolutePosition: { x: 0, y: 0 },
        },
        {
          absolutePosition: { x: 20, y: 200 },
          columns: [
            [
              {
                text: 'To : ' + this.selectAuto.buyerName,
              },
              {
                text: 'Vin No : ' + this.selectAuto.vinNo,
              },
              {
                text: 'Car Name : ' + (this.selectAuto.name ? this.selectAuto.name : 'N/A'),
              },
              {
                text:
                  'Date : ' + this.datePipe.transform(new Date(), 'dd/MM/yyyy'),
                  //style: 'rightdate',
              },
              {
                text: 'Total Amount : ' + this.totaldebit,
                style: 'righttotal',
              },
              {
                text: 'Paid Amount : ' + this.totalcredit,
                style: 'rightdate',
              },
              {
                text: 'Required Amount : ' + this.required,
                style: 'rightdate',
              },
            ],
          ],
        },
        this.tabless(),
        // {
        //   // absolutePosition: { x: 50, y: 578 },
        //   style: 'footer',
        //   columns: [
        //     [
        //       {
        //         text: 'Total Amount : ' + this.totaldebit + '        ' + 'Paid Amount : ' + this.totalcredit + '        ' + 'Required Amount : ' + this.required,
        //       }
        //     ],
        //   ],
        // },
        {
          image: await this.getBase64ImageFromURL(
            '../../assets/images/footerphoto.jpg'
          ),
          fit: [612, 700],
          absolutePosition: { x: 0, y: 592 },
        },
      ],
      styles: {
        header: {
          fontSize: 18,
          bold: true,
          margin: [20, 120, 0, 10],
          decoration: 'underline',
        },
        name: {
          fontSize: 16,
          bold: true,
        },
        jobTitle: {
          fontSize: 14,
          bold: true,
          italics: true,
        },
        sign: {
          margin: [0, 50, 0, 10],
          alignment: 'right',
          italics: true,
        },
        tableHeader: {
          bold: true,
          background: '#17214b',
          color: '#fff',
        },
        righttotal: {
          margin: [400, -45, 0, 0],
        },
        rightdate: {
          margin: [400, 0, 0, 0],
        },
        table: {
          margin: [0, 200, 0, 0],
        },
        footer: {
          margin: [50, 100, 0, 0],
        },
      },
    };
  }

  getBase64ImageFromURL(url: any) {
    return new Promise((resolve, reject) => {
      var img = new Image();
      img.setAttribute('crossOrigin', 'anonymous');

      img.onload = () => {
        var canvas = document.createElement('canvas');
        canvas.width = img.width;
        canvas.height = img.height;

        var ctx = canvas.getContext('2d')!;
        ctx.drawImage(img, 0, 0);

        var dataURL = canvas.toDataURL('image/png');

        resolve(dataURL);
      };

      img.onerror = (error) => {
        reject(error);
      };

      img.src = url;
    });
  }

  tabless() {
    return {
      style: 'table',
      table: {
        widths: ['*', '*', '*', '*', '*'],
        headerRows: 1,
        body: this.getPaymentDetailsAsTable(),
      },
    };
  }
  getPaymentDetailsAsTable() {
    var body = [];
    let columns = [
      'buyTypeStr',
      'cashTypeStr',
      'payDate',
      'paymentType',
      'amount',
    ];
    let columnsDisplayed = [
      'Type',
      'Payment Method',
      'Date',
      'Payment Type',
      'Amount',
    ];
    body.push(columnsDisplayed);

    this.paymentDetails.forEach((row: any) => {
      var dataRow: any = [];
      columns.forEach((column) => {
        if (column == 'paymentType') {
          if (row[column] == 1) dataRow.push('Credit');
          if (row[column] == 2) dataRow.push('Depit');
        } else if (column == 'payDate') {
          dataRow.push(this.datePipe.transform(row[column], 'dd/MM/yyyy'));
        } else if (column == 'buyTypeStr') {
          switch (row[column]) {
            case 'purchaseprice':
              dataRow.push('Purchase price');
              break;
            case 'seafreight':
              dataRow.push('Sea freight');
              break;
            case 'innerfreight':
              dataRow.push('Inner freight');
              break;
            case 'fees':
              dataRow.push('Fees');
              break;
            case 'purchaseorder':
              dataRow.push('Purchase order');
              break;
            case 'customsclearance':
              dataRow.push('Customs clearance');
              break;
            case 'storagefees':
              dataRow.push('Storage fees');
              break;
            case 'other':
              dataRow.push('Other');
              break;
            default:
              dataRow.push(row[column]);
              break;
          }
        } else {
          dataRow.push(row[column]);
        }
      });

      body.push(dataRow);
    });

    return body;
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
}
