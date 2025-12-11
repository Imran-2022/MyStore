
export interface CreateUpdatePurchaseDto {
  purchaseCode?: string;
  supplierName?: string;
  dateTime?: string;
  discount: number;
  paidAmount: number;
  products: CreateUpdatePurchaseProductDto[];
}

export interface CreateUpdatePurchaseProductDto {
  warehouse?: string;
  product?: string;
  quantity: number;
  price: number;
}

export interface PurchaseDto {
  id?: string;
  purchaseCode?: string;
  supplierName?: string;
  dateTime?: string;
  totalAmount: number;
  discount: number;
  payableAmount: number;
  paidAmount: number;
  dueAmount: number;
  products: PurchaseProductDto[];
}

export interface PurchaseProductDto {
  id?: string;
  warehouse?: string;
  product?: string;
  quantity: number;
  price: number;
  total: number;
}
