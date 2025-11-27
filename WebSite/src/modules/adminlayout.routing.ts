import { Routes } from '@angular/router';
import { AuthService } from '../services/index';
import {
  DashboardComponent,
  AutosComponent,
  ClientsComponent,
  ContainersComponent,
  SelectionsComponent,
  ResourcesComponent,
  PaymentComponent,
  BalanceComponent,
  ArchiveComponent
} from '../app/index';

export const AdminLayoutRoutes: Routes = [
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthService],
  },
  {
    path: 'autos',
    component: AutosComponent,
    canActivate: [AuthService],
  },
  {
    path: 'clients',
    component: ClientsComponent,
    canActivate: [AuthService],
  },
  {
    path: 'containers',
    component: ContainersComponent,
    canActivate: [AuthService],
  },
  {
    path: 'selections',
    component: SelectionsComponent,
    canActivate: [AuthService],
  },
  {
    path: 'resources',
    component: ResourcesComponent,
    canActivate: [AuthService],
  },
  {
    path: 'payment',
    component: PaymentComponent,
    canActivate: [AuthService],
  },
  {
    path: 'balance',
    component: BalanceComponent,
    canActivate: [AuthService],
  },
  {
    path: 'archive',
    component: ArchiveComponent,
    canActivate: [AuthService],
  }
];
