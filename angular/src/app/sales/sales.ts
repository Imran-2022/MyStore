import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { StockDto, StockService } from '../proxy/stocks';

interface SaleProduct {
  warehouse: string | null;
  product: string | null;
  quantity: number;
  price: number;
}

interface Sale {
  customer: string;
  dateTime: string;
  products: SaleProduct[];
  grandTotal: number;
}

@Component({
  selector: 'app-sales',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbModule],
  templateUrl: './sales.html',
})
export class Sales implements OnInit {

  // Stock from backend
  stockList: StockDto[] = [];
  warehouses: string[] = [];

  // Sales records (local for now)
  sales: Sale[] = [];

  // New sale being created
  newSale: Sale = this.emptySale();

  constructor(private modal: NgbModal, private stockService: StockService) {}

  ngOnInit() {
    this.loadStock();
  }

  // Load stock from backend
  loadStock() {
    this.stockService.getList().subscribe(res => {
      this.stockList = res;

      // Only warehouses with available stock
      this.warehouses = [...new Set(
        this.stockList.filter(s => s.quantity > 0).map(s => s.warehouse)
      )];
    });
  }

  // Initialize empty sale
  emptySale(): Sale {
    return {
      customer: '',
      dateTime: new Date().toISOString().slice(0, 16),
      products: [{ warehouse: null, product: null, quantity: 1, price: 0 }],
      grandTotal: 0
    };
  }

  // Open new sale modal
  openModal(tpl: any) {
    this.newSale = this.emptySale();
    this.modal.open(tpl, { size: 'xl' });
  }

  // Add / remove rows
  addRow() {
    this.newSale.products.push({ warehouse: null, product: null, quantity: 1, price: 0 });
  }

  removeRow(i: number) {
    this.newSale.products.splice(i, 1);
    this.calculateTotal();
  }

  // Available products in selected warehouse with stock
  getProducts(warehouse: string | null) {
    return this.stockList.filter(
      s => s.warehouse === warehouse && s.quantity > 0
    );
  }

  // When warehouse changes, reset product & price
  onWarehouseChange(i: number) {
    this.newSale.products[i].product = null;
    this.newSale.products[i].price = 0;
    this.calculateTotal();
  }

  // When product changes, optionally auto-fill price
  onProductChange(i: number) {
    const p = this.newSale.products[i];
    const stock = this.stockList.find(
      s => s.warehouse === p.warehouse && s.product === p.product
    );
    if (stock) {
      // Default price = 0, user can edit
      p.price = p.price || 0;
    }
    this.calculateTotal();
  }

  // Calculate grand total
  calculateTotal() {
    this.newSale.grandTotal = this.newSale.products.reduce(
      (sum, p) => sum + (p.quantity * p.price || 0),
      0
    );
  }

  // Validate sale form
  canSave(): boolean {
    return this.newSale.customer.trim() !== '' &&
      this.newSale.products.every(p => p.warehouse && p.product && p.quantity > 0);
  }

  // Save sale (local for now)
  saveSale(modal: any) {
    if (!this.canSave()) return;
    this.sales.push(JSON.parse(JSON.stringify(this.newSale)));
    modal.close();
  }
}
