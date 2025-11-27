export class BaseModel {
  id: number = 0;
  creationUserId?: number;
  creationDate?: Date;
  modificationUserId?: number;
  modificationDate?: Date;
}

export class ResponseResult<T> {
  data: T;
  status: StatusType = 0;
  errors: string[] = [];
  constructor(_data: T) {
    this.data = _data;
  }
}

export class LoginResonse {
  id: number = 0;
  name: string = '';
  type: UserType = 0;
  token: string = '';
}

export enum StatusType {
  Failed = 0,
  Success = 1,
}

export enum UserType {
  Admin = 1,
  SuperiorAdmin = 2,
  Client = 3,
}

export enum ComplaintStatus {
  Pending = 1,
  Resolved = 2,
  Dismissed = 3,
}

export enum ComplaintType {
  Complaint = 1,
  GeneralQuery = 2,
}

export enum PaymentType {
  Credit = 1,
  Debit = 2,
}

export enum CarStatus {
  BoughtNew = 1,
  AwaitingLoad = 2,
  Loaded = 3,
}

export class LookupValue {
  id: number = 0;
  name: string = '';
  code: string = '';
  lookupId: number = 0;
}

export enum Role {
  Client = 'Client',
  Admin = 'Admin',
  SuperiorAdmin = 'Superior Admin',
}

export class ResourceParamDto {
  id: number = 0;
  key: string = '';
  english: string = '';
  arabic: string = '';
}

export class User extends BaseModel {
  name: string = '';
  userName: string = '';
  address: string = '';
  mobile: string = '';
  password: string = '';
  actualPass: string = '';
  company: string = '';
  type: UserType = 0;
  groupId: number = 0;
}

export class Auto extends BaseModel {
  name: string = '';
  brandId: number = 0;
  description: string = '';
  model: string = '';
  vinNo: string = '';
  type: number = 0;
  colorId: number = 0;
  engine: string = '';
  lot: string = '';
  carName: string = '';
  buyingAccountId?: number;
  remainingPayment?: number;
  buyerId?: number;
  loadPortId: number = 0;
  auctionId: number = 0;
  cityId: number = 0;
  destinationId: number = 0;
  containerId?: number | null;
  buyDate: Date = new Date();
  arrivalDate?: Date;
  carStatus: number = 0;
  paperStatus?: number;
  displayStatus: number = 0;
  isArchive: number = 0;
}

export class SearchAuto {
  isSearch: boolean = false;
  vinNumber: string = '';
  lotNumber: string = '';
  client: number = 0;
  auction: number = 0;
  buyAccount: number = 0;
  container: string = '';
  loadPort: number = 0;
  destination: number = 0;
  city: number = 0;
  carId: number = 0;
  deliveryFromDate?: Date;
  deliveryToDate?: Date;
  Status: number = 0;
  purchaseFromDate?: Date;
  purchaseToDate?: Date;
}

export class AutoById extends BaseModel {
  id: number = 0;
  name: string = '';
  brandId: number = 0;
  description: string = '';
  model: string = '';
  vinNo: string = '';
  type: number = 0;
  color: string = '';
  engine: string = '';
  lot: string = '';
  carName: string = '';
  buyingAccountId?: number;
  buyAccountName: string = '';
  remainingPayment?: number;
  buyerId?: number = 0;
  buyerName: string = '';
  loadPortId: number = 0;
  loadPortName: string = '';
  auctionId: number = 0;
  auctionName: string = '';
  cityId: number = 0;
  cityName: string = '';
  destinationId: number = 0;
  destinationName: string = '';
  containerId?: number;
  buyDate: Date = new Date();
  buyDateStr: string = '';
  containerArrivalDateStr: string = '';
  carStatus: number = 0;
  paperStatus?: number;
  displayStatus: number = 0;
  isArchive: number = 0;
  carStatusStr: string = '';
  containerSerial: string = '';
  bookNo: string = '';
  departureDateStr: string = '';
  shippingCompany: string = '';
}

export class Payment extends BaseModel {
  clientName: string = '';

  paymentId: number = 0;
  paymentType: number = 0;
  autoId: number = 0;
  categoryId: number = 0;
  amount: number = 0;
  payDate?: Date;
  paymentMethod: number = 0;
  notes: string = '';
}

export class PaymentDetails {
  id: number = 0;
  cashType: number = 0;
  buyType: number = 0;
  notes: string = '';
  payDate?: Date;
  amount: number = 0;
  paymentId: number = 0;
}

export class SearchContainer {
  isSearch: boolean = false;
  StatusId: number = 0;
  containerNo: string = '';
  bookingNo: string = '';
  loadingFromDate?: Date;
  loadingToDate?: Date;
  loadPortId: number = 0;
  destinationId: number = 0;
  shippingLineId: number = 0;
  clientId: number = 0;
  arrivalFromDate?: Date;
  arrivalToDate?: Date;
}

export class Container extends BaseModel {
  name: string = '';
  serialNumber: string = '';
  departurePortId: number = 0;
  destinationId: number = 0;
  bookNo: string = '';
  departureDate!: Date;
  arrivalDate!: Date;
  shippingCompanyId?: number;
  status?: number;
  isArchive?: number;
  autoIds: number[] = [];
}

export class ContainerById {
  id: number = 0;
  serialNumber: string = '';
  bookNo: string = '';
  departureDateStr: string = '';
  arrivalDateStr: string = '';
  destinationName: string = '';
  containerStatusStr: string = '';
  departurePortName: string = '';
  shippingCompanyName: string = '';
  autoVinNos: any[] = [];
}

export class SearchPayment {
  isSearch: boolean = false;
  autoId: number = 0;
  vinNo: string = '';
  purchaseDate!: Date;
  clientId: number = 0;
}

export class SearchBalance {
  isSearch: boolean = false;
  autoId: number = 0;
  vinNo: string = '';
  purchaseDate!: Date;
  clientId: number = 0;
}
