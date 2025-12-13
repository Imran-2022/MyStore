import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { StockService, StockDto } from '../proxy/stocks';
import { CreateUpdateSaleDto, SaleDto, SaleService } from '../proxy/sales';

interface SaleProduct {
  warehouse: string | null;
  product: string | null;
  quantity: number;
  price: number;
}

interface Sale {
  id?: string;
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

  // Sales records from backend
  sales: SaleDto[] = [];

  // New sale being created
  newSale: Sale = this.emptySale();

  constructor(
    private modal: NgbModal,
    private stockService: StockService,
    private saleService: SaleService
  ) {}

  ngOnInit() {
    this.loadStock();
    this.loadSales();
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

  // Load sales from backend
  loadSales() {
    this.saleService.getList().subscribe(res => this.sales = res);
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

  // Add / remove product rows
  addRow() {
    this.newSale.products.push({ warehouse: null, product: null, quantity: 1, price: 0 });
  }

  removeRow(i: number) {
    this.newSale.products.splice(i, 1);
    this.calculateTotal();
  }

  // Products available in selected warehouse with positive stock
  getProducts(warehouse: string | null) {
    return this.stockList.filter(
      s => s.warehouse === warehouse && s.quantity > 0
    );
  }

  // When warehouse changes, reset product & price
  onWarehouseChange(i: number) {
    const p = this.newSale.products[i];
    p.product = null;
    p.price = 0;
    this.calculateTotal();
  }

  // When product changes, optionally auto-fill price
  onProductChange(i: number) {
    const p = this.newSale.products[i];
    const stock = this.stockList.find(
      s => s.warehouse === p.warehouse && s.product === p.product
    );
    if (stock) {
      // Default price = 0 (can be edited by user)
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

  // Form validation
  canSave(): boolean {
    return this.newSale.customer.trim() !== '' &&
      this.newSale.products.every(p => p.warehouse && p.product && p.quantity > 0);
  }

  // Save sale using SaleService (backend)
  saveSale(modal: any) {
    if (!this.canSave()) return;

    const dto: CreateUpdateSaleDto = {
      customer: this.newSale.customer,
      dateTime: this.newSale.dateTime,
      products: this.newSale.products.map(p => ({
        warehouse: p.warehouse!,
        product: p.product!,
        quantity: p.quantity,
        price: p.price
      }))
    };

    this.saleService.create(dto).subscribe({
      next: () => {
        this.loadSales(); // refresh sales list
        modal.close();
      },
      error: err => {
        console.error('Error saving sale', err);
      }
    });
  }
}
