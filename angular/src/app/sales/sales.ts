import { Component, OnInit, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { StockService, StockDto } from '../proxy/stocks';
import { SaleService, SaleDto, CreateUpdateSaleDto } from '../proxy/sales';

interface SaleProductUI {
  warehouse: string | null;
  product: string | null;
  quantity: number;
  price: number;
}

interface SaleUI {
  id?: string;
  customer: string;
  dateTime: string;
  products: SaleProductUI[];
  grandTotal: number;
}

@Component({
  selector: 'app-sales',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbModule],
  templateUrl: './sales.html',
})
export class Sales implements OnInit {

  stockList: StockDto[] = [];
  warehouses: string[] = [];
  sales: SaleDto[] = [];

  newSale!: SaleUI;
  isEditMode = false;

  constructor(
    private modal: NgbModal,
    private stockService: StockService,
    private saleService: SaleService
  ) { }

  ngOnInit() {
    this.loadStock();
    this.loadSales();
  }

  loadStock() {
    this.stockService.getList().subscribe(res => {
      this.stockList = res;
      this.warehouses = [...new Set(
        res.filter(x => x.quantity > 0).map(x => x.warehouse)
      )];
    });
  }

  loadSales() {
    this.saleService.getList().subscribe(res => this.sales = res);
  }

  emptySale(): SaleUI {
    return {
      customer: '',
      dateTime: new Date().toISOString().slice(0, 16),
      products: [{ warehouse: null, product: null, quantity: 1, price: 0 }],
      grandTotal: 0
    };
  }

  openCreateModal(tpl: any) {
    this.isEditMode = false;
    this.newSale = this.emptySale();
    this.modal.open(tpl, { size: 'xl' });
  }

  openEditModal(sale: SaleDto, tpl: TemplateRef<any>) {
    this.isEditMode = true;

    // Map sale to UI model
    this.newSale = {
      id: sale.id,
      customer: sale.customer || '',
      dateTime: sale.dateTime!.slice(0, 16),
      products: sale.products.map(p => ({
        warehouse: p.warehouse || null,
        product: p.product || null,
        quantity: p.quantity,
        price: p.price
      })),
      grandTotal: this.getSaleTotal(sale)
    };

    // OPEN MODAL
    this.modal.open(tpl, { size: 'xl' });
  }



  getProducts(warehouse: string | null) {
    return this.stockList.filter(s => s.warehouse === warehouse && s.quantity > 0);
  }

  addRow() {
    this.newSale.products.push({ warehouse: null, product: null, quantity: 1, price: 0 });
  }

  removeRow(i: number) {
    this.newSale.products.splice(i, 1);
    this.calculateTotal();
  }

  onWarehouseChange(i: number) {
    this.newSale.products[i].product = null;
    this.calculateTotal();
  }

  calculateTotal() {
    this.newSale.grandTotal = this.newSale.products.reduce(
      (sum, p) => sum + p.quantity * p.price, 0
    );
  }

  canSave() {
    return this.newSale.customer &&
      this.newSale.products.every(p => p.warehouse && p.product && p.quantity > 0);
  }

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

    const request$ = this.isEditMode
      ? this.saleService.update(this.newSale.id!, dto)
      : this.saleService.create(dto);

    request$.subscribe(() => {
      this.loadSales();
      modal.close();
    });
  }

  getSaleTotal(sale: SaleDto): number {
    return sale.products.reduce(
      (sum, p) => sum + p.quantity * p.price, 0
    );
  }
  deleteSale(sale: SaleDto) {
    if (!sale.id) return;

    if (confirm('Are you sure you want to delete this sale?')) {
      this.saleService.delete(sale.id).subscribe(() => {
        this.loadSales();
      });
    }
  }
  onProductChange(i: number) {
    const p = this.newSale.products[i];
    const stock = this.stockList.find(
      s => s.warehouse === p.warehouse && s.product === p.product
    );
    if (stock) {
      p.price = p.price || 0; // keep previous price or default 0
    }
    this.calculateTotal();
  }

}
