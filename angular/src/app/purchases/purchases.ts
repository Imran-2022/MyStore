import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CreateUpdatePurchaseDto, CreateUpdatePurchaseProductDto, PurchaseDto, PurchaseService } from '../proxy/purchases';

@Component({
  selector: 'app-purchases',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbModule],
  templateUrl: './purchases.html',
  styleUrls: ['./purchases.scss'],
})
export class Purchases {

  purchases: PurchaseDto[] = [];
  newPurchase: CreateUpdatePurchaseDto = this.getEmptyPurchase();

  constructor(private modalService: NgbModal, private purchaseService: PurchaseService) {}

  getEmptyPurchase(): CreateUpdatePurchaseDto {
    const now = new Date();
    return {
      purchaseCode: this.generatePurchaseCode(now), // will be entered in modal
      supplierName: '',
      dateTime: new Date().toISOString().slice(0,16), // datetime-local format
      discount: 0,
      paidAmount: 0,
      products: [{ warehouse: '', product: '', quantity: 1, price: 0 }]
    };
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

  calculateItemTotal(p: CreateUpdatePurchaseProductDto): number {
    return p.quantity * p.price;
  }

  calculateTotals() {
    const totalAmount = this.newPurchase.products.reduce((sum, p) => sum + this.calculateItemTotal(p), 0);
    const payableAmount = totalAmount - (this.newPurchase.discount ?? 0);
    const dueAmount = payableAmount - (this.newPurchase.paidAmount ?? 0);

    // Assign calculated values
    (this.newPurchase as any).totalAmount = totalAmount;
    (this.newPurchase as any).payableAmount = payableAmount;
    (this.newPurchase as any).dueAmount = dueAmount < 0 ? 0 : dueAmount;
  }

  canSavePurchase(): boolean {
    return !!this.newPurchase.purchaseCode &&
           !!this.newPurchase.supplierName &&
           this.newPurchase.products.every(p => p.warehouse && p.product && p.quantity > 0 && p.price >= 0);
  }

  savePurchase(modal: any, formValid: boolean) {
    if (!formValid || !this.canSavePurchase()) return;

    this.calculateTotals();

    this.purchaseService.create(this.newPurchase).subscribe({
      next: (res) => {
        this.purchases.push(res); // add returned PurchaseDto to list
        modal.close();
      },
      error: (err) => {
        console.error('Failed to create purchase', err);
      }
    });
  }

  loadPurchases() {
    this.purchaseService.getList().subscribe({
      next: (res) => this.purchases = res,
      error: (err) => console.error(err)
    });
  }

  ngOnInit() {
    this.loadPurchases();
  }
}
