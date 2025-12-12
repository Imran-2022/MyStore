import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StockService, StockDto } from '../proxy/stocks';

@Component({
  selector: 'app-stock-report',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './stock-report.html',
  styleUrls: ['./stock-report.scss']
})
export class StockReport implements OnInit {

  stockRecords: StockDto[] = [];
  filteredStockRecords: StockDto[] = [];

  uniqueProducts: string[] = [];
  uniqueWarehouses: string[] = [];

  selectedProduct: string = 'all';
  selectedWarehouse: string = 'all';

  constructor(private stockService: StockService) {}

  ngOnInit() {
    this.loadStock();
  }

  loadStock() {
    this.stockService.getList().subscribe({
      next: (res) => {
        this.stockRecords = res;
        this.filteredStockRecords = res;

        // Build unique filters
        this.uniqueProducts = [...new Set(res.map(r => r.product))];
        this.uniqueWarehouses = [...new Set(res.map(r => r.warehouse))];
      },
      error: (err) => console.error('Failed to load stock report', err)
    });
  }

  filterData() {
    this.filteredStockRecords = this.stockRecords.filter(record => {
      const matchProduct =
        this.selectedProduct === 'all' || record.product === this.selectedProduct;

      const matchWarehouse =
        this.selectedWarehouse === 'all' || record.warehouse === this.selectedWarehouse;

      return matchProduct && matchWarehouse;
    });
  }
}
