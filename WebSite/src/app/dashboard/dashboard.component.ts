import { Component, Inject, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { SearchAuto, SearchBalance, SearchContainer, StatusType } from 'src/models/models';
import { AutoService, ClientService, ContainerService, LoginService, ResourcesService } from 'src/services';
import { BaseComponent } from '../base/base.component';
import { Chart } from 'chart.js';
import { DOCUMENT } from '@angular/common';
//import Chart from 'chart.js';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent extends BaseComponent implements OnInit {

  
  public canvas : any;
  public ctx: any;
  public chartColor: any;
  public chartEmail: any;
  public chartHours: any;
  currentYear: number = new Date().getFullYear();
  carsData: any = [];
  containersData: any = [];
  totalCars = 0;
  totalArrivedCars = 0;
  totalSeaCars = 0;
  totalBoughtNewCars = 0;
  archivedAuto = 0;
  totalSeaFreight = 0;
  totalStorageFees = 0;
  totalOther = 0;
  totalFees = 0;
  totalCharge = 0;
  clientBalance: any;

  constructor(public autoService: AutoService,
    public clientService: ClientService,
    public containerService: ContainerService,
    public translate: TranslateService,
    private spinner: NgxSpinnerService,
    loginService: LoginService,
    resource: ResourcesService,
    @Inject(DOCUMENT) public document: Document) { 
      super(loginService,translate, resource,document);
    }

  ngOnInit(): void {
    this.loadCarsData();
    this.loadClientData();
    this.loadContainersData();
    this.LoadClientBalances();
  }

  loadCarsData() {   
    this.spinner.show();
    this.autoService.GetAllByUser(new SearchAuto()).subscribe(res => {
      if (res && res.data && res.data.length > 0) {
        this.carsData = res.data;
        this.totalCars = res.data.length;
        this.totalBoughtNewCars = res.data.filter((x: any) => x.carStatusStr == 'Bought New').length;
        this.totalSeaCars = res.data.filter((x: any) => x.carStatusStr == 'loaded').length;
        this.totalArrivedCars = res.data.filter((x: any) => x.carStatusStr == 'arrived').length;
        this.archivedAuto = res.data[0].archivedAuto;
      }
      this.draw();
    }, err => {
    });
  }


  loadClientData() { 
    this.clientService.GetClientsInfo(new SearchBalance()).subscribe(res => {
      if (res && res.status == StatusType.Success) {
        if (this.hasPermission) {
          this.totalSeaFreight = res.data.summation.seaFreight;
          this.totalStorageFees = res.data.summation.storageFees;
          this.totalOther = res.data.summation.other;
          this.totalFees = res.data.summation.fees;
          this.totalCharge = this.totalSeaFreight+this.totalStorageFees+this.totalOther+this.totalFees;
        } else {
          const userId = localStorage.getItem('userId');
          let currentUserId = parseInt(userId!);
          this.totalSeaFreight = res.data.autoDetails.filter((x: any) => x.id == currentUserId)[0].seaFreight;
          this.totalStorageFees = res.data.autoDetails.filter((x: any) => x.id == currentUserId)[0].storageFees;
          this.totalOther = res.data.autoDetails.filter((x: any) => x.id == currentUserId)[0].other;
          this.totalFees = res.data.autoDetails.filter((x: any) => x.id == currentUserId)[0].fees;
          this.totalCharge = this.totalSeaFreight+this.totalStorageFees+this.totalOther+this.totalFees;
        }
      }
    }, err => {
    });
  }

  LoadClientBalances() {
    this.clientService.GetClientsBalance('').subscribe(
      (res) => {
        if (res && res.status == StatusType.Success && res.data != null) {
          this.clientBalance = res.data.autoFooter;
        } else if (
          res.status == StatusType.Failed &&
          res.errors != null &&
          res.errors.length > 0
        ) {
          for (let i = 0; i < res.errors.length; i++) {
          }
        }
      },
      (err) => {
      }
    );

  }

  loadContainersData() {    
    this.containerService.getAllByUser(new SearchContainer()).subscribe(res => {
      this.containersData = res.data;
    }, err => {
    });
  }

  totalCarsStatisitcs() {
    let currentYearCars = this.carsData.filter((x: any) => new Date(x.creationDate).getFullYear() == this.currentYear);
    let data = [];

    let jan = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 0).length;
    let feb = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 1).length;
    let mar = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 2).length;
    let apr = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 3).length;
    let may = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 4).length;
    let jun = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 5).length;
    let jul = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 6).length;
    let aug = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 7).length;
    let sep = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 8).length;
    let oct = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 9).length;
    let nov = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 10).length;
    let dec = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 11).length;
    
    data = [jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec];
    return data;
  }

  totalArrivedStatisitcs() {
    let currentYearCars = this.carsData.filter((x: any) => x.carStatusStr == 'arrived' && new Date(x.creationDate).getFullYear() == this.currentYear);
    let data = [];

    let jan = currentYearCars.filter((x: any) => new Date(x.arrivalDate).getMonth() == 0).length;
    let feb = currentYearCars.filter((x: any) => new Date(x.arrivalDate).getMonth() == 1).length;
    let mar = currentYearCars.filter((x: any) => new Date(x.arrivalDate).getMonth() == 2).length;
    let apr = currentYearCars.filter((x: any) => new Date(x.arrivalDate).getMonth() == 3).length;
    let may = currentYearCars.filter((x: any) => new Date(x.arrivalDate).getMonth() == 4).length;
    let jun = currentYearCars.filter((x: any) => new Date(x.arrivalDate).getMonth() == 5).length;
    let jul = currentYearCars.filter((x: any) => new Date(x.arrivalDate).getMonth() == 6).length;
    let aug = currentYearCars.filter((x: any) => new Date(x.arrivalDate).getMonth() == 7).length;
    let sep = currentYearCars.filter((x: any) => new Date(x.arrivalDate).getMonth() == 8).length;
    let oct = currentYearCars.filter((x: any) => new Date(x.arrivalDate).getMonth() == 9).length;
    let nov = currentYearCars.filter((x: any) => new Date(x.arrivalDate).getMonth() == 10).length;
    let dec = currentYearCars.filter((x: any) => new Date(x.arrivalDate).getMonth() == 11).length;
    
    data = [jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec];
    return data;
  }

  totalBoughtNewStatisitcs() {
    let currentYearCars = this.carsData.filter((x: any) => x.carStatusStr == 'Bought New' && new Date(x.creationDate).getFullYear() == this.currentYear);
    let data = [];

    let jan = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 0).length;
    let feb = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 1).length;
    let mar = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 2).length;
    let apr = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 3).length;
    let may = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 4).length;
    let jun = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 5).length;
    let jul = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 6).length;
    let aug = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 7).length;
    let sep = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 8).length;
    let oct = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 9).length;
    let nov = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 10).length;
    let dec = currentYearCars.filter((x: any) => new Date(x.creationDate).getMonth() == 11).length;
    
    data = [jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec];
    return data;
  }

  totalLoadedStatisitcs() {
    let currentYearCars = this.carsData.filter((x: any) => x.carStatusStr == 'loaded' && new Date(x.creationDate).getFullYear() == this.currentYear);
    let data = [];

    let jan = currentYearCars.filter((x: any) => new Date(x.departureDate).getMonth() == 0).length;
    let feb = currentYearCars.filter((x: any) => new Date(x.departureDate).getMonth() == 1).length;
    let mar = currentYearCars.filter((x: any) => new Date(x.departureDate).getMonth() == 2).length;
    let apr = currentYearCars.filter((x: any) => new Date(x.departureDate).getMonth() == 3).length;
    let may = currentYearCars.filter((x: any) => new Date(x.departureDate).getMonth() == 4).length;
    let jun = currentYearCars.filter((x: any) => new Date(x.departureDate).getMonth() == 5).length;
    let jul = currentYearCars.filter((x: any) => new Date(x.departureDate).getMonth() == 6).length;
    let aug = currentYearCars.filter((x: any) => new Date(x.departureDate).getMonth() == 7).length;
    let sep = currentYearCars.filter((x: any) => new Date(x.departureDate).getMonth() == 8).length;
    let oct = currentYearCars.filter((x: any) => new Date(x.departureDate).getMonth() == 9).length;
    let nov = currentYearCars.filter((x: any) => new Date(x.departureDate).getMonth() == 10).length;
    let dec = currentYearCars.filter((x: any) => new Date(x.departureDate).getMonth() == 11).length;
    
    data = [jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec];
    return data;
  }

  totalArrivedContainers() {
    let currentYearContainers = this.containersData.filter((x: any) => x.containerStatusStr == 'arrived' && new Date(x.creationDate).getFullYear() == this.currentYear);
    let data = [];

    let jan = currentYearContainers.filter((x: any) => new Date(x.arrivalDate).getMonth() == 0).length;
    let feb = currentYearContainers.filter((x: any) => new Date(x.arrivalDate).getMonth() == 1).length;
    let mar = currentYearContainers.filter((x: any) => new Date(x.arrivalDate).getMonth() == 2).length;
    let apr = currentYearContainers.filter((x: any) => new Date(x.arrivalDate).getMonth() == 3).length;
    let may = currentYearContainers.filter((x: any) => new Date(x.arrivalDate).getMonth() == 4).length;
    let jun = currentYearContainers.filter((x: any) => new Date(x.arrivalDate).getMonth() == 5).length;
    let jul = currentYearContainers.filter((x: any) => new Date(x.arrivalDate).getMonth() == 6).length;
    let aug = currentYearContainers.filter((x: any) => new Date(x.arrivalDate).getMonth() == 7).length;
    let sep = currentYearContainers.filter((x: any) => new Date(x.arrivalDate).getMonth() == 8).length;
    let oct = currentYearContainers.filter((x: any) => new Date(x.arrivalDate).getMonth() == 9).length;
    let nov = currentYearContainers.filter((x: any) => new Date(x.arrivalDate).getMonth() == 10).length;
    let dec = currentYearContainers.filter((x: any) => new Date(x.arrivalDate).getMonth() == 11).length;
    
    data = [jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec];
    return data;
  }

  totalawaitingLoadContainers() {
    let currentYearContainers = this.containersData.filter((x: any) => x.containerStatusStr == 'awaitingload' && new Date(x.creationDate).getFullYear() == this.currentYear);
    let data = [];

    let jan = currentYearContainers.filter((x: any) => new Date(x.creationDate).getMonth() == 0).length;
    let feb = currentYearContainers.filter((x: any) => new Date(x.creationDate).getMonth() == 1).length;
    let mar = currentYearContainers.filter((x: any) => new Date(x.creationDate).getMonth() == 2).length;
    let apr = currentYearContainers.filter((x: any) => new Date(x.creationDate).getMonth() == 3).length;
    let may = currentYearContainers.filter((x: any) => new Date(x.creationDate).getMonth() == 4).length;
    let jun = currentYearContainers.filter((x: any) => new Date(x.creationDate).getMonth() == 5).length;
    let jul = currentYearContainers.filter((x: any) => new Date(x.creationDate).getMonth() == 6).length;
    let aug = currentYearContainers.filter((x: any) => new Date(x.creationDate).getMonth() == 7).length;
    let sep = currentYearContainers.filter((x: any) => new Date(x.creationDate).getMonth() == 8).length;
    let oct = currentYearContainers.filter((x: any) => new Date(x.creationDate).getMonth() == 9).length;
    let nov = currentYearContainers.filter((x: any) => new Date(x.creationDate).getMonth() == 10).length;
    let dec = currentYearContainers.filter((x: any) => new Date(x.creationDate).getMonth() == 11).length;
    
    data = [jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec];
    return data;
  }

  totalDeparturedContainers() {
    let currentYearContainers = this.containersData.filter((x: any) => x.containerStatusStr == 'departured' && new Date(x.creationDate).getFullYear() == this.currentYear);
    let data = [];

    let jan = currentYearContainers.filter((x: any) => new Date(x.departureDate).getMonth() == 0).length;
    let feb = currentYearContainers.filter((x: any) => new Date(x.departureDate).getMonth() == 1).length;
    let mar = currentYearContainers.filter((x: any) => new Date(x.departureDate).getMonth() == 2).length;
    let apr = currentYearContainers.filter((x: any) => new Date(x.departureDate).getMonth() == 3).length;
    let may = currentYearContainers.filter((x: any) => new Date(x.departureDate).getMonth() == 4).length;
    let jun = currentYearContainers.filter((x: any) => new Date(x.departureDate).getMonth() == 5).length;
    let jul = currentYearContainers.filter((x: any) => new Date(x.departureDate).getMonth() == 6).length;
    let aug = currentYearContainers.filter((x: any) => new Date(x.departureDate).getMonth() == 7).length;
    let sep = currentYearContainers.filter((x: any) => new Date(x.departureDate).getMonth() == 8).length;
    let oct = currentYearContainers.filter((x: any) => new Date(x.departureDate).getMonth() == 9).length;
    let nov = currentYearContainers.filter((x: any) => new Date(x.departureDate).getMonth() == 10).length;
    let dec = currentYearContainers.filter((x: any) => new Date(x.departureDate).getMonth() == 11).length;
    
    data = [jan, feb, mar, apr, may, jun, jul, aug, sep, oct, nov, dec];
    return data;
  }

  draw() {
    this.chartColor = "#FFFFFF";

    this.canvas = document.getElementById("chartHours");
    this.ctx = this.canvas.getContext("2d");

    this.chartHours = new Chart(this.ctx, {
      type: 'line',

      data: {
        labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
        datasets: [{
            label: 'boughtnew',
            borderColor: "#6bd098",
            backgroundColor: "#6bd098",
            pointBackgroundColor: "#6bd098",
            pointRadius: 4,
            pointHoverRadius: 4,
            pointBorderWidth: 8,
            borderWidth: 3,
            data: this.totalBoughtNewStatisitcs(),
            fill: false,
           // tension: 0.1
          },
          {
            label: 'arrived',
            borderColor: "#51cbce",
            backgroundColor: "#51cbce",
            pointBackgroundColor: "#51cbce",
            pointRadius: 4,
            pointHoverRadius: 4,
            pointBorderWidth: 8,
            borderWidth: 3,
            data: this.totalArrivedStatisitcs(),
            fill: false,
           // tension: 0.1
          },
          {
            label: 'all',
            borderColor: "#fcc468",
            backgroundColor: "#fcc468",
            pointBackgroundColor: "#fcc468",
            pointRadius: 4,
            pointHoverRadius: 4,
            pointBorderWidth: 8,
            borderWidth: 3,
            data: this.totalCarsStatisitcs(),
            fill: false,
          //  tension: 0.1
          },
          {
            label: 'loaded',
            borderColor: "#ef8157",
            backgroundColor: "#ef8157",
            pointBackgroundColor: "#ef8157",
            pointRadius: 4,
            pointHoverRadius: 4,
            pointBorderWidth: 8,
            borderWidth: 3,
            data: this.totalLoadedStatisitcs(),
            fill: false,
          //  tension: 0.1
          }
        ]
      },
      options: {
        legend: {
          display: false,
          position: 'top'
        }
      }
    });


    // this.canvas = document.getElementById("chartEmail");
    // this.ctx = this.canvas.getContext("2d");
    // this.chartEmail = new Chart(this.ctx, {
    //   type: 'pie',
    //   data: {
    //     labels: ['boughtnew', 'arrived', 'loaded'],
    //     datasets: [{
    //       label: "Emails",
    //       pointRadius: 0,
    //       pointHoverRadius: 0,
    //       barPercentage: 1.6,
    //       backgroundColor: [
    //         '#6bd098',
    //         '#4acccd',
    //         '#ef8157'
    //       ],
    //       borderWidth: 0,
    //       data: [this.totalBoughtNewCars, this.totalArrivedCars, this.totalSeaCars]
    //     }]
    //   },

    //   options: {

    //     legend: {
    //       display: false
    //     },

    //     // pieceLabel: {
    //     //   render: 'percentage',
    //     //   fontColor: ['white'],
    //     //   precision: 2
    //     // },

    //     tooltips: {
    //       enabled: true
    //     },

    //     scales: {
    //       yAxes: [{

    //         ticks: {
    //           display: false
    //         },
    //         gridLines: {
    //           drawBorder: true,
    //           zeroLineColor: "transparent",
    //           color: 'rgba(255,255,255,0.05)'
    //         }

    //       }],

    //       xAxes: [{
    //         //barPercentage: 1.6,
    //         gridLines: {
    //           drawBorder: false,
    //           color: 'rgba(255,255,255,0.1)',
    //           zeroLineColor: "transparent"
    //         },
    //         ticks: {
    //           display: false,
    //         }
    //       }]
    //     },
    //   }
    // });

    var speedCanvas : any = document.getElementById("speedChart");

    var awaitingLoad = {
      data: this.totalawaitingLoadContainers(),
      label: 'awaitingload',
      borderColor: '#fbc658',
      backgroundColor: '#fbc658',
      pointBorderColor: '#fbc658',
      pointRadius: 0,
      pointHoverRadius: 0,
      pointBorderWidth: 3
    };

    var departured = {
      data: this.totalDeparturedContainers(),
      label: 'departured',
      borderColor: '#51CACF',
      backgroundColor: '#51CACF',
      pointBorderColor: '#51CACF',
      pointRadius: 0,
      pointHoverRadius: 0,
      pointBorderWidth: 3
    };

    var arrived = {
      data: this.totalArrivedContainers(),
      label: 'arrived',
      borderColor: '#ef8157',
      backgroundColor: '#ef8157',
      pointBorderColor: '#ef8157',
      pointRadius: 0,
      pointHoverRadius: 0,
      pointBorderWidth: 3
    };

    var speedData = {
      labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
      datasets: [/*awaitingLoad, */departured, arrived]
    };

    var chartOptions = {
      legend: {
        display: false,
        position: 'top'
      }
    };

    var lineChart = new Chart(speedCanvas, {
      type: 'line',
      //hover: false,
      data: speedData,
      options: {
        legend: {
          display: false,
          position: 'top'
        }
      }
    });
    this.spinner.hide();
  }

}
