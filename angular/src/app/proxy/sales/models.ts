
export interface CreateUpdateSaleDto {
  customer?: string;
  dateTime?: string;
  products: CreateUpdateSaleProductDto[];
}

export interface CreateUpdateSaleProductDto {
  warehouse?: string;
  product?: string;
  quantity: number;
  price: number;
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
