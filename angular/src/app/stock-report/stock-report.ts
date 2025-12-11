import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

interface StockRecord {
  productName: string;
  warehouseName: string;
  quantity: number;
}

@Component({
  selector: 'app-stock-report',
  standalone: true,
  imports: [CommonModule],   // <-- REQUIRED FOR *ngFor, *ngIf
  templateUrl: './stock-report.html',
  styleUrls: ['./stock-report.scss']
})
export class StockReport {

  stockRecords: StockRecord[] = [
    { productName: 'Apple', warehouseName: 'Warehouse A', quantity: 500 },
    { productName: 'Banana', warehouseName: 'Warehouse A', quantity: 200 },
    { productName: 'Orange', warehouseName: 'Warehouse B', quantity: 300 },
    { productName: 'Mango', warehouseName: 'Warehouse B', quantity: 150 },
    { productName: 'Pineapple', warehouseName: 'Warehouse C', quantity: 80 }
  ];

}
