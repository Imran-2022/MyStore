import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';

interface Product {
  name: string;
  price: number;
}

interface Warehouse {
  name: string;
  products: Product[];
}

interface SaleProduct {
  warehouse?: Warehouse;
  product?: Product;
  quantity: number;
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
  styleUrls: ['./sales.scss']
})
export class Sales {
  // Warehouses with products
  warehouses: Warehouse[] = [
    { name: 'Warehouse A', products: [{ name: 'Apple', price: 2 }, { name: 'Banana', price: 1.5 }] },
    { name: 'Warehouse B', products: [{ name: 'Orange', price: 3 }, { name: 'Mango', price: 4 }] },
    { name: 'Warehouse C', products: [{ name: 'Pineapple', price: 5 }] }
  ];

  sales: Sale[] = [];

  newSale: Sale = this.getEmptySale();

  constructor(private modalService: NgbModal) {}

  getEmptySale(): Sale {
    return {
      customer: '',
      dateTime: this.getCurrentDateTime(),
      products: [{ warehouse: undefined, product: undefined, quantity: 1 }],
      grandTotal: 0
    };
  }

  getCurrentDateTime(): string {
    const now = new Date();
    const year = now.getFullYear();
    const month = (now.getMonth() + 1).toString().padStart(2, '0');
    const day = now.getDate().toString().padStart(2, '0');
    const hour = now.getHours().toString().padStart(2, '0');
    const minute = now.getMinutes().toString().padStart(2, '0');
    return `${year}-${month}-${day}T${hour}:${minute}`;
  }

  openModal(content: any) {
    this.newSale = this.getEmptySale();
    this.modalService.open(content, { size: 'lg' });
  }

  addProductRow() {
    this.newSale.products.push({ warehouse: undefined, product: undefined, quantity: 1 });
  }

  removeProductRow(index: number) {
    this.newSale.products.splice(index, 1);
    this.calculateGrandTotal();
  }

  calculateItemTotal(p: SaleProduct): number {
    return p.product && p.quantity ? p.product.price * p.quantity : 0;
  }

  calculateGrandTotal() {
    this.newSale.grandTotal = this.newSale.products.reduce(
      (sum, p) => sum + this.calculateItemTotal(p),
      0
    );
  }

  canSaveSale(): boolean {
    return (
      this.newSale.customer.trim() !== '' &&
      this.newSale.products.length > 0 &&
      this.newSale.products.every(p => p.warehouse && p.product && p.quantity > 0)
    );
  }

  saveSale(modal: any, formValid: boolean) {
    if (!formValid || !this.canSaveSale()) return;

    const saleToAdd: Sale = {
      customer: this.newSale.customer,
      dateTime: this.newSale.dateTime,
      products: this.newSale.products.map(p => ({ ...p })),
      grandTotal: this.newSale.grandTotal
    };
    console.log('Saving sale:', saleToAdd);
    this.sales.push(saleToAdd);
    modal.close();
  }

  // Products available in selected warehouse, avoid duplicate selection
  availableProducts(i: number): Product[] {
    const p = this.newSale.products[i];
    return p.warehouse
      ? p.warehouse.products.filter(prod => !this.newSale.products.some((sp, idx) => idx !== i && sp.product?.name === prod.name))
      : [];
  }
}
