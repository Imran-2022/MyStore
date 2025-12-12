import type { EntityDto } from '@abp/ng.core';

export interface StockDto extends EntityDto<string> {
  product?: string;
  warehouse?: string;
  quantity: number;
}
