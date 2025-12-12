import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { NgbModal, NgbModalRef, NgbModule } from '@ng-bootstrap/ng-bootstrap';
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
  existingPurchaseMode = false;           // flag for update mode
  private currentModalRef: NgbModalRef;   // store modal reference

  constructor(private modalService: NgbModal, private purchaseService: PurchaseService) {}

  // --- Utility functions ---
  getEmptyPurchase(): CreateUpdatePurchaseDto {
    const now = new Date();
    return {
      purchaseCode: this.generatePurchaseCode(now),
      supplierName: '',
      dateTime: new Date().toISOString().slice(0,16),
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

  // --- Modal handling ---
  openModal(content: any) {
    this.existingPurchaseMode = false;
    this.newPurchase = this.getEmptyPurchase();
    this.currentModalRef = this.modalService.open(content, { size: 'xl' });
  }

  openUpdatePurchase(purchase: PurchaseDto, content: any) {
    this.existingPurchaseMode = true;

    this.newPurchase = {
      purchaseCode: purchase.purchaseCode,
      supplierName: purchase.supplierName,
      dateTime: new Date(purchase.dateTime).toISOString().slice(0,16),
      discount: purchase.discount,
      paidAmount: purchase.paidAmount,
      products: purchase.products.map(prod => ({
        warehouse: prod.warehouse,
        product: prod.product,
        quantity: prod.quantity,
        price: prod.price
      }))
    };

    this.calculateTotals();
    this.currentModalRef = this.modalService.open(content, { size: 'xl' });
  }

  // --- Product row handling ---
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

    (this.newPurchase as any).totalAmount = totalAmount;
    (this.newPurchase as any).payableAmount = payableAmount;
    (this.newPurchase as any).dueAmount = dueAmount < 0 ? 0 : dueAmount;
  }

  canSavePurchase(): boolean {
    return !!this.newPurchase.purchaseCode &&
           !!this.newPurchase.supplierName &&
           this.newPurchase.products.every(p => p.warehouse && p.product && p.quantity > 0 && p.price >= 0);
  }

  // --- CRUD ---
  savePurchase() {
    this.calculateTotals();

    this.purchaseService.create(this.newPurchase).subscribe({
      next: (res) => {
        this.purchases.push(res);
        this.currentModalRef.close();
      },
      error: (err) => console.error('Failed to create purchase', err)
    });
  }

  updatePurchase() {
    this.calculateTotals();

    const existingPurchase = this.purchases.find(p => p.purchaseCode === this.newPurchase.purchaseCode);
    if (!existingPurchase) {
      console.error('Purchase not found for update');
      return;
    }

    this.purchaseService.update(existingPurchase.id, this.newPurchase).subscribe({
      next: () => {
        const index = this.purchases.findIndex(p => p.id === existingPurchase.id);
        if (index !== -1) this.purchases[index] = { ...existingPurchase, ...this.newPurchase } as PurchaseDto;
        this.currentModalRef.close();
      },
      error: (err) => console.error('Failed to update purchase', err)
    });
  }

  saveOrUpdatePurchase(formValid: boolean) {
    if (!formValid || !this.canSavePurchase()) return;
    this.existingPurchaseMode ? this.updatePurchase() : this.savePurchase();
  }

  deletePurchase(purchase: PurchaseDto) {
    if (!confirm(`Are you sure you want to delete purchase ${purchase.purchaseCode}?`)) return;

    this.purchaseService.delete(purchase.id).subscribe({
      next: () => this.purchases = this.purchases.filter(p => p.id !== purchase.id),
      error: (err) => console.error('Failed to delete purchase', err)
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
