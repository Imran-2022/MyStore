
export interface CreateUpdateSaleDto {
  customer?: string;
  dateTime?: string;
  products: SaleProductDto[];
}

export interface SaleDto {
  id?: string;
  customer?: string;
  dateTime?: string;
  products: SaleProductDto[];
}

export interface SaleProductDto {
  id?: string;
  warehouse?: string;
  product?: string;
  quantity: number;
  price: number;
}
