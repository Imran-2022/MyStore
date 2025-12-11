import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PurchaseDto, PurchaseProductDto, PurchaseService } from '../proxy/purchases';

@Component({
  selector: 'app-purchases',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbModule],
  templateUrl: './purchases.html',
  styleUrls: ['./purchases.scss']
})
export class Purchases implements OnInit {
  purchases: PurchaseDto[] = [];

  constructor(private modalService: NgbModal, private purchaseService: PurchaseService) {}

  ngOnInit(): void {
    this.loadPurchases();
  }

  loadPurchases() {
    this.purchaseService.getList().subscribe({
      next: data => this.purchases = data,
      error: err => console.error('Error loading purchases', err)
    });
  }

  newPurchase: PurchaseDto = this.getEmptyPurchase();

  getEmptyPurchase(): PurchaseDto {
    const now = new Date();
    return {
      id: '',
      dateTime: this.getCurrentDateTime(),
      purchaseCode: this.generatePurchaseCode(now),
      supplierName: '',
      products: [{ id: '', warehouse: '', product: '', quantity: 1, price: 0, total: 0 }],
      totalAmount: 0,
      discount: 0,
      payableAmount: 0,
      paidAmount: 0,
      dueAmount: 0
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
  generatePurchaseCode(date: Date): string {
    const y = date.getFullYear();
    const m = (date.getMonth() + 1).toString().padStart(2, '0');
    const d = date.getDate().toString().padStart(2, '0');
    const h = date.getHours().toString().padStart(2, '0');
    const min = date.getMinutes().toString().padStart(2, '0');
    const s = date.getSeconds().toString().padStart(2, '0');
    return `${y}${m}${d}${h}${min}${s}`;
  }

  openModal(content: any) { this.newPurchase = this.getEmptyPurchase(); this.modalService.open(content, { size: 'xl' }); }

  addProductRow() { this.newPurchase.products.push({ id: '', warehouse: '', product: '', quantity: 1, price: 0, total: 0 }); this.calculateTotals(); }
  removeProductRow(i: number) { this.newPurchase.products.splice(i, 1); this.calculateTotals(); }

  calculateItemTotal(p: PurchaseProductDto): number { return p.quantity * p.price; }

  calculateTotals() {
    this.newPurchase.totalAmount = this.newPurchase.products.reduce((sum, p) => sum + this.calculateItemTotal(p), 0);
    this.newPurchase.payableAmount = this.newPurchase.totalAmount - this.newPurchase.discount;
    this.newPurchase.dueAmount = this.newPurchase.payableAmount - this.newPurchase.paidAmount;
    this.newPurchase.products.forEach(p => p.total = this.calculateItemTotal(p));
    if (this.newPurchase.dueAmount < 0) this.newPurchase.dueAmount = 0;
  }

  canSavePurchase(): boolean {
    return this.newPurchase.purchaseCode.trim() !== '' &&
      this.newPurchase.supplierName.trim() !== '' &&
      this.newPurchase.products.every(p => p.warehouse.trim() !== '' && p.product.trim() !== '' && p.quantity > 0 && p.price >= 0);
  }

  savePurchase(modal: any, formValid: boolean) {
    if (!formValid || !this.canSavePurchase()) return;

    const saved = JSON.parse(JSON.stringify(this.newPurchase));
    this.purchases.push(saved);
    modal.close();

    // TODO: Later call backend POST API via `purchaseService.create(...)`
  }
}
