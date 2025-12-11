import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';

interface PurchaseProduct {
  warehouse: string;
  product: string;
  quantity: number;
  price: number; // NEW: price per product
}

interface Purchase {
  dateTime: string;
  purchaseCode: string;
  supplierName: string;
  products: PurchaseProduct[];
  totalAmount: number;
  discount: number;
  payableAmount: number;
  paidAmount: number;
  dueAmount: number;
}

@Component({
  selector: 'app-purchases',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbModule],
  templateUrl: './purchases.html',
  styleUrls: ['./purchases.scss']
})
export class Purchases {

  purchases: Purchase[] = [];

  constructor(private modalService: NgbModal) {}

  newPurchase: Purchase = this.getEmptyPurchase();

  getEmptyPurchase(): Purchase {
    const now = new Date();
    return {
      dateTime: this.getCurrentDateTime(),
      purchaseCode: this.generatePurchaseCode(now), // auto-generate
      supplierName: '',
      products: [{ warehouse: '', product: '', quantity: 1, price: 0 }],
      totalAmount: 0,
      discount: 0,
      payableAmount: 0,
      paidAmount: 0,
      dueAmount: 0
    };
  }
  // New method to generate purchase code from current date/time
generatePurchaseCode(date: Date): string {
  const year = date.getFullYear().toString();
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const day = date.getDate().toString().padStart(2, '0');
  const hour = date.getHours().toString().padStart(2, '0');
  const minute = date.getMinutes().toString().padStart(2, '0');
  const second = date.getSeconds().toString().padStart(2, '0');
  return `${year}${month}${day}${hour}${minute}${second}`; // e.g., "20251211123045"
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
    this.newPurchase = this.getEmptyPurchase();
    this.modalService.open(content, { size: 'xl' });
  }

  addProductRow() {
    this.newPurchase.products.push({ warehouse: '', product: '', quantity: 1, price: 0 });
    this.calculateTotals();
  }

  removeProductRow(i: number) {
    this.newPurchase.products.splice(i, 1);
    this.calculateTotals();
  }

  calculateItemTotal(p: PurchaseProduct): number {
    return p.quantity * p.price;
  }

  calculateTotals() {
    this.newPurchase.totalAmount = this.newPurchase.products.reduce(
      (sum, p) => sum + this.calculateItemTotal(p),
      0
    );

    this.newPurchase.payableAmount = this.newPurchase.totalAmount - this.newPurchase.discount;
    this.newPurchase.dueAmount = this.newPurchase.payableAmount - this.newPurchase.paidAmount;

    if (this.newPurchase.dueAmount < 0) this.newPurchase.dueAmount = 0;
  }

  canSavePurchase(): boolean {
    return (
      this.newPurchase.purchaseCode.trim() !== '' &&
      this.newPurchase.supplierName.trim() !== '' &&
      this.newPurchase.products.every(p =>
        p.warehouse.trim() !== '' &&
        p.product.trim() !== '' &&
        p.quantity > 0 &&
        p.price >= 0
      )
    );
  }

  savePurchase(modal: any, formValid: boolean) {
    if (!formValid || !this.canSavePurchase()) return;

    const saved = JSON.parse(JSON.stringify(this.newPurchase)); // deep copy
    console.log('saved purchase', saved);
    this.purchases.push(saved);

    modal.close();
  }
}
