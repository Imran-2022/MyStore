import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';

interface Product {
  name: string;
  price: number;
}

interface SaleProduct {
  product?: Product;
  quantity: number;
}

interface Sale {
  customer: string;
  date: string; // use string for binding with <input type="date">
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
  // Product list
  productList: Product[] = [
    { name: 'Apple', price: 2 },
    { name: 'Banana', price: 1.5 },
    { name: 'Orange', price: 3 },
    { name: 'Mango', price: 4 },
    { name: 'Pineapple', price: 5 }
  ];

  sales: Sale[] = [];

  newSale: Sale = this.getEmptySale();

  constructor(private modalService: NgbModal) {}

  getEmptySale(): Sale {
    return {
      customer: '',
      date: this.getToday(),
      products: [{ product: undefined, quantity: 1 }],
      grandTotal: 0
    };
  }

  getToday(): string {
    const today = new Date();
    const year = today.getFullYear();
    const month = (today.getMonth() + 1).toString().padStart(2, '0');
    const day = today.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  openModal(content: any) {
    this.newSale = this.getEmptySale();
    this.modalService.open(content, { size: 'lg' });
  }

  addProductRow() {
    this.newSale.products.push({ product: undefined, quantity: 1 });
  }

  removeProductRow(index: number) {
    this.newSale.products.splice(index, 1);
    this.calculateGrandTotal();
  }

  calculateItemTotal(p: SaleProduct): number {
    if (!p.product || !p.quantity) return 0;
    return p.product.price * p.quantity;
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
      this.newSale.products.every(p => p.product && p.quantity > 0)
    );
  }

  saveSale(modal: any, formValid: boolean) {
    if (!formValid || !this.canSaveSale()) return;
    const saleToAdd: Sale = {
      customer: this.newSale.customer,
      date: this.newSale.date,
      products: this.newSale.products.map(p => ({ ...p })),
      grandTotal: this.newSale.grandTotal
    };
    this.sales.push(saleToAdd);
    modal.close();
  }

  // Return products that haven’t been selected yet
  availableProducts(i: number): Product[] {
    const selectedNames = this.newSale.products
      .filter((_, idx) => idx !== i)
      .map(p => p.product?.name);
    return this.productList.filter(p => !selectedNames.includes(p.name));
  }
}
