import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import {
  DashboardComponent,
  AutosComponent,
  ClientsComponent,
  ContainersComponent,
  SelectionsComponent,
  ResourcesComponent,
  PaymentComponent,
  BalanceComponent,
  BaseComponent,
  ArchiveComponent,
} from '../app/index';
import { AdminLayoutRoutes } from './adminlayout.routing';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClient } from '@angular/common/http';
import { NgxPaginationModule } from 'ngx-pagination';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { FileUploadModule } from 'primeng/fileupload';
import { GalleriaModule } from 'primeng/galleria';
import { MultiSelectModule } from 'primeng/multiselect';
import { DropdownModule } from 'primeng/dropdown';

import {
  NGTranslateHelperService,
  ClientService,
  AutoService,
  LookupService,
  UploadService,
  PaymentService,
  AutoImagesService,
  ContainerImagesService,
} from '../services/index';

export function httpTranslateLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

@NgModule({
  imports: [
    RouterModule.forChild(AdminLayoutRoutes),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: httpTranslateLoaderFactory,
        deps: [HttpClient],
      },
    }),
    NgxPaginationModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule.withConfig({ warnOnNgModelWithFormControl: 'never' }),
    FileUploadModule,
    GalleriaModule,
    MultiSelectModule,
    DropdownModule,
  ],
  declarations: [
    DashboardComponent,
    AutosComponent,
    ClientsComponent,
    ContainersComponent,
    SelectionsComponent,
    ResourcesComponent,
    PaymentComponent,
    BalanceComponent,
    BaseComponent,
    ArchiveComponent,
  ],
  exports: [],
  providers: [
    NGTranslateHelperService,
    DatePipe,
    ClientService,
    AutoService,
    LookupService,
    UploadService,
    AutoImagesService,
    ContainerImagesService,
  ],
})
export class AdminLayoutModule {}
