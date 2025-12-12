import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface StockRecord {
  productName: string;
  warehouseName: string;
  quantity: number;
}

@Component({
  selector: 'app-stock-report',
  standalone: true,
  imports: [CommonModule, FormsModule],
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

  // Unique lists
  uniqueProducts: string[] = [...new Set(this.stockRecords.map(r => r.productName))];
  uniqueWarehouses: string[] = [...new Set(this.stockRecords.map(r => r.warehouseName))];

  // Default filters
  selectedProduct: string = "all";
  selectedWarehouse: string = "all";

  filteredStockRecords: StockRecord[] = this.stockRecords;

  filterData() {
    this.filteredStockRecords = this.stockRecords.filter(record => {
      const matchProduct = this.selectedProduct === "all" || record.productName === this.selectedProduct;
      const matchWarehouse = this.selectedWarehouse === "all" || record.warehouseName === this.selectedWarehouse;
      return matchProduct && matchWarehouse;
    });
  }

}
